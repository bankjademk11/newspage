using UnityEngine;
using System.Collections;

/// <summary>
/// ควบคุมพฤติกรรมของศัตรู - AI, Movement, Combat (เวอร์ชันใหม่ - ไม่ใช้ Speed)
/// </summary>
public class EnemyController : MonoBehaviour
{
    [Header("🎯 AI Settings")]
    [Tooltip("โหมด AI")]
    public AIMode aiMode = AIMode.Aggressive;
    [Tooltip("ระยะทางในการตรวจจับผู้เล่น")]
    public float detectionRange = 5f;
    [Tooltip("ระยะทางในการโจมตี")]
    public float attackRange = 1.5f;
    [Tooltip("ความเร็วในการเคลื่อนที่")]
    public float moveSpeed = 2f;
    [Tooltip("ระยะทางที่จะหยุดติดตาม")]
    public float stopChaseDistance = 10f;
    
    [Header("⚔️ Combat Settings")]
    [Tooltip("ความเร็วในการโจมตี (ครั้งต่อวินาที)")]
    public float attackSpeed = 1f;
    [Tooltip("ความเสียหายพื้นฐาน")]
    public int baseDamage = 10;
    [Tooltip("โอกาสคริติคอล (0-1)")]
    [Range(0f, 1f)]
    public float criticalChance = 0.05f;
    
    [Header("🎮 Components")]
    [Tooltip("Enemy Stats")]
    public EnemyStats enemyStats;
    [Tooltip("Rigidbody2D")]
    public Rigidbody2D rb;
    [Tooltip("Animator")]
    public Animator animator;
    [Tooltip("Sprite Renderer")]
    public SpriteRenderer spriteRenderer;
    
    [Header("🎯 Target")]
    [Tooltip("เป้าหมายปัจจุบัน (Player)")]
    public Transform target;
    
    [Header("🎨 UI References")]
    [Tooltip("Combat UI สำหรับแสดง Damage Number")]
    public CombatUI combatUI;
    [Tooltip("Target Manager สำหรับ auto-target")]
    public TargetManager targetManager;
    [Tooltip("Combat Effect Manager สำหรับแสดง Effects")]
    public CombatEffectManager effectManager;
    [Tooltip("Turn Manager สำหรับ Turn-based Combat")]
    public TurnManager turnManager;
    
