using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Components")]
    public Image icon;
    public Image backgroundImage;
    public Text stackText;
    
    [Header("Rarity Colors")]
    public Color commonColor = new Color(0.8f, 0.8f, 0.8f, 1f);     // เทา
    public Color uncommonColor = new Color(0.2f, 0.8f, 0.2f, 1f);   // เขียว
    public Color rareColor = new Color(0.2f, 0.4f, 0.8f, 1f);       // น้ำเงิน
    public Color epicColor = new Color(0.6f, 0.2f, 0.8f, 1f);        // ม่วง
    public Color legendaryColor = new Color(1.0f, 0.6f, 0.0f, 1f);  // ส้ม/ทอง
    
    public ItemData currentItem;
    private Color originalBackgroundColor; // เก็บสีเริ่มต้นของพื้นหลัง

    void Start()
    {
        // หา Component ถ้ายังไม่ได้กำหนด
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();
            
        // เก็บสีเริ่มต้นของพื้นหลังไว้
        if (backgroundImage != null)
            originalBackgroundColor = backgroundImage.color;
            
        if (stackText == null)
        {
            // หา Text ใน child
            Transform textTransform = transform.Find("StackText");
            if (textTransform != null)
                stackText = textTransform.GetComponent<Text>();
        }
    }

    public void SetItem(ItemData item)
    {
        currentItem = item;
        
        if (item == null)
        {
            ClearSlot(); // ถ้าไม่มีไอเท็ม ล้างช่อง
        }
        else
        {
            // ตั้งค่าไอคอน
            if (icon != null)
            {
                icon.sprite = item.icon;
                icon.enabled = true;
            }
            
            // ตั้งค่าสีพื้นหลังตามความหายาก
            SetRarityColor(item.rarity);
            
            // อัปเดตจำนวนไอเท็ม
            UpdateStackDisplay();
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        
        if (icon != null)
        {
            icon.sprite = null;
            icon.enabled = false;
        }
        
        if (backgroundImage != null)
        {
            // ใช้สีเริ่มต้นของ UI แทน commonColor
            backgroundImage.color = originalBackgroundColor;
        }
        
        if (stackText != null)
        {
            stackText.text = "";
            stackText.enabled = false;
        }
    }

    public void UseItem()
    {
        if (currentItem != null)
        {
            Debug.Log("ใช้ไอเท็ม: " + currentItem.itemName);
            
            // เพิ่มเอฟเฟกต์หรือผลลัพธ์ของไอเท็ม
            if (currentItem.healAmount > 0)
            {
                Debug.Log("ฟื้น HP: " + currentItem.healAmount);
                // TODO: เรียกฟังก์ชันฟื้น HP ของ Player
            }
            
            // ลดจำนวนไอเท็มสำหรับของที่ซ้อนกันได้
            if (currentItem.isStackable && currentItem.currentStackSize > 1)
            {
                currentItem.currentStackSize--;
                UpdateStackDisplay();
            }
            else if (currentItem.isConsumable)
            {
                ClearSlot();
            }
        }
    }
    
    // ตั้งค่าสีตามความหายาก
    public void SetRarityColor(ItemRarity rarity)
    {
        if (backgroundImage == null) return;
        
        switch (rarity)
        {
            case ItemRarity.Common:
                backgroundImage.color = commonColor;
                break;
            case ItemRarity.Uncommon:
                backgroundImage.color = uncommonColor;
                break;
            case ItemRarity.Rare:
                backgroundImage.color = rareColor;
                break;
            case ItemRarity.Epic:
                backgroundImage.color = epicColor;
                break;
            case ItemRarity.Legendary:
                backgroundImage.color = legendaryColor;
                break;
        }
    }
    
    // อัปเดตการแสดงผลจำนวนไอเท็ม
    public void UpdateStackDisplay()
    {
        if (stackText == null) return;
        
        if (currentItem != null && currentItem.isStackable && currentItem.currentStackSize > 1)
        {
            stackText.text = currentItem.currentStackSize.ToString();
            stackText.enabled = true;
        }
        else
        {
            stackText.text = "";
            stackText.enabled = false;
        }
    }
    
    // เมื่อเมาส์เข้ามาในช่อง
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem != null && TooltipManager.Instance != null)
        {
            Debug.Log($"🎯 OnPointerEnter: {currentItem.itemName}");
            // ใช้ตำแหน่งเมาส์ปัจจุบันแทนตำแหน่งช่อง
            TooltipManager.Instance.ShowTooltip(currentItem, Input.mousePosition);
        }
        else
        {
            if (currentItem == null)
                Debug.Log("❌ OnPointerEnter: currentItem เป็น null");
            if (TooltipManager.Instance == null)
                Debug.Log("❌ OnPointerEnter: TooltipManager.Instance เป็น null");
        }
    }
    
    // เมื่อเมาส์ออกจากช่อง
    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
        }
    }
    
    // เพิ่มจำนวนไอเท็ม (สำหรับการซ้อน)
    public bool AddToStack(int amount)
    {
        if (currentItem == null || !currentItem.isStackable) return false;
        
        int newAmount = currentItem.currentStackSize + amount;
        if (newAmount <= currentItem.maxStackSize)
        {
            currentItem.currentStackSize = newAmount;
            UpdateStackDisplay();
            return true;
        }
        
        return false; // เกินจำนวนสูงสุด
    }
    
    // แยก stack
    public ItemData SplitStack(int splitAmount)
    {
        if (currentItem == null || !currentItem.isStackable || currentItem.currentStackSize <= 1)
            return null;
            
        if (splitAmount >= currentItem.currentStackSize)
            return null;
            
        // สร้างไอเท็มใหม่สำหรับส่วนที่แยกออก
        ItemData newItem = ScriptableObject.Instantiate(currentItem);
        newItem.currentStackSize = splitAmount;
        
        // ลดจำนวนในช่องเดิม
        currentItem.currentStackSize -= splitAmount;
        UpdateStackDisplay();
        
        return newItem;
    }
}
