using UnityEngine;
using System.Collections;

/// <summary>
/// จัดการการต่อสู้ทั้งหมด - Auto-attack, Skill usage, Combat flow (เวอร์ชันใหม่ - ไม่ใช้ Speed)
/// </summary>
public class CombatManager : MonoBehaviour
{
    [Header("⚔️ Combat Settings")]
    [Tooltip("ระยะโจมตีปกติ")]
    public float attackRange = 1.5f;
    [Tooltip("ดีเลย์ระหว่างการโจมตี (วินาที)")]
    public float attackCooldown = 1f;
    [Tooltip("ดีเลย์หลังใช้ Power Attack (วินาที)")]
    public float powerAttackCooldown = 2f;
    [Tooltip("โอกาสคริติคอล (0-1)")]
    [Range(0f, 1f)]
    public float criticalChance = 0.1f;
    [Tooltip("ตัวคูณคริติคอล")]
    public float criticalMultiplier = 2.0f;
    [Tooltip("เปิด Auto-attack อัตโนมัติ")]
    public bool autoAttackEnabled = true;
    
    [Header("🎯 References")]
    [Tooltip("Target Manager")]
    public TargetManager targetManager;
    [Tooltip("Player Stats Manager")]
    public PlayerStatsManager playerStatsManager;
    [Tooltip("Combat UI")]
    public CombatUI combatUI;
    [Tooltip("Combat Effect Manager")]
    public CombatEffectManager effectManager;
    [Tooltip("Turn Manager")]
    public TurnManager turnManager;
    
    [Header("🎮 Input Settings")]
    [Tooltip("ปุ่มโจมตีแบบ manual")]
    public KeyCode attackButton = KeyCode.Mouse1;
    [Tooltip("ปุ่มสกิล 1")]
    public KeyCode skill1Button = KeyCode.Q;
    [Tooltip("ปุ่มสกิล 2")]
    public KeyCode skill2Button = KeyCode.W;
    
    // Events
    public System.Action<int, GameObject> OnDamageDealt;
    public System.Action<GameObject> OnEnemyKilled;
    public System.Action OnCombatStarted;
    public System.Action OnCombatEnded;
    
    // Private variables
    private bool isInCombat = false;
    private bool canAttack = true;
    private GameObject currentTarget;
    private PlayerStats playerStats;
    private GameObject playerObject;
    
    void Start()
    {
        InitializeComponents();
        SetupEventListeners();
        Debug.Log("⚔️ CombatManager initialized successfully!");
    }
    
    void Update()
    {
        // อัปเดตสถานะการต่อสู้เสมอ
        UpdateCombatState();
        
        // ถ้ามี TurnManager ให้ใช้ระบบ Turn-based
        if (turnManager != null && turnManager.IsInCombat())
        {
            // ใน Turn-based mode ไม่ต้องทำ Auto-attack หรือ Input ปกติ
            // แต่ต้องอนุญาตให้ PerformAttack() ทำงานได้ (ถูกเรียกจาก TurnManager)
            return;
        }
        
        // ถ้าไม่มี TurnManager หรือไม่อยู่ในการต่อสู้ ให้ใช้ระบบเดิม
        HandleInput();
        ProcessAutoAttack();
        CheckEnemyAttacks();
    }
    
