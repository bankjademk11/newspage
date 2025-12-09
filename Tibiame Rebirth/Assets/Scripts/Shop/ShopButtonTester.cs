using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// สคริปต์สำหรับทดสอบปุ่มซื้อในร้านค้า
/// ใช้สำหรับตรวจสอบว่าปุ่มทำงานหรือไม่
/// </summary>
public class ShopButtonTester : MonoBehaviour
{
    [Header("Testing")]
    public bool enableTestMode = true;
    public KeyCode testKey = KeyCode.B;
    
    void Start()
    {
        Debug.Log("=== ShopButtonTester Start() ===");
        Debug.Log("กดปุ่ม B เพื่อทดสอบปุ่มซื้อทั้งหมด");
        
        // ตรวจสอบ ShopSlot ทั้งหมด
        ShopSlot[] allSlots = FindObjectsOfType<ShopSlot>();
        Debug.Log($"พบ ShopSlot ทั้งหมด {allSlots.Length} ช่อง");
        
        for (int i = 0; i < allSlots.Length; i++)
        {
            var slot = allSlots[i];
            Debug.Log($"ช่องที่ {i + 1}: {slot.gameObject.name}");
            Debug.Log($"  - buyButton: {(slot.buyButton != null ? "✅" : "❌")}");
            Debug.Log($"  - itemData: {(slot.itemData != null ? slot.itemData.itemName : "❌ null")}");
            
            if (slot.buyButton != null)
            {
                Debug.Log($"  - Button Interactable: {slot.buyButton.interactable}");
                Debug.Log($"  - Button onClick listeners: {slot.buyButton.onClick.GetPersistentEventCount()}");
            }
        }
        
        Debug.Log("=== ShopButtonTester Start() จบ ===");
    }
    
    void Update()
    {
        if (enableTestMode && Input.GetKeyDown(testKey))
        {
            TestAllButtons();
        }
    }
    
    /// <summary>
    /// ทดสอบปุ่มทั้งหมด
    /// </summary>
    [ContextMenu("Test All Shop Buttons")]
    public void TestAllButtons()
    {
        Debug.Log("=== TestAllButtons() เริ่มทำงาน ===");
        
        ShopSlot[] allSlots = FindObjectsOfType<ShopSlot>();
        
        for (int i = 0; i < allSlots.Length; i++)
        {
            var slot = allSlots[i];
            if (slot != null && slot.itemData != null)
            {
                Debug.Log($"🔥 ทดสอบช่องที่ {i + 1}: {slot.itemData.itemName}");
                
                // เรียกฟังก์ชันทดสอบ
                slot.TestButton();
                
                // ถ้ามีไอเท็ม ลองเรียก OnBuyClicked
                if (slot.itemData != null)
                {
                    Debug.Log($"🛒 ทดสอบการซื้อ: {slot.itemData.itemName}");
                    slot.OnBuyClicked();
                }
            }
        }
        
        Debug.Log("=== TestAllButtons() จบ ===");
    }
    
    /// <summary>
    /// ตรวจสอบสถานะปุ่ม
    /// </summary>
    [ContextMenu("Check Button Status")]
    public void CheckButtonStatus()
    {
        Debug.Log("=== CheckButtonStatus() ===");
        
        ShopSlot[] allSlots = FindObjectsOfType<ShopSlot>();
        
        for (int i = 0; i < allSlots.Length; i++)
        {
            var slot = allSlots[i];
            Debug.Log($"ช่องที่ {i + 1}: {slot.gameObject.name}");
            
            if (slot.buyButton != null)
            {
                Debug.Log($"  - Button พร้อมใช้งาน: ✅");
                Debug.Log($"  - Interactable: {slot.buyButton.interactable}");
                Debug.Log($"  - OnClick listeners: {slot.buyButton.onClick.GetPersistentEventCount()}");
                
                // ตรวจสอบว่ามี listener หรือไม่
                if (slot.buyButton.onClick.GetPersistentEventCount() == 0)
                {
                    Debug.LogWarning($"  ⚠️ ไม่มี OnClick listener!");
                }
            }
            else
            {
                Debug.LogError($"  ❌ Button component หายไป!");
            }
        }
        
        Debug.Log("=== CheckButtonStatus() จบ ===");
    }
    
    /// <summary>
    /// ซ่อมปุ่มทั้งหมด (เชื่อมต่อใหม่)
    /// </summary>
    [ContextMenu("Fix All Buttons")]
    public void FixAllButtons()
    {
        Debug.Log("=== FixAllButtons() เริ่มทำงาน ===");
        
        ShopSlot[] allSlots = FindObjectsOfType<ShopSlot>();
        
        for (int i = 0; i < allSlots.Length; i++)
        {
            var slot = allSlots[i];
            
            if (slot.buyButton == null)
            {
                Debug.LogWarning($"⚠️ ช่องที่ {i + 1} ไม่มี Button กำลังเพิ่มใหม่...");
                slot.buyButton = slot.gameObject.GetComponent<Button>();
                if (slot.buyButton == null)
                {
                    slot.buyButton = slot.gameObject.AddComponent<Button>();
                    Debug.Log($"✅ เพิ่ม Button ใหม่ให้ช่องที่ {i + 1}");
                }
            }
            
            // ลบ listener เก่าและเชื่อมต่อใหม่
            slot.buyButton.onClick.RemoveAllListeners();
            slot.buyButton.onClick.AddListener(slot.TestButton);
            slot.buyButton.onClick.AddListener(slot.OnBuyClicked);
            
            Debug.Log($"🔗 ซ่อมปุ่มช่องที่ {i + 1} เรียบร้อย");
        }
        
        Debug.Log("=== FixAllButtons() จบ ===");
    }
}
