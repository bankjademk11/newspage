using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class EquipmentAutoGenerator : MonoBehaviour
{
    [MenuItem("Tools/Equipment System/Setup Equipment System")]
    public static void SetupEquipmentSystem()
    {
        Debug.Log("=== เริ่มติดตั้งระบบ Equipment ===");
        
        // ตรวจสอบ Canvas
        Canvas canvas = FindOrCreateCanvas();
        
        // สร้าง Equipment Generator
        GameObject generatorObject = new GameObject("EquipmentGenerator");
        generatorObject.transform.SetParent(canvas.transform, false);
        
        EquipmentGenerator generator = generatorObject.AddComponent<EquipmentGenerator>();
        generator.parentCanvas = canvas;
        
        // สร้าง Equipment UI
        generator.GenerateEquipmentUI();
        
        // สร้าง Player Stats Manager
        CreatePlayerStatsManager(canvas);
        
        // สร้าง Equipment Toggle
        CreateEquipmentToggle(canvas);
        
        Debug.Log("=== ติดตั้งระบบ Equipment เรียบร้อยแล้ว! ===");
        Debug.Log("Equipment Panel: สำหรับสวมใส่อุปกรณ์");
        Debug.Log("Player Stats: แสดงสถานะตัวละคร");
        Debug.Log("Equipment Toggle: เปิด/ปิดหน้าต่าง Equipment");
        
        // แสดงหน้าต่างแจ้งเตือน
        EditorUtility.DisplayDialog("Equipment System Setup Complete!", 
            "ระบบ Equipment ได้รับการติดตั้งเรียบร้อยแล้ว!\n\n" +
            "• Equipment Panel - สำหรับสวมใส่อุปกรณ์\n" +
            "• Player Stats - แสดงสถานะตัวละคร\n" +
            "• Equipment Toggle - เปิด/ปิดหน้าต่าง Equipment\n\n" +
            "ลากไอเท็มจาก Inventory ไปยัง Equipment Slots เพื่อสวมใส่", 
            "OK");
    }
    
    [MenuItem("Tools/Equipment System/Destroy Equipment System")]
    public static void DestroyEquipmentSystem()
    {
        Debug.Log("=== ลบระบบ Equipment ===");
        
        // หา Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("ไม่พบ Canvas");
            return;
        }
        
        // ลบ Equipment Panel
        Transform equipmentPanel = canvas.transform.Find("EquipmentPanel");
        if (equipmentPanel != null)
        {
            DestroyImmediate(equipmentPanel.gameObject);
            Debug.Log("ลบ Equipment Panel");
        }
        
        // ลบ Equipment Generator
        Transform generator = canvas.transform.Find("EquipmentGenerator");
        if (generator != null)
        {
            DestroyImmediate(generator.gameObject);
            Debug.Log("ลบ Equipment Generator");
        }
        
        // ลบ Player Stats Manager
        PlayerStatsManager statsManager = FindObjectOfType<PlayerStatsManager>();
        if (statsManager != null)
        {
            DestroyImmediate(statsManager.gameObject);
            Debug.Log("ลบ Player Stats Manager");
        }
        
        // ลบ Equipment Toggle
        Transform equipmentToggle = canvas.transform.Find("EquipmentToggle");
        if (equipmentToggle != null)
        {
            DestroyImmediate(equipmentToggle.gameObject);
            Debug.Log("ลบ Equipment Toggle");
        }
        
        Debug.Log("=== ลบระบบ Equipment เรียบร้อยแล้ว! ===");
        
        EditorUtility.DisplayDialog("Equipment System Destroyed", 
            "ระบบ Equipment ได้รับการลบเรียบร้อยแล้ว", 
            "OK");
    }
    
    [MenuItem("Tools/Equipment System/Create Sample Equipment Items")]
    public static void CreateSampleEquipmentItems()
    {
        Debug.Log("=== สร้างไอเท็ม Equipment ตัวอย่าง ===");
        
        // สร้างโฟลเดอร์ Items ถ้ายังไม่มี
        if (!AssetDatabase.IsValidFolder("Assets/Items"))
        {
            AssetDatabase.CreateFolder("Assets", "Items");
        }
        
        // สร้าง Sword
        CreateSwordItem();
        
        // สร้าง Helmet
        CreateHelmetItem();
        
        // สร้าง Armor
        CreateArmorItem();
        
        // สร้าง Boots
        CreateBootsItem();
        
        // สร้าง Ring
        CreateRingItem();
        
        Debug.Log("=== สร้างไอเท็ม Equipment ตัวอย่างเรียบร้อยแล้ว! ===");
        
        EditorUtility.DisplayDialog("Sample Equipment Created", 
            "สร้างไอเท็ม Equipment ตัวอย่างเรียบร้อยแล้ว:\n\n" +
            "• Iron Sword - อาวุธ\n" +
            "• Iron Helmet - หมวก\n" +
            "• Iron Armor - เกราะ\n" +
            "• Iron Boots - รองเท้า\n" +
            "• Magic Ring - แหวน\n\n" +
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
    
    static void CreatePlayerStatsManager(Canvas canvas)
    {
        // ตรวจสอบว่ามีอยู่แล้วหรือไม่
        if (FindObjectOfType<PlayerStatsManager>() != null)
        {
            Debug.Log("มี PlayerStatsManager อยู่แล้ว");
            return;
        }
        
        GameObject statsManagerObject = new GameObject("PlayerStatsManager");
        statsManagerObject.transform.SetParent(canvas.transform, false);
        
        PlayerStatsManager statsManager = statsManagerObject.AddComponent<PlayerStatsManager>();
        
        // สร้าง PlayerStats ใหม่
        statsManager.stats = new PlayerStats();
        
        Debug.Log("สร้าง PlayerStatsManager");
    }
    
    static void CreateEquipmentToggle(Canvas canvas)
    {
        // ตรวจสอบว่ามีอยู่แล้วหรือไม่
        Transform existingToggle = canvas.transform.Find("EquipmentToggle");
        if (existingToggle != null)
        {
            Debug.Log("มี Equipment Toggle อยู่แล้ว");
            return;
        }
        
        // สร้าง Toggle Button
        GameObject toggleObject = new GameObject("EquipmentToggle");
        toggleObject.transform.SetParent(canvas.transform, false);
        
        // ตั้งค่า RectTransform
        RectTransform toggleRect = toggleObject.AddComponent<RectTransform>();
        toggleRect.sizeDelta = new Vector2(120, 40);
        toggleRect.anchorMin = new Vector2(1, 1);
        toggleRect.anchorMax = new Vector2(1, 1);
        toggleRect.anchoredPosition = new Vector2(-70, -30);
        
        // เพิ่ม Button Component
        UnityEngine.UI.Button button = toggleObject.AddComponent<UnityEngine.UI.Button>();
        
        // เพิ่ม Background Image
        Image buttonImage = toggleObject.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        // สร้าง Text
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(toggleObject.transform, false);
        
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(100, 30);
        textRect.anchoredPosition = Vector2.zero;
        
        Text textComponent = textObject.AddComponent<Text>();
        textComponent.text = "Equipment";
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 14;
        textComponent.color = Color.white;
        textComponent.alignment = TextAnchor.MiddleCenter;
        
        // เพิ่ม EquipmentToggle Component
        EquipmentToggle equipmentToggle = toggleObject.AddComponent<EquipmentToggle>();
        equipmentToggle.targetCanvas = canvas;
        equipmentToggle.equipmentPanelName = "EquipmentPanel";
        
        // เพิ่ม onClick Event โดยใช้ EquipmentToggle
        button.onClick.AddListener(() => {
            equipmentToggle.ToggleEquipmentPanel();
        });
        
        Debug.Log("สร้าง Equipment Toggle พร้อม EquipmentToggle Component");
    }
    
    static void CreateSwordItem()
    {
        ItemData sword = ScriptableObject.CreateInstance<ItemData>();
        sword.itemName = "Iron Sword";
        sword.description = "ดาบเหล็กที่แข็งแกร่ง";
        sword.itemType = ItemType.Weapon;
        sword.rarity = ItemRarity.Common;
        sword.isEquippable = true;
        sword.equipmentType = EquipmentType.Weapon;
        sword.attackPower = 15;
        sword.defense = 0;
        sword.speed = 0;
        sword.requiredLevel = 1;
        sword.weight = 2.0f;
        
        AssetDatabase.CreateAsset(sword, "Assets/Items/IronSword.asset");
    }
    
    static void CreateHelmetItem()
    {
        ItemData helmet = ScriptableObject.CreateInstance<ItemData>();
        helmet.itemName = "Iron Helmet";
        helmet.description = "หมวกเหล็กป้องกันการโจมตี";
        helmet.itemType = ItemType.Armor;
        helmet.rarity = ItemRarity.Common;
        helmet.isEquippable = true;
        helmet.equipmentType = EquipmentType.Helmet;
        helmet.attackPower = 0;
        helmet.defense = 5;
        helmet.speed = 0;
        helmet.requiredLevel = 1;
        helmet.weight = 1.5f;
        
        AssetDatabase.CreateAsset(helmet, "Assets/Items/IronHelmet.asset");
    }
    
    static void CreateArmorItem()
    {
        ItemData armor = ScriptableObject.CreateInstance<ItemData>();
        armor.itemName = "Iron Armor";
        armor.description = "เกราะเหล็กที่ทนทานสูง";
        armor.itemType = ItemType.Armor;
        armor.rarity = ItemRarity.Uncommon;
        armor.isEquippable = true;
        armor.equipmentType = EquipmentType.Armor;
        armor.attackPower = 0;
        armor.defense = 10;
        armor.speed = -1;
        armor.requiredLevel = 2;
        armor.weight = 5.0f;
        
        AssetDatabase.CreateAsset(armor, "Assets/Items/IronArmor.asset");
    }
    
    static void CreateBootsItem()
    {
        ItemData boots = ScriptableObject.CreateInstance<ItemData>();
        boots.itemName = "Iron Boots";
        boots.description = "รองเท้าเหล็กเพิ่มความเร็ว";
        boots.itemType = ItemType.Armor;
        boots.rarity = ItemRarity.Common;
        boots.isEquippable = true;
        boots.equipmentType = EquipmentType.Boots;
        boots.attackPower = 0;
        boots.defense = 3;
        boots.speed = 2;
        boots.requiredLevel = 1;
        boots.weight = 1.0f;
        
        AssetDatabase.CreateAsset(boots, "Assets/Items/IronBoots.asset");
    }
    
    static void CreateRingItem()
    {
        ItemData ring = ScriptableObject.CreateInstance<ItemData>();
        ring.itemName = "Magic Ring";
        ring.description = "แหวนวิเศษเพิ่มพลังโจมตี";
        ring.itemType = ItemType.Accessory;
        ring.rarity = ItemRarity.Rare;
        ring.isEquippable = true;
        ring.equipmentType = EquipmentType.Accessory;
        ring.attackPower = 5;
        ring.defense = 2;
        ring.speed = 1;
        ring.requiredLevel = 3;
        ring.weight = 0.1f;
        
        AssetDatabase.CreateAsset(ring, "Assets/Items/MagicRing.asset");
    }
}