    // AI States
    private enum AIState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Retreat,
        Dead
    }
    
    private AIState currentState = AIState.Idle;
    private Vector2 patrolOrigin;
    private Vector2 patrolTarget;
    private float patrolTimer = 0f;
    private float attackTimer = 0f;
    private bool canAttack = true;
    private bool isFacingRight = true;
    
    // Events
    public System.Action<Transform> OnTargetDetected;
    public System.Action<Transform> OnTargetLost;
    public System.Action<int> OnDamageDealt;
    
    void Start()
    {
        InitializeComponents();
        SetupAI();
        Debug.Log($"🤖 {enemyStats.stats.enemyName} AI initialized in {aiMode} mode");
    }
    
    void Update()
    {
        if (enemyStats.IsDead())
        {
            if (currentState != AIState.Dead)
            {
                SetState(AIState.Dead);
            }
            return;
        }
        
        UpdateAI();
        UpdateTimers();
        HandleAnimation();
    }
    
    void FixedUpdate()
    {
        if (enemyStats.IsDead()) return;
        
        HandleMovement();
    }
    
    /// <summary>
    /// ตั้งค่าคอมโพเนนต์เริ่มต้น
    /// </summary>
    void InitializeComponents()
    {
        // หา EnemyStats ถ้ายังไม่มี
        if (enemyStats == null)
        {
            enemyStats = GetComponent<EnemyStats>();
            if (enemyStats == null)
            {
                Debug.LogError("❌ EnemyStats component not found!");
                return;
            }
        }
        
        // หา Rigidbody2D ถ้ายังไม่มี
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f;
                rb.freezeRotation = true;
            }
        }
        
        // หา Animator ถ้ายังไม่มี
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        
        // หา SpriteRenderer ถ้ายังไม่มี
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        // หา Player เป็นเป้าหมาย
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
        
        // หา UI Components ถ้ายังไม่มี
        if (combatUI == null)
        {
            combatUI = FindObjectOfType<CombatUI>();
        }
        
        if (targetManager == null)
        {
            targetManager = FindObjectOfType<TargetManager>();
        }
        
        // หา CombatEffectManager ถ้ายังไม่มี
        if (effectManager == null)
        {
            effectManager = FindObjectOfType<CombatEffectManager>();
        }
        
        // หา TurnManager ถ้ายังไม่มี
        if (turnManager == null)
        {
            turnManager = FindObjectOfType<TurnManager>();
        }
    }
    
    /// <summary>
    /// ตั้งค่า AI
    /// </summary>
    void SetupAI()
    {
        patrolOrigin = transform.position;
        GenerateNewPatrolTarget();
        
        // อัปเดตค่าจาก EnemyStats
        if (enemyStats != null)
        {
            var stats = enemyStats.GetStats();
            detectionRange = stats.detectionRange;
            attackRange = stats.attackRange;
            moveSpeed = stats.moveSpeed;
            attackSpeed = 1f / stats.attackCooldown;
            baseDamage = stats.attack;
        }
    }
    
    /// <summary>
    /// อัปเดต AI
    /// </summary>
    void UpdateAI()
    {
        switch (currentState)
        {
            case AIState.Idle:
                UpdateIdleState();
                break;
            case AIState.Patrol:
                UpdatePatrolState();
                break;
            case AIState.Chase:
                UpdateChaseState();
                break;
            case AIState.Attack:
                UpdateAttackState();
                break;
            case AIState.Retreat:
                UpdateRetreatState();
                break;
            case AIState.Dead:
                UpdateDeadState();
                break;
        }
    }
    
    /// <summary>
    /// อัปเดต Timers
    /// </summary>
    void UpdateTimers()
    {
        // Attack Timer
        if (!canAttack)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= 1f / attackSpeed)
            {
                canAttack = true;
                attackTimer = 0f;
            }
        }
        
        // Patrol Timer
        if (currentState == AIState.Patrol)
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer >= Random.Range(3f, 8f))
            {
                GenerateNewPatrolTarget();
                patrolTimer = 0f;
            }
        }
    }
    
    /// <summary>
    /// จัดการ Movement
    /// </summary>
    void HandleMovement()
    {
        Vector2 movement = Vector2.zero;
        
        switch (currentState)
        {
            case AIState.Patrol:
                movement = (patrolTarget - (Vector2)transform.position).normalized;
                break;
            case AIState.Chase:
                if (target != null)
                {
                    Vector2 direction = (target.position - transform.position).normalized;
                    movement = direction;
                }
                break;
            case AIState.Retreat:
                if (target != null)
                {
                    Vector2 direction = (transform.position - target.position).normalized;
                    movement = direction;
                }
                break;
        }
        
        // ใช้ Rigidbody2D เคลื่อนที่
        if (rb != null)
        {
            rb.velocity = movement * moveSpeed;
        }
        
        // หันหน้าตามทิศทาง
        if (movement.magnitude > 0.1f)
        {
            HandleFacing(movement.x);
        }
    }
    
    /// <summary>
    /// จัดการการหันหน้า
    /// </summary>
    void HandleFacing(float horizontalMovement)
    {
        if (horizontalMovement > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (horizontalMovement < 0 && isFacingRight)
        {
            Flip();
        }
    }
    
    /// <summary>
    /// กลับด้าน
    /// </summary>
    void Flip()
    {
        isFacingRight = !isFacingRight;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !isFacingRight;
        }
    }
    
    /// <summary>
    /// จัดการ Animation
    /// </summary>
    void HandleAnimation()
    {
        if (animator == null) return;
        
        // ตรวจสอบว่ากำลังเดินหรือไม่
        bool isWalking = rb.velocity.magnitude > 0.1f && 
                        (currentState == AIState.Patrol || 
                         currentState == AIState.Chase || 
                         currentState == AIState.Retreat);
        
        // ตั้งค่า Animation Parameters
        animator.SetBool("IsWalking", isWalking);
        
        // Debug log สำหรับตรวจสอบ animation
        if (Random.value < 0.02f) // 2% ต่อ frame เพื่อไม่ให้ log เยอะเกินไป
        {
            Debug.Log($"🎬 {enemyStats.stats.enemyName} Animation: Walking={isWalking}, Dead={enemyStats.IsDead()}, Attacking={currentState == AIState.Attack}");
        }
    }
    
    /// <summary>
    /// อัปเดตสถานะ Idle
    /// </summary>
    void UpdateIdleState()
    {
        // ตรวจจับเป้าหมาย
        if (DetectTarget())
        {
            SetState(AIState.Chase);
            return;
        }
        
        // เปลี่ยนเป็น Patrol หลังจากหน่วงเวลา
        if (Random.value < 0.01f) // 1% ต่อ frame
        {
            SetState(AIState.Patrol);
        }
    }
    
    /// <summary>
    /// อัปเดตสถานะ Patrol
    /// </summary>
    void UpdatePatrolState()
    {
        // ตรวจจับเป้าหมาย
        if (DetectTarget())
        {
            SetState(AIState.Chase);
            return;
        }
        
        // ตรวจสอบว่าถึงจุดหมายหรือไม่
        float distanceToTarget = Vector2.Distance(transform.position, patrolTarget);
        if (distanceToTarget < 0.5f)
        {
            SetState(AIState.Idle);
        }
    }
    
    /// <summary>
    /// อัปเดตสถานะ Chase
    /// </summary>
    void UpdateChaseState()
    {
        if (target == null)
        {
            SetState(AIState.Idle);
            return;
        }
        
        // ตรวจสอบว่า Player ยังมีชีวิตอยู่หรือไม่
        PlayerStatsManager playerStats = target.GetComponent<PlayerStatsManager>();
        if (playerStats != null && playerStats.IsDead())
        {
            SetState(AIState.Idle);
            OnTargetLost?.Invoke(target);
            return;
        }
        
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        
        // โจมตีถ้าอยู่ในระยะ
        if (distanceToTarget <= attackRange)
        {
            SetState(AIState.Attack);
            return;
        }
        
        // หยุดติดตามถ้าไกลเกินไป
        if (distanceToTarget > stopChaseDistance)
        {
            SetState(AIState.Idle);
            OnTargetLost?.Invoke(target);
            return;
        }
    }
    
    /// <summary>
    /// อัปเดตสถานะ Attack
    /// </summary>
    void UpdateAttackState()
    {
        if (target == null)
        {
            SetState(AIState.Idle);
            return;
        }
        
        // ถ้ามี TurnManager และอยู่ใน Turn-based mode ให้รอรอบ
        if (turnManager != null && turnManager.IsInCombat())
        {
            // รอให้ TurnManager เรียก PerformAttack ในรอบของ Enemy
            return;
        }
        
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        
        // โจมตีถ้าอยู่ในระยะและพร้อมโจมตี
        if (distanceToTarget <= attackRange && canAttack)
        {
            PerformAttack();
        }
        
        // กลับไล่ตามถ้าออกจากระยะโจมตี
        if (distanceToTarget > attackRange)
        {
            SetState(AIState.Chase);
        }
    }
    
    /// <summary>
    /// อัปเดตสถานะ Retreat
    /// </summary>
    void UpdateRetreatState()
    {
        if (target == null)
        {
            SetState(AIState.Idle);
            return;
        }
        
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        
        // หยุดถอยหลังถ้าอยู่ในระยะปลอดภัย
        if (distanceToTarget > detectionRange)
        {
            SetState(AIState.Idle);
        }
    }
    
    /// <summary>
    /// อัปเดตสถานะ Dead
    /// </summary>
    void UpdateDeadState()
    {
        // หยุดการเคลื่อนที่
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }
    
    /// <summary>
    /// ตรวจจับเป้าหมาย
    /// </summary>
    bool DetectTarget()
    {
        if (target == null) return false;
        
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        
        if (distanceToTarget <= detectionRange)
        {
            OnTargetDetected?.Invoke(target);
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// ทำการโจมตี
    /// </summary>
    public void PerformAttack()
    {
        if (!canAttack || target == null) return;
        
        // หา PlayerStatsManager ของเป้าหมาย
        PlayerStatsManager playerStats = target.GetComponent<PlayerStatsManager>();
        if (playerStats == null)
        {
            Debug.LogWarning($"⚠️ Target {target.name} has no PlayerStatsManager!");
            return;
        }
        
        // คำนวณความเสียหาย (เวอร์ชันใหม่ - ไม่ใช้ Speed)
        int playerDefense = playerStats.GetStats().totalDefense;
        int damage = DamageCalculator.CalculateBasicDamage(baseDamage, playerDefense);
        
        // ตรวจสอบคริติคอล
        DamageResult damageResult = DamageCalculator.CalculateCriticalDamage(damage, criticalChance);
        
        // แสดง Effect การโจมตีจาก Enemy
        if (effectManager != null)
        {
            effectManager.ShowEnemyAttackEffect(EnemyAttackType.Normal, transform.position, target.position);
        }
        
        // แสดง Damage Number ที่ตำแหน่ง Player (Enemy attack = สีแดง)
        if (combatUI != null)
        {
            combatUI.ShowDamageNumber(damageResult.damage, target.position, damageResult.isCritical, false, false, false, true);
        }
        
        // แสดง Effect ตอน Player โดนโจมตี
        if (effectManager != null)
        {
            effectManager.ShowHitEffect(target.gameObject, damageResult.isCritical, true);
        }
        
        // ส่งความเสียหายให้ Player
        playerStats.TakeDamage(damageResult.damage);
        
        // Auto-target Enemy ให้ Player (ถ้ายังไม่มีเป้าหมาย)
        if (targetManager != null && targetManager.GetCurrentTarget() == null)
        {
            targetManager.SelectTarget(gameObject);
        }
        
        // เรียก Events
        OnDamageDealt?.Invoke(damageResult.damage);
        
        // ตั้งค่า Cooldown
        canAttack = false;
        attackTimer = 0f;
        
        // Animation โจมตี
        if (animator != null)
        {
            // ตรวจสอบว่ามี Parameter 'Attack' หรือไม่
            if (animator.HasParameter("Attack"))
            {
                animator.SetTrigger("Attack");
            }
            else
            {
                Debug.LogWarning($"⚠️ Animator for {enemyStats.stats.enemyName} doesn't have 'Attack' parameter!");
            }
        }
        
        Debug.Log($"⚔️ {enemyStats.stats.enemyName} attacked {target.name} for {damageResult.damage} damage!");
    }
    
    /// <summary>
    /// สร้างจุด Patrol ใหม่
    /// </summary>
    void GenerateNewPatrolTarget()
    {
        float patrolRadius = 3f;
        Vector2 randomOffset = Random.insideUnitCircle * patrolRadius;
        patrolTarget = patrolOrigin + randomOffset;
    }
    
    /// <summary>
    /// เปลี่ยนสถานะ AI
    /// </summary>
    void SetState(AIState newState)
    {
        if (currentState == newState) return;
        
        AIState oldState = currentState;
        currentState = newState;
        
        Debug.Log($"🤖 {enemyStats.stats.enemyName} AI state: {oldState} → {newState}");
        
        // Reset velocity เมื่อเปลี่ยนสถานะ
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }
    
    /// <summary>
    /// รับความเสียหาย (เรียกจากภายนอก)
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (enemyStats != null)
        {
            enemyStats.TakeDamage(damage);
            
            // ถ้าโจมตีแล้วให้ไล่ตาม
            if (currentState != AIState.Chase && currentState != AIState.Attack)
            {
                SetState(AIState.Chase);
            }
        }
    }
    
    /// <summary>
    /// ตั้งค่าเป้าหมาย
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        
        if (newTarget != null && currentState != AIState.Dead)
        {
            SetState(AIState.Chase);
        }
    }
    
    /// <summary>
    /// ตั้งค่า AI Mode
    /// </summary>
    public void SetAIMode(AIMode mode)
    {
        aiMode = mode;
        
        switch (mode)
        {
            case AIMode.Aggressive:
                // โจมตีทันทีที่เห็น
                break;
            case AIMode.Defensive:
                // โจมตีเฉพาะถ้าโดนโจมตีก่อน
                break;
            case AIMode.Passive:
                // ไม่โจมตีเลย
                break;
        }
    }
    
    void OnDestroy()
    {
        // Cleanup events
        OnTargetDetected = null;
        OnTargetLost = null;
        OnDamageDealt = null;
    }
    
    void OnDrawGizmosSelected()
    {
        // แสดงระยะตรวจจับ
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // แสดงระยะโจมตี
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // แสดงระยะ Patrol
        if (currentState == AIState.Patrol)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(patrolOrigin, 3f);
            Gizmos.DrawLine(transform.position, patrolTarget);
            Gizmos.DrawSphere(patrolTarget, 0.2f);
        }
        
        // แสดงเส้นไปยังเป้าหมาย
        if (target != null && (currentState == AIState.Chase || currentState == AIState.Attack))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}

/// <summary>
/// โหมด AI ของศัตรู
/// </summary>
public enum AIMode
{
    Aggressive,  // โจมตีทันที
    Defensive,   // โจมตีเฉพาะถ้าโดนโจมตี
    Passive      // ไม่โจมตี
}
