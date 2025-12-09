using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ShopGenerator : MonoBehaviour
{
    [Header("Target Canvas")]
    public Canvas targetCanvas;

    [Header("Layout")]
    public int columns = 5;
    public int rows = 4; // รวมเป็น 20 ช่อง (default)
    public Vector2 slotSize = new Vector2(80, 80);
    public float slotSpacing = 8f;
    public Sprite slotSprite;

    [Header("Styling")]
    public Font uiFont;
    public int titleFontSize = 24;
    public int priceFontSize = 18;

    [Header("Names")]
    public string shopTitle = "Shop";

    private GameObject shopRoot;

#if UNITY_EDITOR
    [ContextMenu("Generate Shop UI")]
    public void GenerateShopUI()
    {
        if (targetCanvas == null)
        {
            Debug.LogError("กรุณาลาก Canvas มาใส่ใน targetCanvas ก่อนค่ะนายท่าน!");
            return;
        }

        // ลบถ้ามีของเดิม (ไม่บังคับ แต่สะดวก)
        var existing = targetCanvas.transform.Find("AutoShopUI");
        if (existing != null)
        {
            if (!EditorUtility.DisplayDialog("AutoShopUI exists",
                "พบ AutoShopUI อยู่แล้ว ต้องการลบทิ้งแล้วสร้างใหม่หรือไม่?", "ลบแล้วสร้างใหม่", "ยกเลิก"))
                return;
            DestroyImmediate(existing.gameObject);
        }

        // Root panel (จะเป็น child ของ Canvas)
        shopRoot = new GameObject("AutoShopUI", typeof(RectTransform));
        shopRoot.transform.SetParent(targetCanvas.transform, false);

        var rootRect = shopRoot.GetComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0.5f, 0.5f);
        rootRect.anchorMax = new Vector2(0.5f, 0.5f);
        rootRect.pivot = new Vector2(0.5f, 0.5f);
        // ขนาดโดยประมาณ
        float width = (slotSize.x * columns) + (slotSpacing * (columns - 1)) + 40f;
        float height = (slotSize.y * rows) + (slotSpacing * (rows - 1)) + 120f;
        rootRect.sizeDelta = new Vector2(width, height);
        rootRect.anchoredPosition = Vector2.zero;

        // Background Image
        var bg = shopRoot.AddComponent<Image>();
        bg.color = new Color(0.08f, 0.06f, 0.04f, 0.95f); // สีเข้มแบบร้าน
        var bgOutline = shopRoot.AddComponent<Outline>();
        bgOutline.effectColor = Color.black;
        
        // เพิ่ม CanvasGroup สำหรับการ fade in/out
        var canvasGroup = shopRoot.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0; // เริ่มต้นให้มองไม่เห็น (ปิดร้านค้า)
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        // บังคับปิด GameObject ด้วยเพื่อให้แน่ใจว่าปิดสนิท
        shopRoot.SetActive(false);
        
        Debug.Log("🏪 ร้านค้าถูกสร้างและบังคับปิดเรียบร้อย (GameObject.SetActive(false))");
        Debug.Log("🏪 CanvasGroup.alpha = 0, interactable = false, blocksRaycasts = false");

        // Header (title + close)
        var headerGO = new GameObject("Header", typeof(RectTransform));
        headerGO.transform.SetParent(shopRoot.transform, false);
        var headerRect = headerGO.GetComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0f, 1f);
        headerRect.anchorMax = new Vector2(1f, 1f);
        headerRect.pivot = new Vector2(0.5f, 1f);
        headerRect.anchoredPosition = Vector2.zero;
        headerRect.sizeDelta = new Vector2(0f, 50f);

        // Title text
        var titleGO = new GameObject("Title", typeof(RectTransform));
        titleGO.transform.SetParent(headerGO.transform, false);
        var titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.5f);
        titleRect.anchorMax = new Vector2(0.5f, 0.5f);
        titleRect.pivot = new Vector2(0.5f, 0.5f);
        titleRect.anchoredPosition = new Vector2(-20, 0);
        var titleText = titleGO.AddComponent<Text>();
        titleText.text = shopTitle;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.fontSize = titleFontSize;
        if (uiFont != null) titleText.font = uiFont;
        else titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.color = Color.white;

        // Close button
        var closeGO = new GameObject("CloseBtn", typeof(RectTransform));
        closeGO.transform.SetParent(headerGO.transform, false);
        var closeRect = closeGO.GetComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(1f, 0.5f);
        closeRect.anchorMax = new Vector2(1f, 0.5f);
        closeRect.pivot = new Vector2(1f, 0.5f);
        closeRect.anchoredPosition = new Vector2(-10, 0);
        closeRect.sizeDelta = new Vector2(80f, 34f);
        var closeImg = closeGO.AddComponent<Image>();
        closeImg.color = new Color(0.6f, 0.15f, 0.15f);
        var closeBtn = closeGO.AddComponent<Button>();
        var closeTxtGO = new GameObject("Text", typeof(RectTransform));
        closeTxtGO.transform.SetParent(closeGO.transform, false);
        var closeTxt = closeTxtGO.AddComponent<Text>();
        closeTxt.text = "Close";
        closeTxt.alignment = TextAnchor.MiddleCenter;
        closeTxt.fontSize = 14;
        if (uiFont != null) closeTxt.font = uiFont;
        else closeTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        closeTxt.color = Color.white;
        closeTxt.rectTransform.sizeDelta = closeRect.sizeDelta;
        // ปิด panel เมื่อกด (ใช้ CanvasGroup)
        closeBtn.onClick.AddListener(() => {
            CloseShopPanel();
        });

        // Content area (Grid)
        var contentGO = new GameObject("Content", typeof(RectTransform));
        contentGO.transform.SetParent(shopRoot.transform, false);
        var contentRect = contentGO.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.5f, 0.5f);
        contentRect.anchorMax = new Vector2(0.5f, 0.5f);
        contentRect.pivot = new Vector2(0.5f, 0.5f);
        float contentHeight = (slotSize.y * rows) + (slotSpacing * (rows - 1));
        float contentWidth = (slotSize.x * columns) + (slotSpacing * (columns - 1));
        contentRect.sizeDelta = new Vector2(contentWidth, contentHeight);
        contentRect.anchoredPosition = new Vector2(0, -20);

        var grid = contentGO.AddComponent<GridLayoutGroup>();
        grid.cellSize = slotSize;
        grid.spacing = new Vector2(slotSpacing, slotSpacing);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
        grid.childAlignment = TextAnchor.UpperLeft;

        // สร้าง slots
        int total = rows * columns;
        for (int i = 0; i < total; i++)
        {
            var slot = new GameObject($"Slot_{i + 1}", typeof(RectTransform));
            slot.transform.SetParent(contentGO.transform, false);
            var slotRect = slot.GetComponent<RectTransform>();
            slotRect.sizeDelta = slotSize;

            var slotImg = slot.AddComponent<Image>();
            if (slotSprite != null) slotImg.sprite = slotSprite;
            else slotImg.color = new Color(0.35f, 0.25f, 0.18f); // leather-ish

            // add outline
            var outl = slot.AddComponent<Outline>();
            outl.effectColor = Color.black;

            // icon child
            var iconGO = new GameObject("Icon", typeof(RectTransform));
            iconGO.transform.SetParent(slot.transform, false);
            var iconRect = iconGO.GetComponent<RectTransform>();
            iconRect.sizeDelta = slotSize - new Vector2(16f, 30f); // leave space for price
            iconRect.anchoredPosition = new Vector2(0, 8);
            var iconImg = iconGO.AddComponent<Image>();
            iconImg.enabled = false;

            // price text
            var priceGO = new GameObject("Price", typeof(RectTransform));
            priceGO.transform.SetParent(slot.transform, false);
            var priceRect = priceGO.GetComponent<RectTransform>();
            priceRect.anchorMin = new Vector2(0.5f, 0f);
            priceRect.anchorMax = new Vector2(0.5f, 0f);
            priceRect.pivot = new Vector2(0.5f, 0f);
            priceRect.anchoredPosition = new Vector2(0, 6);
            priceRect.sizeDelta = new Vector2(slotSize.x, 20f);
            var priceText = priceGO.AddComponent<Text>();
            if (uiFont != null) priceText.font = uiFont;
        else priceText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            priceText.alignment = TextAnchor.MiddleCenter;
            priceText.fontSize = priceFontSize;
            priceText.color = Color.yellow;
            priceText.text = "";

            // Button to buy
            var btn = slot.AddComponent<Button>();
            
            // เพิ่ม ShopSlot component
            var slotScript = slot.AddComponent<ShopSlot>();
            slotScript.icon = iconImg;
            slotScript.priceText = priceText;
            slotScript.buyButton = btn;
            
            // เพิ่ม Tooltip trigger
            var tooltipTrigger = slot.AddComponent<ShopTooltipTrigger>();
            
            // hook buy event (แบบชัดเจน)
            Debug.Log($"🔗 กำลังเชื่อมต่อปุ่มซื้อสำหรับช่องที่ {i + 1}");
            btn.onClick.AddListener(slotScript.TestButton);
            btn.onClick.AddListener(slotScript.OnBuyClicked);
            
            Debug.Log($"✅ สร้าง ShopSlot {i + 1} เรียบร้อย ชื่อ: {slot.name}");
            Debug.Log($"   - Button: {(btn != null ? "✅" : "❌")}");
            Debug.Log($"   - ShopSlot: {(slotScript != null ? "✅" : "❌")}");
            Debug.Log($"   - Icon: {(iconImg != null ? "✅" : "❌")}");
            Debug.Log($"   - PriceText: {(priceText != null ? "✅" : "❌")}");
        }

        Debug.Log("✅ Auto Shop UI created (" + total + " slots).");
    }
    
    /// <summary>
    /// ปิดหน้าต่างร้านค้า
    /// </summary>
    public void CloseShopPanel()
    {
        if (shopRoot != null)
        {
            var cg = shopRoot.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 0;
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
            // บังคับปิด GameObject ด้วยเพื่อให้แน่ใจว่าปิดสนิท
            shopRoot.SetActive(false);
            Debug.Log("ปิดร้านค้า (GameObject.SetActive(false))");
        }
        else
        {
            // ถ้า shopRoot เป็น null ให้หาจาก Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                Transform shopPanel = canvas.transform.Find("AutoShopUI");
                if (shopPanel != null)
                {
                    var cg = shopPanel.GetComponent<CanvasGroup>();
                    if (cg != null)
                    {
                        cg.alpha = 0;
                        cg.interactable = false;
                        cg.blocksRaycasts = false;
                    }
                    // บังคับปิด GameObject ด้วยเพื่อให้แน่ใจว่าปิดสนิท
                    shopPanel.gameObject.SetActive(false);
                    Debug.Log("ปิดร้านค้า (จาก Canvas)");
                }
            }
        }
    }
    
    /// <summary>
    /// เปิดหน้าต่างร้านค้า
    /// </summary>
    public void OpenShopPanel()
    {
        if (shopRoot != null)
        {
            // เปิด GameObject ก่อน
            shopRoot.SetActive(true);
            
            var cg = shopRoot.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 1;
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }
            Debug.Log("เปิดร้านค้า (GameObject.SetActive(true))");
        }
        else
        {
            // ถ้า shopRoot เป็น null ให้หาจาก Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                Transform shopPanel = canvas.transform.Find("AutoShopUI");
                if (shopPanel != null)
                {
                    // เปิด GameObject ก่อน
                    shopPanel.gameObject.SetActive(true);
                    
                    var cg = shopPanel.GetComponent<CanvasGroup>();
                    if (cg != null)
                    {
                        cg.alpha = 1;
                        cg.interactable = true;
                        cg.blocksRaycasts = true;
                    }
                    Debug.Log("เปิดร้านค้า (จาก Canvas)");
                }
            }
        }
    }
    
    /// <summary>
    /// เปิด/ปิดหน้าต่างร้านค้า (สำหรับปุ่ม Toggle)
    /// </summary>
    public void ToggleShopPanel()
    {
        if (shopRoot != null)
        {
            var cg = shopRoot.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                bool isVisible = cg.alpha > 0;
                cg.alpha = isVisible ? 0 : 1;
                cg.interactable = !isVisible;
                cg.blocksRaycasts = !isVisible;
                Debug.Log(isVisible ? "ปิดร้านค้า" : "เปิดร้านค้า");
            }
            else
            {
                bool isActive = shopRoot.activeSelf;
                shopRoot.SetActive(!isActive);
                Debug.Log(isActive ? "ปิดร้านค้า" : "เปิดร้านค้า");
            }
        }
        else
        {
            // ถ้า shopRoot เป็น null ให้หาจาก Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                Transform shopPanel = canvas.transform.Find("AutoShopUI");
                if (shopPanel != null)
                {
                    var cg = shopPanel.GetComponent<CanvasGroup>();
                    if (cg != null)
                    {
                        bool isVisible = cg.alpha > 0;
                        cg.alpha = isVisible ? 0 : 1;
                        cg.interactable = !isVisible;
                        cg.blocksRaycasts = !isVisible;
                        Debug.Log(isVisible ? "ปิดร้านค้า (จาก Canvas)" : "เปิดร้านค้า (จาก Canvas)");
                    }
                    else
                    {
                        bool isActive = shopPanel.gameObject.activeSelf;
                        shopPanel.gameObject.SetActive(!isActive);
                        Debug.Log(isActive ? "ปิดร้านค้า (จาก Canvas)" : "เปิดร้านค้า (จาก Canvas)");
                    }
                }
            }
        }
    }
#endif
}
