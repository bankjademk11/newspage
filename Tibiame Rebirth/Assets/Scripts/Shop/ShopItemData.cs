using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem", menuName = "Shop/ShopItemData")]
public class ShopItemData : ScriptableObject
{
    public string itemName;
    public string description = "";
    public Sprite icon;
    public int price = 10;
    public bool stackable = true;
    public int maxStackSize = 99;
    
    [Header("Item Properties")]
    public ItemType itemType = ItemType.Misc;
    public ItemRarity rarity = ItemRarity.Common;
    
    [Header("Stats")]
    public int healAmount = 0;
    public int attackPower = 0;
    public int defense = 0;
    public int speed = 0;
    public int requiredLevel = 1;
    public float weight = 1.0f;
}
