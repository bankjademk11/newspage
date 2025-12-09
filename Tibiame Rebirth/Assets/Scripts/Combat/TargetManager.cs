using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// จัดการระบบเลือกเป้าหมายสำหรับการต่อสู้แบบ Tibia
/// </summary>
public class TargetManager : MonoBehaviour
{
    [Header("🎯 Target Settings")]
    [Tooltip("ระยะทางสูงสุดในการเลือกเป้าหมาย")]
    public float maxTargetDistance = 10f;
    [Tooltip("เลเย์เออร์ของศัตรู")]
    public LayerMask enemyLayer;
    
    [Header("🎮 Input Settings")]
    [Tooltip("ปุ่มสำหรับเลือกเป้าหมาย")]
    public KeyCode targetButton = KeyCode.Mouse0;
    [Tooltip("ปุ่มสำหรับยกเลิกเป้าหมาย")]
    public KeyCode cancelTargetButton = KeyCode.Escape;
    
    [Header("🔍 Visual Settings")]
    [Tooltip("สีของเป้าหมายที่เลือก")]
    public Color targetHighlightColor = Color.red;
    [Tooltip("ความโปร่งของไฮไลท์")]
    [Range(0.3f, 1f)]
    public float highlightAlpha = 0.8f;
    [Tooltip("เปิดใช้งานไฮไลท์เป้าหมาย")]
    public bool enableHighlight = false;
    
    // Events
    public System.Action<GameObject> OnTargetSelected;
    public System.Action<GameObject> OnTargetDeselected;
    public System.Action<GameObject> OnTargetChanged;
    
    // Private variables
    private GameObject currentTarget;
    private GameObject player;
    private Camera mainCamera;
    private Dictionary<GameObject, SpriteRenderer> targetRenderers = new Dictionary<GameObject, SpriteRenderer>();
    private Dictionary<GameObject, Color> originalColors = new Dictionary<GameObject, Color>();
    private List<GameObject> enemiesInRange = new List<GameObject>();
    
    void Start()
    {
        InitializeComponents();
        Debug.Log("🎯 TargetManager initialized successfully!");
    }
    
