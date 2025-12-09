using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ShopManager : MonoBehaviour
{
    [Header("Shop Items")]
    public List<ShopItemData> shopItems = new List<ShopItemData>();
    
    [Header("Shop Settings")]
    public int startingGold = 1000;
    public bool autoLoadItems = false;
    
    private int playerGold;
    
    void Start()
    {
        Debug.Log("=== ShopManager Start() ===");
        playerGold = startingGold;
        
        // โหลดไอเท็มทั้งหมดก่อน
        LoadShopItems();
        
        // รอให้ UI สร้างเสร็จก่อนค่อย populate แต่ไม่เปิดร้านค้า
        StartCoroutine(DelayedPopulateShop());
        
        Debug.Log("=== ShopManager Start() จบ ===");
    }
    
    System.Collections.IEnumerator DelayedPopulateShop()
    {
        // รอ 1 frame ให้ UI สร้างเสร็จ
        yield return null;
        
        Debug.Log("🏪 เริ่ม PopulateShop แต่ไม่เปิดร้านค้า");
        PopulateShop();
        
        // บังคับปิดร้านค้าอีกครั้งเพื่อให้แน่ใจ
        ForceCloseShop();
        
        Debug.Log("🏪 PopulateShop เสร็จสิ้น ร้านค้าถูกบังคับปิดแล้ว");
    }
    
    /// <summary>
    /// บังคับปิดร้านค้าให้แน่นอน
    /// </summary>
    void ForceCloseShop()
    {
        Debug.Log("🔒 กำลังบังคับปิดร้านค้า...");
        
        // หา ShopGenerator และบังคับปิด
        ShopGenerator generator = FindObjectOfType<ShopGenerator>();
        if (generator != null)
        {
            generator.CloseShopPanel();
            Debug.Log("✅ ใช้ ShopGenerator.CloseShopPanel() ปิดร้านค้าแล้ว");
        }
        else
        {
            Debug.LogWarning("⚠️ ไม่พบ ShopGenerator จะปิดร้านค้าด้วยวิธีอื่น");
            
            // หาจาก Canvas โดยตรง
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                Transform shopPanel = canvas.transform.Find("AutoShopUI");
                if (shopPanel != null)
                {
                    // บังคับปิดทุกวิธี
                    shopPanel.gameObject.SetActive(false);
                    
                    var canvasGroup = shopPanel.GetComponent<CanvasGroup>();
                    if (canvasGroup != null)
                    {
                        canvasGroup.alpha = 0;
                        canvasGroup.interactable = false;
                        canvasGroup.blocksRaycasts = false;
                    }
                    
                    Debug.Log("✅ บังคับปิดร้านค้าจาก Canvas สำเร็จ");
                }
                else
                {
                    Debug.LogError("❌ ไม่พบ AutoShopUI ใน Canvas");
                }
            }
            else
            {
                Debug.LogError("❌ ไม่พบ Canvas");
            }
        }
    }
    
    void LoadShopItems()
    {
        // ถ้าไม่ได้เปิดการโหลดอัตโนมัติ จะไม่ทำอะไร
        if (!autoLoadItems)
        {
            Debug.Log("ปิดการโหลดไอเท็มอัตโนมัติ - รอการตั้งค่าจาก Inspector");
            return;
        }
        
        // ถ้ามีการตั้งค่าไอเท็มไว้แล้ว ไม่ต้องโหลดอัตโนมัติ
        if (shopItems.Count > 0)
        {
            Debug.Log($"ใช้ไอเท็มที่ตั้งค่าไว้ {shopItems.Count} ชิ้น");
            return;
        }
        
        // โหลด ShopItemData ทั้งหมดจาก Assets/Items โดยตรง
        string[] guids = AssetDatabase.FindAssets("t:ShopItemData", new[] { "Assets/Items" });
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ShopItemData item = AssetDatabase.LoadAssetAtPath<ShopItemData>(path);
            if (item != null)
            {
                shopItems.Add(item);
            }
        }
        
        if (shopItems.Count > 0)
        {
            Debug.Log($"โหลดไอเท็มร้านค้า {shopItems.Count} ชิ้นจาก Assets/Items");
        }
        else
        {
            Debug.LogWarning("ไม่พบ ShopItemData ในโฟลเดอร์ Assets/Items");
        }
    }
    
    void PopulateShop()
    {
        // หา ShopSlot ทั้งหมด
        ShopSlot[] slots = FindObjectsOfType<ShopSlot>();
        
        if (slots.Length == 0)
        {
            Debug.LogWarning("ไม่พบ ShopSlot ในฉาก");
            return;
        }
        
        // ใส่ไอเท็มในช่อง
        for (int i = 0; i < Mathf.Min(shopItems.Count, slots.Length); i++)
        {
            slots[i].SetItem(shopItems[i]);
        }
        
        Debug.Log($"ใส่ไอเท็มในร้านค้า {Mathf.Min(shopItems.Count, slots.Length)} ชิ้น");
    }
    
    public bool TryPurchaseItem(ShopItemData item)
    {
        Debug.Log("=== TryPurchaseItem() เริ่มทำงาน ===");
        
        if (item == null)
        {
            Debug.LogError("❌ ไอเท็มเป็น null");
            return false;
        }
        
        Debug.Log($"ตรวจสอบการซื้อ: {item.itemName} ราคา {item.price} เงินปัจจุบัน {playerGold}");
        
        if (playerGold >= item.price)
        {
            Debug.Log("✅ เงินพอ กำลังทำการซื้อ...");
            
            // หักเงิน
            playerGold -= item.price;
            Debug.Log($"หักเงินแล้ว เหลือ: {playerGold}");
            
            // เพิ่มไอเท็มใน Inventory
            InventoryManager inventory = FindObjectOfType<InventoryManager>();
            if (inventory != null)
            {
                Debug.Log("พบ InventoryManager แล้ว");
                
                // สร้าง ItemData จาก ShopItemData
                ItemData newItem = CreateItemDataFromShopItem(item);
                if (newItem != null)
                {
                    Debug.Log($"สร้าง ItemData '{newItem.itemName}' สำเร็จ");
                    
                    bool added = inventory.AddItem(newItem, 1);
                    if (added)
                    {
                        Debug.Log($"✅ เพิ่มไอเท็มลง Inventory สำเร็จ! ซื้อ {item.itemName} สำเร็จ! เหลือเงิน: {playerGold}");
                        return true;
                    }
                    else
                    {
                        Debug.LogError("❌ เพิ่มไอเท็มลง Inventory ไม่สำเร็จ");
                        playerGold += item.price; // คืนเงิน
                        return false;
                    }
                }
                else
                {
                    Debug.LogError("❌ ไม่สามารถสร้าง ItemData จาก ShopItemData ได้");
                    playerGold += item.price; // คืนเงิน
                    return false;
                }
            }
            else
            {
                Debug.LogError("❌ ไม่พบ InventoryManager");
                playerGold += item.price; // คืนเงิน
                return false;
            }
        }
        else
        {
            Debug.Log($"❌ เงินไม่พอ! ต้องการ {item.price} มี {playerGold}");
            return false;
        }
    }
    
    public int GetPlayerGold()
    {
        return playerGold;
    }
    
    public void AddGold(int amount)
    {
        playerGold += amount;
        Debug.Log($"เพิ่มเงิน {amount} เหลือทั้งหมด: {playerGold}");
    }
    
    public void SetPlayerGold(int amount)
    {
        playerGold = amount;
        Debug.Log($"ตั้งค่าเงินเป็น: {playerGold}");
    }
    
    /// <summary>
    /// สร้าง ItemData จาก ShopItemData
    /// </summary>
    ItemData CreateItemDataFromShopItem(ShopItemData shopItem)
    {
        if (shopItem == null) return null;
        
        // สร้าง ItemData ใหม่
        ItemData newItem = ScriptableObject.CreateInstance<ItemData>();
        newItem.itemName = shopItem.itemName;
        newItem.description = shopItem.description ?? "";
        newItem.icon = shopItem.icon;
        newItem.isStackable = shopItem.stackable;
        newItem.maxStackSize = shopItem.maxStackSize > 0 ? shopItem.maxStackSize : 99;
        newItem.currentStackSize = 1;
        
        // กำหนดค่าเริ่มต้นอื่นๆ
        newItem.itemType = ItemType.Consumable; // ถือว่าเป็น Consumable โดยค่าเริ่มต้น
        newItem.rarity = ItemRarity.Common;
        newItem.isEquippable = false;
        newItem.weight = 1.0f;
        
        // ตรวจสอบจากชื่อไอเท็มเพื่อกำหนดประเภท
        if (shopItem.itemName.ToLower().Contains("sword") || shopItem.itemName.ToLower().Contains("weapon"))
        {
            newItem.itemType = ItemType.Weapon;
            newItem.isEquippable = true;
        }
        else if (shopItem.itemName.ToLower().Contains("armor") || shopItem.itemName.ToLower().Contains("helmet") || shopItem.itemName.ToLower().Contains("boots"))
        {
            newItem.itemType = ItemType.Armor;
            newItem.isEquippable = true;
        }
        else if (shopItem.itemName.ToLower().Contains("ring") || shopItem.itemName.ToLower().Contains("amulet"))
        {
            newItem.itemType = ItemType.Accessory;
            newItem.isEquippable = true;
        }
        
        Debug.Log($"สร้าง ItemData '{newItem.itemName}' ประเภท {newItem.itemType}");
        return newItem;
    }
}
