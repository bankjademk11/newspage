using UnityEngine;

/// <summary>
/// Test script for verifying Combat System functionality
/// </summary>
public class CombatSystemTester : MonoBehaviour
{
    [Header("🧪 Test Settings")]
    [Tooltip("Enable combat system testing")]
    public bool enableTesting = true;
    [Tooltip("Test damage amount")]
    public int testDamage = 25;
    [Tooltip("Test enemy prefab")]
    public GameObject testEnemyPrefab;
    
    [Header("🎮 References")]
    [Tooltip("Combat Manager")]
    public CombatManager combatManager;
    [Tooltip("Target Manager")]
    public TargetManager targetManager;
    [Tooltip("Player Stats Manager")]
    public PlayerStatsManager playerStatsManager;
    
    void Start()
    {
        if (enableTesting)
        {
            StartCoroutine(TestCombatSystem());
        }
    }
    
    /// <summary>
    /// Test the combat system functionality
    /// </summary>
    System.Collections.IEnumerator TestCombatSystem()
    {
        Debug.Log("🧪 Starting Combat System Test...");
        
        // Wait for systems to initialize
        yield return new WaitForSeconds(1f);
        
        // Test 1: Check if all components are found
        yield return StartCoroutine(TestComponentReferences());
        
        // Test 2: Test damage calculation
        yield return StartCoroutine(TestDamageCalculation());
        
        // Test 3: Test enemy spawning
        yield return StartCoroutine(TestEnemySpawning());
        
        // Test 4: Test combat flow
        yield return StartCoroutine(TestCombatFlow());
        
        Debug.Log("✅ Combat System Test Completed!");
    }
    
    /// <summary>
    /// Test component references
    /// </summary>
    System.Collections.IEnumerator TestComponentReferences()
    {
        Debug.Log("🔍 Testing Component References...");
        
        // Test CombatManager
        if (combatManager == null)
        {
            combatManager = FindObjectOfType<CombatManager>();
        }
        
        if (combatManager != null)
        {
            Debug.Log("✅ CombatManager found");
        }
        else
        {
            Debug.LogError("❌ CombatManager not found!");
        }
        
        // Test TargetManager
        if (targetManager == null)
        {
            targetManager = FindObjectOfType<TargetManager>();
        }
        
        if (targetManager != null)
        {
            Debug.Log("✅ TargetManager found");
        }
        else
        {
            Debug.LogError("❌ TargetManager not found!");
        }
        
        // Test PlayerStatsManager
        if (playerStatsManager == null)
        {
            playerStatsManager = FindObjectOfType<PlayerStatsManager>();
        }
        
        if (playerStatsManager != null)
        {
            Debug.Log("✅ PlayerStatsManager found");
        }
        else
        {
            Debug.LogError("❌ PlayerStatsManager not found!");
        }
        
        yield return new WaitForSeconds(0.5f);
    }
    
    /// <summary>
    /// Test damage calculation
    /// </summary>
    System.Collections.IEnumerator TestDamageCalculation()
    {
        Debug.Log("⚔️ Testing Damage Calculation...");
        
        // Test basic damage calculation
        int attack = 50;
        int defense = 10;
        int expectedDamage = attack - defense;
        int actualDamage = DamageCalculator.CalculateBasicDamage(attack, defense);
        
        if (actualDamage == expectedDamage)
        {
            Debug.Log($"✅ Basic damage calculation: {attack} - {defense} = {actualDamage}");
        }
        else
        {
            Debug.LogError($"❌ Basic damage calculation failed: expected {expectedDamage}, got {actualDamage}");
        }
        
        // Test critical damage
        DamageResult critResult = DamageCalculator.CalculateCriticalDamage(actualDamage, 1.0f, 2.0f);
        Debug.Log($"✅ Critical damage test: {critResult.damage} (isCritical: {critResult.isCritical})");
        
        // Test miss calculation
        bool missed = DamageCalculator.IsAttackMissed(50, 100);
        Debug.Log($"✅ Miss calculation test: missed = {missed}");
        
        yield return new WaitForSeconds(0.5f);
    }
    
