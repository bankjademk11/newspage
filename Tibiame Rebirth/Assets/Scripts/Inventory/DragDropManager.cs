using UnityEngine;
using System.Collections.Generic;

public class DragDropManager : MonoBehaviour
{
    public static DragDropManager Instance { get; private set; }
    
    [Header("การตั้งค่า Drag & Drop")]
    public bool enableDebugLog = true;
    public bool enableSoundEffects = false;
    public AudioClip dragSound;
    public AudioClip dropSound;
    public AudioClip swapSound;
    
    [Header("ข้อจำกัดไอเท็ม")]
    public bool allowSameTypeStack = false;
    public int maxStackSize = 99;
    
    private AudioSource audioSource;
    private Dictionary<InventorySlot, string> inventoryTypes = new Dictionary<InventorySlot, string>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // ย้าย GameObject ไปเป็น root ก่อนใช้ DontDestroyOnLoad
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }
    
    // ลงทะเบียน inventory type
    public void RegisterInventorySlot(InventorySlot slot, string inventoryType)
    {
        if (slot != null && !string.IsNullOrEmpty(inventoryType))
        {
            inventoryTypes[slot] = inventoryType;
            
            if (enableDebugLog)
                Debug.Log($"ลงทะเบียนช่อง {slot.name} ใน {inventoryType}");
        }
    }
    
    // เรียกเมื่อมีการสลับไอเท็ม
    public void OnItemSwapped(InventorySlot fromSlot, InventorySlot toSlot)
    {
        if (enableDebugLog)
        {
            string fromType = GetInventoryType(fromSlot);
            string toType = GetInventoryType(toSlot);
            
            Debug.Log($"สลับไอเท็ม: {fromType} → {toType}");
        }
        
        // เล่นเสียง
        if (enableSoundEffects && swapSound != null)
        {
            audioSource.PlayOneShot(swapSound);
        }
        
        // ส่ง event ให้ระบบอื่น (ถ้าต้องการ)
        // EventManager.Instance.OnInventoryChanged();
    }
    
    // ตรวจสอบว่าสามารถลากไปที่ช่องนั้นได้หรือไม่
    public bool CanDropItem(InventorySlot fromSlot, InventorySlot toSlot)
    {
        if (fromSlot == null || toSlot == null) return false;
        
        // ไม่สามารถลากไปช่องเดิว
        if (fromSlot == toSlot) return false;
        
        // ตรวจสอบข้อจำกัดอื่นๆ
        string fromType = GetInventoryType(fromSlot);
        string toType = GetInventoryType(toSlot);
        
        // สามารถเพิ่มเงื่อนไขพิเศษได้ที่นี่
        // เช่น: บางไอเท็มไม่สามารถไปอยู่ใน quickslot ได้
        // if (toType == "Quickslot" && fromSlot.currentItem.itemType == ItemType.Equipment) return false;
        
        return true;
    }
    
    // จัดการการซ้อนไอเท็ม (ถ้าเปิดใช้)
    public bool TryStackItems(InventorySlot fromSlot, InventorySlot toSlot)
    {
        if (!allowSameTypeStack) return false;
        
        ItemData fromItem = fromSlot.currentItem;
        ItemData toItem = toSlot.currentItem;
        
        // ตรวจสอบว่าเป็นไอเท็มชนิดเดียวกันและสามารถซ้อนได้
        if (fromItem != null && toItem != null && 
            fromItem.itemName == toItem.itemName && 
            fromItem.rarity == toItem.rarity &&
            toItem.isStackable)
        {
            // คำนวณจำนวนที่จะซ้อนได้
            int availableSpace = toItem.maxStackSize - toItem.currentStackSize;
            
            if (availableSpace >= fromItem.currentStackSize)
            {
                // ซ้อนได้ทั้งหมด
                toItem.currentStackSize += fromItem.currentStackSize;
                fromSlot.ClearSlot();
                
                // อัปเดตการแสดงผล
                toSlot.UpdateStackDisplay();
                
                if (enableDebugLog)
                    Debug.Log($"ซ้อนไอเท็มสำเร็จ: {fromItem.itemName} +{fromItem.currentStackSize} (ทั้งหมด: {toItem.currentStackSize})");
                
                return true;
            }
            else if (availableSpace > 0)
            {
                // ซ้อนได้บางส่วน
                toItem.currentStackSize = toItem.maxStackSize;
                fromItem.currentStackSize -= availableSpace;
                
                // อัปเดตการแสดงผล
                toSlot.UpdateStackDisplay();
                fromSlot.UpdateStackDisplay();
                
                if (enableDebugLog)
                    Debug.Log($"ซ้อนไอเท็มบางส่วน: {fromItem.itemName} +{availableSpace} (เต็มแล้ว, เหลือ {fromItem.currentStackSize})");
                
                return true;
            }
            else
            {
                // ช่องเป้าหมายเต็มแล้ว
                if (enableDebugLog)
                    Debug.Log($"ช่องเป้าหมายเต็ม: {toItem.itemName} ({toItem.currentStackSize}/{toItem.maxStackSize})");
                
                return false;
            }
        }
        
        return false;
    }
    
    // จัดการการแยก stack (Shift + คลิกขวา)
    public bool TrySplitStack(InventorySlot slot, int splitAmount = 1)
    {
        if (slot == null || slot.currentItem == null || !slot.currentItem.isStackable)
            return false;
            
        if (slot.currentItem.currentStackSize <= 1 || splitAmount >= slot.currentItem.currentStackSize)
            return false;
            
        // หาช่องว่างสำหรับไอเท็มที่แยกออก
        InventorySlot[] allSlots = FindObjectsOfType<InventorySlot>();
        
        foreach (InventorySlot emptySlot in allSlots)
        {
            if (emptySlot != slot && emptySlot.currentItem == null)
            {
                // แยก stack
                ItemData splitItem = slot.SplitStack(splitAmount);
                if (splitItem != null)
                {
                    emptySlot.SetItem(splitItem);
                    
                    if (enableDebugLog)
                        Debug.Log($"แยกไอเท็ม: {splitItem.itemName} x{splitAmount} ไปช่องว่าง");
                    
                    return true;
                }
            }
        }
        
        if (enableDebugLog)
            Debug.Log("ไม่มีช่องว่างสำหรับแยกไอเท็ม");
            
        return false;
    }
    
    // เรียกเมื่อเริ่มลาก
    public void OnDragStarted(InventorySlot slot)
    {
        if (enableDebugLog)
            Debug.Log($"เริ่มลาก: {slot.currentItem?.itemName}");
        
        if (enableSoundEffects && dragSound != null)
        {
            audioSource.PlayOneShot(dragSound);
        }
    }
    
    // เรียกเมื่อวางไอเท็ม
    public void OnItemDropped(InventorySlot slot)
    {
        if (enableDebugLog)
            Debug.Log($"วางไอเท็ม: {slot.currentItem?.itemName}");
        
        if (enableSoundEffects && dropSound != null)
        {
            audioSource.PlayOneShot(dropSound);
        }
    }
    
    // ดึงข้อมูลประเภทของ inventory
    private string GetInventoryType(InventorySlot slot)
    {
        if (inventoryTypes.ContainsKey(slot))
            return inventoryTypes[slot];
        
        // ตรวจสอบจากชื่อ parent
        Transform parent = slot.transform.parent;
        while (parent != null)
        {
            if (parent.name.Contains("Main"))
                return "MainInventory";
            else if (parent.name.Contains("Short"))
                return "ShortInventory";
            else if (parent.name.Contains("Quick"))
                return "Quickslot";
            
            parent = parent.parent;
        }
        
        return "Unknown";
    }
    
    // ล้างข้อมูลทั้งหมด
    public void ClearAllRegistrations()
    {
        inventoryTypes.Clear();
        
        if (enableDebugLog)
            Debug.Log("ล้างการลงทะเบียน inventory ทั้งหมดแล้ว");
    }
    
    // แสดงสถิติการใช้งาน
    [ContextMenu("Show Inventory Stats")]
    public void ShowInventoryStats()
    {
        Debug.Log("=== Inventory Stats ===");
        
        Dictionary<string, int> typeCount = new Dictionary<string, int>();
        foreach (var kvp in inventoryTypes)
        {
            string type = kvp.Value;
            if (typeCount.ContainsKey(type))
                typeCount[type]++;
            else
                typeCount[type] = 0;
        }
        
        foreach (var kvp in typeCount)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value} ช่อง");
        }
        
        Debug.Log($"Total: {inventoryTypes.Count} ช่อง");
    }
}
