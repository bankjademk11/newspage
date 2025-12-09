using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class ShopAutoGenerator : MonoBehaviour
{
    [MenuItem("Tools/Shop System/Setup Shop System")]
    public static void SetupShopSystem()
    {
        Debug.Log("=== เริ่มติดตั้งระบบ Shop ===");
        
        // ตรวจสอบ Canvas
        Canvas canvas = FindOrCreateCanvas();
        
        // สร้าง Tooltip Manager ถ้ายังไม่มี
        CreateTooltipManager(canvas);
        
        // สร้าง Shop Generator
        GameObject generatorObject = new GameObject("ShopGenerator");
        generatorObject.transform.SetParent(canvas.transform, false);
        
        ShopGenerator generator = generatorObject.AddComponent<ShopGenerator>();
        generator.targetCanvas = canvas;
        
        // สร้าง Shop UI
        generator.GenerateShopUI();
        
        // สร้าง Shop Toggle
        CreateShopToggle(canvas);
        
        // สร้าง Shop Manager
        CreateShopManager(canvas);
        
        Debug.Log("=== ติดตั้งระบบ Shop เรียบร้อยแล้ว! ===");
        Debug.Log("Shop Panel: สำหรับซื้อขายไอเท็ม");
        Debug.Log("Shop Toggle: เปิด/ปิดหน้าต่างร้านค้า");
        Debug.Log("Shop Manager: จัดการการซื้อขาย");
        Debug.Log("Tooltip Manager: แสดงข้อมูลไอเท็มเมื่อชี้เมาส์");
        
        // แสดงหน้าต่างแจ้งเตือน
        EditorUtility.DisplayDialog("Shop System Setup Complete!", 
            "ระบบ Shop ได้รับการติดตั้งเรียบร้อยแล้ว!\n\n" +
            "• Shop Panel - สำหรับซื้อขายไอเท็ม\n" +
            "• Shop Toggle - เปิด/ปิดหน้าต่างร้านค้า\n" +
            "• Shop Manager - จัดการการซื้อขาย\n" +
            "• Tooltip Manager - แสดงข้อมูลไอเท็ม\n\n" +
            "กดปุ่ม Shop เพื่อเปิดร้านค้า", 
            "OK");
    }
    
    [MenuItem("Tools/Shop System/Destroy Shop System")]
    public static void DestroyShopSystem()
    {
        Debug.Log("=== ลบระบบ Shop ===");
        
        // หา Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("ไม่พบ Canvas");
            return;
        }
        
        // ลบ Shop Panel
        Transform shopPanel = canvas.transform.Find("AutoShopUI");
        if (shopPanel != null)
        {
            DestroyImmediate(shopPanel.gameObject);
            Debug.Log("ลบ Shop Panel");
        }
        
        // ลบ Shop Generator
        Transform generator = canvas.transform.Find("ShopGenerator");
        if (generator != null)
        {
            DestroyImmediate(generator.gameObject);
            Debug.Log("ลบ Shop Generator");
        }
        
        // ลบ Shop Manager
        ShopManager shopManager = FindObjectOfType<ShopManager>();
        if (shopManager != null)
        {
            DestroyImmediate(shopManager.gameObject);
            Debug.Log("ลบ Shop Manager");
        }
        
        // ลบ Shop Toggle
        Transform shopToggle = canvas.transform.Find("ShopToggle");
        if (shopToggle != null)
        {
            DestroyImmediate(shopToggle.gameObject);
            Debug.Log("ลบ Shop Toggle");
        }
        
        Debug.Log("=== ลบระบบ Shop เรียบร้อยแล้ว! ===");
        
        EditorUtility.DisplayDialog("Shop System Destroyed", 
            "ระบบ Shop ได้รับการลบเรียบร้อยแล้ว", 
            "OK");
    }
    
    [MenuItem("Tools/Shop System/Create Sample Shop Items")]
    public static void CreateSampleShopItems()
    {
        Debug.Log("=== สร้างไอเท็ม Shop ตัวอย่าง ===");
        
        // สร้างโฟลเดอร์ Items ถ้ายังไม่มี
        if (!AssetDatabase.IsValidFolder("Assets/Items"))
        {
            AssetDatabase.CreateFolder("Assets", "Items");
        }
        
        // สร้าง Health Potion
        CreateHealthPotionItem();
        
        // สร้าง Mana Potion
        CreateManaPotionItem();
        
        // สร้าง Iron Sword
        CreateIronSwordItem();
        
        // สร้าง Iron Armor
        CreateIronArmorItem();
        
        // สร้าง Magic Ring
        CreateMagicRingItem();
        
        Debug.Log("=== สร้างไอเท็ม Shop ตัวอย่างเรียบร้อยแล้ว! ===");
        
        EditorUtility.DisplayDialog("Sample Shop Items Created", 
            "สร้างไอเท็ม Shop ตัวอย่างเรียบร้อยแล้ว:\n\n" +
            "• Health Potion - ยาเพิ่มพลังชีวิต\n" +
            "• Mana Potion - ยาเพิ่มพลังมานา\n" +
            "• Iron Sword - ดาบเหล็ก\n" +
            "• Iron Armor - เกราะเหล็ก\n" +
            "• Magic Ring - แหวนวิเศษ\n\n" +
            "ไอเท็มอยู่ในโฟลเดอร์ Assets/Items", 
            "OK");
    }
    
    static Canvas FindOrCreateCanvas()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        
        if (canvas == null)
        {
            // สร้าง Canvas ใหม่
            GameObject canvasObject = new GameObject("Canvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            // เพิ่ม Canvas Scaler
            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // เพิ่ม Graphic Raycaster
            canvasObject.AddComponent<GraphicRaycaster>();
            
            // เพิ่ม Event System
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
            
            Debug.Log("สร้าง Canvas ใหม่");
        }
        
        return canvas;
    }
    
    static void CreateShopToggle(Canvas canvas)
    {
        // ตรวจสอบว่ามีอยู่แล้วหรือไม่
        Transform existingToggle = canvas.transform.Find("ShopToggle");
        if (existingToggle != null)
        {
            Debug.Log("มี Shop Toggle อยู่แล้ว");
            return;
        }
        
        // สร้าง Toggle Button
        GameObject toggleObject = new GameObject("ShopToggle");
        toggleObject.transform.SetParent(canvas.transform, false);
        
        // ตั้งค่า RectTransform
        RectTransform toggleRect = toggleObject.AddComponent<RectTransform>();
        toggleRect.sizeDelta = new Vector2(120, 40);
        toggleRect.anchorMin = new Vector2(1, 1);
        toggleRect.anchorMax = new Vector2(1, 1);
        toggleRect.anchoredPosition = new Vector2(-70, -80); // อยู่ใต้ Equipment Toggle
        
        // เพิ่ม Button Component
        UnityEngine.UI.Button button = toggleObject.AddComponent<UnityEngine.UI.Button>();
        
        // เพิ่ม Background Image
        Image buttonImage = toggleObject.AddComponent<Image>();
        buttonImage.color = new Color(0.1f, 0.4f, 0.1f, 0.8f); // สีเขียว
        
        // สร้าง Text
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(toggleObject.transform, false);
        
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(100, 30);
        textRect.anchoredPosition = Vector2.zero;
        
        Text textComponent = textObject.AddComponent<Text>();
        textComponent.text = "Shop";
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 14;
        textComponent.color = Color.white;
        textComponent.alignment = TextAnchor.MiddleCenter;
        
        // เพิ่ม onClick Event
        button.onClick.AddListener(() => {
            ToggleShopPanel(canvas);
        });
        
        Debug.Log("สร้าง Shop Toggle");
    }
    
    static void CreateShopManager(Canvas canvas)
    {
        // ตรวจสอบว่ามีอยู่แล้วหรือไม่
        if (FindObjectOfType<ShopManager>() != null)
        {
            Debug.Log("มี ShopManager อยู่แล้ว");
            return;
        }
        
        GameObject shopManagerObject = new GameObject("ShopManager");
        shopManagerObject.transform.SetParent(canvas.transform, false);
        
        ShopManager shopManager = shopManagerObject.AddComponent<ShopManager>();
        
        Debug.Log("สร้าง ShopManager");
    }
    
    static void CreateTooltipManager(Canvas canvas)
    {
        // ตรวจสอบว่ามีอยู่แล้วหรือไม่
        if (FindObjectOfType<TooltipManager>() != null)
        {
            Debug.Log("มี TooltipManager อยู่แล้ว");
            return;
        }
        
        GameObject tooltipManagerObject = new GameObject("TooltipManager");
        tooltipManagerObject.transform.SetParent(canvas.transform, false);
        
        TooltipManager tooltipManager = tooltipManagerObject.AddComponent<TooltipManager>();
        
        Debug.Log("สร้าง TooltipManager");
    }
    
    static void CreateHealthPotionItem()
    {
        ShopItemData potion = ScriptableObject.CreateInstance<ShopItemData>();
        potion.itemName = "Health Potion";
        potion.description = "ยาเพิ่มพลังชีวิต 50 HP";
        potion.price = 50;
        potion.stackable = true;
        potion.itemType = ItemType.Consumable;
        potion.rarity = ItemRarity.Common;
        potion.healAmount = 50;
        
        AssetDatabase.CreateAsset(potion, "Assets/Items/HealthPotion.asset");
    }
    
    static void CreateManaPotionItem()
    {
        ShopItemData potion = ScriptableObject.CreateInstance<ShopItemData>();
        potion.itemName = "Mana Potion";
        potion.description = "ยาเพิ่มพลังมานา 30 MP";
        potion.price = 75;
        potion.stackable = true;
        potion.itemType = ItemType.Consumable;
        potion.rarity = ItemRarity.Common;
        
        AssetDatabase.CreateAsset(potion, "Assets/Items/ManaPotion.asset");
    }
    
    static void CreateIronSwordItem()
    {
        ShopItemData sword = ScriptableObject.CreateInstance<ShopItemData>();
        sword.itemName = "Iron Sword";
        sword.description = "ดาบเหล็กคุณภาพดี เพิ่มพลังโจมตี 15";
        sword.price = 200;
        sword.stackable = false;
        sword.itemType = ItemType.Weapon;
        sword.rarity = ItemRarity.Uncommon;
        sword.attackPower = 15;
        sword.requiredLevel = 5;
        sword.weight = 3.5f;
        
        AssetDatabase.CreateAsset(sword, "Assets/Items/ShopIronSword.asset");
    }
    
    static void CreateIronArmorItem()
    {
        ShopItemData armor = ScriptableObject.CreateInstance<ShopItemData>();
        armor.itemName = "Iron Armor";
        armor.description = "เกราะเหล็กหนา ป้องกันได้ดี เพิ่มพลังป้องกัน 10";
        armor.price = 350;
        armor.stackable = false;
        armor.itemType = ItemType.Armor;
        armor.rarity = ItemRarity.Uncommon;
        armor.defense = 10;
        armor.requiredLevel = 8;
        armor.weight = 8.0f;
        
        AssetDatabase.CreateAsset(armor, "Assets/Items/ShopIronArmor.asset");
    }
    
    static void CreateMagicRingItem()
    {
        ShopItemData ring = ScriptableObject.CreateInstance<ShopItemData>();
        ring.itemName = "Magic Ring";
        ring.description = "แหวนวิเศษเพิ่มพลังโจมตี 5 และความเร็ว 2";
        ring.price = 500;
        ring.stackable = false;
        ring.itemType = ItemType.Accessory;
        ring.rarity = ItemRarity.Rare;
        ring.attackPower = 5;
        ring.speed = 2;
        ring.requiredLevel = 10;
        ring.weight = 0.5f;
        
        AssetDatabase.CreateAsset(ring, "Assets/Items/ShopMagicRing.asset");
    }
    
    /// <summary>
    /// เปิด/ปิดหน้าต่างร้านค้า
    /// </summary>
    static void ToggleShopPanel(Canvas canvas)
    {
        Transform shopPanel = canvas.transform.Find("AutoShopUI");
        if (shopPanel != null)
        {
            CanvasGroup canvasGroup = shopPanel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                bool isVisible = canvasGroup.alpha > 0;
                canvasGroup.alpha = isVisible ? 0 : 1;
                canvasGroup.interactable = !isVisible;
                canvasGroup.blocksRaycasts = !isVisible;
                
                Debug.Log(isVisible ? "ปิดร้านค้า" : "เปิดร้านค้า");
            }
            else
            {
                bool isActive = shopPanel.gameObject.activeSelf;
                shopPanel.gameObject.SetActive(!isActive);
                
                Debug.Log(isActive ? "ปิดร้านค้า" : "เปิดร้านค้า");
            }
        }
        else
        {
            Debug.LogError("ไม่พบ AutoShopUI ใน Canvas");
        }
    }
    
    /// <summary>
    /// เปิดหน้าต่างร้านค้า
    /// </summary>
    public static void OpenShopPanel(Canvas canvas)
    {
        Transform shopPanel = canvas.transform.Find("AutoShopUI");
        if (shopPanel != null)
        {
            CanvasGroup canvasGroup = shopPanel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
            else
            {
                shopPanel.gameObject.SetActive(true);
            }
            Debug.Log("เปิดร้านค้า");
        }
        else
        {
            Debug.LogError("ไม่พบ AutoShopUI ใน Canvas");
        }
    }
    
    /// <summary>
    /// ปิดหน้าต่างร้านค้า
    /// </summary>
    public static void CloseShopPanel(Canvas canvas)
    {
        Transform shopPanel = canvas.transform.Find("AutoShopUI");
        if (shopPanel != null)
        {
            CanvasGroup canvasGroup = shopPanel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            else
            {
                shopPanel.gameObject.SetActive(false);
            }
            Debug.Log("ปิดร้านค้า");
        }
        else
        {
            Debug.LogError("ไม่พบ AutoShopUI ใน Canvas");
        }
    }
}
