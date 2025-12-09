using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Equipment Slot Info")]
    public EquipmentType slotType;
    public Image icon;
    public Image backgroundImage;
    
    [Header("Visual Settings")]
    public Color emptyColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
    public Color equippedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    
    [Header("Drag Settings")]
    public Canvas canvas;
    public float draggedAlpha = 0.8f;
    public float draggedScale = 1.2f;
    
    private ItemData equippedItem;
    private Color originalBackgroundColor;
    private GameObject draggedItem;
    private Image draggedImage;
    private Vector3 originalPosition;
    
    void Start()
    {
        // หา Component ถ้ายังไม่ได้กำหนด
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();
            
        if (icon == null)
        {
            // หา Icon ใน child
            Transform iconTransform = transform.Find("Icon");
            if (iconTransform != null)
                icon = iconTransform.GetComponent<Image>();
        }
        
        // หา Canvas อัตโนมัติถ้าไม่ได้ตั้งค่า
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
        
        // เก็บสีเริ่มต้น
        if (backgroundImage != null)
            originalBackgroundColor = backgroundImage.color;
            
        // ตั้งค่าช่องว่าง
        UpdateSlotVisual();
    }
    
    public bool CanEquipItem(ItemData item)
    {
        if (item == null || !item.isEquippable) return false;
        return item.equipmentType == slotType;
    }
    
    public bool EquipItem(ItemData item)
    {
        if (!CanEquipItem(item)) return false;
        
        equippedItem = item;
        UpdateSlotVisual();
        
        Debug.Log($"สวมใส่ {item.itemName} ที่ช่อง {slotType}");
        return true;
    }
    
    public ItemData UnequipItem()
    {
        ItemData item = equippedItem;
        equippedItem = null;
        UpdateSlotVisual();
        
        if (item != null)
            Debug.Log($"ถอด {item.itemName} จากช่อง {slotType}");
            
        return item;
    }
    
    public ItemData GetEquippedItem()
    {
        return equippedItem;
    }
    
    public bool IsEmpty()
    {
        return equippedItem == null;
    }
    
    void UpdateSlotVisual()
    {
        if (icon != null)
        {
            if (equippedItem != null)
            {
                icon.sprite = equippedItem.icon;
                icon.enabled = true;
                icon.color = Color.white;
            }
            else
            {
                icon.sprite = null;
                icon.enabled = false;
            }
        }
        
        if (backgroundImage != null)
        {
            backgroundImage.color = equippedItem != null ? equippedColor : emptyColor;
        }
    }
    
    // Drag & Drop
    public void OnDrop(PointerEventData eventData)
    {
        DragDropHandler draggedHandler = eventData.pointerDrag?.GetComponent<DragDropHandler>();
        
        if (draggedHandler != null && draggedHandler.currentSlot != null)
        {
            ItemData draggedItem = draggedHandler.currentSlot.currentItem;
            
            if (CanEquipItem(draggedItem))
            {
                // ถอดไอเท็มเก่าออกก่อน (ถ้ามี)
                ItemData oldItem = UnequipItem();
                
                // สวมใส่ไอเท็มใหม่
                EquipItem(draggedItem);
                
                // ลบไอเท็มจาก Inventory
                draggedHandler.currentSlot.ClearSlot();
                
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
            }
            else
            {
                Debug.Log($"ไม่สามารถสวมใส่ {draggedItem?.itemName} ที่ช่อง {slotType} ได้");
            }
        }
    }
    
    // Drag from Equipment Slot
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (equippedItem == null) return;
        
        originalPosition = transform.position;
        
        // สร้างไอเท็มที่กำลังลาก
        draggedItem = new GameObject("DraggedEquipmentItem");
        draggedItem.transform.SetParent(canvas.transform, false);
        
        draggedImage = draggedItem.AddComponent<Image>();
        draggedImage.sprite = equippedItem.icon;
        draggedImage.raycastTarget = false;
        draggedImage.color = new Color(1, 1, 1, draggedAlpha);
        
        RectTransform draggedRect = draggedItem.GetComponent<RectTransform>();
        draggedRect.sizeDelta = new Vector2(80, 80);
        draggedRect.localScale = Vector3.one * draggedScale;
        
        // ตั้งตำแหน่งเริ่มต้น
        draggedItem.transform.position = Input.mousePosition;
        
        // ทำให้ไอคอนจางลง
        if (icon != null)
        {
            icon.color = new Color(1, 1, 1, 0.5f);
        }
        
        Debug.Log($"เริ่มลากอุปกรณ์: {equippedItem.itemName} จากช่อง {slotType}");
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
        if (icon != null)
        {
            icon.color = Color.white;
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
            if (targetInventorySlot != null)
            {
                HandleDropOnInventorySlot(targetInventorySlot);
                droppedSuccessfully = true;
            }
        }
        
        if (!droppedSuccessfully)
        {
            // วางไม่สำเร็จ กลับที่เดิม
            Debug.Log("วางอุปกรณ์ไม่สำเร็จ - กลับที่เดิม");
        }
        
        // ทำลายไอเท็มที่ลาก
        Destroy(draggedItem);
        draggedItem = null;
        draggedImage = null;
    }
    
    void HandleDropOnInventorySlot(InventorySlot targetSlot)
    {
        if (targetSlot == null || equippedItem == null) return;
        
        // ถ้าช่องเป้าหมายว่าง - ย้ายไอเท็ม
        if (targetSlot.currentItem == null)
        {
            // สร้าง instance ใหม่สำหรับ inventory
            ItemData newItem = ScriptableObject.Instantiate(equippedItem);
            newItem.currentStackSize = 1;
            
            targetSlot.SetItem(newItem);
            
            // ถอดไอเท็มออกจาก Equipment
            UnequipItem();
            
            // อัปเดตสถานะผู้เล่น
            if (EquipmentManager.Instance != null)
            {
                EquipmentManager.Instance.UpdatePlayerStats();
            }
            
            Debug.Log($"ถอด {newItem.itemName} และใส่ลงกระเป๋าสำเร็จ");
        }
        // ถ้าช่องเป้าหมายมีไอเท็ม - ลองซ้อนก่อน
        else if (targetSlot.currentItem.itemName == equippedItem.itemName && 
                 targetSlot.currentItem.rarity == equippedItem.rarity &&
                 targetSlot.currentItem.isStackable)
        {
            // ถ้าเป็นไอเท็มเดียวกันและซ้อนได้
            if (targetSlot.currentItem.currentStackSize < targetSlot.currentItem.maxStackSize)
            {
                targetSlot.currentItem.currentStackSize++;
                targetSlot.UpdateStackDisplay();
                
                // ถอดไอเท็มออกจาก Equipment
                UnequipItem();
                
                // อัปเดตสถานะผู้เล่น
                if (EquipmentManager.Instance != null)
                {
                    EquipmentManager.Instance.UpdatePlayerStats();
                }
                
                Debug.Log($"ถอด {equippedItem.itemName} และซ้อนในกระเป๋าสำเร็จ");
            }
            else
            {
                Debug.Log("ช่องเป้าหมายเต็มแล้ว ไม่สามารถซ้อนได้");
            }
        }
        else
        {
            Debug.Log($"ไม่สามารถวาง {equippedItem.itemName} บน {targetSlot.currentItem.itemName} ได้");
        }
    }
    
    // Tooltip
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (equippedItem != null && TooltipManager.Instance != null)
        {
            TooltipManager.Instance.ShowTooltip(equippedItem, Input.mousePosition);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
        }
    }
}
