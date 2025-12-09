using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// จัดการ UI การต่อสู้ - Target Health Bar, Damage Numbers, Combat Info
/// </summary>
public class CombatUI : MonoBehaviour
{
    [Header("🎯 Target Info UI")]
    [Tooltip("Panel แสดงข้อมูลเป้าหมาย")]
    public GameObject targetInfoPanel;
    [Tooltip("ข้อความชื่อเป้าหมาย")]
    public TextMeshProUGUI targetNameText;
    [Tooltip("แถบ HP ของเป้าหมาย")]
    public Slider targetHealthBar;
    [Tooltip("ข้อความ HP ของเป้าหมาย")]
    public TextMeshProUGUI targetHealthText;
    [Tooltip("ข้อความเลเวลเป้าหมาย")]
    public TextMeshProUGUI targetLevelText;
    
    [Header("💥 Damage Numbers")]
    [Tooltip("Prefab สำหรับแสดงตัวเลขความเสียหาย")]
    public GameObject damageNumberPrefab;
    [Tooltip("Transform ที่จะเป็นพาเรนต์ของ Damage Numbers")]
    public Transform damageNumberParent;
    [Tooltip("ระยะเวลาแสดงตัวเลขความเสียหาย (วินาที)")]
    public float damageNumberDuration = 2f;
    [Tooltip("ความเร็วในการลอยขึ้นของตัวเลข")]
    public float floatSpeed = 1f;
    [Tooltip("จำนวน Damage Numbers สูงสุดในเวลาเดียวกัน")]
    public int maxDamageNumbers = 50;
    
    [Header("🎨 Damage Number Colors")]
    [Tooltip("สีความเสียหายปกติ (Player)")]
    public Color normalDamageColor = Color.white;
    [Tooltip("สีความเสียหายจาก Enemy")]
    public Color enemyDamageColor = Color.red;
    [Tooltip("สีความเสียหายคริติคอล")]
    public Color criticalDamageColor = Color.yellow;
    [Tooltip("สีความเสียหายพลังสูง")]
    public Color powerDamageColor = Color.red;
    [Tooltip("สีการพลาด")]
    public Color missColor = Color.gray;
    [Tooltip("สีการฟื้นฟู")]
    public Color healColor = Color.green;
    
    [Header("⚔️ Combat Status")]
    [Tooltip("Panel แสดงสถานะการต่อสู้")]
    public GameObject combatStatusPanel;
    [Tooltip("ข้อความสถานะการต่อสู้")]
    public TextMeshProUGUI combatStatusText;
    [Tooltip("ไอคอนสถานะการต่อสู้")]
    public Image combatStatusIcon;
    
    [Header("🎮 Skill Cooldowns")]
    [Tooltip("Panel สกิล")]
    public GameObject skillPanel;
    [Tooltip("ไอคอนสกิล 1")]
    public Image skill1Icon;
    [Tooltip("ไอคอนสกิล 2")]
    public Image skill2Icon;
    [Tooltip("ข้อความ Cooldown สกิล 1")]
    public TextMeshProUGUI skill1CooldownText;
    [Tooltip("ข้อความ Cooldown สกิล 2")]
    public TextMeshProUGUI skill2CooldownText;
    
    // Private variables
    private TargetManager targetManager;
    private CombatManager combatManager;
    private Camera mainCamera;
    private GameObject currentTarget;
    private EnemyStats currentTargetStats;
    private int currentDamageNumberCount = 0;
    
    void Start()
    {
        InitializeComponents();
        SetupEventListeners();
        HideTargetInfo();
        HideCombatStatus();
        Debug.Log("🎨 CombatUI initialized successfully!");
    }
    
    void Update()
    {
        UpdateTargetInfo();
        UpdateDamageNumbersPosition();
    }
    
    /// <summary>
    /// ตั้งค่าคอมโพเนนต์เริ่มต้น
    /// </summary>
    void InitializeComponents()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
        
        // หา Managers
        targetManager = FindObjectOfType<TargetManager>();
        combatManager = FindObjectOfType<CombatManager>();
        
        // สร้าง damageNumberParent ถ้ายังไม่มี
        if (damageNumberParent == null)
        {
            GameObject parentObj = new GameObject("DamageNumberParent");
            damageNumberParent = parentObj.transform;
        }
        
        // ซ่อน UI ทั้งหมดเริ่มต้น
        HideTargetInfo();
        HideCombatStatus();
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
        
