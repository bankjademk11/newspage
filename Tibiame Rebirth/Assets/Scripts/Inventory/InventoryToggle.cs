using UnityEngine;
using UnityEngine.UI;

public class InventoryToggle : MonoBehaviour
{
    [Header("การตั้งค่า UI")]
    public GameObject mainInventoryUI;
    public Button toggleButton;
    
    [Header("ตัวเลือกเพิ่มเติม")]
    public bool pauseGameWhenOpen = false;
    public bool showCursorWhenOpen = true;
    
    private bool isInventoryOpen = false;

    void Start()
    {
        // ซ่อน UI ตอนเริ่มเกม
        if (mainInventoryUI != null)
        {
            mainInventoryUI.SetActive(false);
        }
        
        // เชื่อมปุ่ม (ถ้ามี)
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleInventory);
        }
    }

    void Update()
    {
        // กดปุ่ม I เพื่อเปิด/ปิด
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }
    
    public void ToggleInventory()
    {
        if (mainInventoryUI == null) return;
        
        // เปลี่ยนสถานะ
        isInventoryOpen = !isInventoryOpen;
        mainInventoryUI.SetActive(isInventoryOpen);
        
        // จัดการเวลาเกม
        if (pauseGameWhenOpen)
        {
            Time.timeScale = isInventoryOpen ? 0f : 1f;
        }
        
        // จัดการ Cursor
        if (showCursorWhenOpen)
        {
            Cursor.lockState = isInventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isInventoryOpen;
        }
        
        Debug.Log(isInventoryOpen ? "🎒 เปิด Inventory" : "🎒 ปิด Inventory");
    }
    
    // ฟังก์ชันสำหรับเรียกจากที่อื่น
    public void OpenInventory()
    {
        if (!isInventoryOpen)
            ToggleInventory();
    }
    
    public void CloseInventory()
    {
        if (isInventoryOpen)
            ToggleInventory();
    }
}
