using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// จัดการสถานะผู้เล่นทั้งหมด - คำนวณสถานะรวมจาก Equipment และอัปเดต UI
/// </summary>
public class PlayerStatsManager : MonoBehaviour
{
    [Header("📊 Player Stats Data")]
    [Tooltip("ข้อมูลสถานะผู้เล่น")]
    public PlayerStats stats;
    
    [Header("🎯 UI References - Health & Mana")]
    [Tooltip("ข้อความแสดง HP")]
    public Text healthText;
    [Tooltip("ข้อความแสดง MP")]
    public Text manaText;
    [Tooltip("แถบ HP")]
    public Slider healthBar;
    [Tooltip("แถบ MP")]
    public Slider manaBar;
    
    [Header("⚔️ UI References - Combat Stats")]
    [Tooltip("ข้อความแสดง Attack Power")]
    public Text attackText;
    [Tooltip("ข้อความแสดง Defense")]
    public Text defenseText;
    [Tooltip("ข้อความแสดง Speed")]
    public Text speedText;
    
    [Header("📈 UI References - Additional Stats (Optional)")]
    [Tooltip("ข้อความแสดง Level")]
    public Text levelText;
    [Tooltip("ข้อความแสดง Experience")]
    public Text experienceText;
    [Tooltip("ข้อความแสดง Base Stats")]
    public Text baseStatsText;
    [Tooltip("ข้อความแสดง Equipment Bonus")]
    public Text equipmentBonusText;
    
    [Header("🔧 Settings")]
    [Tooltip("อัปเดต UI แบบ real-time ทุก frame")]
    public bool realTimeUpdate = true;
    [Tooltip("เวลาในการ Regenerate HP/MP (วินาที)")]
    public float regenerationInterval = 1f;
    [Tooltip("HP ที่ฟื้นต่อครั้ง")]
    public int healthRegenAmount = 1;
    [Tooltip("MP ที่ฟื้นต่อครั้ง")]
    public int manaRegenAmount = 2;
    
    // Events
    public event Action<int, int> OnHealthChanged;
    public event Action<int, int> OnManaChanged;
    public event Action<int> OnLevelUp;
    public event Action OnDeath;
    public event Action OnRevive;
    
    // Private variables
    private EquipmentManager equipmentManager;
    private float regenerationTimer;
    private bool isDead = false;
    
    void Start()
    {
        InitializeStats();
        FindEquipmentManager();
        CalculateTotalStats();
        UpdateAllUI();
        
        Debug.Log("✅ PlayerStatsManager initialized successfully!");
    }
    
    void Update()
    {
        if (realTimeUpdate)
        {
            UpdateAllUI();
        }
        
        // Regeneration system
        HandleRegeneration();
    }
    
    /// <summary>
    /// ตั้งค่าเริ่มต้นสถานะผู้เล่น
    /// </summary>
    void InitializeStats()
    {
        if (stats == null)
        {
            stats = new PlayerStats();
            Debug.Log("📊 Created new PlayerStats with default values");
        }
        
        // ตรวจสอบค่าไม่ให้ติดลบ
        stats.currentHealth = Mathf.Max(stats.currentHealth, 0);
        stats.currentMana = Mathf.Max(stats.currentMana, 0);
        stats.currentHealth = Mathf.Min(stats.currentHealth, stats.maxHealth);
        stats.currentMana = Mathf.Min(stats.currentMana, stats.maxMana);
    }
    
    /// <summary>
    /// หา EquipmentManager ใน Scene
    /// </summary>
    void FindEquipmentManager()
    {
        equipmentManager = FindObjectOfType<EquipmentManager>();
        if (equipmentManager == null)
        {
            Debug.LogWarning("⚠️ EquipmentManager not found! Equipment bonuses won't be calculated.");
        }
        else
        {
            Debug.Log("🔗 EquipmentManager found and linked!");
        }
    }
    
