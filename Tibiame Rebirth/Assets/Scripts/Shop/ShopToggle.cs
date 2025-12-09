using UnityEngine;

/// <summary>
/// สคริปต์สำหรับปุ่มเปิด/ปิดร้านค้า
/// ใช้สำหรับควบคุมการแสดงร้านค้า
/// </summary>
public class ShopToggle : MonoBehaviour
{
    [Header("Shop Settings")]
    public KeyCode toggleKey = KeyCode.S;
    public bool requireShiftKey = true;
    
    [Header("References")]
    public ShopGenerator shopGenerator;
    
    void Start()
    {
        Debug.Log("=== ShopToggle Start() ===");
        
        // หา ShopGenerator อัตโนมัติถ้าไม่ได้กำหนด
        if (shopGenerator == null)
        {
            shopGenerator = FindObjectOfType<ShopGenerator>();
            if (shopGenerator != null)
            {
                Debug.Log("✅ พบ ShopGenerator อัตโนมัติ");
            }
            else
            {
                Debug.LogError("❌ ไม่พบ ShopGenerator ในฉาก!");
            }
        }
        
        Debug.Log($"ปุ่มเปิดร้านค้า: {(requireShiftKey ? "Shift + " : "")}{toggleKey}");
        Debug.Log("=== ShopToggle Start() จบ ===");
    }
    
    void Update()
    {
        // ตรวจสอบการกดปุ่ม
        bool keyPressed = Input.GetKeyDown(toggleKey);
        bool shiftPressed = !requireShiftKey || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        
        if (keyPressed && shiftPressed)
        {
            ToggleShop();
        }
    }
    
    /// <summary>
    /// เปิด/ปิดร้านค้า
    /// </summary>
    public void ToggleShop()
    {
        Debug.Log("🏪 กดปุ่มเปิด/ปิดร้านค้า");
        
        if (shopGenerator != null)
        {
            shopGenerator.ToggleShopPanel();
        }
        else
        {
            Debug.LogError("❌ ไม่พบ ShopGenerator!");
        }
    }
    
    /// <summary>
    /// เปิดร้านค้า
    /// </summary>
    public void OpenShop()
    {
        Debug.Log("🏪 เปิดร้านค้า");
        
        if (shopGenerator != null)
        {
            shopGenerator.OpenShopPanel();
        }
        else
        {
            Debug.LogError("❌ ไม่พบ ShopGenerator!");
        }
    }
    
    /// <summary>
    /// ปิดร้านค้า
    /// </summary>
    public void CloseShop()
    {
        Debug.Log("🏪 ปิดร้านค้า");
        
        if (shopGenerator != null)
        {
            shopGenerator.CloseShopPanel();
        }
        else
        {
            Debug.LogError("❌ ไม่พบ ShopGenerator!");
        }
    }
    
    /// <summary>
    /// ตรวจสอบว่าร้านค้าเปิดอยู่หรือไม่
    /// </summary>
    public bool IsShopOpen()
    {
        if (shopGenerator == null) return false;
        
        // หา Canvas และตรวจสอบสถานะ
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            Transform shopPanel = canvas.transform.Find("AutoShopUI");
            if (shopPanel != null)
            {
                var canvasGroup = shopPanel.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    return canvasGroup.alpha > 0;
                }
                else
                {
                    return shopPanel.gameObject.activeSelf;
                }
            }
        }
        
        return false;
    }
}