    /// <summary>
    /// Test enemy spawning
    /// </summary>
    System.Collections.IEnumerator TestEnemySpawning()
    {
        Debug.Log("👾 Testing Enemy Spawning...");
        
        if (testEnemyPrefab != null)
        {
            // Spawn test enemy
            Vector3 spawnPosition = transform.position + Vector3.right * 3f;
            GameObject enemyObj = Instantiate(testEnemyPrefab, spawnPosition, Quaternion.identity);
            
            // Check if enemy has required components
            EnemyStats enemyStats = enemyObj.GetComponent<EnemyStats>();
            EnemyController enemyController = enemyObj.GetComponent<EnemyController>();
            
            if (enemyStats != null)
            {
                Debug.Log($"✅ Enemy spawned with {enemyStats.GetStats().currentHealth} HP");
            }
            else
            {
                Debug.LogError("❌ Enemy missing EnemyStats component!");
            }
            
            if (enemyController != null)
            {
                Debug.Log("✅ Enemy has EnemyController component");
            }
            else
            {
                Debug.LogError("❌ Enemy missing EnemyController component!");
            }
            
            // Clean up after test
            yield return new WaitForSeconds(2f);
            if (enemyObj != null)
            {
                DestroyImmediate(enemyObj);
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No test enemy prefab assigned");
        }
        
        yield return new WaitForSeconds(0.5f);
    }
    
    /// <summary>
    /// Test combat flow
    /// </summary>
    System.Collections.IEnumerator TestCombatFlow()
    {
        Debug.Log("⚔️ Testing Combat Flow...");
        
        if (combatManager != null && playerStatsManager != null)
        {
            // Test player taking damage
            int initialHealth = playerStatsManager.GetStats().currentHealth;
            playerStatsManager.TakeDamage(testDamage);
            
            yield return new WaitForSeconds(0.1f);
            
            int newHealth = playerStatsManager.GetStats().currentHealth;
            int actualDamage = initialHealth - newHealth;
            
            if (actualDamage == testDamage)
            {
                Debug.Log($"✅ Player damage test: took {actualDamage} damage");
            }
            else
            {
                Debug.LogError($"❌ Player damage test failed: expected {testDamage}, took {actualDamage}");
            }
            
            // Test player healing
            playerStatsManager.Heal(testDamage / 2);
            yield return new WaitForSeconds(0.1f);
            
            int healedHealth = playerStatsManager.GetStats().currentHealth;
            Debug.Log($"✅ Player heal test: healed to {healedHealth} HP");
        }
        else
        {
            Debug.LogWarning("⚠️ Cannot test combat flow - missing components");
        }
        
        yield return new WaitForSeconds(0.5f);
    }
    
    /// <summary>
    /// Manual test function (called from inspector button)
    /// </summary>
    [ContextMenu("Test Combat System")]
    public void ManualTest()
    {
        if (Application.isPlaying)
        {
            StartCoroutine(TestCombatSystem());
        }
        else
        {
            Debug.LogWarning("⚠️ Combat System Test can only be run in Play Mode");
        }
    }
    
    /// <summary>
    /// Spawn test enemy manually
    /// </summary>
    [ContextMenu("Spawn Test Enemy")]
    public void SpawnTestEnemy()
    {
        if (testEnemyPrefab != null)
        {
            Vector3 spawnPosition = transform.position + Vector3.right * 3f;
            Instantiate(testEnemyPrefab, spawnPosition, Quaternion.identity);
            Debug.Log("👾 Test enemy spawned");
        }
        else
        {
            Debug.LogError("❌ No test enemy prefab assigned!");
        }
    }
    
    void OnDrawGizmos()
    {
        // Draw test area
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(6f, 4f, 1f));
        
        // Draw spawn position
        if (testEnemyPrefab != null)
        {
            Gizmos.color = Color.red;
            Vector3 spawnPosition = transform.position + Vector3.right * 3f;
            Gizmos.DrawWireSphere(spawnPosition, 0.5f);
            Gizmos.DrawLine(transform.position, spawnPosition);
        }
    }
}
