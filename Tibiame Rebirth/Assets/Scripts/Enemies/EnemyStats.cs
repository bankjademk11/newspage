using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// สถานะของศัตรู - คล้ายกับ PlayerStats แต่สำหรับศัตรู
/// </summary>
[System.Serializable]
public class EnemyStatsData
{
    [Header("📊 Basic Stats")]
    public string enemyName = "Enemy";
    public int level = 1;
    public int maxHealth = 100;
    public int currentHealth = 100;
    public int attack = 10;
    public int defense = 5;
    public int speed = 5;
    
    [Header("💰 Rewards")]
    public int experienceReward = 10;
    public int goldReward = 5;
    
    [Header("🎯 AI Settings")]
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public float moveSpeed = 2f;
    public float attackCooldown = 2f;
    
    // Properties
    public bool IsDead => currentHealth <= 0;
    public float HealthPercentage => (float)currentHealth / maxHealth;
    
    /// <summary>
    /// รับความเสียหาย
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (IsDead) return;
        
        currentHealth = Mathf.Max(0, currentHealth - damage);
        Debug.Log($"{enemyName} took {damage} damage! HP: {currentHealth}/{maxHealth}");
    }
    
    /// <summary>
    /// ฟื้น HP
    /// </summary>
    public void Heal(int amount)
    {
        if (IsDead) return;
        
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        Debug.Log($"{enemyName} healed {amount} HP! HP: {currentHealth}/{maxHealth}");
    }
    
    /// <summary>
    /// ฟื้นสถานะทั้งหมด
    /// </summary>
    public void FullRestore()
    {
        currentHealth = maxHealth;
        Debug.Log($"{enemyName} fully restored!");
    }
    
    /// <summary>
    /// ตาย
    /// </summary>
    public void Kill()
    {
        currentHealth = 0;
        Debug.Log($"{enemyName} died!");
    }
    
    /// <summary>
    /// ฟื้นชีวิต
    /// </summary>
    public void Revive()
    {
        currentHealth = maxHealth;
        Debug.Log($"{enemyName} revived!");
    }
}

/// <summary>
/// Component สำหรับจัดการสถานะศัตรูในเกม
/// </summary>
public class EnemyStats : MonoBehaviour
{
    [Header("📊 Enemy Stats Data")]
    [Tooltip("ข้อมูลสถานะศัตรู")]
    public EnemyStatsData stats;
    
    [Header("🎮 Components")]
    [Tooltip("Sprite Renderer สำหรับแสดงสถานะ")]
    public SpriteRenderer spriteRenderer;
    [Tooltip("Animator สำหรับ Animation")]
    public Animator animator;
    
    [Header("💫 Visual Effects")]
    [Tooltip("สีเมื่อได้รับความเสียหาย")]
    public Color damageColor = Color.red;
    [Tooltip("ระยะเวลาแสดงสีความเสียหาย")]
    public float damageFlashDuration = 0.1f;
    [Tooltip("Prefab สำหรับ Death Effect")]
    public GameObject deathEffectPrefab;
    
    // Events
    public event Action<int, int> OnHealthChanged;
    public event Action<int> OnLevelChanged;
    public event Action OnDeath;
    public event Action OnRevive;
    public event Action OnDamaged;
    public event Action OnHealed;
    
    // Private variables
    private Color originalColor;
    private bool isDead = false;
    private bool isFlashing = false;
    
    void Start()
    {
        InitializeStats();
        SetupComponents();
        Debug.Log($"👾 {stats.enemyName} initialized with {stats.currentHealth}/{stats.maxHealth} HP");
    }
    
    /// <summary>
    /// ตั้งค่าสถานะเริ่มต้น
    /// </summary>
    void InitializeStats()
    {
        if (stats == null)
        {
            stats = new EnemyStatsData();
            stats.enemyName = gameObject.name;
        }
        
        // ตรวจสอบค่าไม่ให้ติดลบ
        stats.currentHealth = Mathf.Max(stats.currentHealth, 0);
        stats.currentHealth = Mathf.Min(stats.currentHealth, stats.maxHealth);
    }
    
    /// <summary>
    /// ตั้งค่าคอมโพเนนต์
    /// </summary>
    void SetupComponents()
    {
        // หา SpriteRenderer ถ้ายังไม่มี
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
        }
        
        // เก็บสีเดิม
        originalColor = spriteRenderer.color;
        
