using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject สำหรับเก็บข้อมูลศัตรู
/// </summary>
[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Tibiame Rebirth/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("📊 Basic Information")]
    [Tooltip("ชื่อศัตรู")]
    public string enemyName = "Enemy";
    [Tooltip("คำอธิบาย")]
    [TextArea(2, 4)]
    public string description = "A dangerous enemy";
    [Tooltip("เลเวลของศัตรู")]
    public int level = 1;
    [Tooltip("ประเภทของศัตรู")]
    public EnemyType enemyType = EnemyType.Humanoid;
    
    [Header("❤️ Health Stats")]
    [Tooltip("HP สูงสุด")]
    public int maxHealth = 100;
    [Tooltip("การฟื้น HP ต่อวินาที")]
    public float healthRegen = 0f;
    
    [Header("⚔️ Combat Stats")]
    [Tooltip("พลังโจมตี")]
    public int attack = 10;
    [Tooltip("พลังป้องกัน")]
    public int defense = 5;
    [Tooltip("ความเร็ว")]
    public int speed = 5;
    [Tooltip("ความแม่นยำ")]
    public int accuracy = 80;
    [Tooltip("ความว่องไว")]
    public int evasion = 10;
    
    [Header("🎯 AI Behavior")]
    [Tooltip("โหมด AI")]
    public AIMode aiMode = AIMode.Aggressive;
    [Tooltip("ระยะตรวจจับ")]
    public float detectionRange = 5f;
    [Tooltip("ระยะโจมตี")]
    public float attackRange = 1.5f;
    [Tooltip("ความเร็วเคลื่อนที่")]
    public float moveSpeed = 2f;
    [Tooltip("ความเร็วโจมตี (ครั้งต่อวินาที)")]
    public float attackSpeed = 1f;
    [Tooltip("ระยะที่จะหยุดไล่ตาม")]
    public float stopChaseDistance = 10f;
    
    [Header("💰 Rewards")]
    [Tooltip("ประสบการณ์ที่ได้")]
    public int experienceReward = 10;
    [Tooltip("เงินที่ได้")]
    public int goldReward = 5;
    [Tooltip("ไอเท็มที่ดรอป (สามารถว่างได้)")]
    public ItemData[] dropItems;
    [Tooltip("โอกาสดรอปไอเท็ม (0-1)")]
    [Range(0f, 1f)]
    public float dropChance = 0.1f;
    
    [Header("🎨 Visual")]
    [Tooltip("Sprite ของศัตรู")]
    public Sprite enemySprite;
    [Tooltip("สีของศัตรู")]
    public Color enemyColor = Color.white;
    [Tooltip("ขนาดของศัตรู")]
    public Vector2 enemySize = Vector2.one;
    [Tooltip("Prefab สำหรับ Death Effect")]
    public GameObject deathEffectPrefab;
    
    [Header("🔊 Audio")]
    [Tooltip("เสียงเมื่อโจมตี")]
    public AudioClip attackSound;
    [Tooltip("เสียงเมื่อได้รับความเสียหาย")]
    public AudioClip hurtSound;
    [Tooltip("เสียงเมื่อตาย")]
    public AudioClip deathSound;
    
    [Header("🎮 Special Abilities")]
    [Tooltip("สกิลพิเศษที่มี")]
    public EnemyAbility[] abilities;
    
    /// <summary>
    /// สร้าง EnemyStatsData จากข้อมูลนี้
    /// </summary>
    public EnemyStatsData CreateEnemyStatsData()
    {
        EnemyStatsData stats = new EnemyStatsData();
        
        // Basic Stats
        stats.enemyName = enemyName;
        stats.level = level;
        stats.maxHealth = maxHealth;
        stats.currentHealth = maxHealth;
        stats.attack = attack;
        stats.defense = defense;
        stats.speed = speed;
        
        // Rewards
        stats.experienceReward = experienceReward;
        stats.goldReward = goldReward;
        
        // AI Settings
        stats.detectionRange = detectionRange;
        stats.attackRange = attackRange;
        stats.moveSpeed = moveSpeed;
        stats.attackCooldown = 1f / attackSpeed;
        
        return stats;
    }
    
    /// <summary>
    /// สร้าง GameObject ศัตรูจากข้อมูลนี้
    /// </summary>
    public GameObject CreateEnemy(Vector3 position)
    {
        GameObject enemy = new GameObject(enemyName);
        enemy.transform.position = position;
        
        // เพิ่ม Components
        EnemyStats enemyStats = enemy.AddComponent<EnemyStats>();
        enemyStats.stats = CreateEnemyStatsData();
        
        EnemyController enemyController = enemy.AddComponent<EnemyController>();
        enemyController.aiMode = aiMode;
        enemyController.detectionRange = detectionRange;
        enemyController.attackRange = attackRange;
        enemyController.moveSpeed = moveSpeed;
        enemyController.attackSpeed = attackSpeed;
        enemyController.stopChaseDistance = stopChaseDistance;
        
        SpriteRenderer spriteRenderer = enemy.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = enemy.AddComponent<SpriteRenderer>();
        }
        
        // ตั้งค่า Sprite
        if (enemySprite != null)
        {
            spriteRenderer.sprite = enemySprite;
        }
        // ใช้สีเดิมของ Sprite 100% - ไม่เปลี่ยนสี
        spriteRenderer.color = Color.white;
        
        // ตั้งค่าขนาด
        enemy.transform.localScale = enemySize;
        
        // เพิ่ม Collider
        Collider2D collider = enemy.GetComponent<Collider2D>();
        if (collider == null)
        {
            BoxCollider2D boxCollider = enemy.AddComponent<BoxCollider2D>();
            boxCollider.size = Vector2.one;
        }
        
        // เพิ่ม Rigidbody2D
        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = enemy.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
        
        // เพิ่ม Animator (ถ้ามี Animation)
        if (attackSound != null || hurtSound != null || deathSound != null)
        {
            AudioSource audioSource = enemy.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // ตั้งค่า Layer เป็น Enemy
        enemy.layer = LayerMask.NameToLayer("Enemy");
        
        Debug.Log($"👾 Created enemy: {enemyName} at {position}");
        
        return enemy;
    }
    
    /// <summary>
    /// สุ่มไอเท็มที่จะดรอป
    /// </summary>
    public ItemData GetRandomDrop()
    {
        if (dropItems == null || dropItems.Length == 0) return null;
        
        if (Random.value > dropChance) return null;
        
        int randomIndex = Random.Range(0, dropItems.Length);
        return dropItems[randomIndex];
    }
    
    /// <summary>
    /// คำนวณสถานะตามเลเวล
    /// </summary>
    public void ScaleToLevel(int targetLevel)
    {
        if (targetLevel <= 0) return;
        
        float levelMultiplier = 1f + (targetLevel - 1) * 0.2f; // 20% ต่อเลเวล
        
        maxHealth = Mathf.RoundToInt(maxHealth * levelMultiplier);
        attack = Mathf.RoundToInt(attack * levelMultiplier);
        defense = Mathf.RoundToInt(defense * levelMultiplier);
        speed = Mathf.RoundToInt(speed * levelMultiplier);
        
        experienceReward = Mathf.RoundToInt(experienceReward * levelMultiplier);
        goldReward = Mathf.RoundToInt(goldReward * levelMultiplier);
        
        level = targetLevel;
    }
    
    /// <summary>
    /// คัดลอกข้อมูล
    /// </summary>
    public EnemyData Clone()
    {
        EnemyData clone = CreateInstance<EnemyData>();
        
        // คัดลอกทุกฟิลด์
        clone.enemyName = enemyName;
        clone.description = description;
        clone.level = level;
        clone.enemyType = enemyType;
        
        clone.maxHealth = maxHealth;
        clone.healthRegen = healthRegen;
        
        clone.attack = attack;
        clone.defense = defense;
        clone.speed = speed;
        clone.accuracy = accuracy;
        clone.evasion = evasion;
        
        clone.aiMode = aiMode;
        clone.detectionRange = detectionRange;
        clone.attackRange = attackRange;
        clone.moveSpeed = moveSpeed;
        clone.attackSpeed = attackSpeed;
        clone.stopChaseDistance = stopChaseDistance;
        
        clone.experienceReward = experienceReward;
        clone.goldReward = goldReward;
        clone.dropItems = dropItems;
        clone.dropChance = dropChance;
        
        clone.enemySprite = enemySprite;
        clone.enemyColor = enemyColor;
        clone.enemySize = enemySize;
        clone.deathEffectPrefab = deathEffectPrefab;
        
        clone.attackSound = attackSound;
        clone.hurtSound = hurtSound;
        clone.deathSound = deathSound;
        
        clone.abilities = abilities;
        
        return clone;
    }
}

