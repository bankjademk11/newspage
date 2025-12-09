using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ShortInventoryGenerator : MonoBehaviour
{
    [Header("ใส่ Canvas ที่ต้องการสร้าง UI ลงในนี้")]
    public Canvas targetCanvas;

    [Header("ตั้งค่า UI")]
    public Vector2 slotSize = new Vector2(80, 80);
    public float slotSpacing = 10f;
    public Sprite slotSprite; // ถ้ามี sprite สำหรับช่องไอเท็ม

    private GameObject inventoryParent;

#if UNITY_EDITOR
    [ContextMenu("Generate UI")]
    public void GenerateUI()
    {
        if (targetCanvas == null)
        {
            Debug.LogError("กรุณาลาก Canvas มาใส่ในช่อง targetCanvas ก่อนค่ะ!");
            return;
        }

        // สร้าง GameObject หลักของ ShortInventory
        inventoryParent = new GameObject("ShortInventoryUI");
        inventoryParent.transform.SetParent(targetCanvas.transform, false);

        RectTransform parentRect = inventoryParent.AddComponent<RectTransform>();
        parentRect.sizeDelta = new Vector2(
            (slotSize.x * 3) + (slotSpacing * 2),
            (slotSize.y * 3) + (slotSpacing * 2)
        );
        parentRect.anchorMin = new Vector2(0.5f, 0.5f);
        parentRect.anchorMax = new Vector2(0.5f, 0.5f);
        parentRect.pivot = new Vector2(0.5f, 0.5f);
        parentRect.anchoredPosition = Vector2.zero;

        GridLayoutGroup grid = inventoryParent.AddComponent<GridLayoutGroup>();
        grid.cellSize = slotSize;
        grid.spacing = new Vector2(slotSpacing, slotSpacing);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 3;
        grid.childAlignment = TextAnchor.MiddleCenter;

        // สร้างช่อง 3x3
        for (int i = 0; i < 9; i++)
        {
            GameObject slot = new GameObject($"Slot_{i + 1}");
            slot.transform.SetParent(inventoryParent.transform, false);

            Image slotImage = slot.AddComponent<Image>();
            if (slotSprite != null)
                slotImage.sprite = slotSprite;
            else
                slotImage.color = new Color(1, 1, 1, 0.25f); // สีเทาอ่อนถ้าไม่มี sprite

            // เพิ่ม Outline เพื่อให้เห็นชัด
            Outline outline = slot.AddComponent<Outline>();
            outline.effectColor = Color.black;
            outline.effectDistance = new Vector2(2, -2);

            // เพิ่ม Button และ InventorySlot
            Button btn = slot.AddComponent<Button>();
            InventorySlot slotScript = slot.AddComponent<InventorySlot>();

            // เพิ่มไอคอนภายในช่อง
            GameObject iconObj = new GameObject("Icon");
            iconObj.transform.SetParent(slot.transform, false);
            Image iconImage = iconObj.AddComponent<Image>();
            iconImage.rectTransform.sizeDelta = slotSize - new Vector2(10, 10);
            iconImage.enabled = false;
            slotScript.icon = iconImage;

            // เพิ่ม Text สำหรับแสดงจำนวนไอเท็ม
            GameObject stackTextObj = new GameObject("StackText");
            stackTextObj.transform.SetParent(slot.transform, false);
            Text stackText = stackTextObj.AddComponent<Text>();
            stackText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            stackText.fontSize = 14;
            stackText.color = Color.white;
            stackText.alignment = TextAnchor.LowerRight;
            
            RectTransform stackRect = stackText.GetComponent<RectTransform>();
            stackRect.sizeDelta = new Vector2(30, 20);
            stackRect.anchorMin = new Vector2(1, 0);
            stackRect.anchorMax = new Vector2(1, 0);
            stackRect.pivot = new Vector2(1, 0);
            stackRect.anchoredPosition = new Vector2(-5, 5);
            
            slotScript.stackText = stackText;

            // กำหนดให้คลิกแล้วเรียก UseItem
            btn.onClick.AddListener(() => slotScript.UseItem());
            
            // เพิ่ม DragDropHandler
            DragDropHandler dragHandler = slot.AddComponent<DragDropHandler>();
            dragHandler.currentSlot = slotScript;
            dragHandler.canvas = targetCanvas;
            
            // ลงทะเบียนกับ DragDropManager
            if (DragDropManager.Instance != null)
            {
                DragDropManager.Instance.RegisterInventorySlot(slotScript, "ShortInventory");
            }
        }

        Debug.Log("✅ สร้าง ShortInventory 3x3 พร้อม Drag & Drop เรียบร้อยแล้ว!");
    }
#endif
}