    /// <summary>
    /// ตั้งค่าคอมโพเนนต์เริ่มต้น
    /// </summary>
    void InitializeComponents()
    {
        // หา Player object
        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null)
            {
                playerObject = gameObject; // ถ้าไม่เจอ Player ให้ใช้ตัวเอง
            }
        }
        
        // หา TargetManager ถ้ายังไม่มี
        if (targetManager == null)
        {
            targetManager = FindObjectOfType<TargetManager>();
            if (targetManager == null)
            {
                Debug.LogWarning("⚠️ TargetManager not found! Creating new one...");
                GameObject targetManagerObj = new GameObject("TargetManager");
                targetManager = targetManagerObj.AddComponent<TargetManager>();
            }
        }
        
        // หา PlayerStatsManager ถ้ายังไม่มี
        if (playerStatsManager == null)
        {
            playerStatsManager = FindObjectOfType<PlayerStatsManager>();
            if (playerStatsManager == null)
            {
                Debug.LogError("❌ PlayerStatsManager not found! Combat system won't work properly.");
            }
        }
        
        // หา CombatUI ถ้ายังไม่มี
        if (combatUI == null)
        {
            combatUI = FindObjectOfType<CombatUI>();
        }
        
        // หา CombatEffectManager ถ้ายังไม่มี
        if (effectManager == null)
        {
            effectManager = FindObjectOfType<CombatEffectManager>();
            if (effectManager == null)
            {
                Debug.LogWarning("⚠️ CombatEffectManager not found! Creating new one...");
                GameObject effectManagerObj = new GameObject("CombatEffectManager");
                effectManager = effectManagerObj.AddComponent<CombatEffectManager>();
            }
        }
        
        // หา TurnManager ถ้ายังไม่มี
        if (turnManager == null)
        {
            turnManager = FindObjectOfType<TurnManager>();
            if (turnManager == null)
            {
                Debug.LogWarning("⚠️ TurnManager not found! Creating new one...");
                GameObject turnManagerObj = new GameObject("TurnManager");
                turnManager = turnManagerObj.AddComponent<TurnManager>();
            }
        }
        
        // รับข้อมูลสถานะผู้เล่น
        if (playerStatsManager != null)
        {
            playerStats = playerStatsManager.GetStats();
        }
    }
    
    /// <summary>
    /// ตั้งค่า Event Listeners
    /// </summary>
    void SetupEventListeners()
    {
        if (targetManager != null)
        {
            targetManager.OnTargetSelected += OnTargetSelected;
            targetManager.OnTargetDeselected += OnTargetDeselected;
            targetManager.OnTargetChanged += OnTargetChanged;
        }
        
        if (playerStatsManager != null)
        {
            playerStatsManager.OnDeath += OnPlayerDeath;
            playerStatsManager.OnRevive += OnPlayerRevive;
        }
    }
    
    /// <summary>
    /// อัปเดตสถานะการต่อสู้
    /// </summary>
    void UpdateCombatState()
    {
        bool wasInCombat = isInCombat;
        isInCombat = currentTarget != null && !playerStatsManager.IsDead();
        
        // ตรวจสอบการเริ่ม/จบการต่อสู้
        if (!wasInCombat && isInCombat)
        {
            OnCombatStarted?.Invoke();
            Debug.Log("⚔️ Combat started!");
        }
        else if (wasInCombat && !isInCombat)
        {
            OnCombatEnded?.Invoke();
            Debug.Log("🏁 Combat ended!");
        }
    }
    
    /// <summary>
    /// จัดการ Input
    /// </summary>
    void HandleInput()
    {
        if (playerStatsManager.IsDead()) return;
        
        // Manual attack
        if (Input.GetKeyDown(attackButton) && currentTarget != null)
        {
            TryAttack();
        }
        
        // Skills
        if (Input.GetKeyDown(skill1Button))
        {
            UseSkill(1);
        }
        
        if (Input.GetKeyDown(skill2Button))
        {
            UseSkill(2);
        }
    }
    
    /// <summary>
    /// ประมวลผล Auto-attack
    /// </summary>
    void ProcessAutoAttack()
    {
        if (!autoAttackEnabled || !isInCombat || !canAttack) return;
        
        if (targetManager.IsTargetInRange(attackRange))
        {
            TryAttack();
        }
    }
    
    /// <summary>
    /// ตรวจสอบการโจมตีจาก Enemy
    /// </summary>
    void CheckEnemyAttacks()
    {
        // ถ้า Player โดนโจมตีและยังไม่มีเป้าหมาย ให้ auto-target Enemy ที่โจมตี
        if (playerStatsManager != null && targetManager != null && targetManager.GetCurrentTarget() == null)
        {
            // หา Enemy ที่อยู่ใกล้ๆ และกำลังโจมตี Player
            EnemyController[] enemies = FindObjectsOfType<EnemyController>();
            foreach (EnemyController enemy in enemies)
            {
                if (enemy != null && enemy.target != null && enemy.target.gameObject.CompareTag("Player"))
                {
                    float distance = Vector2.Distance(enemy.transform.position, playerStatsManager.transform.position);
                    if (distance <= enemy.attackRange)
                    {
                        // Auto-target Enemy ที่โจมตี Player
                        targetManager.SelectTarget(enemy.gameObject);
                        Debug.Log($"🎯 Auto-targeted {enemy.name} after being attacked!");
                        break;
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// พยายามโจมตี
    /// </summary>
    void TryAttack()
    {
        if (!canAttack || currentTarget == null || playerStatsManager.IsDead()) return;
        
        if (!targetManager.IsTargetInRange(attackRange))
        {
            Debug.Log($"📏 Target {currentTarget.name} is out of attack range!");
            return;
        }
        
        PerformAttack();
    }
    
    /// <summary>
    /// ทำการโจมตี
    /// </summary>
    public void PerformAttack()
    {
        Debug.Log("🔥 CombatManager.PerformAttack() called!");
        
        if (playerStats == null) 
        {
            Debug.LogWarning("⚠️ Player stats is null!");
            return;
        }
        
        if (currentTarget == null)
        {
            Debug.LogWarning("⚠️ Current target is null!");
            return;
        }
        
        // เก็บค่า target ไว้ก่อนเพื่อป้องกัน null reference,
        // เพราะ TakeDamage อาจจะทำให้ currentTarget กลายเป็น null ได้
        GameObject attackedTarget = currentTarget;
        Debug.Log($"🎯 Attacking target: {attackedTarget.name}");
        
        // หาสถานะของเป้าหมาย
        EnemyStats enemyStats = attackedTarget.GetComponent<EnemyStats>();
        if (enemyStats == null)
        {
            Debug.LogWarning($"⚠️ Target {attackedTarget.name} has no EnemyStats component!");
            return;
        }
        
        // ตรวจสอบว่าโจมตีพลาดหรือไม่ (เวอร์ชันใหม่ - ไม่มีการพลาด)
        if (DamageCalculator.IsAttackMissed())
        {
            Debug.Log($"❌ Missed attack on {attackedTarget.name}!");
            ShowDamageNumber(0, attackedTarget.transform.position, false, true);
            return;
        }
        
        // คำนวณความเสียหาย
        int baseDamage = DamageCalculator.CalculateBasicDamage(playerStats.totalAttack, enemyStats.GetStats().defense);
        DamageResult damageResult = DamageCalculator.CalculateCriticalDamage(baseDamage, criticalChance, criticalMultiplier);
        
        // แสดง Effect การโจมตี
        if (effectManager != null)
        {
            effectManager.ShowPlayerAttackEffect(PlayerAttackType.Normal, playerObject.transform.position, attackedTarget.transform.position);
        }
        
        // สร้างเอฟเฟกต์ความเสียหาย
        ShowDamageNumber(damageResult.damage, attackedTarget.transform.position, damageResult.isCritical, false);
        
        // ส่งความเสียหายให้เป้าหมาย
        enemyStats.TakeDamage(damageResult.damage);
        
        // แสดง Effect ตอนโดนโจมตี
        if (effectManager != null && attackedTarget != null)
        {
            effectManager.ShowHitEffect(attackedTarget, damageResult.isCritical, false);
        }
        
        // Log การโจมตี ก่อนจะเช็คว่าตายหรือไม่
        Debug.Log($"⚔️ Attacked {attackedTarget.name} for {damageResult.damage} damage{(damageResult.isCritical ? " (CRITICAL!)" : "")}");
        
        // ตรวจสอบว่าศัตรูตายหรือไม่
        if (enemyStats.IsDead())
        {
            OnEnemyKilled?.Invoke(attackedTarget);
            Debug.Log($"💀 Killed {attackedTarget.name}!");

            // Deselect target only if not in turn-based combat
            if (turnManager == null || !turnManager.IsInCombat())
            {
                targetManager.DeselectTarget();
            }
        }
        
        // เรียก Events หลังจากตรวจสอบทุกอย่างแล้ว
        OnDamageDealt?.Invoke(damageResult.damage, attackedTarget);
        
        // ตั้งค่า Cooldown ที่ท้ายฟังก์ชันเสมอ
        canAttack = false;
        StartCoroutine(CooldownCoroutine(attackCooldown));
    }

    /// <summary>
    /// Coroutine สำหรับจัดการดีเลย์การโจมตี
    /// </summary>
    private IEnumerator CooldownCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        canAttack = true;
    }
    
    /// <summary>
    /// ใช้สกิล
    /// </summary>
    /// <param name="skillId">ID ของสกิล</param>
    void UseSkill(int skillId)
    {
        if (currentTarget == null || playerStatsManager.IsDead()) return;
        
        switch (skillId)
        {
            case 1:
                UseSkill1();
                break;
            case 2:
                UseSkill2();
                break;
            default:
                Debug.LogWarning($"⚠️ Unknown skill ID: {skillId}");
                break;
        }
    }
    
    /// <summary>
    /// สกิล 1: Quick Attack
    /// </summary>
    void UseSkill1()
    {
        if (playerStats == null) return;
        
        // ตรวจสอบ MP
        int manaCost = 10;
        if (!playerStatsManager.UseMana(manaCost))
        {
            Debug.Log("💨 Not enough mana for Quick Attack!");
            return;
        }
        
        Debug.Log("⚡ Using Quick Attack!");
        
        // โจมตีเร็วพร้อมความเสียหายลดลงเล็กน้อย
        PerformQuickAttack();
    }
    
    /// <summary>
    /// ทำการโจมตีเร็ว
    /// </summary>
    void PerformQuickAttack()
    {
        if (currentTarget == null || playerStats == null)
        {
            Debug.LogWarning("⚠️ Cannot perform Quick Attack - missing target or player stats!");
            return;
        }

        GameObject attackedTarget = currentTarget;
        EnemyStats enemyStats = attackedTarget.GetComponent<EnemyStats>();
        if (enemyStats == null)
        {
            Debug.LogWarning($"⚠️ Target {attackedTarget.name} has no EnemyStats component!");
            return;
        }

        // คำนวณความเสียหาย (Quick Attack - ความเสียหายน้อยลงแต่เร็ว)
        int baseDamage = DamageCalculator.CalculateBasicDamage(playerStats.totalAttack, enemyStats.GetStats().defense);
        baseDamage = Mathf.RoundToInt(baseDamage * 0.8f); // ลดความเสียหาย 20%
        float quickCritChance = criticalChance * 1.5f; // เพิ่มโอกาสคริติคอล
        DamageResult damageResult = DamageCalculator.CalculateCriticalDamage(baseDamage, quickCritChance, criticalMultiplier);

        // แสดง Effect
        if (effectManager != null)
        {
            effectManager.ShowPlayerAttackEffect(PlayerAttackType.Normal, playerObject.transform.position, attackedTarget.transform.position);
        }
        ShowDamageNumber(damageResult.damage, attackedTarget.transform.position, damageResult.isCritical, false, true);

        // ส่งความเสียหายและ Events
        enemyStats.TakeDamage(damageResult.damage);
        if (effectManager != null && attackedTarget != null)
        {
            effectManager.ShowHitEffect(attackedTarget, damageResult.isCritical, false);
        }

        // Log
        Debug.Log($"⚡ Quick Attack dealt {damageResult.damage} damage to {attackedTarget.name}!");

        // Logic หลังการโจมตี
        if (enemyStats.IsDead())
        {
            OnEnemyKilled?.Invoke(attackedTarget);
            
            // Deselect target only if not in turn-based combat
            if (turnManager == null || !turnManager.IsInCombat())
            {
                targetManager.DeselectTarget();
            }
        }

        // เรียก Events หลังจากตรวจสอบทุกอย่างแล้ว
        OnDamageDealt?.Invoke(damageResult.damage, attackedTarget);

        // Cooldown (เร็วกว่าปกติ)
        canAttack = false;
        StartCoroutine(CooldownCoroutine(attackCooldown * 0.7f)); // ลดเวลา cooldown 30%
    }
    
    /// <summary>
    /// เรียกเมื่อ Enemy ตาย
    /// </summary>
    public void OnEnemyDied(GameObject deadEnemy)
    {
        if (deadEnemy == null) return;
        
        Debug.Log($"💀 CombatManager: Enemy {deadEnemy.name} died");
        
        // ถ้า Enemy ที่ตายเป็น currentTarget ให้เคลียร์เป้าหมาย
        if (currentTarget == deadEnemy)
        {
            currentTarget = null;
            Debug.Log("🎯 Cleared dead enemy from current target");
        }
        
        // เรียก Events
        OnEnemyKilled?.Invoke(deadEnemy);
    }
    
    /// <summary>
    /// สกิล 2: Power Attack
    /// </summary>
    void UseSkill2()
    {
        if (playerStats == null) return;
        
        // ตรวจสอบ MP
        int manaCost = 15;
        if (!playerStatsManager.UseMana(manaCost))
        {
            Debug.Log("💨 Not enough mana for Power Attack!");
            return;
        }
        
        Debug.Log("💪 Using Power Attack!");
        
        // โจมตีพลังสูง
        PerformPowerAttack();
    }
    
    /// <summary>
    /// ทำการโจมตีพลังสูง
    /// </summary>
    void PerformPowerAttack()
    {
        if (currentTarget == null || playerStats == null)
        {
            Debug.LogWarning("⚠️ Cannot perform Power Attack - missing target or player stats!");
            return;
        }

        GameObject attackedTarget = currentTarget;
        EnemyStats enemyStats = attackedTarget.GetComponent<EnemyStats>();
        if (enemyStats == null)
        {
            Debug.LogWarning($"⚠️ Target {attackedTarget.name} has no EnemyStats component!");
            return;
        }

        // คำนวณความเสียหาย (Power Attack)
        int baseDamage = DamageCalculator.CalculateBasicDamage(playerStats.totalAttack, enemyStats.GetStats().defense);
        baseDamage = Mathf.RoundToInt(baseDamage * 1.5f);
        float powerCritChance = criticalChance * 2f;
        DamageResult damageResult = DamageCalculator.CalculateCriticalDamage(baseDamage, powerCritChance, criticalMultiplier);

        // แสดง Effect
        if (effectManager != null)
        {
            effectManager.ShowPlayerAttackEffect(PlayerAttackType.Power, playerObject.transform.position, attackedTarget.transform.position);
        }
        ShowDamageNumber(damageResult.damage, attackedTarget.transform.position, damageResult.isCritical, false, true);

        // ส่งความเสียหายและ Events
        enemyStats.TakeDamage(damageResult.damage);
        if (effectManager != null && attackedTarget != null)
        {
            effectManager.ShowHitEffect(attackedTarget, damageResult.isCritical, false);
        }

        // Log
        Debug.Log($"💪 Power Attack dealt {damageResult.damage} damage to {attackedTarget.name}!");

        // Logic หลังการโจมตี
        if (enemyStats.IsDead())
        {
            OnEnemyKilled?.Invoke(attackedTarget);
            
            // Deselect target only if not in turn-based combat
            if (turnManager == null || !turnManager.IsInCombat())
            {
                targetManager.DeselectTarget();
            }
        }

        // เรียก Events หลังจากตรวจสอบทุกอย่างแล้ว
        OnDamageDealt?.Invoke(damageResult.damage, attackedTarget);

        // Cooldown (นานกว่าปกติ)
        canAttack = false;
        StartCoroutine(CooldownCoroutine(powerAttackCooldown));
    }
    
    /// <summary>
    /// แสดงตัวเลขความเสียหาย
    /// </summary>
    /// <param name="damage">ความเสียหาย</param>
    /// <param name="position">ตำแหน่งที่จะแสดง</param>
    /// <param name="isCritical">เป็นคริติคอลหรือไม่</param>
    /// <param name="isMissed">พลาดหรือไม่</param>
    /// <param name="isPowerAttack">เป็น Power Attack หรือไม่</param>
    void ShowDamageNumber(int damage, Vector3 position, bool isCritical = false, bool isMissed = false, bool isPowerAttack = false)
    {
        if (combatUI != null)
        {
            combatUI.ShowDamageNumber(damage, position, isCritical, isMissed, isPowerAttack);
        }
    }
    
    /// <summary>
    /// Event: เมื่อเลือกเป้าหมาย
    /// </summary>
    void OnTargetSelected(GameObject target)
    {
        currentTarget = target;
        Debug.Log($"🎯 Target selected: {target.name}");
    }
    
    /// <summary>
    /// Event: เมื่อยกเลิกเป้าหมาย
    /// </summary>
    void OnTargetDeselected(GameObject target)
    {
        currentTarget = null;
        Debug.Log($"❌ Target deselected: {target.name}");
    }
    
    /// <summary>
    /// Event: เมื่อเปลี่ยนเป้าหมาย
    /// </summary>
    void OnTargetChanged(GameObject newTarget)
    {
        currentTarget = newTarget;
    }
    
    /// <summary>
    /// Event: เมื่อผู้เล่นตาย
    /// </summary>
    void OnPlayerDeath()
    {
        isInCombat = false;
        currentTarget = null;
        Debug.Log("💀 Player died - combat ended");
    }
    
    /// <summary>
    /// Event: เมื่อผู้เล่นฟื้นชีวิต
    /// </summary>
    void OnPlayerRevive()
    {
        Debug.Log("✨ Player revived - ready for combat");
    }
    
    /// <summary>
    /// รับเป้าหมายปัจจุบัน
    /// </summary>
    public GameObject GetCurrentTarget()
    {
        return currentTarget;
    }
    
    /// <summary>
    /// ตรวจสอบว่าอยู่ในการต่อสู้หรือไม่
    /// </summary>
    public bool IsInCombat()
    {
        return isInCombat;
    }
    
    /// <summary>
    /// ตั้งค่า TargetManager
    /// </summary>
    public void SetTargetManager(TargetManager manager)
    {
        targetManager = manager;
        SetupEventListeners();
    }
    
    /// <summary>
    /// ตั้งค่า PlayerStatsManager
    /// </summary>
    public void SetPlayerStatsManager(PlayerStatsManager manager)
    {
        playerStatsManager = manager;
        if (manager != null)
        {
            playerStats = manager.GetStats();
            manager.OnDeath += OnPlayerDeath;
            manager.OnRevive += OnPlayerRevive;
        }
    }
    
    /// <summary>
    /// ตั้งค่า CombatUI
    /// </summary>
    public void SetCombatUI(CombatUI ui)
    {
        combatUI = ui;
    }
    
    /// <summary>
    /// ตั้งค่า CombatEffectManager
    /// </summary>
    public void SetCombatEffectManager(CombatEffectManager manager)
    {
        effectManager = manager;
    }
    

    
    void OnDestroy()
    {
        // Cleanup events
        if (targetManager != null)
        {
            targetManager.OnTargetSelected -= OnTargetSelected;
            targetManager.OnTargetDeselected -= OnTargetDeselected;
            targetManager.OnTargetChanged -= OnTargetChanged;
        }
        
        if (playerStatsManager != null)
        {
            playerStatsManager.OnDeath -= OnPlayerDeath;
            playerStatsManager.OnRevive -= OnPlayerRevive;
        }
        
        OnDamageDealt = null;
        OnEnemyKilled = null;
        OnCombatStarted = null;
        OnCombatEnded = null;
    }
    
    void OnDrawGizmosSelected()
    {
        // แสดงระยะโจมตี
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
