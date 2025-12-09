using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("ข้อมูลพื้นฐาน")]
    public string itemName;
    public string description = "";
    public Sprite icon;
    
    [Header("ประเภทและความหายาก")]
    public ItemType itemType;
    public ItemRarity rarity = ItemRarity.Common;
    
    [Header("คุณสมบัติการใช้งาน")]
    public bool isConsumable;
    public int healAmount; // สำหรับ Potion ฟื้น HP
    public int manaRestore; // สำหรับ Potion ฟื้น MP
    public bool isStackable = true; // สำหรับระบบซ้อนไอเท็ม
    public int maxStackSize = 99;
    
    [Header("ค่าสถานะ (สำหรับอุปกรณ์)")]
    public int attackPower = 0;
    public int defense = 0;
    public int speed = 0;
    
    [Header("ข้อมูล Equipment")]
    public EquipmentType equipmentType;
    public bool isEquippable;
    
    [Header("ราคา")]
    public int price = 100;
    public int sellPrice = 50;
    
    [Header("ข้อจำกัดการใช้งาน")]
    public int requiredLevel = 1;
    public int itemLevel = 1;
    public float weight = 1.0f;
    
    // จำนวนไอเท็มใน stack (สำหรับ runtime)
    [System.NonSerialized]
    public int currentStackSize = 1;
}

// Enum สำหรับประเภทไอเท็ม
public enum ItemType
{
    Consumable,    // ของใช้สิ้นเปลือง
    Weapon,        // อาวุธ
    Armor,         // เกราะ
    Accessory,     // เครื่องประดับ
    Material,      // วัตถุดิบ
    Quest,         // ของเควส
    Misc           // อื่นๆ
}

// Enum สำหรับความหายาก
public enum ItemRarity
{
    Common,    // สีเทา - ธรรมดา
    Uncommon,  // สีเขียว - ไม่ธรรมดา
    Rare,      // สีน้ำเงิน - หายาก
    Epic,      // สีม่วง - หายากมาก
    Legendary  // สีส้ม/ทอง - ตำนาน
}

// Enum สำหรับประเภท Equipment
public enum EquipmentType
{
    None,       // ไม่ใช่ Equipment
    Weapon,     // อาวุธ
    Helmet,     // หมวก
    Armor,      // เกราะ
    Boots,      // รองเท้า
    Accessory   // เครื่องประดับ
}