        // หา Animator ถ้ายังไม่มี
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }
    
    /// <summary>
    /// รับความเสียหาย
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0) return;
        
        int oldHealth = stats.currentHealth;
        stats.TakeDamage(damage);
        
        // ไม่แสดงเอฟเฟกต์ความเสียหาย - ให้ Enemy แสดงสีปกติ 100%
        // ShowDamageEffect();
        
        // เรียก Events
        OnHealthChanged?.Invoke(oldHealth, stats.currentHealth);
        OnDamaged?.Invoke();
        
        // ตรวจสอบการตาย
        if (stats.IsDead && !isDead)
        {
            Die();
        }
        
        // ไม่ต้องแสดง Log ซ้ำ เพราะ EnemyStatsData.TakeDamage() แสดงไปแล้ว
    }
    
    /// <summary>
    /// ฟื้น HP
    /// </summary>
    public void Heal(int amount)
    {
        if (isDead || amount <= 0) return;
        
        int oldHealth = stats.currentHealth;
        stats.Heal(amount);
        
        // เรียก Events
        OnHealthChanged?.Invoke(oldHealth, stats.currentHealth);
        OnHealed?.Invoke();
        
        Debug.Log($"💚 {stats.enemyName} healed {amount} HP! HP: {stats.currentHealth}/{stats.maxHealth}");
    }
    
    /// <summary>
    /// ฟื้นสถานะทั้งหมด
    /// </summary>
    public void FullRestore()
    {
        if (!isDead) return;
        
        int oldHealth = stats.currentHealth;
        stats.FullRestore();
        isDead = false;
        
        // คืนสีเดิม
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        
        // เรียก Events
        OnHealthChanged?.Invoke(oldHealth, stats.currentHealth);
        OnRevive?.Invoke();
        
        Debug.Log($"✨ {stats.enemyName} fully restored!");
    }
    
    /// <summary>
    /// ตาย
    /// </summary>
    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        // สร้าง Death Effect
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
        
        // ปิด Collider ทันที
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        
        // หยุดการเคลื่อนที่ทันที
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }
        
        // แจ้ง TurnManager และ CombatManager ว่า Enemy ตายแล้ว
        NotifyCombatSystemOfDeath();
        
        // เรียก Events
        OnDeath?.Invoke();
        
        Debug.Log($"💀 {stats.enemyName} died! Rewards: {stats.experienceReward} EXP, {stats.goldReward} Gold");
        
        // ทำลาย GameObject หลังจากแจ้งระบบแล้ว
        StartCoroutine(DestroyAfterDelay(0.1f));
    }
    
    /// <summary>
    /// แจ้งระบบต่อสู้ว่า Enemy ตายแล้ว
    /// </summary>
    void NotifyCombatSystemOfDeath()
    {
        // แจ้ง TurnManager
        TurnManager turnManager = FindObjectOfType<TurnManager>();
        if (turnManager != null)
        {
            turnManager.OnEnemyDied(gameObject);
        }
        
        // แจ้ง CombatManager
        CombatManager combatManager = FindObjectOfType<CombatManager>();
        if (combatManager != null)
        {
            combatManager.OnEnemyDied(gameObject);
        }
        
        // แจ้ง TargetManager
        TargetManager targetManager = FindObjectOfType<TargetManager>();
        if (targetManager != null)
        {
            targetManager.OnEnemyDied(gameObject);
        }
        
        Debug.Log($"📢 Notified combat system that {stats.enemyName} died");
    }
    
    /// <summary>
    /// แสดงเอฟเฟกต์ความเสียหาย - ปิดการใช้งานเพื่อให้ Enemy แสดงสีปกติ 100%
    /// </summary>
    void ShowDamageEffect()
    {
        // ไม่ทำอะไรเลย - ให้ Enemy แสดงสีเดิมของ Sprite ตลอดเวลา
        // if (spriteRenderer == null || isFlashing) return;
        // StartCoroutine(DamageFlashCoroutine());
    }
    
    /// <summary>
    /// Coroutine สำหรับ Damage Flash - ปิดการใช้งาน
    /// </summary>
    IEnumerator DamageFlashCoroutine()
    {
        // ไม่ทำอะไรเลย - ไม่เปลี่ยนสี Enemy
        yield return null;
    }
    
    /// <summary>
    /// ทำลาย GameObject หลังจาก delay
    /// </summary>
    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// รับข้อมูลสถานะ
    /// </summary>
    public EnemyStatsData GetStats()
    {
        return stats;
    }
    
    /// <summary>
    /// ตรวจสอบว่าตายหรือไม่
    /// </summary>
    public bool IsDead()
    {
        return isDead || stats.IsDead;
    }
    
    /// <summary>
    /// ตรวจสอบว่ามีชีวิตอยู่หรือไม่
    /// </summary>
    public bool IsAlive()
    {
        return !IsDead();
    }
    
    /// <summary>
    /// รับ HP เป็นเปอร์เซ็นต์
    /// </summary>
    public float GetHealthPercentage()
    {
        return stats.HealthPercentage;
    }
    
    /// <summary>
    /// ตั้งค่าสถานะ
    /// </summary>
    public void SetStats(EnemyStatsData newStats)
    {
        if (newStats == null) return;
        
        stats = newStats;
        InitializeStats();
        
        Debug.Log($"📊 Updated {stats.enemyName} stats");
    }
    
    /// <summary>
    /// อัปเกรดเลเวล
    /// </summary>
    public void LevelUp()
    {
        if (isDead) return;
        
        int oldLevel = stats.level;
        stats.level++;
        
        // เพิ่มสถานะตามเลเวล
        stats.maxHealth += 20;
        stats.attack += 3;
        stats.defense += 2;
        stats.speed += 1;
        stats.experienceReward += 5;
        stats.goldReward += 2;
        
        // ฟื้น HP เต็ม
        stats.currentHealth = stats.maxHealth;
        
        // เรียก Events
        OnLevelChanged?.Invoke(stats.level);
        OnHealthChanged?.Invoke(stats.currentHealth, stats.currentHealth);
        
        Debug.Log($"⬆️ {stats.enemyName} leveled up to {stats.level}!");
    }
    
    /// <summary>
    /// ตั้งค่า SpriteRenderer
    /// </summary>
    public void SetSpriteRenderer(SpriteRenderer renderer)
    {
        spriteRenderer = renderer;
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }
    
    /// <summary>
    /// ตั้งค่า Animator
    /// </summary>
    public void SetAnimator(Animator anim)
    {
        animator = anim;
    }
    
    void OnDestroy()
    {
        // Cleanup events
        OnHealthChanged = null;
        OnLevelChanged = null;
        OnDeath = null;
        OnRevive = null;
        OnDamaged = null;
        OnHealed = null;
    }
    
    void OnDrawGizmosSelected()
    {
        // แสดงระยะตรวจจับ
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stats.detectionRange);
        
        // แสดงระยะโจมตี
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stats.attackRange);
    }
}
