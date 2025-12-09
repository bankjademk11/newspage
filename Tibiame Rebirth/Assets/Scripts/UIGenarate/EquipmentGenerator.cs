using UnityEngine;
using UnityEngine.UI;

public class EquipmentGenerator : MonoBehaviour
{
    [Header("Equipment Panel Settings")]
    public Canvas parentCanvas;
    public string panelName = "EquipmentPanel";
    
    [Header("Layout Settings")]
    public float panelWidth = 300f;
    public float panelHeight = 400f;
    public float slotSize = 60f;
    public float spacing = 10f;
    
    [Header("Visual Settings")]
    public Color panelColor = new Color(0.1f, 0.1f, 0.1f, 0.9f);
    public Color slotColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
    public Color headerColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    
    [Header("Stats Display")]
    public bool showStatsPanel = true;
    public float statsPanelHeight = 120f;
    
    private GameObject equipmentPanel;
    private GameObject statsPanel;
    
    [ContextMenu("Generate Equipment UI")]
    public void GenerateEquipmentUI()
    {
        if (parentCanvas == null)
        {
            Debug.LogError("กรุณากำหนด Parent Canvas ก่อนสร้าง Equipment UI");
            return;
        }
        
        // ลบ Panel เก่าถ้ามี
        Transform oldPanel = parentCanvas.transform.Find(panelName);
        if (oldPanel != null)
        {
            DestroyImmediate(oldPanel.gameObject);
        }
        
        // สร้าง Equipment Panel หลัก
        CreateEquipmentPanel();
        
        // สร้าง Equipment Slots
        CreateEquipmentSlots();
        
        // สร้าง Stats Panel
        if (showStatsPanel)
        {
            CreateStatsPanel();
        }
        
        // สร้าง Equipment Manager
        CreateEquipmentManager();
        
        Debug.Log("สร้าง Equipment UI เรียบร้อยแล้ว!");
    }
    
    void CreateEquipmentPanel()
    {
        // สร้าง Panel หลัก
        equipmentPanel = new GameObject(panelName);
        equipmentPanel.transform.SetParent(parentCanvas.transform, false);
        
        // เพิ่ม Image Component
        Image panelImage = equipmentPanel.AddComponent<Image>();
        panelImage.color = panelColor;
        
        // เพิ่ม Canvas Group (สำหรับ Alpha)
        CanvasGroup canvasGroup = equipmentPanel.AddComponent<CanvasGroup>();
        
        // ตั้งค่าเริ่มต้นให้ปิด UI
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        // บังคับปิด GameObject ด้วยเพื่อให้แน่ใจว่าปิดสนิท
        equipmentPanel.SetActive(false);
        
        // ตั้งค่า RectTransform
        RectTransform rectTransform = equipmentPanel.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(panelWidth, panelHeight);
        rectTransform.anchoredPosition = Vector2.zero;
        
        Debug.Log("🛡️ Equipment Panel ถูกสร้างและบังคับปิดเรียบร้อย (GameObject.SetActive(false))");
        Debug.Log("🛡️ CanvasGroup.alpha = 0, interactable = false, blocksRaycasts = false");
        
        // สร้าง Header
        CreateHeader();
    }
    
    void CreateHeader()
    {
        GameObject header = new GameObject("Header");
        header.transform.SetParent(equipmentPanel.transform, false);
        
        // Header Background
        Image headerImage = header.AddComponent<Image>();
        headerImage.color = headerColor;
        
        RectTransform headerRect = header.GetComponent<RectTransform>();
        headerRect.sizeDelta = new Vector2(panelWidth, 40f);
        headerRect.anchorMin = new Vector2(0, 1);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.anchoredPosition = new Vector2(0, -20f);
        
        // Header Text
        GameObject headerText = new GameObject("HeaderText");
        headerText.transform.SetParent(header.transform, false);
        
        Text textComponent = headerText.AddComponent<Text>();
        textComponent.text = "EQUIPMENT";
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 16;
        textComponent.color = Color.white;
        textComponent.alignment = TextAnchor.MiddleCenter;
        
        RectTransform textRect = headerText.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(panelWidth, 40f);
        textRect.anchoredPosition = Vector2.zero;
    }
    
    void CreateEquipmentSlots()
    {
        // ตำแหน่งของแต่ละช่อง
        Vector2[] slotPositions = {
            new Vector2(0, 80f),      // Helmet (บนสุด)
            new Vector2(-70f, 20f),   // Weapon (ซ้าย)
            new Vector2(70f, 20f),    // Accessory (ขวา)
            new Vector2(0, -20f),     // Armor (กลาง)
            new Vector2(0, -80f)      // Boots (ล่างสุด)
        };
        
        EquipmentType[] slotTypes = {
            EquipmentType.Helmet,
            EquipmentType.Weapon,
            EquipmentType.Accessory,
            EquipmentType.Armor,
            EquipmentType.Boots
        };
        
        string[] slotNames = {
            "HelmetSlot",
            "WeaponSlot", 
            "AccessorySlot",
            "ArmorSlot",
            "BootsSlot"
        };
        
        for (int i = 0; i < slotTypes.Length; i++)
        {
            CreateEquipmentSlot(slotPositions[i], slotTypes[i], slotNames[i]);
        }
    }
    
