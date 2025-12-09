using UnityEngine;
using System.Collections;

/// <summary>
/// จัดการระบบ Turn-Based Combat สำหรับเกมแบบ Tibia
/// </summary>
public class TurnManager : MonoBehaviour
{
    [Header("⚔️ Turn Settings")]
    [Tooltip("ระยะเริ่มการต่อสู้")]
    public float combatStartRange = 2.0f;
    [Tooltip("ความเร็วในการวิ่งไปหาเป้าหมาย")]
    public float moveSpeed = 3.0f;
    [Tooltip("ระยะโจมตี")]
    public float attackRange = 1.5f;
    [Tooltip("ดีเลย์ระหว่างรอบ (วินาที)")]
    public float turnDelay = 1.0f;
    
    [Header("🎯 References")]
    [Tooltip("Player GameObject")]
    public GameObject player;
    [Tooltip("Combat Manager")]
    public CombatManager combatManager;
    [Tooltip("Target Manager")]
    public TargetManager targetManager;
    [Tooltip("Player Controller")]
    public PlayerController playerController;
    
    // Enum สำหรับสถานะการต่อสู้
    public enum CombatState
    {
        None,           // ไม่มีการต่อสู้
        Moving,         // Player กำลังวิ่งไปหา Enemy
        Combat,         // อยู่ในการต่อสู้แบบ Turn-based
        PlayerTurn,     // รอบของ Player
        EnemyTurn,      // รอบของ Enemy
        CombatEnd       // จบการต่อสู้
    }
    
    // Events
    public System.Action OnCombatStarted;
    public System.Action OnPlayerTurnStart;
    public System.Action OnEnemyTurnStart;
    public System.Action OnCombatEnded;
    public System.Action<GameObject> OnEnemyKilled;
    
    // Private variables
    private CombatState currentState = CombatState.None;
    private GameObject currentTarget;
    private bool isMovingToTarget = false;
    private bool canStartCombat = true;
    
