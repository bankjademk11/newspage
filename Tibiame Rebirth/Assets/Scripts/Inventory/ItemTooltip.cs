using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTooltip : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI typeText;
    public TextMeshProUGUI rarityText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI requirementsText;
    public TextMeshProUGUI stackText;
    
    [Header("Rarity Colors")]
    public Color commonColor = Color.gray;
    public Color uncommonColor = Color.green;
    public Color rareColor = Color.blue;
    public Color epicColor = new Color(0.6f, 0.2f, 0.8f);
    public Color legendaryColor = new Color(1.0f, 0.6f, 0.0f);
    
    [Header("Layout")]
    public float padding = 10f;
    public float maxWidth = 300f;
    
    private Canvas canvas;
    private RectTransform rectTransform;
    private ContentSizeFitter contentSizeFitter;

    void Awake()
    {
        // หา Component
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        contentSizeFitter = GetComponent<ContentSizeFitter>();
        
        // ถ้าไม่มี ContentSizeFitter ให้เพิ่ม
        if (contentSizeFitter == null)
            contentSizeFitter = gameObject.AddComponent<ContentSizeFitter>();
        
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        // ซ่อน tooltip ตอนเริ่ม
        gameObject.SetActive(false);
    }

    public void ShowTooltip(ItemData item, Vector3 slotPosition)
    {
        if (item == null) return;
        
        gameObject.SetActive(true);
        
        // แสดงข้อมูลไอเท็ม
        DisplayItemInfo(item);
        
        // ปรับตำแหน่ง
        PositionTooltip(slotPosition);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    private void DisplayItemInfo(ItemData item)
    {
        // ชื่อไอเท็มและสีตาม rarity
        if (itemNameText != null)
        {
            itemNameText.text = item.itemName;
            itemNameText.color = GetRarityColor(item.rarity);
        }
        
        // คำอธิบาย
        if (descriptionText != null)
        {
            descriptionText.text = !string.IsNullOrEmpty(item.description) ? item.description : "No description";
        }
        
        // ประเภทไอเท็ม
        if (typeText != null)
        {
            typeText.text = $"Type: {GetItemTypeDisplayName(item.itemType)}";
        }
        
        // ความหายาก
        if (rarityText != null)
        {
            rarityText.text = $"Rarity: {GetRarityDisplayName(item.rarity)}";
            rarityText.color = GetRarityColor(item.rarity);
        }
        
        // ค่าสถานะ
        if (statsText != null)
        {
            string stats = GetStatsDisplay(item);
            statsText.text = !string.IsNullOrEmpty(stats) ? stats : "No special stats";
            statsText.gameObject.SetActive(!string.IsNullOrEmpty(stats));
        }
        
        // ข้อจำกัด
        if (requirementsText != null)
        {
            string requirements = GetRequirementsDisplay(item);
            requirementsText.text = !string.IsNullOrEmpty(requirements) ? requirements : "";
            requirementsText.gameObject.SetActive(!string.IsNullOrEmpty(requirements));
        }
        
        // จำนวน (สำหรับไอเท็มที่ซ้อนกันได้)
        if (stackText != null)
        {
            if (item.isStackable && item.currentStackSize > 1)
            {
                stackText.text = $"Stack: {item.currentStackSize}/{item.maxStackSize}";
                stackText.gameObject.SetActive(true);
            }
            else
            {
                stackText.gameObject.SetActive(false);
            }
        }
    }

    private string GetItemTypeDisplayName(ItemType type)
    {
        switch (type)
        {
            case ItemType.Consumable: return "Consumable";
            case ItemType.Weapon: return "Weapon";
            case ItemType.Armor: return "Armor";
            case ItemType.Accessory: return "Accessory";
            case ItemType.Material: return "Material";
            case ItemType.Quest: return "Quest";
            case ItemType.Misc: return "Misc";
            default: return "Unknown";
        }
    }

    private string GetRarityDisplayName(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Common: return "Common";
            case ItemRarity.Uncommon: return "Uncommon";
            case ItemRarity.Rare: return "Rare";
            case ItemRarity.Epic: return "Epic";
            case ItemRarity.Legendary: return "Legendary";
            default: return "Unknown";
        }
    }

    private Color GetRarityColor(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Common: return commonColor;
            case ItemRarity.Uncommon: return uncommonColor;
            case ItemRarity.Rare: return rareColor;
            case ItemRarity.Epic: return epicColor;
            case ItemRarity.Legendary: return legendaryColor;
            default: return Color.white;
        }
    }

    private string GetStatsDisplay(ItemData item)
    {
        string stats = "";
        
        if (item.healAmount > 0)
            stats += $"Heal HP: +{item.healAmount}\n";
            
        if (item.attackPower > 0)
            stats += $"Attack Power: +{item.attackPower}\n";
            
        if (item.defense > 0)
            stats += $"Defense: +{item.defense}\n";
            
        if (item.speed > 0)
            stats += $"Speed: +{item.speed}\n";
            
        return stats.Trim();
    }

    private string GetRequirementsDisplay(ItemData item)
    {
        string requirements = "";
        
        if (item.requiredLevel > 1)
            requirements += $"Required Lv.{item.requiredLevel}\n";
            
        if (item.weight != 1.0f)
            requirements += $"Weight: {item.weight:F1}\n";
            
        return requirements.Trim();
    }

    private void PositionTooltip(Vector3 slotPosition)
    {
        if (rectTransform == null || canvas == null) return;
        
        // ถ้า GameObject ถูกปิด ให้เปิดมันก่อน
        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
            Debug.Log("🔄 เปิด ItemTooltip อีกครั้งเพื่อแสดง tooltip");
        }
        
        // รอให้ ContentSizeFitter ทำงานก่อนคำนวณตำแหน่ง
        StartCoroutine(DelayedPositioning(slotPosition));
    }
    
    private System.Collections.IEnumerator DelayedPositioning(Vector3 slotPosition)
    {
        // รอ 1 frame ให้ UI อัปเดตขนาด
        yield return null;
        
        // แปลงตำแหน่งเป็น screen space
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, slotPosition);
        
        // ตั้งตำแหน่งเริ่มต้น (ขวาบนของช่อง)
        Vector2 position = screenPoint + new Vector2(50, 50);
        
        // ตรวจสอบว่า tooltip ออกนอกจอหรือไม่
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        
        // ใช้ขนาจริงของ tooltip
        float tooltipWidth = rectTransform.rect.width;
        float tooltipHeight = rectTransform.rect.height;
        
        // ปรับตำแหน่งถ้าออกขอบจอขวา
        if (position.x + tooltipWidth > Screen.width)
        {
            position.x = screenPoint.x - tooltipWidth - 50;
        }
        
        // ปรับตำแหน่งถ้าออกขอบจอบน
        if (position.y + tooltipHeight > Screen.height)
        {
            position.y = screenPoint.y - tooltipHeight - 50;
        }
        
        // แปลงกลับเป็น local position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, 
            position, 
            canvas.worldCamera, 
            out Vector2 localPoint
        );
        
        rectTransform.localPosition = localPoint;
    }

    // สำหรับ debug
    [ContextMenu("Test Tooltip")]
    public void TestTooltip()
    {
        // สร้างไอเท็มทดสอบ
        ItemData testItem = ScriptableObject.CreateInstance<ItemData>();
        testItem.itemName = "ดาบแห่งวีรบุรุษ";
        testItem.description = "ดาบโบราณที่เคยสังหารมังกรได้ มีพลังอันน่าเกรงขาม";
        testItem.itemType = ItemType.Weapon;
        testItem.rarity = ItemRarity.Legendary;
        testItem.attackPower = 150;
        testItem.requiredLevel = 50;
        testItem.weight = 5.5f;
        
        ShowTooltip(testItem, Input.mousePosition);
    }
}