        if (combatManager != null)
        {
            combatManager.OnCombatStarted += OnCombatStarted;
            combatManager.OnCombatEnded += OnCombatEnded;
        }
    }
    
    /// <summary>
    /// อัปเดตข้อมูลเป้าหมาย
    /// </summary>
    void UpdateTargetInfo()
    {
        if (currentTarget == null || currentTargetStats == null)
        {
            HideTargetInfo();
            return;
        }
        
        // อัปเดตข้อความ
        if (targetNameText != null)
            targetNameText.text = currentTarget.name;
            
        if (targetLevelText != null)
            targetLevelText.text = $"Lv.{currentTargetStats.GetStats().level}";
            
        if (targetHealthBar != null)
        {
            targetHealthBar.maxValue = currentTargetStats.GetStats().maxHealth;
            targetHealthBar.value = currentTargetStats.GetStats().currentHealth;
        }
        
        if (targetHealthText != null)
            targetHealthText.text = $"{currentTargetStats.GetStats().currentHealth}/{currentTargetStats.GetStats().maxHealth}";
    }
    
    /// <summary>
    /// แสดงข้อมูลเป้าหมาย
    /// </summary>
    void ShowTargetInfo()
    {
        if (targetInfoPanel != null)
            targetInfoPanel.SetActive(true);
    }
    
    /// <summary>
    /// ซ่อนข้อมูลเป้าหมาย
    /// </summary>
    void HideTargetInfo()
    {
        if (targetInfoPanel != null)
            targetInfoPanel.SetActive(false);
    }
    
    /// <summary>
    /// แสดงตัวเลขความเสียหาย
    /// </summary>
    /// <param name="damage">ความเสียหาย</param>
    /// <param name="worldPosition">ตำแหน่งในโลก</param>
    /// <param name="isCritical">เป็นคริติคอลหรือไม่</param>
    /// <param name="isMissed">พลาดหรือไม่</param>
    /// <param name="isPowerAttack">เป็น Power Attack หรือไม่</param>
    /// <param name="isHeal">เป็นการฟื้นฟูหรือไม่</param>
    /// <param name="isEnemyAttack">เป็นการโจมตีจาก Enemy หรือไม่</param>
    public void ShowDamageNumber(int damage, Vector3 worldPosition, bool isCritical = false, bool isMissed = false, bool isPowerAttack = false, bool isHeal = false, bool isEnemyAttack = false)
    {
        if (damageNumberPrefab == null || damageNumberParent == null) return;
        
        // ตรวจสอบจำนวน Damage Numbers เพื่อป้องกัน Memory Leak
        if (currentDamageNumberCount >= maxDamageNumbers)
        {
            Debug.LogWarning("⚠️ Maximum damage numbers reached! Skipping new damage number.");
            return;
        }
        
        // สร้าง Damage Number
        GameObject damageObj = Instantiate(damageNumberPrefab, damageNumberParent);
        currentDamageNumberCount++;
        TextMeshProUGUI damageText = damageObj.GetComponent<TextMeshProUGUI>();
        
        if (damageText == null)
        {
            Debug.LogWarning("⚠️ DamageNumber prefab missing TextMeshProUGUI component!");
        Destroy(damageObj);
        currentDamageNumberCount--;
            return;
        }
        
        // แปลงตำแหน่งจากโลกเป็นหน้าจอ และปรับตำแหน่งให้เหมือน TibiaME
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
        // เพิ่ม offset ให้สูงขึ้นเล็กน้อยเหมือนใน TibiaME
        screenPosition.y += 30f;
        damageObj.transform.position = screenPosition;
        
        // ตั้งค่าข้อความและสี
        if (isMissed)
        {
            damageText.text = "MISS";
            damageText.color = missColor;
        }
        else if (isHeal)
        {
            damageText.text = $"+{damage}";
            damageText.color = healColor;
        }
        else
        {
            damageText.text = damage.ToString();
            
            if (isPowerAttack)
                damageText.color = powerDamageColor;
            else if (isCritical)
                damageText.color = criticalDamageColor;
            else if (isEnemyAttack)
                damageText.color = enemyDamageColor;
            else
                damageText.color = normalDamageColor;
        }
        
        // ขยายตัวอักษรให้ใหญ่ขึ้นทั่วไป (ไม่ใหญ่มาก)
        damageText.fontSize *= 1.5f;
        
        // ขยายตัวอักษรเพิ่มเติมสำหรับคริติคอลและ Power Attack
        if (isCritical || isPowerAttack)
        {
            damageText.fontSize *= 1.3f;
        }
        
        // เริ่ม Animation
        StartCoroutine(AnimateDamageNumber(damageObj, isCritical || isPowerAttack));
    }
    
    /// <summary>
    /// Animation สำหรับ Damage Number
    /// </summary>
    IEnumerator AnimateDamageNumber(GameObject damageObj, bool isBig)
    {
        Vector3 startPosition = damageObj.transform.position;
        // ปรับระยะการลอยให้เหมือน TibiaME (ลอยขึ้นเล็กน้อย)
        Vector3 endPosition = startPosition + Vector3.up * (isBig ? 60f : 40f);
        
        float elapsed = 0f;
        Color startColor = damageObj.GetComponent<TextMeshProUGUI>().color;
        
        while (elapsed < damageNumberDuration)
        {
            elapsed += Time.deltaTime;
            
            // ลอยขึ้น
            damageObj.transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / damageNumberDuration);
            
            // จางหาย
            TextMeshProUGUI text = damageObj.GetComponent<TextMeshProUGUI>();
            text.color = new Color(startColor.r, startColor.g, startColor.b, 1f - (elapsed / damageNumberDuration));
            
            yield return null;
        }
        
        Destroy(damageObj);
    }
    
    /// <summary>
    /// อัปเดตตำแหน่งของ Damage Numbers (ตามกล้อง)
    /// </summary>
    void UpdateDamageNumbersPosition()
    {
        // Damage Numbers จะอัปเดตตำแหน่งตามกล้องอัตโนมัติผ่าน Screen Space - Overlay
    }
    
    /// <summary>
    /// แสดงสถานะการต่อสู้
    /// </summary>
    /// <param name="status">ข้อความสถานะ</param>
    /// <param name="isInCombat">อยู่ในการต่อสู้หรือไม่</param>
    void ShowCombatStatus(string status, bool isInCombat)
    {
        if (combatStatusPanel == null) return;
        
        combatStatusPanel.SetActive(true);
        
        if (combatStatusText != null)
            combatStatusText.text = status;
            
        if (combatStatusIcon != null)
        {
            combatStatusIcon.color = isInCombat ? Color.red : Color.green;
        }
    }
    
    /// <summary>
    /// ซ่อนสถานะการต่อสู้
    /// </summary>
    void HideCombatStatus()
    {
        if (combatStatusPanel != null)
            combatStatusPanel.SetActive(false);
    }
    
    /// <summary>
    /// อัปเดต Cooldown ของสกิล
    /// </summary>
    /// <param name="skillId">ID ของสกิล</param>
    /// <param name="cooldown">เวลา Cooldown ที่เหลือ</param>
    public void UpdateSkillCooldown(int skillId, float cooldown)
    {
        TextMeshProUGUI cooldownText = null;
        Image skillIcon = null;
        
        switch (skillId)
        {
            case 1:
                cooldownText = skill1CooldownText;
                skillIcon = skill1Icon;
                break;
            case 2:
                cooldownText = skill2CooldownText;
                skillIcon = skill2Icon;
                break;
        }
        
        if (cooldownText != null)
        {
            if (cooldown > 0)
            {
                cooldownText.text = cooldown.ToString("F1");
                cooldownText.gameObject.SetActive(true);
                
                if (skillIcon != null)
                    skillIcon.color = Color.gray;
            }
            else
            {
                cooldownText.gameObject.SetActive(false);
                
                if (skillIcon != null)
                    skillIcon.color = Color.white;
            }
        }
    }
    
    /// <summary>
    /// Event: เมื่อเลือกเป้าหมาย
    /// </summary>
    void OnTargetSelected(GameObject target)
    {
        currentTarget = target;
        currentTargetStats = target?.GetComponent<EnemyStats>();
        ShowTargetInfo();
    }
    
    /// <summary>
    /// Event: เมื่อยกเลิกเป้าหมาย
    /// </summary>
    void OnTargetDeselected(GameObject target)
    {
        currentTarget = null;
        currentTargetStats = null;
        HideTargetInfo();
    }
    
    /// <summary>
    /// Event: เมื่อเปลี่ยนเป้าหมาย
    /// </summary>
    void OnTargetChanged(GameObject newTarget)
    {
        currentTarget = newTarget;
        currentTargetStats = newTarget?.GetComponent<EnemyStats>();
        
        if (newTarget != null)
            ShowTargetInfo();
        else
            HideTargetInfo();
    }
    
    /// <summary>
    /// Event: เมื่อเริ่มการต่อสู้
    /// </summary>
    void OnCombatStarted()
    {
        ShowCombatStatus("IN COMBAT", true);
    }
    
    /// <summary>
    /// Event: เมื่อจบการต่อสู้
    /// </summary>
    void OnCombatEnded()
    {
        ShowCombatStatus("COMBAT ENDED", false);
        
        // ซ่อนหลัง 2 วินาที
        StartCoroutine(HideCombatStatusDelayed());
    }
    
    /// <summary>
    /// ซ่อนสถานะการต่อสู้แบบ delayed
    /// </summary>
    IEnumerator HideCombatStatusDelayed()
    {
        yield return new WaitForSeconds(2f);
        HideCombatStatus();
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
    /// ตั้งค่า CombatManager
    /// </summary>
    public void SetCombatManager(CombatManager manager)
    {
        combatManager = manager;
        SetupEventListeners();
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
        
        if (combatManager != null)
        {
            combatManager.OnCombatStarted -= OnCombatStarted;
            combatManager.OnCombatEnded -= OnCombatEnded;
        }
    }
}
