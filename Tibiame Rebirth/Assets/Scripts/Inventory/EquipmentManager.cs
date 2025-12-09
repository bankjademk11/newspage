using UnityEngine;
using System.Collections.Generic;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance { get; private set; }
    
    [Header("Equipment Slots")]
    public EquipmentSlot weaponSlot;
    public EquipmentSlot helmetSlot;
    public EquipmentSlot armorSlot;
    public EquipmentSlot bootsSlot;
    public EquipmentSlot accessorySlot;
    
    [Header("Player Stats")]
    public PlayerStatsManager playerStatsManager;
    
    private InventoryManager inventoryManager;
    
    void Awake()
    {
        // Singleton Pattern
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
    }
    
    void Start()
    {
        // หา InventoryManager
        inventoryManager = FindObjectOfType<InventoryManager>();
        
        // หา PlayerStatsManager ถ้ายังไม่ได้กำหนด
        if (playerStatsManager == null)
            playerStatsManager = FindObjectOfType<PlayerStatsManager>();
            
        // อัปเดตสถานะเริ่มต้น
        UpdatePlayerStats();
        
        // บังคับปิด Equipment Panel อีกครั้งเพื่อให้แน่ใจ
        ForceCloseEquipmentPanel();
    }
    
    /// <summary>
    /// บังคับปิด Equipment Panel ให้แน่นอน
    /// </summary>
    void ForceCloseEquipmentPanel()
    {
        Debug.Log("🔒 EquipmentManager: กำลังบังคับปิดหน้าต่าง Equipment...");
        
        // หา Equipment Panel จาก Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            Transform equipmentPanel = canvas.transform.Find("EquipmentPanel");
            if (equipmentPanel != null)
            {
                // บังคับปิดทุกวิธี
                equipmentPanel.gameObject.SetActive(false);
                
                var canvasGroup = equipmentPanel.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0;
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                }
                
                Debug.Log("✅ EquipmentManager: บังคับปิดหน้าต่าง Equipment สำเร็จ");
            }
            else
            {
                Debug.LogWarning("⚠️ EquipmentManager: ไม่พบ EquipmentPanel ใน Canvas");
            }
        }
        else
        {
            Debug.LogError("❌ EquipmentManager: ไม่พบ Canvas");
        }
    }
    
    // สวมใส่ไอเท็ม
    public bool EquipItem(ItemData item)
    {
        if (item == null || !item.isEquippable) return false;
        
        EquipmentSlot targetSlot = GetEquipmentSlot(item.equipmentType);
        if (targetSlot == null) return false;
        
        // ถอดไอเท็มเก่าออกก่อน
        ItemData oldItem = targetSlot.UnequipItem();
        
        // สวมใส่ไอเท็มใหม่
        bool success = targetSlot.EquipItem(item);
        
        if (success)
        {
            // ถ้ามีไอเท็มเก่า ใส่กลับไปใน Inventory
            if (oldItem != null)
            {
                AddItemToInventory(oldItem);
            }
            
            // อัปเดตสถานะผู้เล่น
            UpdatePlayerStats();
            
            Debug.Log($"สวมใส่ {item.itemName} สำเร็จ");
        }
        
        return success;
    }
    
    // ถอดไอเท็ม
    public ItemData UnequipItem(EquipmentType equipmentType)
    {
        EquipmentSlot slot = GetEquipmentSlot(equipmentType);
        if (slot == null) return null;
        
        ItemData item = slot.UnequipItem();
        
        if (item != null)
        {
            // ใส่ไอเท็มกลับไปใน Inventory
            AddItemToInventory(item);
            
            // อัปเดตสถานะผู้เล่น
            UpdatePlayerStats();
            
            Debug.Log($"ถอด {item.itemName} สำเร็จ");
        }
        
        return item;
    }
    
    // ดูว่าสวมใส่ไอเท็มประเภทนี้อยู่หรือไม่
    public bool IsEquipped(EquipmentType equipmentType)
    {
        EquipmentSlot slot = GetEquipmentSlot(equipmentType);
        return slot != null && !slot.IsEmpty();
    }
    
    // ดูว่ามีไอเท็มนี้สวมใส่อยู่หรือไม่
    public bool IsEquipped(ItemData item)
    {
        if (item == null) return false;
        
        foreach (var slot in GetAllEquipmentSlots())
        {
            ItemData equippedItem = slot.GetEquippedItem();
            if (equippedItem == item) return true;
        }
        
        return false;
    }
    
    // หา EquipmentSlot ตามประเภท
    public EquipmentSlot GetEquipmentSlot(EquipmentType equipmentType)
    {
        switch (equipmentType)
        {
            case EquipmentType.Weapon:
                return weaponSlot;
            case EquipmentType.Helmet:
                return helmetSlot;
            case EquipmentType.Armor:
                return armorSlot;
            case EquipmentType.Boots:
                return bootsSlot;
            case EquipmentType.Accessory:
                return accessorySlot;
            default:
                return null;
        }
    }
    
    // ดึง EquipmentSlot ทั้งหมด
    public EquipmentSlot[] GetAllEquipmentSlots()
    {
        return new EquipmentSlot[]
        {
            weaponSlot, helmetSlot, armorSlot, bootsSlot, accessorySlot
        };
    }
    
    // ดึงไอเท็มที่สวมใส่ทั้งหมด
    public ItemData[] GetAllEquippedItems()
    {
        List<ItemData> equippedItems = new List<ItemData>();
        
        foreach (var slot in GetAllEquipmentSlots())
        {
            ItemData item = slot.GetEquippedItem();
            if (item != null)
                equippedItems.Add(item);
        }
        
        return equippedItems.ToArray();
    }
    
    // เพิ่มไอเท็มไปใน Inventory
    public bool AddItemToInventory(ItemData item)
    {
        if (inventoryManager != null)
        {
            return inventoryManager.AddItem(item);
        }
        else
        {
            Debug.LogWarning("ไม่พบ InventoryManager");
            return false;
        }
    }
    
    // อัปเดตสถานะผู้เล่น
    public void UpdatePlayerStats()
    {
        if (playerStatsManager != null)
        {
            playerStatsManager.UpdateStats();
        }
        else
        {
            Debug.LogWarning("ไม่พบ PlayerStatsManager");
        }
    }
    
    // ถอดไอเท็มทั้งหมด
    public void UnequipAll()
    {
        foreach (var slot in GetAllEquipmentSlots())
        {
            ItemData item = slot.UnequipItem();
            if (item != null)
            {
                AddItemToInventory(item);
            }
        }
        
        UpdatePlayerStats();
        Debug.Log("ถอดไอเท็มทั้งหมดแล้ว");
    }
    
    // ดูค่าสถานะรวมจาก Equipment
    public void GetEquipmentStats(out int totalAttack, out int totalDefense, out int totalSpeed)
    {
        totalAttack = 0;
        totalDefense = 0;
        totalSpeed = 0;
        
        foreach (var slot in GetAllEquipmentSlots())
        {
            ItemData item = slot.GetEquippedItem();
            if (item != null)
            {
                totalAttack += item.attackPower;
                totalDefense += item.defense;
                totalSpeed += item.speed;
            }
        }
    }
    
    // บันทึกข้อมูล Equipment (สำหรับ Save/Load)
    public void SaveEquipment()
    {
        // TODO: ทำระบบบันทึกข้อมูล Equipment
        Debug.Log("บันทึกข้อมูล Equipment");
    }
    
    // โหลดข้อมูล Equipment (สำหรับ Save/Load)
    public void LoadEquipment()
    {
        // TODO: ทำระบบโหลดข้อมูล Equipment
        Debug.Log("โหลดข้อมูล Equipment");
    }
}
