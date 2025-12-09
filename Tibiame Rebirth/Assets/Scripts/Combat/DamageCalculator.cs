using UnityEngine;

/// <summary>
/// คำนวณความเสียหายสำหรับระบบต่อสู้แบบ Tibia (เวอร์ชันใหม่ - ไม่ใช้ Speed)
/// </summary>
public static class DamageCalculator
{
    /// <summary>
    /// คำนวณความเสียหายพื้นฐาน
    /// </summary>
    /// <param name="attackerAttack">พลังโจมตีของผู้โจมตี</param>
    /// <param name="targetDefense">พลังป้องกันของเป้าหมาย</param>
    /// <returns>ความเสียหายที่คำนวณได้</returns>
    public static int CalculateBasicDamage(int attackerAttack, int targetDefense)
    {
        // สูตรความเสียหายแบบ Tibia: Max(1, Attack - Defense/2)
        int defenseReduction = targetDefense / 2;
        int baseDamage = attackerAttack - defenseReduction;
        
        // ความเสียหายขั้นต่ำคือ 1
        int finalDamage = Mathf.Max(1, baseDamage);
        
        // เพิ่มความสุ่ม ±20%
        float randomFactor = Random.Range(0.8f, 1.2f);
        finalDamage = Mathf.RoundToInt(finalDamage * randomFactor);
        
        return Mathf.Max(1, finalDamage);
    }
    
    /// <summary>
    /// คำนวณความเสียหายเวทมนตร์
    /// </summary>
    /// <param name="magicPower">พลังเวทมนตร์</param>
    /// <param name="targetMagicDefense">พลังป้องกันเวทมนตร์</param>
    /// <returns>ความเสียหายเวทมนตร์</returns>
    public static int CalculateMagicDamage(int magicPower, int targetMagicDefense)
    {
        int baseDamage = magicPower - (targetMagicDefense / 3);
        int finalDamage = Mathf.Max(1, baseDamage);
        
        // เวทมนตร์มีความสุ่มมากกว่า ±30%
        float randomFactor = Random.Range(0.7f, 1.3f);
        finalDamage = Mathf.RoundToInt(finalDamage * randomFactor);
        
        return Mathf.Max(1, finalDamage);
    }
    
    /// <summary>
    /// คำนวณความเสียหายระยะไกล
    /// </summary>
    /// <param name="distanceAttack">พลังโจมตีระยะไกล</param>
    /// <param name="targetDefense">พลังป้องกันของเป้าหมาย</param>
    /// <param name="distance">ระยะห่าง</param>
    /// <param name="optimalRange">ระยะที่เหมาะสมที่สุด</param>
    /// <returns>ความเสียหายระยะไกล</returns>
    public static int CalculateRangedDamage(int distanceAttack, int targetDefense, float distance, float optimalRange = 5f)
    {
        int baseDamage = CalculateBasicDamage(distanceAttack, targetDefense);
        
        // ปรับความเสียหายตามระยะห่าง
        float distanceModifier = CalculateDistanceModifier(distance, optimalRange);
        baseDamage = Mathf.RoundToInt(baseDamage * distanceModifier);
        
        return baseDamage;
    }
    
    /// <summary>
    /// คำนวณตัวคูณระยะห่าง
    /// </summary>
    /// <param name="distance">ระยะห่างจริง</param>
    /// <param name="optimalRange">ระยะที่เหมาะสม</param>
    /// <returns>ตัวคูณระยะห่าง (0.5 - 1.0)</returns>
    private static float CalculateDistanceModifier(float distance, float optimalRange)
    {
        if (distance <= optimalRange)
        {
            return 1.0f; // ระยะเหมาะสม เต็มประสิทธิภาพ
        }
        
        // เริ่มลดประสิทธิภาพหลังระยะเหมาะสม
        float excessDistance = distance - optimalRange;
        float penalty = Mathf.Min(0.5f, excessDistance * 0.1f); // ลดสูงสุด 50%
        
        return 1.0f - penalty;
    }
    
