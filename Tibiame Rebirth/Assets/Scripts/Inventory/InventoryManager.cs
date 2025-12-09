using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public ItemData[] testItems;
    public InventorySlot[] slots;

    private void Start()
    {
        // ใส่ไอเท็มทดสอบตอนเริ่มเกม
        for (int i = 0; i < slots.Length && i < testItems.Length; i++)
        {
            if (slots[i] != null && testItems[i] != null)
            {
                // สร้าง instance ใหม่เพื่อไม่ให้มันแชร์ข้อมูล
                ItemData newItem = ScriptableObject.Instantiate(testItems[i]);
                newItem.currentStackSize = 1;
                slots[i].SetItem(newItem);
            }
        }
    }
    
    // เพิ่มไอเท็มเข้ากระเป๋า (รองรับการซ้อน)
    public bool AddItem(ItemData item, int amount = 1)
    {
        Debug.Log("=== InventoryManager.AddItem() เริ่มทำงาน ===");
        Debug.Log($"พยายามเพิ่ม: {item?.itemName} x{amount}");
        
        if (item == null) 
        {
            Debug.LogError("❌ ไอเท็มเป็น null!");
            return false;
        }
        
        if (slots == null || slots.Length == 0)
        {
            Debug.LogError("❌ slots เป็น null หรือว่าง!");
            return false;
        }
        
        Debug.Log($"มีช่องทั้งหมด {slots.Length} ช่อง");
        
        // ถ้าเป็นไอเท็มที่ซ้อนกันได้ ลองเพิ่มลงช่องเดิมก่อน
        if (item.isStackable)
        {
            Debug.Log($"ไอเท็ม {item.itemName} สามารถซ้อนกันได้");
            
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != null && slots[i].currentItem != null && 
                    slots[i].currentItem.itemName == item.itemName && 
                    slots[i].currentItem.rarity == item.rarity)
                {
                    Debug.Log($"พบช่องเดิมที่ {i + 1} มี {slots[i].currentItem.currentStackSize}/{slots[i].currentItem.maxStackSize}");
                    
                    // ลองเพิ่มจำนวนในช่องเดิม
                    if (slots[i].AddToStack(amount))
                    {
                        Debug.Log($"✅ เพิ่ม {item.itemName} +{amount} ในช่องที่ {i + 1} (ทั้งหมด: {slots[i].currentItem.currentStackSize})");
                        return true;
                    }
                    else if (slots[i].currentItem.currentStackSize < slots[i].currentItem.maxStackSize)
                    {
                        // เพิ่มได้บางส่วน
                        int canAdd = slots[i].currentItem.maxStackSize - slots[i].currentItem.currentStackSize;
                        slots[i].AddToStack(canAdd);
                        amount -= canAdd;
                        
                        if (amount <= 0)
                        {
                            Debug.Log($"✅ เพิ่ม {item.itemName} ในช่องที่ {i + 1} (ทั้งหมด: {slots[i].currentItem.currentStackSize})");
                            return true;
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log($"ไอเท็ม {item.itemName} ไม่สามารถซ้อนกันได้");
        }
        
        // หาช่องว่างสำหรับไอเท็มใหม่
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].currentItem == null)
            {
                Debug.Log($"พบช่องว่างที่ {i + 1}");
                
                // สร้าง instance ใหม่
                ItemData newItem = ScriptableObject.Instantiate(item);
                newItem.currentStackSize = amount;
                slots[i].SetItem(newItem);
                
                Debug.Log($"✅ เพิ่ม {item.itemName} x{amount} ลงช่องที่ {i + 1}");
                return true;
            }
        }
        
        // ไม่มีช่องว่าง
        Debug.LogError($"❌ กระเป๋าเต็ม! ไม่สามารถเก็บ {item.itemName} x{amount}");
        return false;
    }
    
    // ตรวจสอบว่ามีไอเท็มนี้ในกระเป๋าหรือไม่
    public bool HasItem(ItemData item, int requiredAmount = 1)
    {
        if (item == null) return false;
        
        int totalAmount = 0;
        
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].currentItem != null && 
                slots[i].currentItem.itemName == item.itemName && 
                slots[i].currentItem.rarity == item.rarity)
            {
                totalAmount += slots[i].currentItem.currentStackSize;
                
                if (totalAmount >= requiredAmount)
                    return true;
            }
        }
        
        return false;
    }
    
    // ลบไอเท็มจากกระเป๋า
    public bool RemoveItem(ItemData item, int amount = 1)
    {
        if (item == null) return false;
        
        // หาไอเท็มที่ตรงกันและลบ
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].currentItem != null && 
                slots[i].currentItem.itemName == item.itemName && 
                slots[i].currentItem.rarity == item.rarity)
            {
                if (slots[i].currentItem.currentStackSize >= amount)
                {
                    // ลดจำนวนในช่องเดิม
                    slots[i].currentItem.currentStackSize -= amount;
                    
                    if (slots[i].currentItem.currentStackSize <= 0)
                    {
                        slots[i].ClearSlot();
                    }
                    else
                    {
                        slots[i].UpdateStackDisplay();
                    }
                    
                    Debug.Log($"ลบ {item.itemName} x{amount} จากช่องที่ {i + 1}");
                    return true;
                }
                else
                {
                    // ลบทั้งช่องแล้วไปหาช่องอื่นต่อ
                    amount -= slots[i].currentItem.currentStackSize;
                    slots[i].ClearSlot();
                }
            }
        }
        
        Debug.Log($"ไม่สามารถลบ {item.itemName} x{amount} ได้ (ไม่มีพอ)");
        return false;
    }
    
    // นับจำนวนไอเท็มทั้งหมดในกระเป๋า
    public int GetItemCount(ItemData item)
    {
        if (item == null) return 0;
        
        int totalCount = 0;
        
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].currentItem != null && 
                slots[i].currentItem.itemName == item.itemName && 
                slots[i].currentItem.rarity == item.rarity)
            {
                totalCount += slots[i].currentItem.currentStackSize;
            }
        }
        
        return totalCount;
    }
    
    // นับช่องว่างทั้งหมด
    public int GetEmptySlotCount()
    {
        int emptyCount = 0;
        
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].currentItem == null)
            {
                emptyCount++;
            }
        }
        
        return emptyCount;
    }
    
    /// <summary>
    /// ตรวจสอบว่ามีที่ว่างสำหรับไอเท็มหรือไม่
    /// </summary>
    public bool HasSpaceForItem(ItemData item, int amount = 1)
    {
        if (item == null) return false;
        
        // ถ้าเป็นไอเท็มที่ซ้อนกันได้ ตรวจสอบช่องเดิมก่อน
        if (item.isStackable)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != null && slots[i].currentItem != null && 
                    slots[i].currentItem.itemName == item.itemName && 
                    slots[i].currentItem.rarity == item.rarity)
                {
                    int availableSpace = slots[i].currentItem.maxStackSize - slots[i].currentItem.currentStackSize;
                    if (availableSpace > 0)
                    {
                        amount -= availableSpace;
                        if (amount <= 0)
                            return true;
                    }
                }
            }
        }
        
        // ตรวจสอบช่องว่างสำหรับไอเท็มใหม่
        int emptySlots = GetEmptySlotCount();
        int neededSlots = Mathf.CeilToInt((float)amount / item.maxStackSize);
        
        return emptySlots >= neededSlots;
    }
    
    /// <summary>
    /// คืนค่ารายการไอเท็มทั้งหมดในกระเป๋า
    /// </summary>
    public ItemData[] GetAllItems()
    {
        System.Collections.Generic.List<ItemData> allItems = new System.Collections.Generic.List<ItemData>();
        
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].currentItem != null)
            {
                allItems.Add(slots[i].currentItem);
            }
        }
        
        return allItems.ToArray();
    }
}
