using UnityEngine;

/// <summary>
/// ควบคุมการติดตาม Player ของกล้องแบบ Tibiame
/// กล้องจะติดตาม Player อย่างราบรื่น
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // Player ที่จะติดตาม
    public float smoothSpeed = 5f; // ความราบรื่นในการติดตาม
    
    [Header("Camera Bounds")]
    public bool useBounds = false;
    public Vector2 minBounds;
    public Vector2 maxBounds;
    
    [Header("Offset Settings")]
    public Vector3 offset = new Vector3(0, 0, -10); // ระยะห่างจาก Player (Z=-10 สำหรับ 2D)
    
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        // หา Player อัตโนมัติถ้าไม่ได้กำหนด
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("🎯 พบ Player อัตโนมัติสำหรับกล้อง");
            }
            else
            {
                Debug.LogWarning("❌ ไม่พบ Player กรุณากำหนด Target หรือตั้ง Tag 'Player' ให้ Player GameObject");
            }
        }
        
        // ตั้งค่าตำแหน่งเริ่มต้นของกล้อง
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;
        
        // คำนวณตำแหน่งเป้าหมาย
        Vector3 desiredPosition = target.position + offset;
        
        // ใช้ SmoothDamp สำหรับการเคลื่อนที่ที่ราบรื่น
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, 1f / smoothSpeed);
        
        // ตรวจสอบขอบเขตถ้าเปิดใช้งาน
        if (useBounds)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minBounds.x, maxBounds.x);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minBounds.y, maxBounds.y);
        }
        
        // อัปเดตตำแหน่งกล้อง
        transform.position = smoothedPosition;
    }
    
    /// <summary>
    /// ตั้งค่า Target ใหม่ (สำหรับเปลี่ยน Player หรือวัตถุอื่น)
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            Debug.Log($"🎯 เปลี่ยน Target ของกล้องเป็น: {target.name}");
        }
    }
    
    /// <summary>
    /// ตั้งค่าขอบเขตของกล้อง
    /// </summary>
    public void SetBounds(Vector2 min, Vector2 max)
    {
        minBounds = min;
        maxBounds = max;
        useBounds = true;
        Debug.Log($"📏 ตั้งค่าขอบเขตกล้อง: Min={min}, Max={max}");
    }
    
    /// <summary>
    /// สลับการใช้งานขอบเขต
    /// </summary>
    public void ToggleBounds()
    {
        useBounds = !useBounds;
        Debug.Log($"📏 {(useBounds ? "เปิด" : "ปิด")} การใช้งานขอบเขตกล้อง");
    }
    
    /// <summary>
    /// ย้ายกล้องไปยังตำแหน่งทันที (ไม่ใช้ Smooth)
    /// </summary>
    public void TeleportToTarget()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            Debug.Log("⚡ ย้ายกล้องไปยัง Target ทันที");
        }
    }
    
    // วาด Gizmos สำหรับดูขอบเขตใน Editor
    void OnDrawGizmosSelected()
    {
        if (useBounds)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(new Vector3((minBounds.x + maxBounds.x) / 2, (minBounds.y + maxBounds.y) / 2, 0), 
                               new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0));
        }
    }
}
