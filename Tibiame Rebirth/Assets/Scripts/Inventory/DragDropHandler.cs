using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("การตั้งค่า Drag & Drop")]
    public InventorySlot currentSlot;
    public Canvas canvas;
    
    [Header("Visual Effect")]
    public float draggedAlpha = 0.8f;
    public float draggedScale = 1.2f;
    
    private GameObject draggedItem;
    private Image draggedImage;
    private ItemData draggedItemData;
    private Vector3 originalPosition;
    
    void Start()
    {
        // หา Canvas อัตโนมัติถ้าไม่ได้ตั้งค่า
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentSlot == null || currentSlot.currentItem == null) return;
        
        draggedItemData = currentSlot.currentItem;
        originalPosition = transform.position;
        
        // สร้างไอเท็มที่กำลังลาก
        draggedItem = new GameObject("DraggedItem");
        draggedItem.transform.SetParent(canvas.transform, false);
        
        draggedImage = draggedItem.AddComponent<Image>();
        draggedImage.sprite = currentSlot.currentItem.icon;
        draggedImage.raycastTarget = false;
        draggedImage.color = new Color(1, 1, 1, draggedAlpha);
        
        RectTransform draggedRect = draggedItem.GetComponent<RectTransform>();
        draggedRect.sizeDelta = new Vector2(80, 80); // ขนาดตามช่อง
        draggedRect.localScale = Vector3.one * draggedScale;
        
        // ตั้งตำแหน่งเริ่มต้น
        draggedItem.transform.position = Input.mousePosition;
        
        // ทำให้ไอคอนจางลงเท่านั้น ไม่แก้ไขสีพื้นหลัง
        if (currentSlot.icon != null)
        {
            currentSlot.icon.color = new Color(1, 1, 1, 0.5f);
        }
        
        Debug.Log($"เริ่มลาก: {draggedItemData.itemName}");
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (draggedItem != null)
        {
            draggedItem.transform.position = Input.mousePosition;
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedItem == null) return;
        
        // คืนสีช่องต้นทาง
        if (currentSlot.icon != null)
        {
            currentSlot.icon.color = Color.white;
        }
        
        // หาช่องที่วาง
        GameObject droppedOn = eventData.pointerCurrentRaycast.gameObject;
        bool droppedSuccessfully = false;
        
        if (droppedOn != null)
        {
            // ลองหา InventorySlot ก่อน
            InventorySlot targetInventorySlot = droppedOn.GetComponent<InventorySlot>();
            if (targetInventorySlot == null && droppedOn.transform.parent != null)
            {
                targetInventorySlot = droppedOn.transform.parent.GetComponent<InventorySlot>();
            }
            
            // ถ้าเป็น InventorySlot
            if (targetInventorySlot != null && targetInventorySlot != currentSlot)
            {
                SwapItems(currentSlot, targetInventorySlot);
                droppedSuccessfully = true;
            }
            else
            {
                // ลองหา EquipmentSlot
                EquipmentSlot targetEquipmentSlot = droppedOn.GetComponent<EquipmentSlot>();
                if (targetEquipmentSlot == null && droppedOn.transform.parent != null)
                {
                    targetEquipmentSlot = droppedOn.transform.parent.GetComponent<EquipmentSlot>();
                }
                
                // ถ้าเป็น EquipmentSlot
                if (targetEquipmentSlot != null)
                {
                    HandleDropOnEquipmentSlot(targetEquipmentSlot);
                    droppedSuccessfully = true;
                }
            }
        }
        
        if (!droppedSuccessfully)
        {
            // วางไม่สำเร็จ กลับที่เดิม
            Debug.Log("วางไอเท็มไม่สำเร็จ - กลับที่เดิม");
        }
        
        // ทำลายไอเท็มที่ลาก
        Destroy(draggedItem);
        draggedItem = null;
        draggedImage = null;
        draggedItemData = null;
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        // จัดการการรับไอเท็มที่วาง
        DragDropHandler draggedHandler = eventData.pointerDrag?.GetComponent<DragDropHandler>();
        
        if (draggedHandler != null && draggedHandler != this)
        {
            Debug.Log($"รับไอเท็ม: {draggedHandler.draggedItemData?.itemName} ที่ช่อง {gameObject.name}");
        }
    }
    
    void SwapItems(InventorySlot fromSlot, InventorySlot toSlot)
    {
        if (fromSlot == null || toSlot == null) return;
        
        ItemData fromItem = fromSlot.currentItem;
        ItemData toItem = toSlot.currentItem;
        
        // ถ้าช่องเป้าหมายว่าง - ย้ายไอเท็ม
        if (toItem == null)
        {
            toSlot.SetItem(fromItem);
            fromSlot.ClearSlot();
            
            Debug.Log($"ย้ายไอเท็ม: {fromItem.itemName} → ช่องว่าง");
        }
        // ถ้าช่องเป้าหมายมีไอเท็ม - ลองซ้อนก่อน
        else if (DragDropManager.Instance != null && DragDropManager.Instance.TryStackItems(fromSlot, toSlot))
        {
            // การซ้อนสำเร็จแล้วใน TryStackItems
            return;
        }
        // ถ้าไม่สามารถซ้อนได้ - สลับไอเท็ม
        else
        {
            toSlot.SetItem(fromItem);
            fromSlot.SetItem(toItem);
            
            string fromName = fromItem?.itemName ?? "ว่าง";
            string toName = toItem?.itemName ?? "ว่าง";
            
            Debug.Log($"สลับไอเท็มสำเร็จ: {fromName} ↔ {toName}");
        }
        
        // ส่ง event ให้ระบบอื่นรับรู้ (ถ้าต้องการ)
        if (DragDropManager.Instance != null)
        {
            DragDropManager.Instance.OnItemSwapped(fromSlot, toSlot);
        }
    }
    
    // ฟังก์ชันสำหรับตรวจสอบว่าสามารถลากได้หรือไม่
    public bool CanDrag()
    {
        return currentSlot != null && currentSlot.currentItem != null;
    }
    
    // ฟังก์ชันสำหรับตรวจสอบว่าสามารถวางได้หรือไม่
    public bool CanDrop(InventorySlot targetSlot)
    {
        // สามารถเพิ่มเงื่อนไขได้ที่นี่
        // เช่น ตรวจสอบประเภทไอเท็ม ข้อจำกัดต่างๆ
        return targetSlot != null;
    }
    
    // ฟังก์ชันสำหรับจัดการการวางบน EquipmentSlot
    void HandleDropOnEquipmentSlot(EquipmentSlot equipmentSlot)
    {
        if (equipmentSlot == null || currentSlot == null || currentSlot.currentItem == null) return;
        
        ItemData draggedItem = currentSlot.currentItem;
        
        // ตรวจสอบว่าสามารถสวมใส่ได้หรือไม่
        if (equipmentSlot.CanEquipItem(draggedItem))
        {
            // ถอดไอเท็มเก่าออกก่อน (ถ้ามี)
            ItemData oldItem = equipmentSlot.UnequipItem();
            
            // สวมใส่ไอเท็มใหม่
            equipmentSlot.EquipItem(draggedItem);
            
            // ลบไอเท็มจาก Inventory
            currentSlot.ClearSlot();
            
            // ถ้ามีไอเท็มเก่า ใส่กลับไปใน Inventory
            if (oldItem != null && EquipmentManager.Instance != null)
            {
                EquipmentManager.Instance.AddItemToInventory(oldItem);
            }
            
            // อัปเดตสถานะผู้เล่น
            if (EquipmentManager.Instance != null)
            {
                EquipmentManager.Instance.UpdatePlayerStats();
            }
            
            Debug.Log($"สวมใส่ {draggedItem.itemName} ที่ช่อง {equipmentSlot.slotType} สำเร็จ");
        }
        else
        {
            Debug.Log($"ไม่สามารถสวมใส่ {draggedItem.itemName} ที่ช่อง {equipmentSlot.slotType} ได้");
        }
    }
}
