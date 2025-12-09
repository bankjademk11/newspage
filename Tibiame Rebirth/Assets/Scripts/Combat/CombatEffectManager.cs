using UnityEngine;

/// <summary>
/// จัดการ Particle Effects สำหรับการต่อสู้ - แยกระหว่าง Player และ Enemy
/// </summary>
public class CombatEffectManager : MonoBehaviour
{
    [Header("🗡️ Player Attack Effects")]
    [Tooltip("Effect สำหรับการโจมตีปกติของ Player")]
    public GameObject playerSlashEffect;
    [Tooltip("Effect สำหรับ Power Attack ของ Player")]
    public GameObject playerPowerAttackEffect;
    [Tooltip("Effect สำหรับ Double Strike ของ Player")]
    public GameObject playerDoubleStrikeEffect;
    
    [Header("🛡️ Player Hit Effects")]
    [Tooltip("Effect ตอน Player โดนโจมตีปกติ")]
    public GameObject playerHitEffect;
    [Tooltip("Effect ตอน Player โดนคริติคอล")]
    public GameObject playerCriticalHitEffect;
    
    [Header("👹 Enemy Attack Effects")]
    [Tooltip("Effect สำหรับการโจมตีปกติของ Enemy")]
    public GameObject enemySlashEffect;
    [Tooltip("Effect สำหรับการโจมตีพิเศษของ Enemy")]
    public GameObject enemySpecialAttackEffect;
    
    [Header("💀 Enemy Hit Effects")]
    [Tooltip("Effect ตอน Enemy โดนโจมตีปกติ")]
    public GameObject enemyHitEffect;
    [Tooltip("Effect ตอน Enemy โดนคริติคอล")]
    public GameObject enemyCriticalHitEffect;
    
    [Header("⚙️ Settings")]
    [Tooltip("ระยะเวลาของ Effect (วินาที)")]
    public float effectDuration = 1.0f;
    [Tooltip("ความเร็วในการเคลื่อนที่ของ Effect")]
    public float effectSpeed = 5.0f;
    