    void CreateEquipmentSlot(Vector2 position, EquipmentType slotType, string slotName)
    {
        // สร้าง Slot GameObject
        GameObject slot = new GameObject(slotName);
        slot.transform.SetParent(equipmentPanel.transform, false);
        
        // ตั้งค่า RectTransform
        RectTransform slotRect = slot.AddComponent<RectTransform>();
        slotRect.sizeDelta = new Vector2(slotSize, slotSize);
        slotRect.anchoredPosition = position;
        
        // เพิ่ม Background Image
        Image backgroundImage = slot.AddComponent<Image>();
        backgroundImage.color = slotColor;
        backgroundImage.type = Image.Type.Sliced;
        
        // เพิ่ม EquipmentSlot Component
        EquipmentSlot equipmentSlot = slot.AddComponent<EquipmentSlot>();
        equipmentSlot.slotType = slotType;
        equipmentSlot.backgroundImage = backgroundImage;
        
        // สร้าง Icon Child
        GameObject icon = new GameObject("Icon");
        icon.transform.SetParent(slot.transform, false);
        
        RectTransform iconRect = icon.AddComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(slotSize * 0.8f, slotSize * 0.8f);
        iconRect.anchoredPosition = Vector2.zero;
        
        Image iconImage = icon.AddComponent<Image>();
        iconImage.color = Color.white;
        
        equipmentSlot.icon = iconImage;
        
        // เพิ่ม Slot Label
        CreateSlotLabel(slot, slotType.ToString());
    }
    
    void CreateSlotLabel(GameObject slot, string labelText)
    {
        GameObject label = new GameObject("Label");
        label.transform.SetParent(slot.transform, false);
        
        Text labelComponent = label.AddComponent<Text>();
        labelComponent.text = labelText;
        labelComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        labelComponent.fontSize = 8;
        labelComponent.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        labelComponent.alignment = TextAnchor.MiddleCenter;
        
        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.sizeDelta = new Vector2(slotSize, 15f);
        labelRect.anchorMin = new Vector2(0, 0);
        labelRect.anchorMax = new Vector2(1, 0);
        labelRect.anchoredPosition = new Vector2(0, -slotSize/2 - 10f);
    }
    
    void CreateStatsPanel()
    {
        // สร้าง Stats Panel
        statsPanel = new GameObject("StatsPanel");
        statsPanel.transform.SetParent(equipmentPanel.transform, false);
        
        // Stats Background
        Image statsImage = statsPanel.AddComponent<Image>();
        statsImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        
        RectTransform statsRect = statsPanel.GetComponent<RectTransform>();
        statsRect.sizeDelta = new Vector2(panelWidth - 20f, statsPanelHeight);
        statsRect.anchorMin = new Vector2(0, 0);
        statsRect.anchorMax = new Vector2(1, 0);
        statsRect.anchoredPosition = new Vector2(0, statsPanelHeight/2 + 10f);
        
        // สร้าง Stats Text
        CreateStatsText(statsPanel);
    }
    
    void CreateStatsText(GameObject parent)
    {
        GameObject statsText = new GameObject("StatsText");
        statsText.transform.SetParent(parent.transform, false);
        
        Text textComponent = statsText.AddComponent<Text>();
        textComponent.text = "STATS\n" +
                          "HP: 100/100\n" +
                          "ATK: 10\n" +
                          "DEF: 5\n" +
                          "SPD: 3";
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 12;
        textComponent.color = Color.white;
        textComponent.alignment = TextAnchor.UpperLeft;
        
        RectTransform textRect = statsText.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(parent.GetComponent<RectTransform>().sizeDelta.x - 20f, parent.GetComponent<RectTransform>().sizeDelta.y - 20f);
        textRect.anchoredPosition = new Vector2(-parent.GetComponent<RectTransform>().sizeDelta.x/2 + 10f, parent.GetComponent<RectTransform>().sizeDelta.y/2 - 10f);
    }
    
    void CreateEquipmentManager()
    {
        // ตรวจสอบว่ามี EquipmentManager อยู่แล้วหรือไม่
        EquipmentManager existingManager = FindObjectOfType<EquipmentManager>();
        if (existingManager != null)
        {
            Debug.LogWarning("มี EquipmentManager อยู่แล้ว จะไม่สร้างใหม่");
            return;
        }
        
        // สร้าง Equipment Manager GameObject
        GameObject managerObject = new GameObject("EquipmentManager");
        managerObject.transform.SetParent(parentCanvas.transform, false);
        
        // เพิ่ม EquipmentManager Component
        EquipmentManager manager = managerObject.AddComponent<EquipmentManager>();
        
        // ผูก Equipment Slots เข้ากับ Manager
        manager.weaponSlot = equipmentPanel.transform.Find("WeaponSlot")?.GetComponent<EquipmentSlot>();
        manager.helmetSlot = equipmentPanel.transform.Find("HelmetSlot")?.GetComponent<EquipmentSlot>();
        manager.armorSlot = equipmentPanel.transform.Find("ArmorSlot")?.GetComponent<EquipmentSlot>();
        manager.bootsSlot = equipmentPanel.transform.Find("BootsSlot")?.GetComponent<EquipmentSlot>();
        manager.accessorySlot = equipmentPanel.transform.Find("AccessorySlot")?.GetComponent<EquipmentSlot>();
        
        // หา PlayerStatsManager
        manager.playerStatsManager = FindObjectOfType<PlayerStatsManager>();
        
        Debug.Log("สร้าง EquipmentManager เรียบร้อยแล้ว");
    }
    
    [ContextMenu("Destroy Equipment UI")]
    public void DestroyEquipmentUI()
    {
        Transform oldPanel = parentCanvas.transform.Find(panelName);
        if (oldPanel != null)
        {
            DestroyImmediate(oldPanel.gameObject);
            Debug.Log("ลบ Equipment UI เรียบร้อยแล้ว");
        }
        else
        {
            Debug.Log("ไม่พบ Equipment UI ที่จะลบ");
        }
    }
}