/// <summary>
/// ประเภทของศัตรู
/// </summary>
public enum EnemyType
{
    Humanoid,    // มนุษย์
    Beast,       // สัตว์
    Undead,      // ซอมบี้
    Demon,       // ปีศาจ
    Elemental,   // ธาตุ
    Dragon,      // มังกร
    Plant,       // พืช
    Machine      // จักรกล
}

/// <summary>
/// ความสามารถพิเศษของศัตรู
/// </summary>
[System.Serializable]
public class EnemyAbility
{
    [Header("📋 Ability Info")]
    [Tooltip("ชื่อความสามารถ")]
    public string abilityName = "Ability";
    [Tooltip("คำอธิบาย")]
    [TextArea(2, 3)]
    public string description = "Special ability";
    
    [Header("⚔️ Combat")]
    [Tooltip("ความเสียหาย")]
    public int damage = 0;
    [Tooltip("ระยะเวลา Cooldown (วินาที)")]
    public float cooldown = 5f;
    [Tooltip("ระยะทาง")]
    public float range = 1.5f;
    
    [Header("🎯 Target")]
    [Tooltip("เป้าหมาย")]
    public AbilityTarget target = AbilityTarget.Player;
    [Tooltip("ประเภท")]
    public AbilityType type = AbilityType.Damage;
    
    [Header("🎨 Visual")]
    [Tooltip("Prefab สำหรับ Effect")]
    public GameObject effectPrefab;
    [Tooltip("เสียง")]
    public AudioClip sound;
}

/// <summary>
/// เป้าหมายของความสามารถ
/// </summary>
public enum AbilityTarget
{
    Player,      // ผู้เล่น
    Self,        // ตัวเอง
    AllEnemies,  // ศัตรูทั้งหมด
    Area         // พื้นที่
}

/// <summary>
/// ประเภทของความสามารถ
/// </summary>
public enum AbilityType
{
    Damage,      // ความเสียหาย
    Heal,        // ฟื้นฟู
    Buff,        // เพิ่มพลัง
    Debuff,      // ลดพลัง
    Summon       // เรียกสัตว์
}