    /// <summary>
    /// คำนวณความเสียหายคริติคอล (Critical Hit)
    /// </summary>
    /// <param name="baseDamage">ความเสียหายพื้นฐาน</param>
    /// <param name="criticalChance">โอกาสคริติคอล (0-1)</param>
    /// <param name="criticalMultiplier">ตัวคูณคริติคอล</param>
    /// <returns>ความเสียหายพร้อมข้อมูลคริติคอล</returns>
    public static DamageResult CalculateCriticalDamage(int baseDamage, float criticalChance = 0.1f, float criticalMultiplier = 2.0f)
    {
        bool isCritical = Random.value <= criticalChance;
        int finalDamage = baseDamage;
        
        if (isCritical)
        {
            finalDamage = Mathf.RoundToInt(baseDamage * criticalMultiplier);
            Debug.Log($"💥 CRITICAL HIT! {baseDamage} → {finalDamage}");
        }
        
        return new DamageResult(finalDamage, isCritical);
    }
    
    /// <summary>
    /// คำนวณความเสียหายจากสกิล
    /// </summary>
    /// <param name="skillDamage">ความเสียหายของสกิล</param>
    /// <param name="attackerStats">สถานะผู้โจมตี</param>
    /// <param name="targetStats">สถานะเป้าหมาย</param>
    /// <param name="skillType">ประเภทสกิล</param>
    /// <returns>ความเสียหายจากสกิล</returns>
    public static int CalculateSkillDamage(int skillDamage, PlayerStats attackerStats, PlayerStats targetStats, SkillType skillType)
    {
        int baseDamage = skillDamage;
        
        switch (skillType)
        {
            case SkillType.Physical:
                baseDamage += attackerStats.totalAttack;
                return CalculateBasicDamage(baseDamage, targetStats.totalDefense);
                
            case SkillType.Magical:
                baseDamage += attackerStats.totalAttack; // ใช้ Attack เป็น Magic Power ชั่วคราว
                return CalculateMagicDamage(baseDamage, targetStats.totalDefense / 2);
                
            case SkillType.Healing:
                // สกิลฟื้นฟู ไม่คำนวณความเสียหาย
                return baseDamage;
                
            default:
                return baseDamage;
        }
    }
    
    /// <summary>
    /// ตรวจสอบว่าโจมตีพลาดหรือไม่ (เวอร์ชันใหม่ - ไม่ใช้ Speed)
    /// </summary>
    /// <param name="attackerAccuracy">ความแม่นยำของผู้โจมตี (ไม่ใช้แล้ว)</param>
    /// <param name="targetEvasion">ความว่องไวของเป้าหมาย (ไม่ใช้แล้ว)</param>
    /// <returns>true ถ้าโจมตีพลาด</returns>
    public static bool IsAttackMissed(int attackerAccuracy, int targetEvasion)
    {
        // เวอร์ชันใหม่: โจมตีโดน 100% ถ้าอยู่ในระยะ
        // เหลือไว้เผื่อ backward compatibility
        return false;
    }
    
    /// <summary>
    /// ตรวจสอบว่าโจมตีพลาดหรือไม่ (เวอร์ชันใหม่ล้วน)
    /// </summary>
    /// <returns>เสมอคือ false (โจมตีโดนเสมอ)</returns>
    public static bool IsAttackMissed()
    {
        // ระบบใหม่: ไม่มีการพลาด โจมตีโดนเสมอถ้าอยู่ในระยะ
        return false;
    }
    
    /// <summary>
    /// คำนวณความเสียหายที่ลดลงจาก Armor
    /// </summary>
    /// <param name="damage">ความเสียหายก่อนลด</param>
    /// <param name="armor">ค่า Armor</param>
    /// <returns>ความเสียหายหลังลด</returns>
    public static int ApplyArmorReduction(int damage, int armor)
    {
        // Armor ลดความเสียหาย 1 ต่อ 10 armor
        float reductionPercentage = armor * 0.01f; // 1% ต่อ 1 armor
        reductionPercentage = Mathf.Min(0.8f, reductionPercentage); // ลดสูงสุด 80%
        
        int reducedDamage = Mathf.RoundToInt(damage * (1f - reductionPercentage));
        return Mathf.Max(1, reducedDamage);
    }
}

/// <summary>
/// ผลลัพธ์การคำนวณความเสียหาย
/// </summary>
public struct DamageResult
{
    public int damage;
    public bool isCritical;
    public bool isMissed;
    
    public DamageResult(int damage, bool isCritical, bool isMissed = false)
    {
        this.damage = damage;
        this.isCritical = isCritical;
        this.isMissed = isMissed;
    }
}

/// <summary>
/// ประเภทของสกิล
/// </summary>
public enum SkillType
{
    Physical,
    Magical,
    Healing
}