    /// <summary>
    /// คำนวณสถานะรวมจาก Equipment
    /// </summary>
    public void CalculateTotalStats()
    {
        if (stats == null) return;
        
        // รีเซ็ตค่ารวมเป็นค่าพื้นฐาน
        stats.totalAttack = stats.baseAttack;
        stats.totalDefense = stats.baseDefense;
        stats.totalSpeed = stats.baseSpeed;
        
        // เพิ่มสถานะจาก Equipment
        if (equipmentManager != null)
        {
            var equipmentSlots = equipmentManager.GetAllEquipmentSlots();
            foreach (var slot in equipmentSlots)
            {
                ItemData equippedItem = slot.GetEquippedItem();
                if (equippedItem != null)
                {
                    stats.totalAttack += equippedItem.attackPower;
                    stats.totalDefense += equippedItem.defense;
                    stats.totalSpeed += equippedItem.speed;
                    
                    Debug.Log($"⚔️ Equipment bonus from {equippedItem.itemName}: ATK+{equippedItem.attackPower} DEF+{equippedItem.defense} SPD+{equippedItem.speed}");
                }
            }
        }
        
        Debug.Log($"📈 Total Stats - ATK: {stats.totalAttack} DEF: {stats.totalDefense} SPD: {stats.totalSpeed}");
    }
    
    /// <summary>
    /// อัปเดต UI ทั้งหมด
    /// </summary>
    public void UpdateAllUI()
    {
        if (stats == null) return;
        
        UpdateHealthUI();
        UpdateManaUI();
        UpdateCombatStatsUI();
        UpdateAdditionalStatsUI();
    }
    
    /// <summary>
    /// อัปเดต UI ของ HP
    /// </summary>
    void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = $"❤️ HP: {stats.currentHealth}/{stats.maxHealth}";
            
