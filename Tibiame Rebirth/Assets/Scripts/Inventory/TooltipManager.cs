using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }
    
    [Header("Tooltip Settings")]
    public GameObject tooltipPrefab;
    public Canvas parentCanvas;
    public bool enableDebugLog = true;
    
    [Header("Timing")]
    public float showDelay = 0.5f; // รอเวลาก่อนแสดง tooltip (วินาที)
    public float hideDelay = 0.1f;  // รอเวลาก่อนซ่อน tooltip (วินาที)
    
    private ItemTooltip currentTooltip;
    private Coroutine showCoroutine;
    private Coroutine hideCoroutine;
    private ItemData pendingItem;
    private Vector3 pendingPosition;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // หา Canvas ถ้ายังไม่ได้กำหนด
        if (parentCanvas == null)
        {
            parentCanvas = FindObjectOfType<Canvas>();
            if (parentCanvas == null)
            {
                Debug.LogError("ไม่พบ Canvas ในฉาก! กรุณาเพิ่ม Canvas หรือกำหนด parentCanvas ใน TooltipManager");
                return;
            }
        }
        
        // สร้าง tooltip ตอนเริ่ม
        CreateTooltip();
    }

    void Start()
    {
        // ซ่อน tooltip ตอนเริ่ม
        if (currentTooltip != null)
        {
            currentTooltip.HideTooltip();
        }
    }

    private void CreateTooltip()
    {
        if (tooltipPrefab == null)
        {
            Debug.LogError("ไม่ได้กำหนด tooltipPrefab! กรุณาลาก ItemTooltip prefab มาใส่");
            return;
        }
        
        // สร้าง tooltip instance
        GameObject tooltipObj = Instantiate(tooltipPrefab, parentCanvas.transform);
        currentTooltip = tooltipObj.GetComponent<ItemTooltip>();
        
        if (currentTooltip == null)
        {
            Debug.LogError("tooltipPrefab ไม่มี Component ItemTooltip!");
            Destroy(tooltipObj);
            return;
        }
        
        // ตั้งค่าเริ่มต้น
        tooltipObj.name = "ItemTooltip_Instance";
        
        if (enableDebugLog)
            Debug.Log("สร้าง Tooltip สำเร็จแล้ว");
    }

    // แสดง tooltip (มี delay)
    public void ShowTooltip(ItemData item, Vector3 slotPosition)
    {
        if (item == null || currentTooltip == null) 
        {
            if (enableDebugLog)
                Debug.Log("❌ ShowTooltip: item หรือ currentTooltip เป็น null");
            return;
        }
        
        // ยกเลิก coroutine เก่า
        if (showCoroutine != null)
            StopCoroutine(showCoroutine);
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);
        
        // เก็บข้อมูลไว้แสดง
        pendingItem = item;
        pendingPosition = slotPosition;
        
        if (enableDebugLog)
            Debug.Log($"🎯 เริ่ม ShowTooltip: {item.itemName}");
        
        // เริ่ม coroutine แสดง tooltip
        showCoroutine = StartCoroutine(ShowTooltipDelayed());
    }

    private System.Collections.IEnumerator ShowTooltipDelayed()
    {
        yield return new WaitForSeconds(showDelay);
        
        if (pendingItem != null && currentTooltip != null)
        {
            currentTooltip.ShowTooltip(pendingItem, pendingPosition);
            
            if (enableDebugLog)
                Debug.Log($"แสดง Tooltip: {pendingItem.itemName}");
        }
        
        showCoroutine = null;
    }

    // ซ่อน tooltip (ทันที)
    public void HideTooltip()
    {
        // ยกเลิก coroutine ทั้งหมด
        if (showCoroutine != null)
        {
            StopCoroutine(showCoroutine);
            showCoroutine = null;
        }
        
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        
        // เริ่ม coroutine ซ่อน tooltip
        hideCoroutine = StartCoroutine(HideTooltipDelayed());
    }

    private System.Collections.IEnumerator HideTooltipDelayed()
    {
        yield return new WaitForSeconds(hideDelay);
        
        if (currentTooltip != null)
        {
            currentTooltip.HideTooltip();
            
            if (enableDebugLog)
                Debug.Log("ซ่อน Tooltip");
        }
        
        // ล้างข้อมูลรอการแสดง
        pendingItem = null;
        pendingPosition = Vector3.zero;
        
        hideCoroutine = null;
    }

    // ซ่อน tooltip ทันที (ไม่มี delay)
    public void HideTooltipImmediate()
    {
        // ยกเลิก coroutine ทั้งหมด
        if (showCoroutine != null)
        {
            StopCoroutine(showCoroutine);
            showCoroutine = null;
        }
        
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }
        
        if (currentTooltip != null)
        {
            currentTooltip.HideTooltip();
        }
        
        // ล้างข้อมูลรอการแสดง
        pendingItem = null;
        pendingPosition = Vector3.zero;
    }

    // อัปเดตตำแหน่ง tooltip (สำหรับ tooltip ที่กำลังแสดงอยู่)
    public void UpdateTooltipPosition(Vector3 slotPosition)
    {
        if (currentTooltip != null && pendingItem != null)
        {
            // ใช้ reflection เพื่อเรียก private method หรือสร้าง public method ใน ItemTooltip
            currentTooltip.ShowTooltip(pendingItem, slotPosition);
        }
    }

    // ตรวจสอบว่ากำลังแสดง tooltip อยู่หรือไม่
    public bool IsTooltipVisible()
    {
        if (currentTooltip != null)
        {
            return currentTooltip.gameObject.activeInHierarchy;
        }
        return false;
    }

    // สร้าง tooltip ใหม่ (ถ้าต้องการ recreate)
    public void RecreateTooltip()
    {
        if (currentTooltip != null)
        {
            Destroy(currentTooltip.gameObject);
            currentTooltip = null;
        }
        
        CreateTooltip();
    }

    // ตั้งค่า delay times
    public void SetDelays(float showDelayTime, float hideDelayTime)
    {
        showDelay = showDelayTime;
        hideDelay = hideDelayTime;
    }

    // Debug function
    [ContextMenu("Test Tooltip")]
    public void TestTooltip()
    {
        if (currentTooltip != null)
        {
            currentTooltip.TestTooltip();
        }
        else
        {
            Debug.LogWarning("ไม่มี tooltip สำหรับทดสอบ");
        }
    }

    void OnDestroy()
    {
        // ล้าง coroutine ทั้งหมด
        if (showCoroutine != null)
            StopCoroutine(showCoroutine);
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);
        
        // ล้าง singleton
        if (Instance == this)
            Instance = null;
    }
}
