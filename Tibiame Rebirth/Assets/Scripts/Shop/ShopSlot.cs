using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public Image icon;
    public Text priceText;
    public Button buyButton;
    public ShopItemData itemData;
    public ShopItemData currentItem { get { return itemData; } }
    
    void Start()
    {
        Debug.Log($"=== ShopSlot Start() === ชื่อ GameObject: {gameObject.name}");
        
        // ตรวจสอบ component ที่จำเป็น
        if (buyButton == null)
        {
            Debug.LogError($"❌ buyButton เป็น null บน {gameObject.name}");
            buyButton = GetComponent<Button>();
            if (buyButton == null)
            {
                Debug.LogError($"❌ ไม่พบ Button component บน {gameObject.name}");
                return;
            }
        }
        else
        {
            Debug.Log($"✅ buyButton พร้อมใช้งานบน {gameObject.name}");
        }
        
        if (icon == null)
        {
            Debug.LogWarning($"⚠️ icon เป็น null บน {gameObject.name}");
            icon = transform.Find("Icon")?.GetComponent<Image>();
        }
        
        if (priceText == null)
        {
            Debug.LogWarning($"⚠️ priceText เป็น null บน {gameObject.name}");
            priceText = transform.Find("Price")?.GetComponent<Text>();
        }
        
        // เชื่อมต่อปุ่มใหม่ (ถ้ายังไม่ได้เชื่อม)
        if (buyButton != null)
        {
            Debug.Log($"🔗 เชื่อมต่อปุ่มซื้อบน {gameObject.name}");
            buyButton.onClick.AddListener(TestButton);
            buyButton.onClick.AddListener(OnBuyClicked);
        }
        
        Debug.Log($"=== ShopSlot Start() จบ ===");
    }
    
    /// <summary>
    /// ฟังก์ชันทดสอบปุ่มง่ายๆ
    /// </summary>
    public void TestButton()
    {
        Debug.Log($"🔥 ปุ่มถูกกด! GameObject: {gameObject.name}");
    }

    /// <summary>
    /// ตั้งค่า slot ให้แสดงไอเท็ม
    /// </summary>
    public void SetItem(ShopItemData data)
    {
        itemData = data;
        if (data != null)
        {
            icon.sprite = data.icon;
            icon.enabled = data.icon != null;
            priceText.text = data.price.ToString();
            buyButton.interactable = true;
        }
        else
        {
            icon.sprite = null;
            icon.enabled = false;
            priceText.text = "";
            buyButton.interactable = false;
        }
    }

    public void OnBuyClicked()
    {
        Debug.Log("=== OnBuyClicked() เริ่มทำงาน ===");
        
        if (itemData == null) 
        {
            Debug.LogError("itemData เป็น null!");
            return;
        }
        
        Debug.Log($"พยายามซื้อ: {itemData.itemName} ราคา {itemData.price}");
        
        // หา ShopManager
        ShopManager shopManager = FindObjectOfType<ShopManager>();
        if (shopManager != null)
        {
            Debug.Log("พบ ShopManager แล้ว");
            bool success = shopManager.TryPurchaseItem(itemData);
            if (success)
            {
                Debug.Log($"✅ ซื้อ {itemData.itemName} สำเร็จ!");
                // สามารถเพิ่มเอฟเฟกต์เสียงหรือภาพเคลื่อนไหวได้ที่นี่
            }
            else
            {
                Debug.Log($"❌ ซื้อ {itemData.itemName} ไม่สำเร็จ!");
                // สามารถแสดงข้อความ error ได้ที่นี่
            }
        }
        else
        {
            Debug.LogError("❌ ไม่พบ ShopManager!");
        }
        
        Debug.Log("=== OnBuyClicked() จบการทำงาน ===");
    }
}
