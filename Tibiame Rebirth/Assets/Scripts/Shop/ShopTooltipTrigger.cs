using UnityEngine;
using UnityEngine.EventSystems;

public class ShopTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ShopSlot shopSlot;
    
    void Awake()
    {
        // หา ShopSlot component ถ้ายังไม่มี
        if (shopSlot == null)
            shopSlot = GetComponent<ShopSlot>();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (shopSlot != null && shopSlot.currentItem != null)
        {
            // แปลง ShopItemData เป็น ItemData สำหรับแสดง tooltip
            ItemData itemData = CreateItemDataFromShopItem(shopSlot.currentItem);
            
            // แสดง tooltip
            TooltipManager.Instance.ShowTooltip(itemData, transform.position);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.HideTooltip();
    }
    
    private ItemData CreateItemDataFromShopItem(ShopItemData shopItem)
    {
        // สร้าง ItemData ชั่วคราวจาก ShopItemData
        ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
        
        itemData.itemName = shopItem.itemName;
        itemData.description = shopItem.description;
        itemData.itemType = shopItem.itemType;
        itemData.rarity = shopItem.rarity;
        itemData.price = shopItem.price;
        itemData.isStackable = shopItem.stackable;
        itemData.maxStackSize = shopItem.maxStackSize;
        itemData.currentStackSize = shopItem.maxStackSize;
        
        // ค่าสถานะพื้นฐาน
        itemData.healAmount = shopItem.healAmount;
        itemData.attackPower = shopItem.attackPower;
        itemData.defense = shopItem.defense;
        itemData.speed = shopItem.speed;
        itemData.requiredLevel = shopItem.requiredLevel;
        itemData.weight = shopItem.weight;
        
        return itemData;
    }
}
