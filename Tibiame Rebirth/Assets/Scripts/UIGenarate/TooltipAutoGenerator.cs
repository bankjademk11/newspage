using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class TooltipAutoGenerator : MonoBehaviour
{
    [Header("ตั้งค่า Tooltip")]
    public Canvas targetCanvas;
    public string prefabPath = "Prefabs/ItemTooltip";
    
    [Header("สีตามความหายาก")]
    public Color commonColor = Color.gray;
    public Color uncommonColor = Color.green;
    public Color rareColor = Color.blue;
    public Color epicColor = new Color(0.6f, 0.2f, 0.8f);
    public Color legendaryColor = new Color(1.0f, 0.6f, 0.0f);

#if UNITY_EDITOR
    [ContextMenu("Generate Complete Tooltip System")]
    public void GenerateTooltipSystem()
    {
        if (targetCanvas == null)
        {
            Debug.LogError("⚠ กรุณาลาก Canvas มาใส่ในช่อง targetCanvas ก่อนค่ะ!");
            return;
        }

        Debug.Log("🚀 เริ่มสร้างระบบ Tooltip อัตโนมัติ...");

        // 1. สร้าง ItemTooltip prefab
        GameObject tooltipPrefab = CreateTooltipPrefab();
        
        // 2. สร้าง TooltipManager
        CreateTooltipManager(tooltipPrefab);
        
        // 3. เชื่อมกับ InventorySlot ทั้งหมด
        ConnectToInventorySlots();

        Debug.Log("✅ สร้างระบบ Tooltip สำเร็จเรียบร้อยแล้ว!");
    }

    private GameObject CreateTooltipPrefab()
    {
        // สร้าง GameObject หลัก
        GameObject tooltipObj = new GameObject("ItemTooltip");
        tooltipObj.transform.SetParent(targetCanvas.transform, false);

        // เพิ่ม RectTransform
        RectTransform rectTransform = tooltipObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(250, 200);

        // เพิ่ม Image (พื้นหลัง)
        Image backgroundImage = tooltipObj.AddComponent<Image>();
        backgroundImage.color = new Color(0, 0, 0, 0.8f);
        backgroundImage.raycastTarget = false;

        // เพิ่ม ItemTooltip component
        ItemTooltip itemTooltip = tooltipObj.AddComponent<ItemTooltip>();
        itemTooltip.commonColor = commonColor;
        itemTooltip.uncommonColor = uncommonColor;
        itemTooltip.rareColor = rareColor;
        itemTooltip.epicColor = epicColor;
        itemTooltip.legendaryColor = legendaryColor;

        // เพิ่ม Vertical Layout Group
        VerticalLayoutGroup layoutGroup = tooltipObj.AddComponent<VerticalLayoutGroup>();
        layoutGroup.padding = new RectOffset(10, 10, 10, 10);
        layoutGroup.spacing = 5f;
        layoutGroup.childControlHeight = true;
        layoutGroup.childControlWidth = true;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = false;

        // เพิ่ม Content Size Fitter
        ContentSizeFitter sizeFitter = tooltipObj.AddComponent<ContentSizeFitter>();
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        // สร้าง Text components ทั้งหมด
        CreateTextComponent(tooltipObj, "ItemName", 18, TextAlignmentOptions.Center, Color.white, ref itemTooltip.itemNameText);
        CreateTextComponent(tooltipObj, "Description", 14, TextAlignmentOptions.Left, Color.gray, ref itemTooltip.descriptionText);
        CreateTextComponent(tooltipObj, "Type", 12, TextAlignmentOptions.Left, Color.gray, ref itemTooltip.typeText);
        CreateTextComponent(tooltipObj, "Rarity", 12, TextAlignmentOptions.Left, Color.white, ref itemTooltip.rarityText);
        CreateTextComponent(tooltipObj, "Stats", 12, TextAlignmentOptions.Left, Color.green, ref itemTooltip.statsText);
        CreateTextComponent(tooltipObj, "Requirements", 12, TextAlignmentOptions.Left, new Color(1f, 0.6f, 0f), ref itemTooltip.requirementsText);
        CreateTextComponent(tooltipObj, "Stack", 12, TextAlignmentOptions.Right, Color.white, ref itemTooltip.stackText);

        // ซ่อน tooltip ตอนเริ่ม
        tooltipObj.SetActive(false);

        // สร้าง Prefab
        string fullPath = "Assets/" + prefabPath + ".prefab";
        EnsureFolderExists("Assets/Prefabs");
        PrefabUtility.SaveAsPrefabAsset(tooltipObj, fullPath);

        // ลบ GameObject ในฉาก (เก็บไว้แค่ prefab)
        DestroyImmediate(tooltipObj);

        Debug.Log($"✅ สร้าง ItemTooltip prefab ที่: {fullPath}");
        
        // โหลด prefab กลับมาเพื่อใช้งาน
        return AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
    }

    private void CreateTextComponent(GameObject parent, string name, int fontSize, TextAlignmentOptions alignment, Color color, ref TextMeshProUGUI textRef)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform, false);

        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.fontSize = fontSize;
        textComponent.alignment = alignment;
        textComponent.color = color;
        textComponent.raycastTarget = false;
        
        // ตั้งค่า font - ใช้ LiberationSans (มีอยู่จริงใน Unity)
        textComponent.font = Resources.GetBuiltinResource<TMP_FontAsset>("LiberationSans SDF");
        
        // ตั้งค่า margin
        textComponent.margin = new Vector4(0, 0, 0, 0);

        // กำหนด reference
        textRef = textComponent;
    }

    private void CreateTooltipManager(GameObject tooltipPrefab)
    {
        // สร้าง TooltipManager GameObject
        GameObject managerObj = new GameObject("TooltipManager");
        
        // เพิ่ม TooltipManager component
        TooltipManager manager = managerObj.AddComponent<TooltipManager>();
        manager.tooltipPrefab = tooltipPrefab;
        manager.parentCanvas = targetCanvas;
        manager.enableDebugLog = true;
        manager.showDelay = 0.5f;
        manager.hideDelay = 0.1f;

        Debug.Log("✅ สร้าง TooltipManager เรียบร้อยแล้ว");
    }

    private void ConnectToInventorySlots()
    {
        // หา InventorySlot ทั้งหมดในฉาก
        InventorySlot[] allSlots = FindObjectsOfType<InventorySlot>();
        
        foreach (InventorySlot slot in allSlots)
        {
            // เพิ่ม EventTrigger ถ้ายังไม่มี
            if (slot.GetComponent<UnityEngine.EventSystems.EventTrigger>() == null)
            {
                UnityEngine.EventSystems.EventTrigger trigger = slot.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
                
                // Pointer Enter Event
                UnityEngine.EventSystems.EventTrigger.Entry enterEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
                enterEntry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
                enterEntry.callback.AddListener((data) => {
                    if (slot.currentItem != null && TooltipManager.Instance != null)
                    {
                        TooltipManager.Instance.ShowTooltip(slot.currentItem, slot.transform.position);
                    }
                });
                trigger.triggers.Add(enterEntry);
                
                // Pointer Exit Event
                UnityEngine.EventSystems.EventTrigger.Entry exitEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
                exitEntry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
                exitEntry.callback.AddListener((data) => {
                    if (TooltipManager.Instance != null)
                    {
                        TooltipManager.Instance.HideTooltip();
                    }
                });
                trigger.triggers.Add(exitEntry);
            }
        }
        
        Debug.Log($"✅ เชื่อมต่อ Tooltip กับ InventorySlot {allSlots.Length} ช่องเรียบร้อยแล้ว");
    }

    private void EnsureFolderExists(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string[] folders = path.Split('/');
            string currentPath = "";
            
            for (int i = 0; i < folders.Length; i++)
            {
                if (i == 0)
                {
                    currentPath = folders[i];
                }
                else
                {
                    string parentPath = currentPath;
                    currentPath = parentPath + "/" + folders[i];
                    
                    if (!AssetDatabase.IsValidFolder(currentPath))
                    {
                        AssetDatabase.CreateFolder(parentPath, folders[i]);
                    }
                }
            }
        }
    }

    [ContextMenu("Test Tooltip System")]
    public void TestTooltipSystem()
    {
        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.TestTooltip();
        }
        else
        {
            Debug.LogWarning("❌ ไม่พบ TooltipManager กรุณาสร้างระบบก่อนทดสอบ");
        }
    }

    [ContextMenu("Clean Up Tooltip System")]
    public void CleanUpTooltipSystem()
    {
        // ลบ TooltipManager
        GameObject manager = GameObject.Find("TooltipManager");
        if (manager != null)
        {
            DestroyImmediate(manager);
        }

        // ลบ ItemTooltip ในฉาก
        GameObject[] tooltips = GameObject.FindGameObjectsWithTag("Untagged");
        foreach (GameObject obj in tooltips)
        {
            if (obj.name == "ItemTooltip")
            {
                DestroyImmediate(obj);
            }
        }

        Debug.Log("🧹 ลบระบบ Tooltip เก่าเรียบร้อยแล้ว");
    }
#endif
}
