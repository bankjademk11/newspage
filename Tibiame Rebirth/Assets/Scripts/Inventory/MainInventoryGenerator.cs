using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class MainInventoryGenerator : MonoBehaviour
{
    [Header("ใส่ Canvas ที่ต้องการสร้าง UI ลงในนี้")]
    public Canvas targetCanvas;

    [Header("ตั้งค่า UI")]
    public Vector2 slotSize = new Vector2(80, 80);
    public float slotSpacing = 10f;
    public Sprite slotSprite; // Sprite ของช่องไอเท็ม (สามารถตั้งได้ใน Inspector)
    
    [Header("การตั้งค่า Inventory Toggle")]
    public bool createToggleSystem = true;
    public bool pauseGameWhenOpen = false;
    public bool showCursorWhenOpen = true;

    private GameObject inventoryParent;
    public static GameObject InventoryParent { get; private set; } // สำหรับให้ script อื่นเรียกใช้

#if UNITY_EDITOR
    [ContextMenu("Generate UI")]
    public void GenerateUI()
    {
        if (targetCanvas == null)
        {
            Debug.LogError("⚠ กรุณาลาก Canvas มาใส่ในช่อง targetCanvas ก่อนค่ะ!");
            return;
        }

        // 🔹 สร้าง GameObject หลักของ MainInventory
        inventoryParent = new GameObject("MainInventoryUI");
        inventoryParent.transform.SetParent(targetCanvas.transform, false);

        RectTransform parentRect = inventoryParent.AddComponent<RectTransform>();
        parentRect.sizeDelta = new Vector2(
            (slotSize.x * 5) + (slotSpacing * 4),
            (slotSize.y * 4) + (slotSpacing * 3)
        );
        parentRect.anchorMin = new Vector2(0.5f, 0.5f);
        parentRect.anchorMax = new Vector2(0.5f, 0.5f);
        parentRect.pivot = new Vector2(0.5f, 0.5f);
        parentRect.anchoredPosition = Vector2.zero;

        // 🔹 เพิ่ม GridLayoutGroup
        GridLayoutGroup grid = inventoryParent.AddComponent<GridLayoutGroup>();
        grid.cellSize = slotSize;
        grid.spacing = new Vector2(slotSpacing, slotSpacing);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 5; // 5 คอลัมน์
        grid.childAlignment = TextAnchor.MiddleCenter;

        // 🔹 สร้าง 20 ช่อง
        for (int i = 0; i < 20; i++)
        {
            GameObject slot = new GameObject($"Slot_{i + 1}");
            slot.transform.SetParent(inventoryParent.transform, false);

            Image slotImage = slot.AddComponent<Image>();
            if (slotSprite != null)
                slotImage.sprite = slotSprite;
            else
                slotImage.color = new Color(0.5f, 0.4f, 0.3f, 0.3f); // สีน้ำตาลอ่อน (เหมือนกระเป๋า)

            // เพิ่ม Outline รอบช่อง
            Outline outline = slot.AddComponent<Outline>();
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(2, -2);

            // เพิ่ม Button และ InventorySlot
            Button btn = slot.AddComponent<Button>();
            InventorySlot slotScript = slot.AddComponent<InventorySlot>();

            // เพิ่มไอคอนในช่อง
            GameObject iconObj = new GameObject("Icon");
            iconObj.transform.SetParent(slot.transform, false);
            Image iconImage = iconObj.AddComponent<Image>();
            iconImage.rectTransform.sizeDelta = slotSize - new Vector2(10, 10);
            iconImage.enabled = false;
            slotScript.icon = iconImage;

            // กดแล้วใช้ไอเท็ม
            btn.onClick.AddListener(() => slotScript.UseItem());
            
            // เพิ่ม DragDropHandler
            DragDropHandler dragHandler = slot.AddComponent<DragDropHandler>();
            dragHandler.currentSlot = slotScript;
            dragHandler.canvas = targetCanvas;
            
            // ลงทะเบียนกับ DragDropManager
            if (DragDropManager.Instance != null)
            {
                DragDropManager.Instance.RegisterInventorySlot(slotScript, "MainInventory");
            }
        }

        // 🔹 สร้างระบบเปิด/ปิด (ถ้าเลือก)
        if (createToggleSystem)
        {
            CreateInventoryToggle();
        }
        
        // เก็บ reference ไว้ให้ script อื่นเรียกใช้
        InventoryParent = inventoryParent;
        
        Debug.Log("✅ สร้าง MainInventory 20 ช่องเรียบร้อยแล้ว!");
    }
    
    void CreateInventoryToggle()
    {
        // สร้าง GameObject สำหรับ InventoryToggle
        GameObject toggleObj = new GameObject("InventoryToggle");
        toggleObj.transform.SetParent(targetCanvas.transform, false);
        
        // เพิ่ม InventoryToggle script
        InventoryToggle toggle = toggleObj.AddComponent<InventoryToggle>();
        
        // ตั้งค่า
        toggle.mainInventoryUI = inventoryParent;
        toggle.pauseGameWhenOpen = pauseGameWhenOpen;
        toggle.showCursorWhenOpen = showCursorWhenOpen;
        
        Debug.Log("✅ สร้างระบบเปิด/ปิด Inventory (กด I) เรียบร้อยแล้ว!");
    }
#endif
}