        if (healthBar != null)
        {
            healthBar.maxValue = stats.maxHealth;
            healthBar.value = stats.currentHealth;
        }
    }
    
    /// <summary>
    /// อัปเดต UI ของ MP
    /// </summary>
    void UpdateManaUI()
    {
        if (manaText != null)
            manaText.text = $"💙 MP: {stats.currentMana}/{stats.maxMana}";
            
        if (manaBar != null)
        {
            manaBar.maxValue = stats.maxMana;
            manaBar.value = stats.currentMana;
        }
    }
    
    /// <summary>
    /// อัปเดต UI ของสถานะการต่อสู้
    /// </summary>
    void UpdateCombatStatsUI()
    {
        if (attackText != null)
            attackText.text = $"⚔️ ATK: {stats.totalAttack}";
            
        if (defenseText != null)
            defenseText.text = $"🛡️ DEF: {stats.totalDefense}";
            
        if (speedText != null)
            speedText.text = $"💨 SPD: {stats.totalSpeed}";
    }
    
    /// <summary>
    /// อัปเดต UI ของสถานะเพิ่มเติม
    /// </summary>
    void UpdateAdditionalStatsUI()
    {
        if (levelText != null)
            levelText.text = $"Lv. 1"; // สามารถเพิ่มระบบ Level ได้
            
        if (experienceText != null)
            experienceText.text = $"EXP: 0/100"; // สามารถเพิ่มระบบ EXP ได้
            
        if (baseStatsText != null)
            baseStatsText.text = $"Base: ATK{stats.baseAttack} DEF{stats.baseDefense} SPD{stats.baseSpeed}";
            
        if (equipmentBonusText != null)
        {
            int bonusAtk = stats.totalAttack - stats.baseAttack;
            int bonusDef = stats.totalDefense - stats.baseDefense;
            int bonusSpd = stats.totalSpeed - stats.baseSpeed;
            equipmentBonusText.text = $"Equip: ATK+{bonusAtk} DEF+{bonusDef} SPD+{bonusSpd}";
        }
    }
    
    /// <summary>
    /// จัดการการฟื้นฟู HP/MP อัตโนมัติ
    /// </summary>
    void HandleRegeneration()
    {
        if (isDead) return;
        
        regenerationTimer += Time.deltaTime;
        
        if (regenerationTimer >= regenerationInterval)
        {
            regenerationTimer = 0f;
            
            bool healed = false;
            bool restoredMana = false;
            
            // ฟื้น HP
            if (stats.currentHealth < stats.maxHealth)
            {
                Heal(healthRegenAmount);
                healed = true;
            }
            
            // ฟื้น MP
            if (stats.currentMana < stats.maxMana)
            {
                RestoreMana(manaRegenAmount);
                restoredMana = true;
            }
            
            if (healed || restoredMana)
            {
                UpdateAllUI();
            }
        }
    }
    
    /// <summary>
    /// ฟื้น HP
    /// </summary>
    public void Heal(int amount)
    {
        if (stats == null || isDead) return;
        
        int oldHealth = stats.currentHealth;
        stats.Heal(amount);
        
        OnHealthChanged?.Invoke(oldHealth, stats.currentHealth);
        
        if (amount > 0)
            Debug.Log($"💚 Healed {amount} HP. Current: {stats.currentHealth}/{stats.maxHealth}");
    }
    
    /// <summary>
    /// รับความเสียหาย
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (stats == null || isDead) return;
        
        int oldHealth = stats.currentHealth;
        stats.TakeDamage(damage);
        
        OnHealthChanged?.Invoke(oldHealth, stats.currentHealth);
        
        Debug.Log($"💔 Took {damage} damage. Current: {stats.currentHealth}/{stats.maxHealth}");
        
        // ตรวจสอบการตาย
        if (stats.IsDead() && !isDead)
        {
            isDead = true;
            OnDeath?.Invoke();
            Debug.Log("💀 Player died!");
        }
    }
    
    /// <summary>
    /// ฟื้น MP
    /// </summary>
    public void RestoreMana(int amount)
    {
        if (stats == null || isDead) return;
        
        int oldMana = stats.currentMana;
        stats.RestoreMana(amount);
        
        OnManaChanged?.Invoke(oldMana, stats.currentMana);
        
        if (amount > 0)
            Debug.Log($"💙 Restored {amount} MP. Current: {stats.currentMana}/{stats.maxMana}");
    }
    
    /// <summary>
    /// ใช้ MP
    /// </summary>
    public bool UseMana(int amount)
    {
        if (stats == null || isDead) return false;
        
        int oldMana = stats.currentMana;
        bool success = stats.UseMana(amount);
        
        if (success)
        {
            OnManaChanged?.Invoke(oldMana, stats.currentMana);
            Debug.Log($"💨 Used {amount} MP. Current: {stats.currentMana}/{stats.maxMana}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Not enough MP! Need {amount}, have {stats.currentMana}");
        }
        
        return success;
    }
    
    /// <summary>
    /// ใช้ไอเท็ม
    /// </summary>
    public void UseItem(ItemData item)
    {
        if (stats == null || isDead) return;
        
        if (item == null)
        {
            Debug.LogWarning("⚠️ Item is null!");
            return;
        }
        
        bool used = false;
        
        // ฟื้น HP
        if (item.healAmount > 0)
        {
            Heal(item.healAmount);
            used = true;
        }
        
        // ฟื้น MP (ถ้ามีฟิลด์ manaRestore)
        if (item.manaRestore > 0)
        {
            RestoreMana(item.manaRestore);
            used = true;
        }
        
        // เพิ่มสถานะชั่วคราว (สามารถเพิ่มได้)
        // เช่น Buff, Debuff ฯลฯ
        
        if (used)
        {
            UpdateAllUI();
            Debug.Log($"🧪 Used item: {item.itemName}");
        }
    }
    
    /// <summary>
    /// ฟื้นสถานะทั้งหมด
    /// </summary>
    public void FullRestore()
    {
        if (stats == null) return;
        
        stats.FullRestore();
        isDead = false;
        
        OnRevive?.Invoke();
        UpdateAllUI();
        
        Debug.Log("✨ Full restore! HP and MP restored to maximum.");
    }
    
    /// <summary>
    /// อัปเดตสถานะ (เรียกเมื่อ Equipment เปลี่ยน)
    /// </summary>
    public void UpdateStats()
    {
        CalculateTotalStats();
        UpdateAllUI();
    }
    
    /// <summary>
    /// รับข้อมูลสถานะ
    /// </summary>
    public PlayerStats GetStats()
    {
        return stats;
    }
    
    /// <summary>
    /// ตรวจสอบว่าตายหรือไม่
    /// </summary>
    public bool IsDead()
    {
        return isDead || (stats != null && stats.IsDead());
    }
    
    /// <summary>
    /// ตั้งค่า EquipmentManager (สำหรับการเชื่อมต่อแบบ manual)
    /// </summary>
    public void SetEquipmentManager(EquipmentManager manager)
    {
        equipmentManager = manager;
        CalculateTotalStats();
        UpdateAllUI();
    }
    
    void OnDestroy()
    {
        // Cleanup events
        OnHealthChanged = null;
        OnManaChanged = null;
        OnLevelUp = null;
        OnDeath = null;
        OnRevive = null;
    }
}
