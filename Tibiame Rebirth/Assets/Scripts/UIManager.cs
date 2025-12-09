using UnityEngine;

/// <summary>
/// จัดการสถานะ UI ทั้งหมดในเกม
/// คอยตรวจสอบว่ามี UI เปิดอยู่หรือไม่เพื่อป้องกันการเคลื่อนที่ของ Player
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public InventoryToggle inventoryToggle;
    public EquipmentToggle equipmentToggle;
    public ShopToggle shopToggle;
    
    // Singleton pattern
    public static UIManager Instance { get; private set; }
    
    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // หา UI components อัตโนมัติถ้าไม่ได้กำหนด
        FindUIComponents();
    }
    
    /// <summary>
    /// ตรวจสอบว่ามี UI ใดเปิดอยู่หรือไม่
    /// </summary>
    /// <returns>true ถ้ามี UI เปิดอยู่, false ถ้าไม่มี</returns>
    public bool IsAnyUIOpen()
    {
        bool inventoryOpen = false;
        bool equipmentOpen = false;
        bool shopOpen = false;
        
        // ตรวจสอบ Inventory
        if (inventoryToggle != null)
        {
            // ตรวจสอบจาก GameObject ที่เปิดอยู่
            if (inventoryToggle.mainInventoryUI != null)
            {
                inventoryOpen = inventoryToggle.mainInventoryUI.activeSelf;
            }
        }
        
        // ตรวจสอบ Equipment
        if (equipmentToggle != null)
        {
            // หา Equipment Panel จาก Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                Transform equipmentPanel = canvas.transform.Find("EquipmentPanel");
                if (equipmentPanel != null)
                {
                    equipmentOpen = equipmentPanel.gameObject.activeSelf;
                }
            }
        }
        
        // ตรวจสอบ Shop
        if (shopToggle != null)
        {
            shopOpen = shopToggle.IsShopOpen();
        }
        
        bool anyUIOpen = inventoryOpen || equipmentOpen || shopOpen;
        
        if (anyUIOpen)
        {
            Debug.Log($"🚫 UI เปิดอยู่: Inventory={inventoryOpen}, Equipment={equipmentOpen}, Shop={shopOpen}");
        }
        
        return anyUIOpen;
    }
    
    /// <summary>
    /// หา UI components อัตโนมัติ
    /// </summary>
    void FindUIComponents()
    {
        if (inventoryToggle == null)
        {
            inventoryToggle = FindObjectOfType<InventoryToggle>();
        }
        
        if (equipmentToggle == null)
        {
            equipmentToggle = FindObjectOfType<EquipmentToggle>();
        }
        
        if (shopToggle == null)
        {
            shopToggle = FindObjectOfType<ShopToggle>();
        }
        
        Debug.Log("🔍 ค้นหา UI Components เสร็จสิ้น");
    }
    
    /// <summary>
    /// ปิด UI ทั้งหมด (สำหรับกรณีฉุกเฉิน)
    /// </summary>
    public void CloseAllUI()
    {
        Debug.Log("🔒 กำลังปิด UI ทั้งหมด...");
        
        if (inventoryToggle != null)
        {
            inventoryToggle.CloseInventory();
        }
        
        if (equipmentToggle != null)
        {
            equipmentToggle.CloseEquipmentPanel();
        }
        
        if (shopToggle != null)
        {
            shopToggle.CloseShop();
        }
        
        Debug.Log("✅ ปิด UI ทั้งหมดเรียบร้อย");
    }
}
