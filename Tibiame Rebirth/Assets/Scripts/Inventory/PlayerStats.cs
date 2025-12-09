using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    [Header("สถานะพื้นฐาน")]
    public int maxHealth = 100;
    public int currentHealth = 100;
    public int maxMana = 50;
    public int currentMana = 50;
    
    [Header("สถานะการต่อสู้")]
    public int baseAttack = 10;
    public int baseDefense = 5;
    public int baseSpeed = 3;
    
    [Header("💰 Money")]
    public int gold = 100;
    public int maxGold = 999999;
    
    [Header("🎯 Experience")]
    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;
    
    [Header("สถานะรวม (รวมจาก Equipment)")]
    public int totalAttack;
    public int totalDefense;
    public int totalSpeed;
    
    // คำนวณสถานะรวม
    public void CalculateTotalStats(EquipmentManager equipmentManager)
    {
        totalAttack = baseAttack;
        totalDefense = baseDefense;
        totalSpeed = baseSpeed;
        
        if (equipmentManager != null)
        {
            // เพิ่มสถานะจาก Equipment ที่สวมใส่
            foreach (var slot in equipmentManager.GetAllEquipmentSlots())
            {
                ItemData equippedItem = slot.GetEquippedItem();
                if (equippedItem != null)
                {
                    totalAttack += equippedItem.attackPower;
                    totalDefense += equippedItem.defense;
                    totalSpeed += equippedItem.speed;
                }
            }
        }
    }
    
    // ฟื้น HP
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }
    
    // ลด HP
    public void TakeDamage(int damage)
    {
        int actualDamage = Mathf.Max(damage - totalDefense, 1); // ลดความเสียหายตาม Defense
        currentHealth = Mathf.Max(currentHealth - actualDamage, 0);
    }
    
    // ฟื้น Mana
    public void RestoreMana(int amount)
    {
        currentMana = Mathf.Min(currentMana + amount, maxMana);
    }
    
    // ใช้ Mana
    public bool UseMana(int amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            return true;
        }
        return false;
    }
    
    // ตรวจสอบว่าตายหรือไม่
    public bool IsDead()
    {
        return currentHealth <= 0;
    }
    
    // ฟื้นสถานะทั้งหมด
    public void FullRestore()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
    }
}

// PlayerStatsManager class moved to separate file: PlayerStatsManager.cs
