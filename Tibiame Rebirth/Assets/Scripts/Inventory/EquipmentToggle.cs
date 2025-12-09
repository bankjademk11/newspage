using UnityEngine;
using UnityEngine.UI;

public class EquipmentToggle : MonoBehaviour
{
    [Header("Equipment Panel Settings")]
    public Canvas targetCanvas;
    public string equipmentPanelName = "EquipmentPanel";
    
    private GameObject equipmentPanel;
    
    void Start()
    {
        // หา Equipment Panel จาก Canvas
        if (targetCanvas != null)
        {
            equipmentPanel = targetCanvas.transform.Find(equipmentPanelName)?.gameObject;
        }
        else
        {
            // ถ้าไม่ได้กำหนด Canvas ให้หาจากฉาก
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                equipmentPanel = canvas.transform.Find(equipmentPanelName)?.gameObject;
            }
        }
        
        // บังคับปิด Equipment Panel ตอนเริ่มเกม
        ForceCloseEquipmentPanel();
    }
    
    /// <summary>
    /// เปิด/ปิดหน้าต่าง Equipment (สำหรับปุ่ม Toggle)
    /// </summary>
    public void ToggleEquipmentPanel()
    {
        if (equipmentPanel != null)
        {
            var cg = equipmentPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                bool isVisible = cg.alpha > 0;
                cg.alpha = isVisible ? 0 : 1;
                cg.interactable = !isVisible;
                cg.blocksRaycasts = !isVisible;
                
                // เปิด/ปิด GameObject ด้วย
                equipmentPanel.SetActive(!isVisible);
                
                Debug.Log(isVisible ? "ปิดหน้าต่าง Equipment" : "เปิดหน้าต่าง Equipment");
            }
            else
            {
                bool isActive = equipmentPanel.activeSelf;
                equipmentPanel.SetActive(!isActive);
                Debug.Log(isActive ? "ปิดหน้าต่าง Equipment (GameObject)" : "เปิดหน้าต่าง Equipment (GameObject)");
            }
        }
        else
        {
            Debug.LogWarning("ไม่พบ Equipment Panel กรุณาตรวจสอบชื่อหรือการตั้งค่า Canvas");
        }
    }
    
    /// <summary>
    /// เปิดหน้าต่าง Equipment
    /// </summary>
    public void OpenEquipmentPanel()
    {
        if (equipmentPanel != null)
        {
            // เปิด GameObject ก่อน
            equipmentPanel.SetActive(true);
            
            var cg = equipmentPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 1;
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }
            Debug.Log("เปิดหน้าต่าง Equipment (GameObject.SetActive(true))");
        }
        else
        {
            Debug.LogWarning("ไม่พบ Equipment Panel");
        }
    }
    
    /// <summary>
    /// ปิดหน้าต่าง Equipment
    /// </summary>
    public void CloseEquipmentPanel()
    {
        if (equipmentPanel != null)
        {
            var cg = equipmentPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 0;
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
            // บังคับปิด GameObject ด้วยเพื่อให้แน่ใจว่าปิดสนิท
            equipmentPanel.SetActive(false);
            Debug.Log("ปิดหน้าต่าง Equipment (GameObject.SetActive(false))");
        }
        else
        {
            Debug.LogWarning("ไม่พบ Equipment Panel");
        }
    }
    
    /// <summary>
    /// บังคับปิด Equipment Panel ให้แน่นอน
    /// </summary>
    void ForceCloseEquipmentPanel()
    {
        Debug.Log("🔒 กำลังบังคับปิดหน้าต่าง Equipment...");
        
        if (equipmentPanel != null)
        {
            // บังคับปิดทุกวิธี
            equipmentPanel.SetActive(false);
            
            var cg = equipmentPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 0;
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
            
            Debug.Log("✅ บังคับปิดหน้าต่าง Equipment สำเร็จ");
        }
        else
        {
            // ถ้าไม่พบ equipmentPanel ให้หาจาก Canvas โดยตรง
            Canvas canvas = targetCanvas != null ? targetCanvas : FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                Transform panel = canvas.transform.Find(equipmentPanelName);
                if (panel != null)
                {
                    // บังคับปิดทุกวิธี
                    panel.gameObject.SetActive(false);
                    
                    var cg = panel.GetComponent<CanvasGroup>();
                    if (cg != null)
                    {
                        cg.alpha = 0;
                        cg.interactable = false;
                        cg.blocksRaycasts = false;
                    }
                    
                    Debug.Log("✅ บังคับปิดหน้าต่าง Equipment จาก Canvas สำเร็จ");
                }
                else
                {
                    Debug.LogError("❌ ไม่พบ Equipment Panel ใน Canvas");
                }
            }
            else
            {
                Debug.LogError("❌ ไม่พบ Canvas");
            }
        }
    }
    
    /// <summary>
    /// ตั้งค่า Equipment Panel จากภายนอก
    /// </summary>
    public void SetEquipmentPanel(GameObject panel)
    {
        equipmentPanel = panel;
        if (panel != null)
        {
            Debug.Log($"ตั้งค่า Equipment Panel: {panel.name}");
        }
    }
}