    void Start()
    {
        InitializeComponents();
        Debug.Log("🔄 TurnManager initialized successfully!");
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
                Debug.LogError("❌ Player not found! TurnManager requires a Player object.");
            }
        }
        
        // หา CombatManager ถ้ายังไม่มี
        if (combatManager == null)
        {
            combatManager = FindObjectOfType<CombatManager>();
        }
        
        // หา TargetManager ถ้ายังไม่มี
        if (targetManager == null)
        {
            targetManager = FindObjectOfType<TargetManager>();
        }
        
        // หา PlayerController ถ้ายังไม่มี
        if (playerController == null && player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }
    }
    
    void Update()
    {
        HandleCombatFlow();
        CheckForCombatStart();
    }
    
    /// <summary>
    /// จบการต่อสู้
    /// </summary>
    public void EndCombat()
    {
        if (!IsInCombat()) return;
        
        currentState = CombatState.CombatEnd;
        
        // เรียก Events
        OnCombatEnded?.Invoke();
        
        Debug.Log("🏁 Combat ended!");
    }
    
    /// <summary>
    /// เรียกเมื่อ Enemy ตาย
    /// </summary>
    public void OnEnemyDied(GameObject deadEnemy)
    {
        if (deadEnemy == null) return;
        
        Debug.Log($"💀 TurnManager: Enemy {deadEnemy.name} died");
        
        // ถ้า Enemy ที่ตายเป็น currentTarget ให้เคลียร์เป้าหมาย
        if (currentTarget == deadEnemy)
        {
            currentTarget = null;
            Debug.Log("🎯 Cleared dead enemy from current target");
        }
        
        // ตรวจสอบว่ามี Enemy อื่นอยู่ในระบบหรือไม่
        CheckForRemainingEnemies();
        
        // ถ้าไม่มี Enemy เหลืออยู่ ให้จบการต่อสู้
        if (!HasRemainingEnemies())
        {
            Debug.Log("🎉 All enemies defeated! Ending combat.");
            EndCombat();
        }
        else
        {
            // ถ้ายังมี Enemy อยู่ ให้กลับไป Player Turn เพื่อเลือกเป้าหมายใหม่
            if (currentState == CombatState.EnemyTurn)
            {
                Debug.Log("🔄 Enemy died during enemy turn, switching to player turn");
                StartCoroutine(SwitchToPlayerTurnAfterDelay());
            }
        }
    }
    
    /// <summary>
    /// ตรวจสอบว่ามี Enemy เหลืออยู่ในระบบหรือไม่
    /// </summary>
    void CheckForRemainingEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int aliveEnemies = 0;
        
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
                if (enemyStats != null && !enemyStats.IsDead())
                {
                    aliveEnemies++;
                }
            }
        }
        
        Debug.Log($"🔍 Found {aliveEnemies} alive enemies");
    }
    
    /// <summary>
    /// ตรวจสอบว่ามี Enemy ที่ยังมีชีวิตอยู่หรือไม่
    /// </summary>
    bool HasRemainingEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
                if (enemyStats != null && !enemyStats.IsDead())
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// สลับไป Player Turn หลังจาก delay
    /// </summary>
    IEnumerator SwitchToPlayerTurnAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        
        if (IsInCombat())
        {
            currentState = CombatState.PlayerTurn;
            Debug.Log("🔄 Switched to Player Turn after enemy death");
        }
    }
    
    /// <summary>
    /// จัดการการไหลของการต่อสู้
    /// </summary>
    void HandleCombatFlow()
    {
        switch (currentState)
        {
            case CombatState.Moving:
                // การวิ่งไปหาเป้าหมายจัดการโดย MoveToTargetCoroutine แล้ว
                break;
                
            case CombatState.PlayerTurn:
                // รอ Player โจมตี
                break;
                
            case CombatState.EnemyTurn:
                // รอ Enemy โจมตี
                break;
        }
    }
    
    /// <summary>
    /// ตรวจสอบการเริ่มการต่อสู้
    /// </summary>
    void CheckForCombatStart()
    {
        if (!canStartCombat) return;
        
        // อนุญาตให้เริ่มการต่อสู้ใหม่ถ้ามีเป้าหมายใหม่หลังจาก Enemy ตาย
        if (currentState != CombatState.None && currentState != CombatState.CombatEnd) return;
        
        if (targetManager != null && targetManager.HasTarget())
        {
            GameObject newTarget = targetManager.GetCurrentTarget();
            if (newTarget != null && newTarget != currentTarget)
            {
                currentTarget = newTarget;
                Debug.Log($"🎯 New target detected: {currentTarget.name}");
            }
            
            if (currentTarget != null)
            {
                float distance = Vector2.Distance(player.transform.position, currentTarget.transform.position);
                
                // ตรวจสอบว่า Enemy ยังมีชีวิตอยู่
                EnemyStats enemyStats = currentTarget.GetComponent<EnemyStats>();
                if (enemyStats != null && enemyStats.IsDead())
                {
                    Debug.Log($"💀 Target {currentTarget.name} is already dead, skipping...");
                    return;
                }
                
                // ถ้าอยู่ในระยะเริ่มการต่อสู้ ให้เริ่มวิ่งไปหาเป้าหมาย
                if (distance <= combatStartRange && distance > attackRange)
                {
                    StartMovingToTarget();
                }
                // ถ้าอยู่ในระยะโจมตีแล้ว ให้เริ่มการต่อสู้ทันที
                else if (distance <= attackRange)
                {
                    StartCombat();
                }
            }
        }
    }
    
    /// <summary>
    /// เริ่มการวิ่งไปหาเป้าหมาย
    /// </summary>
    void StartMovingToTarget()
    {
        if (currentTarget == null) return;
        
        currentState = CombatState.Moving;
        isMovingToTarget = true;
        
        // ปิดการควบคุม Player ชั่วคราว
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        
        Debug.Log($"🏃 Moving to target: {currentTarget.name}");
        StartCoroutine(MoveToTargetCoroutine());
    }
    
    /// <summary>
    /// Coroutine สำหรับการวิ่งไปหาเป้าหมาย
    /// </summary>
    IEnumerator MoveToTargetCoroutine()
    {
        while (isMovingToTarget && currentTarget != null)
        {
            float distance = Vector2.Distance(player.transform.position, currentTarget.transform.position);
            
            // ถ้าถึงระยะโจมตีแล้ว
            if (distance <= attackRange)
            {
                isMovingToTarget = false;
                StartCombat();
                yield break;
            }
            
            // วิ่งไปหาเป้าหมาย
            Vector2 direction = (currentTarget.transform.position - player.transform.position).normalized;
            player.transform.position = Vector2.MoveTowards(
                player.transform.position, 
                currentTarget.transform.position, 
                moveSpeed * Time.deltaTime
            );
            
            // หันหน้าตามทิศทาง
            if (direction.x > 0)
            {
                player.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (direction.x < 0)
            {
                player.transform.localScale = new Vector3(-1, 1, 1);
            }
            
            yield return null;
        }
        
        // ถ้าหาเป้าหมายไม่ได้ ให้กลับสู่สถานะปกติ
        if (isMovingToTarget)
        {
            StopMovingToTarget();
        }
    }
    
    /// <summary>
    /// หยุดการวิ่งไปหาเป้าหมาย
    /// </summary>
    void StopMovingToTarget()
    {
        isMovingToTarget = false;
        currentState = CombatState.None;
        
        // เปิดการควบคุม Player คืน
        if (playerController != null)
        {
            playerController.enabled = true;
        }
        
        Debug.Log("🛑 Stopped moving to target");
    }
    
    /// <summary>
    /// เริ่มการต่อสู้แบบ Turn-based
    /// </summary>
    void StartCombat()
    {
        if (currentState == CombatState.Combat) return;
        
        currentState = CombatState.Combat;
        
        // เปิดการควบคุม Player คืน
        if (playerController != null)
        {
            playerController.enabled = true;
        }
        
        Debug.Log("⚔️ Combat started! Enemy attacks first!");
        
        OnCombatStarted?.Invoke();
        
        // Enemy โจมตีก่อนเสมอ
        StartCoroutine(StartEnemyTurn());
    }
    
    /// <summary>
    /// เริ่มรอบของ Enemy
    /// </summary>
    IEnumerator StartEnemyTurn()
    {
        currentState = CombatState.EnemyTurn;
        OnEnemyTurnStart?.Invoke();
        
        Debug.Log("👹 Enemy's turn!");
        
        // ให้ Enemy โจมตี
        if (currentTarget != null)
        {
            EnemyController enemyController = currentTarget.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                // บังคับให้ Enemy โจมตีในรอบของมัน
                enemyController.PerformAttack();
            }
        }
        
        // รอให้ Enemy โจมตีเสร็จ
        yield return new WaitForSeconds(1.5f);
        
        // ตรวจสอบว่า Enemy ยังมีชีวิตอยู่หรือไม่
        if (currentTarget != null)
        {
            EnemyStats enemyStats = currentTarget.GetComponent<EnemyStats>();
            if (enemyStats != null && enemyStats.IsDead())
            {
                EndCombat(true);
                yield break;
            }
        }
        
        // ตรวจสอบว่า Player ตายหรือไม่
        if (player != null)
        {
            PlayerStatsManager playerStats = player.GetComponent<PlayerStatsManager>();
            if (playerStats != null && playerStats.IsDead())
            {
                EndCombat(false);
                yield break;
            }
        }
        
        // เริ่มรอบของ Player ถ้ายังต่อสู้อยู่
        if (currentState == CombatState.EnemyTurn)
        {
            StartCoroutine(StartPlayerTurn());
        }
    }
    
    /// <summary>
    /// เริ่มรอบของ Player
    /// </summary>
    IEnumerator StartPlayerTurn()
    {
        currentState = CombatState.PlayerTurn;
        OnPlayerTurnStart?.Invoke();
        
        Debug.Log("🗡️ Player's turn!");
        
        // Player โจมตีอัตโนมัติในรอบของตัวเอง
        if (combatManager != null && currentTarget != null)
        {
            // ตรวจสอบว่า CombatManager มีเป้าหมายเดียวกับ TurnManager
            if (combatManager.GetCurrentTarget() != currentTarget)
            {
                // ซิงโครไนซ์เป้าหมายกับ TargetManager
                if (targetManager != null)
                {
                    targetManager.SelectTarget(currentTarget);
                }
            }
            
            combatManager.PerformAttack();
        }
        
        // รอให้ Player โจมตีเสร็จ
        yield return new WaitForSeconds(1.5f);
        
        // ตรวจสอบว่า Enemy ตายหรือไม่
        if (currentTarget != null)
        {
            EnemyStats enemyStats = currentTarget.GetComponent<EnemyStats>();
            if (enemyStats != null && enemyStats.IsDead())
            {
                EndCombat(true);
                yield break;
            }
        }
        
        // ตรวจสอบว่า Player ตายหรือไม่
        if (player != null)
        {
            PlayerStatsManager playerStats = player.GetComponent<PlayerStatsManager>();
            if (playerStats != null && playerStats.IsDead())
            {
                EndCombat(false);
                yield break;
            }
        }
        
        // กลับไปรอบของ Enemy อีกถ้ายังต่อสู้อยู่
        if (currentState == CombatState.PlayerTurn)
        {
            StartCoroutine(StartEnemyTurn());
        }
    }
    
    /// <summary>
    /// จบการต่อสู้
    /// </summary>
    /// <param name="playerWon">Player ชนะหรือไม่</param>
    void EndCombat(bool playerWon)
    {
        currentState = CombatState.CombatEnd;
        
        Debug.Log($"🏁 Combat ended! Player {(playerWon ? "won" : "lost")}!");
        
        OnCombatEnded?.Invoke();
        
        if (playerWon && currentTarget != null)
        {
            OnEnemyKilled?.Invoke(currentTarget);
        }
        
        // รีเซ็ตสถานะหลังจากดีเลย์
        StartCoroutine(ResetCombatState());
    }
    
    /// <summary>
    /// รีเซ็ตสถานะการต่อสู้
    /// </summary>
    IEnumerator ResetCombatState()
    {
        yield return new WaitForSeconds(1.0f); // ลดเวลารอเพื่อให้ตอบสนองเร็วขึ้น
        
        currentState = CombatState.None;
        currentTarget = null;
        isMovingToTarget = false;
        canStartCombat = true;
        
        // ตรวจสอบว่ามี Enemy อื่นอยู่ใกล้ๆ หรือไม่
        if (targetManager != null)
        {
            // หา Enemy ที่ใกล้ที่สุดที่ยังมีชีวิตอยู่
            GameObject nextEnemy = FindNearestAliveEnemy();
            if (nextEnemy != null)
            {
                Debug.Log($"🎯 Found next enemy: {nextEnemy.name}");
                targetManager.SelectTarget(nextEnemy);
            }
            else
            {
                // ไม่มี Enemy แล้ว ยกเลิกเป้าหมาย
                targetManager.DeselectTarget();
            }
        }
        
        Debug.Log("🔄 Combat state reset");
    }
    
    /// <summary>
    /// หา Enemy ที่ใกล้ที่สุดที่ยังมีชีวิตอยู่
    /// </summary>
    GameObject FindNearestAliveEnemy()
    {
        if (player == null) return null;
        
        // หา Enemy ทั้งหมดในฉาก
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;
        
        foreach (GameObject enemy in allEnemies)
        {
            // ตรวจสอบว่า Enemy ยังมีชีวิตอยู่
            EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
            if (enemyStats == null || enemyStats.IsDead()) continue;
            
            // ตรวจสอบระยะทาง
            float distance = Vector2.Distance(player.transform.position, enemy.transform.position);
            if (distance < nearestDistance && distance <= combatStartRange * 2) // ใช้ระยะที่กว้างขึ้นเพื่อหา Enemy ถัดไป
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }
        
        return nearestEnemy;
    }
    
    /// <summary>
    /// รับสถานะการต่อสู้ปัจจุบัน
    /// </summary>
    public CombatState GetCurrentState()
    {
        return currentState;
    }
    
    /// <summary>
    /// ตรวจสอบว่าอยู่ในการต่อสู้หรือไม่
    /// </summary>
    public bool IsInCombat()
    {
        return currentState == CombatState.Combat || 
               currentState == CombatState.PlayerTurn || 
               currentState == CombatState.EnemyTurn;
    }
    
    /// <summary>
    /// ตรวจสอบว่ากำลังวิ่งไปหาเป้าหมายหรือไม่
    /// </summary>
    public bool IsMovingToTarget()
    {
        return isMovingToTarget;
    }
    
    /// <summary>
    /// ตั้งค่าเป้าหมายปัจจุบัน
    /// </summary>
    public void SetCurrentTarget(GameObject target)
    {
        currentTarget = target;
    }
    
    /// <summary>
    /// บังคับให้จบการต่อสู้
    /// </summary>
    public void ForceEndCombat()
    {
        if (IsInCombat())
        {
            EndCombat(false);
        }
    }
    
    void OnDestroy()
    {
        // Cleanup events
        OnCombatStarted = null;
        OnPlayerTurnStart = null;
        OnEnemyTurnStart = null;
        OnCombatEnded = null;
        OnEnemyKilled = null;
    }
    
    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            // แสดงระยะเริ่มการต่อสู้
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.transform.position, combatStartRange);
            
            // แสดงระยะโจมตี
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.transform.position, attackRange);
            
            // แสดงเส้นไปยังเป้าหมาย
            if (currentTarget != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(player.transform.position, currentTarget.transform.position);
            }
        }
    }
}