    /// <summary>
    /// ตั้งค่าคอมโพเนนต์เริ่มต้น
    /// </summary>
    void InitializeComponents()
    {
        // หา Player ถ้ายังไม่มี
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("❌ Player not found! TargetManager requires a Player object.");
            }
        }
        
        // หา Camera ถ้ายังไม่มี
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("❌ Main camera not found! TargetManager requires a camera.");
            }
        }
    }
    
    void Update()
    {
        HandleInput();
        UpdateTargetHighlight();
        CheckTargetDistance();
    }
    
    /// <summary>
    /// จัดการการเลือกเป้าหมาย
    /// </summary>
    void HandleTargetSelection()
    {
        if (Input.GetMouseButtonDown(0)) // คลิกซ้าย
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, enemyLayer);
            
            if (hit.collider != null)
            {
                GameObject clickedObject = hit.collider.gameObject;
                
                // ตรวจสอบว่าเป็น Enemy หรือไม่
                if (clickedObject.CompareTag("Enemy"))
                {
                    // ตรวจสอบว่า Enemy ยังมีชีวิตอยู่หรือไม่
                    EnemyStats enemyStats = clickedObject.GetComponent<EnemyStats>();
                    if (enemyStats != null && enemyStats.IsDead())
                    {
                        Debug.Log($"💀 {clickedObject.name} is already dead!");
                        return;
                    }
                    
                    if (currentTarget == clickedObject)
                    {
                        DeselectTarget();
                    }
                    else
                    {
                        SelectTarget(clickedObject);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// เรียกเมื่อ Enemy ตาย
    /// </summary>
    public void OnEnemyDied(GameObject deadEnemy)
    {
        if (deadEnemy == null) return;
        
        Debug.Log($"💀 TargetManager: Enemy {deadEnemy.name} died");
        
        // ถ้า Enemy ที่ตายเป็น currentTarget ให้เคลียร์เป้าหมาย
        if (currentTarget == deadEnemy)
        {
            currentTarget = null;
            Debug.Log("🎯 Cleared dead enemy from current target");
            
            // เรียก Events
            OnTargetDeselected?.Invoke(deadEnemy);
        }
        
        // ลบ Enemy ที่ตายออกจากรายการ
        if (enemiesInRange.Contains(deadEnemy))
        {
            enemiesInRange.Remove(deadEnemy);
            Debug.Log($"🗑️ Removed dead enemy from enemies list");
        }
    }
    
    /// <summary>
    /// จัดการ Input
    /// </summary>
    void HandleInput()
    {
        // เลือกเป้าหมายจากเมาส์
        HandleTargetSelection();
        
        // ยกเลิกเป้าหมาย (ESC)
        if (Input.GetKeyDown(cancelTargetButton))
        {
            DeselectTarget();
        }
        
        // Tab เปลี่ยนเป้าหมาย
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SelectNextTarget();
        }
    }
    
    /// <summary>
    /// พยายามเลือกเป้าหมายจากตำแหน่งเมาส์
    /// </summary>
    void TrySelectTarget()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, enemyLayer);
        
        if (hit.collider != null)
        {
            GameObject target = hit.collider.gameObject;
            
            // ตรวจสอบระยะทาง
            if (IsWithinRange(target))
            {
                SelectTarget(target);
            }
            else
            {
                Debug.Log($"📏 Target {target.name} is too far away!");
            }
        }
    }
    
    /// <summary>
    /// เลือกเป้าหมาย
    /// </summary>
    public void SelectTarget(GameObject target)
    {
        if (target == currentTarget) return;
        
        GameObject previousTarget = currentTarget;
        currentTarget = target;
        
        // เก็บข้อมูลการแสดงผลของเป้าหมายใหม่
        StoreTargetVisualInfo(target);
        
        // ไฮไลท์เป้าหมายใหม่
        HighlightTarget(target);
        
        // ยกเลิกไฮไลท์เป้าหมายเก่า
        if (previousTarget != null)
        {
            RemoveHighlight(previousTarget);
        }
        
        Debug.Log($"🎯 Selected target: {target.name}");
        
        // เรียก Events
        OnTargetSelected?.Invoke(target);
        OnTargetChanged?.Invoke(target);
    }
    
    /// <summary>
    /// ยกเลิกการเลือกเป้าหมาย
    /// </summary>
    public void DeselectTarget()
    {
        if (currentTarget == null) return;
        
        GameObject previousTarget = currentTarget;
        RemoveHighlight(currentTarget);
        currentTarget = null;
        
        Debug.Log("❌ Deselected target");
        
        // เรียก Events
        OnTargetDeselected?.Invoke(previousTarget);
        OnTargetChanged?.Invoke(null);
    }
    
    /// <summary>
    /// เลือกเป้าหมายถัดไป (Tab)
    /// </summary>
    void SelectNextTarget()
    {
        GameObject[] enemies = FindAllEnemiesInRange();
        
        if (enemies.Length == 0)
        {
            Debug.Log("🔍 No enemies in range!");
            return;
        }
        
        if (enemies.Length == 1)
        {
            SelectTarget(enemies[0]);
            return;
        }
        
        // หาเป้าหมายถัดไปในลิสต์
        int currentIndex = -1;
        if (currentTarget != null)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i] == currentTarget)
                {
                    currentIndex = i;
                    break;
                }
            }
        }
        
        int nextIndex = (currentIndex + 1) % enemies.Length;
        SelectTarget(enemies[nextIndex]);
    }
    
    /// <summary>
    /// หาศัตรูทั้งหมดในระยะ
    /// </summary>
    GameObject[] FindAllEnemiesInRange()
    {
        List<GameObject> enemiesInRange = new List<GameObject>();
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.transform.position, maxTargetDistance, enemyLayer);
        
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != player)
            {
                enemiesInRange.Add(collider.gameObject);
            }
        }
        
        return enemiesInRange.ToArray();
    }
    
    /// <summary>
    /// ตรวจสอบว่าเป้าหมายอยู่ในระยะหรือไม่
    /// </summary>
    bool IsWithinRange(GameObject target)
    {
        if (player == null || target == null) return false;
        
        float distance = Vector2.Distance(player.transform.position, target.transform.position);
        return distance <= maxTargetDistance;
    }
    
    /// <summary>
    /// เก็บข้อมูลการแสดงผลของเป้าหมาย
    /// </summary>
    void StoreTargetVisualInfo(GameObject target)
    {
        if (target == null) return;
        
        SpriteRenderer renderer = target.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            if (!targetRenderers.ContainsKey(target))
            {
                targetRenderers[target] = renderer;
                originalColors[target] = renderer.color;
            }
        }
    }
    
    /// <summary>
    /// ไฮไลท์เป้าหมาย - ปิดการใช้งานเพื่อให้ Enemy แสดงสีปกติ 100%
    /// </summary>
    void HighlightTarget(GameObject target)
    {
        // ไม่ทำอะไรเลย - ให้ Enemy แสดงสีเดิมของ Sprite ตลอดเวลา
        // if (target == null || !enableHighlight) return;
        // 
        // SpriteRenderer renderer = target.GetComponent<SpriteRenderer>();
        // if (renderer != null)
        // {
        //     Color highlightColor = targetHighlightColor;
        //     highlightColor.a = highlightAlpha;
        //     renderer.color = highlightColor;
        // }
    }
    
    /// <summary>
    /// ยกเลิกไฮไลท์เป้าหมาย
    /// </summary>
    void RemoveHighlight(GameObject target)
    {
        if (target == null) return;
        
        if (targetRenderers.ContainsKey(target) && originalColors.ContainsKey(target))
        {
            SpriteRenderer renderer = targetRenderers[target];
            renderer.color = originalColors[target];
        }
    }
    
    /// <summary>
    /// อัปเดตไฮไลท์เป้าหมาย
    /// </summary>
    void UpdateTargetHighlight()
    {
        if (currentTarget != null)
        {
            // ตรวจสอบว่าเป้าหมายถูกทำลายไปแล้วหรือไม่
            if (currentTarget == null)
            {
                DeselectTarget();
                return;
            }
            
            // ตรวจสอบว่าเป้าหมายยังมีชีวิตอยู่หรือไม่
            EnemyStats enemyStats = currentTarget.GetComponent<EnemyStats>();
            if (enemyStats != null && enemyStats.IsDead())
            {
                DeselectTarget();
                CleanupDeadTarget(currentTarget);
                return;
            }
            
            // อัปเดตไฮไลท์
            HighlightTarget(currentTarget);
        }
    }
    
    /// <summary>
    /// ตรวจสอบระยะทางของเป้าหมายปัจจุบัน
    /// </summary>
    void CheckTargetDistance()
    {
        if (currentTarget != null && !IsWithinRange(currentTarget))
        {
            Debug.Log($"📏 Target {currentTarget.name} is now out of range!");
            DeselectTarget();
        }
    }
    
    /// <summary>
    /// รับเป้าหมายปัจจุบัน
    /// </summary>
    public GameObject GetCurrentTarget()
    {
        return currentTarget;
    }
    
    /// <summary>
    /// ตรวจสอบว่ามีเป้าหมายหรือไม่
    /// </summary>
    public bool HasTarget()
    {
        return currentTarget != null;
    }
    
    /// <summary>
    /// ตรวจสอบว่าเป้าหมายอยู่ในระยะโจมตีหรือไม่
    /// </summary>
    public bool IsTargetInRange(float attackRange)
    {
        if (currentTarget == null || player == null) return false;
        
        float distance = Vector2.Distance(player.transform.position, currentTarget.transform.position);
        return distance <= attackRange;
    }
    
    /// <summary>
    /// รับระยะทางไปยังเป้าหมาย
    /// </summary>
    public float GetDistanceToTarget()
    {
        if (currentTarget == null || player == null) return Mathf.Infinity;
        
        return Vector2.Distance(player.transform.position, currentTarget.transform.position);
    }
    
    /// <summary>
    /// ลบข้อมูลเป้าหมายที่ตายแล้วออกจาก Dictionary
    /// </summary>
    void CleanupDeadTarget(GameObject deadTarget)
    {
        if (deadTarget == null) return;
        
        if (targetRenderers.ContainsKey(deadTarget))
        {
            targetRenderers.Remove(deadTarget);
        }
        
        if (originalColors.ContainsKey(deadTarget))
        {
            originalColors.Remove(deadTarget);
        }
        
        Debug.Log($"🧹 Cleaned up dead target: {deadTarget.name}");
    }
    
    void OnDestroy()
    {
        // Cleanup
        foreach (var kvp in targetRenderers)
        {
            if (kvp.Value != null && originalColors.ContainsKey(kvp.Key))
            {
                kvp.Value.color = originalColors[kvp.Key];
            }
        }
        
        targetRenderers.Clear();
        originalColors.Clear();
        
        // Cleanup events
        OnTargetSelected = null;
        OnTargetDeselected = null;
        OnTargetChanged = null;
    }
    
    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            // แสดงระยะเลือกเป้าหมาย
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.transform.position, maxTargetDistance);
            
            // แสดงเส้นไปยังเป้าหมาย
            if (currentTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(player.transform.position, currentTarget.transform.position);
            }
        }
    }
}