    // Singleton instance
    public static CombatEffectManager Instance { get; private set; }
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        Debug.Log("✨ CombatEffectManager initialized!");
    }
    
    /// <summary>
    /// แสดง Effect การโจมตีของ Player
    /// </summary>
    /// <param name="attackType">ประเภทการโจมตี</param>
    /// <param name="startPos">ตำแหน่งเริ่มต้น (Player)</param>
    /// <param name="targetPos">ตำแหน่งเป้าหมาย (Enemy)</param>
    public void ShowPlayerAttackEffect(PlayerAttackType attackType, Vector3 startPos, Vector3 targetPos)
    {
        try
        {
            GameObject effectPrefab = GetPlayerAttackEffect(attackType);
            if (effectPrefab != null)
            {
                CreateMovingEffect(effectPrefab, startPos, targetPos);
            }
            else
            {
                Debug.LogWarning($"⚠️ Player attack effect prefab is null for type: {attackType}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error in ShowPlayerAttackEffect: {e.Message}");
        }
    }
    
    /// <summary>
    /// แสดง Effect การโจมตีของ Enemy
    /// </summary>
    /// <param name="attackType">ประเภทการโจมตี</param>
    /// <param name="startPos">ตำแหน่งเริ่มต้น (Enemy)</param>
    /// <param name="targetPos">ตำแหน่งเป้าหมาย (Player)</param>
    public void ShowEnemyAttackEffect(EnemyAttackType attackType, Vector3 startPos, Vector3 targetPos)
    {
        try
        {
            GameObject effectPrefab = GetEnemyAttackEffect(attackType);
            if (effectPrefab != null)
            {
                CreateMovingEffect(effectPrefab, startPos, targetPos);
            }
            else
            {
                Debug.LogWarning($"⚠️ Enemy attack effect prefab is null for type: {attackType}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error in ShowEnemyAttackEffect: {e.Message}");
        }
    }
    
    /// <summary>
    /// แสดง Effect ตอนโดนโจมตี
    /// </summary>
    /// <param name="target">เป้าหมายที่โดนโจมตี</param>
    /// <param name="isCritical">เป็นคริติคอลหรือไม่</param>
    /// <param name="isPlayer">เป็น Player หรือไม่</param>
    public void ShowHitEffect(GameObject target, bool isCritical = false, bool isPlayer = true)
    {
        try
        {
            if (target == null)
            {
                Debug.LogWarning("⚠️ Target is null in ShowHitEffect!");
                return;
            }
            
            GameObject effectPrefab = GetHitEffect(isCritical, isPlayer);
            if (effectPrefab != null)
            {
                CreateStaticEffect(effectPrefab, target.transform.position);
            }
            else
            {
                Debug.LogWarning($"⚠️ Hit effect prefab is null - Critical: {isCritical}, IsPlayer: {isPlayer}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error in ShowHitEffect: {e.Message}");
        }
    }
    
    /// <summary>
    /// สร้าง Effect ที่เคลื่อนที่จากจุดเริ่มต้นไปยังเป้าหมาย
    /// </summary>
    private void CreateMovingEffect(GameObject effectPrefab, Vector3 startPos, Vector3 targetPos)
    {
        // ตรวจสอบว่า prefab ไม่ใช่ null
        if (effectPrefab == null)
        {
            Debug.LogWarning("⚠️ Effect prefab is null!");
            return;
        }
        
        GameObject effect = null;
        try
        {
            effect = Instantiate(effectPrefab, startPos, Quaternion.identity);
            
            // ตรวจสอบว่า effect สร้างสำเร็จ
            if (effect == null)
            {
                Debug.LogError("❌ Failed to instantiate effect!");
                return;
            }
            
            // คำนวณทิศทาง
            Vector3 direction = (targetPos - startPos).normalized;
            float distance = Vector3.Distance(startPos, targetPos);
            
            // หมุน Effect ให้ตามทิศทาง
            if (direction != Vector3.zero)
            {
                effect.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
            }
            
            // เคลื่อนที่ Effect ไปยังเป้าหมาย
            StartCoroutine(MoveEffectCoroutine(effect, startPos, targetPos, distance));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error creating moving effect: {e.Message}");
            if (effect != null)
            {
                Destroy(effect);
            }
        }
    }
    
    /// <summary>
    /// สร้าง Effect ที่อยู่กับที่
    /// </summary>
    private void CreateStaticEffect(GameObject effectPrefab, Vector3 position)
    {
        // ตรวจสอบว่า prefab ไม่ใช่ null
        if (effectPrefab == null)
        {
            Debug.LogWarning("⚠️ Static effect prefab is null!");
            return;
        }
        
        GameObject effect = null;
        try
        {
            effect = Instantiate(effectPrefab, position, Quaternion.identity);
            
            // ตรวจสอบว่า effect สร้างสำเร็จ
            if (effect == null)
            {
                Debug.LogError("❌ Failed to instantiate static effect!");
                return;
            }
            
            // ทำลาย Effect หลังจากเวลาผ่านไป
            Destroy(effect, effectDuration);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error creating static effect: {e.Message}");
            if (effect != null)
            {
                Destroy(effect);
            }
        }
    }
    
    /// <summary>
    /// Coroutine สำหรับเคลื่อนที่ Effect
    /// </summary>
    private System.Collections.IEnumerator MoveEffectCoroutine(GameObject effect, Vector3 startPos, Vector3 targetPos, float distance)
    {
        if (effect == null)
        {
            Debug.LogWarning("⚠️ Effect is null in MoveEffectCoroutine!");
            yield break;
        }
        
        float travelTime = distance / effectSpeed;
        float elapsedTime = 0f;
        
        while (elapsedTime < travelTime)
        {
            // ตรวจสอบว่า effect ยังมีอยู่
            if (effect == null)
            {
                Debug.Log("🔄 Effect was destroyed, stopping coroutine");
                yield break;
            }
            
            effect.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / travelTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // ทำลาย Effect หลังจากถึงเป้าหมาย
        if (effect != null)
        {
            Destroy(effect);
        }
    }
    
    /// <summary>
    /// ดึง Effect การโจมตีของ Player ตามประเภท
    /// </summary>
    private GameObject GetPlayerAttackEffect(PlayerAttackType attackType)
    {
        try
        {
            switch (attackType)
            {
                case PlayerAttackType.Normal:
                    return playerSlashEffect;
                case PlayerAttackType.Power:
                    return playerPowerAttackEffect;
                case PlayerAttackType.DoubleStrike:
                    return playerDoubleStrikeEffect;
                default:
                    return playerSlashEffect;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error getting player attack effect: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// ดึง Effect การโจมตีของ Enemy ตามประเภท
    /// </summary>
    private GameObject GetEnemyAttackEffect(EnemyAttackType attackType)
    {
        try
        {
            switch (attackType)
            {
                case EnemyAttackType.Normal:
                    return enemySlashEffect;
                case EnemyAttackType.Special:
                    return enemySpecialAttackEffect;
                default:
                    return enemySlashEffect;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error getting enemy attack effect: {e.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// ดึง Effect การโดนโจมตี
    /// </summary>
    private GameObject GetHitEffect(bool isCritical, bool isPlayer)
    {
        try
        {
            if (isPlayer)
            {
                return isCritical ? playerCriticalHitEffect : playerHitEffect;
            }
            else
            {
                return isCritical ? enemyCriticalHitEffect : enemyHitEffect;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error getting hit effect: {e.Message}");
            return null;
        }
    }
    
    void OnDestroy()
    {
        // Cleanup singleton
        if (Instance == this)
        {
            Instance = null;
        }
    }
}

/// <summary>
/// ประเภทการโจมตีของ Player
/// </summary>
public enum PlayerAttackType
{
    Normal,
    Power,
    DoubleStrike
}

/// <summary>
/// ประเภทการโจมตีของ Enemy
/// </summary>
public enum EnemyAttackType
{
    Normal,
    Special
}
