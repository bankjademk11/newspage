using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

/// <summary>
/// 🚀 Combat Auto Generator 2.0 - Smart Edition x10
/// Editor Tool สำหรับสร้าง Combat System อัตโนมัติแบบฉลาด
/// </summary>
public class CombatAutoGenerator : EditorWindow
{
    private GameObject combatSystemParent;
    private bool createTargetManager = true;
    private bool createCombatManager = true;
    private bool createCombatUI = true;
    private bool createEnemyPrefabs = true;
    private bool createEnemyDataAssets = true;
    
    // Combat Settings
    private float defaultAttackRange = 1.5f;
    private float defaultAttackSpeed = 1.0f;
    private float defaultCriticalChance = 0.1f;
    private float defaultCriticalMultiplier = 2.0f;
    
    // UI Settings
    private string combatUIName = "CombatUI";
    private string damageNumberPrefabName = "DamageNumber";
    private bool useExistingCanvas = true;
    private Canvas existingCanvas;
    
    // Enemy Settings
    private int numberOfEnemyTypes = 3;
    private string[] enemyNames = { "Goblin", "Orc", "Skeleton" };
    
    // 🆕 Smart Features
    private bool showAdvancedOptions = false;
    private bool showSystemStatus = true;
    private bool autoFixEnabled = true;
    private bool createBackup = true;
    private System.DateTime lastScanTime;
    private string systemStatusMessage = "Ready";
    private SystemMessageType statusType = SystemMessageType.Info;
    
    // 🆕 System Analysis
    private SystemAnalysisData analysisData = new SystemAnalysisData();
    private Vector2 scrollPosition;
    private bool showAnalysisDetails = false;
    
    // 🆕 Reset Options
    private ResetMode resetMode = ResetMode.Selective;
    private bool confirmBeforeReset = true;
    
    private enum SystemMessageType
    {
        Info, Warning, Error, Success
    }
    
    private enum ResetMode
    {
        Quick, Full, Selective, Safe
    }
    
    [System.Serializable]
    private class SystemAnalysisData
    {
        public int targetManagersFound;
        public int combatManagersFound;
        public int combatUIsFound;
        public int canvasesFound;
        public int enemyLayersFound;
        public List<string> issues = new List<string>();
        public List<string> suggestions = new List<string>();
        public bool needsAttention;
        public float systemHealth;
    }
    
    [MenuItem("Tools/UI Generator/Combat Auto Generator 2.0")]
    public static void ShowWindow()
    {
        GetWindow<CombatAutoGenerator>("🗡️ Combat Generator 2.0");
    }
    
    void OnEnable()
    {
        PerformSystemScan();
    }
    
    void OnGUI()
    {
        DrawHeader();
        DrawSystemStatus();
        DrawQuickActions();
        
        EditorGUILayout.Space(10);
        
        if (showAdvancedOptions)
        {
            DrawAdvancedOptions();
        }
        
        DrawGenerationSettings();
        DrawResetSection();
        
        // Auto-scan every 30 seconds
        if (EditorApplication.timeSinceStartup - lastScanTime.Ticks > 30000)
        {
            PerformSystemScan();
        }
    }
    
    /// <summary>
    /// 🎨 วาด Header สวยงาม
    /// </summary>
    void DrawHeader()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        
        // Title with gradient effect simulation
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 16;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.normal.textColor = Color.cyan;
        
        GUILayout.Label("🗡️ COMBAT AUTO GENERATOR 2.0 - SMART EDITION", titleStyle);
        
        EditorGUILayout.EndHorizontal();
        
        // Subtitle
        GUIStyle subtitleStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
        subtitleStyle.fontSize = 10;
        GUILayout.Label("Intelligent Combat System Creation & Management", subtitleStyle);
        
        EditorGUILayout.Space(5);
    }
    
    /// <summary>
    /// 📊 แสดงสถานะระบบแบบ Real-time
    /// </summary>
    void DrawSystemStatus()
    {
        if (!showSystemStatus) return;
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        // Status header
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("📊 SYSTEM STATUS", EditorStyles.boldLabel);
        
        // Status indicator with color
        Color originalColor = GUI.color;
        switch (statusType)
        {
            case SystemMessageType.Success:
                GUI.color = Color.green;
                break;
            case SystemMessageType.Warning:
                GUI.color = Color.yellow;
                break;
            case SystemMessageType.Error:
                GUI.color = Color.red;
                break;
            default:
                GUI.color = Color.cyan;
                break;
        }
        
        GUILayout.Label(systemStatusMessage, EditorStyles.boldLabel);
        GUI.color = originalColor;
        
        // Toggle button
        showAnalysisDetails = EditorGUILayout.Foldout(showAnalysisDetails, "📋 Details");
        EditorGUILayout.EndHorizontal();
        
        // Quick status overview
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label($"✅ TargetManager: {analysisData.targetManagersFound} Found", GUILayout.Width(150));
        GUILayout.Label($"⚔️ CombatManager: {analysisData.combatManagersFound} Found", GUILayout.Width(150));
        GUILayout.Label($"🎮 CombatUI: {analysisData.combatUIsFound} Found", GUILayout.Width(150));
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label($"🖼️ Canvas: {analysisData.canvasesFound} Found", GUILayout.Width(150));
        GUILayout.Label($"👾 Enemy Layer: {(analysisData.enemyLayersFound > 0 ? "✅" : "❌")}", GUILayout.Width(150));
        GUILayout.Label($"💚 Health: {analysisData.systemHealth:F0}%", GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();
        
        // Detailed analysis
        if (showAnalysisDetails)
        {
            EditorGUILayout.Space(5);
            GUILayout.Label("🔍 DETAILED ANALYSIS:", EditorStyles.boldLabel);
            
            if (analysisData.issues.Count > 0)
            {
                GUILayout.Label("⚠️ ISSUES DETECTED:", EditorStyles.boldLabel);
                foreach (string issue in analysisData.issues)
                {
                    GUILayout.Label($"  • {issue}", EditorStyles.helpBox);
                }
            }
            
            if (analysisData.suggestions.Count > 0)
            {
                GUILayout.Label("💡 SUGGESTIONS:", EditorStyles.boldLabel);
                foreach (string suggestion in analysisData.suggestions)
                {
                    GUILayout.Label($"  • {suggestion}", EditorStyles.helpBox);
                }
            }
        }
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);
    }
    
    /// <summary>
    /// 🚀 ปุ่ม Quick Actions สำหรับการทำงานรวดเร็ว
    /// </summary>
    void DrawQuickActions()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("🔍 QUICK ACTIONS", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        
        // Deep Scan button
        Color originalColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("🔍 Deep Scan", GUILayout.Height(30)))
        {
            PerformSystemScan();
            ShowNotification(new GUIContent("🔍 System scan completed!"));
        }
        
        // Auto-Fix button
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("🛠️ Auto-Fix", GUILayout.Height(30)))
        {
            PerformAutoFix();
        }
        
        // Smart Reset button
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("🔄 Smart Reset", GUILayout.Height(30)))
        {
            PerformSmartReset();
        }
        
        // Generate button
        GUI.backgroundColor = Color.magenta;
        if (GUILayout.Button("📋 Generate", GUILayout.Height(30)))
        {
            GenerateCombatSystem();
        }
        
        GUI.backgroundColor = originalColor;
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);
    }
    
    /// <summary>
    /// ⚙️ ตัวเลือกขั้นสูง
    /// </summary>
    void DrawAdvancedOptions()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("⚙️ ADVANCED OPTIONS", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        autoFixEnabled = EditorGUILayout.Toggle("🤖 Auto-Fix Enabled", autoFixEnabled);
        createBackup = EditorGUILayout.Toggle("💾 Create Backup", createBackup);
        showSystemStatus = EditorGUILayout.Toggle("📊 Show Status", showSystemStatus);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("🔄 Reset Mode:", GUILayout.Width(80));
        resetMode = (ResetMode)EditorGUILayout.EnumPopup(resetMode);
        confirmBeforeReset = EditorGUILayout.Toggle("⚠️ Confirm Reset", confirmBeforeReset);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);
    }
    
    /// <summary>
    /// 📋 ตั้งค่าการสร้าง
    /// </summary>
    void DrawGenerationSettings()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        // Toggle advanced options
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("📋 GENERATION SETTINGS", EditorStyles.boldLabel);
        showAdvancedOptions = EditorGUILayout.Foldout(showAdvancedOptions, "⚙️ Advanced");
        EditorGUILayout.EndHorizontal();
        
        // Parent Object
        combatSystemParent = (GameObject)EditorGUILayout.ObjectField("📦 Combat System Parent", combatSystemParent, typeof(GameObject), true);
        
        if (combatSystemParent == null)
        {
            EditorGUILayout.HelpBox("Please assign a parent object for the combat system (or leave empty to create new one)", MessageType.Info);
        }
        
        // Components to Create
        GUILayout.Space(5);
        GUILayout.Label("📦 Components to Create", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        createTargetManager = EditorGUILayout.Toggle("TargetManager", createTargetManager);
        createCombatManager = EditorGUILayout.Toggle("CombatManager", createCombatManager);
        createCombatUI = EditorGUILayout.Toggle("CombatUI", createCombatUI);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        createEnemyPrefabs = EditorGUILayout.Toggle("Enemy Prefabs", createEnemyPrefabs);
        createEnemyDataAssets = EditorGUILayout.Toggle("Enemy Data", createEnemyDataAssets);
        EditorGUILayout.EndHorizontal();
        
        // Combat Settings
        GUILayout.Space(5);
        GUILayout.Label("⚔️ Combat Settings", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        defaultAttackRange = EditorGUILayout.FloatField("Attack Range", defaultAttackRange);
        defaultAttackSpeed = EditorGUILayout.FloatField("Attack Speed", defaultAttackSpeed);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        defaultCriticalChance = EditorGUILayout.Slider("Crit Chance", defaultCriticalChance, 0f, 1f);
        defaultCriticalMultiplier = EditorGUILayout.FloatField("Crit Mult", defaultCriticalMultiplier);
        EditorGUILayout.EndHorizontal();
        
        // Enemy Settings
        GUILayout.Space(5);
        GUILayout.Label("👾 Enemy Settings", EditorStyles.boldLabel);
        numberOfEnemyTypes = EditorGUILayout.IntField("Number of Types", numberOfEnemyTypes);
        
        if (enemyNames.Length != numberOfEnemyTypes)
        {
            System.Array.Resize(ref enemyNames, numberOfEnemyTypes);
        }
        
        for (int i = 0; i < numberOfEnemyTypes; i++)
        {
            enemyNames[i] = EditorGUILayout.TextField($"Enemy {i + 1} Name", enemyNames[i]);
        }
        
        EditorGUILayout.EndVertical();
    }
    
    /// <summary>
    /// 🔄 ส่วน Reset System
    /// </summary>
    void DrawResetSection()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("🔄 RESET SYSTEM", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        
        // Quick Reset
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("🔄 Quick Reset", GUILayout.Height(25)))
        {
            if (!confirmBeforeReset || EditorUtility.DisplayDialog("Confirm Reset", 
                "Are you sure you want to perform a Quick Reset?\nThis will delete problematic components only.", 
                "Reset", "Cancel"))
            {
                PerformReset(ResetMode.Quick);
            }
        }
        
        // Full Reset
        GUI.backgroundColor = new Color(0.8f, 0.2f, 0.2f);
        if (GUILayout.Button("💥 Full Reset", GUILayout.Height(25)))
        {
            if (!confirmBeforeReset || EditorUtility.DisplayDialog("Confirm Full Reset", 
                "⚠️ WARNING: This will delete ALL Combat System components!\nAre you absolutely sure?", 
                "Full Reset", "Cancel"))
            {
                PerformReset(ResetMode.Full);
            }
        }
        
        // Selective Reset
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("🎯 Selective Reset", GUILayout.Height(25)))
        {
            ShowSelectiveResetDialog();
        }
        
        // Safe Reset
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("🛡️ Safe Reset", GUILayout.Height(25)))
        {
            PerformReset(ResetMode.Safe);
        }
        
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
    }
    
    /// <summary>
    /// 🔍 สแกนระบบอัตโนมัติ
    /// </summary>
    void PerformSystemScan()
    {
        analysisData = new SystemAnalysisData();
        
        // Scan for components
        analysisData.targetManagersFound = FindObjectsOfType<TargetManager>().Length;
        analysisData.combatManagersFound = FindObjectsOfType<CombatManager>().Length;
        analysisData.combatUIsFound = FindObjectsOfType<CombatUI>().Length;
        analysisData.canvasesFound = FindObjectsOfType<Canvas>().Length;
        
        // Check for Enemy layer
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        analysisData.enemyLayersFound = (enemyLayer != -1) ? 1 : 0;
        
        // Analyze issues
        if (analysisData.targetManagersFound == 0)
            analysisData.issues.Add("No TargetManager found in scene");
        
        if (analysisData.combatManagersFound == 0)
            analysisData.issues.Add("No CombatManager found in scene");
        
        if (analysisData.combatUIsFound == 0)
            analysisData.issues.Add("No CombatUI found in scene");
        
        if (analysisData.canvasesFound == 0)
            analysisData.issues.Add("No Canvas found in scene");
        
        if (analysisData.enemyLayersFound == 0)
            analysisData.issues.Add("Enemy layer not defined");
        
        if (analysisData.canvasesFound > 1)
            analysisData.issues.Add("Multiple canvases detected - may cause conflicts");
        
        // Generate suggestions
        if (analysisData.issues.Count > 0)
        {
            analysisData.suggestions.Add("Use Auto-Fix to resolve issues automatically");
            analysisData.suggestions.Add("Consider using Smart Reset for clean start");
        }
        
        // Calculate system health
        int maxComponents = 5; // TargetManager, CombatManager, CombatUI, Canvas, EnemyLayer
        int foundComponents = (analysisData.targetManagersFound > 0 ? 1 : 0) +
                             (analysisData.combatManagersFound > 0 ? 1 : 0) +
                             (analysisData.combatUIsFound > 0 ? 1 : 0) +
                             (analysisData.canvasesFound > 0 ? 1 : 0) +
                             (analysisData.enemyLayersFound > 0 ? 1 : 0);
        
        analysisData.systemHealth = (float)foundComponents / maxComponents * 100f;
        analysisData.needsAttention = analysisData.issues.Count > 0;
        
        // Update status
        if (analysisData.needsAttention)
        {
            systemStatusMessage = "⚠️ Needs Attention";
            statusType = SystemMessageType.Warning;
        }
        else if (analysisData.systemHealth < 100f)
        {
            systemStatusMessage = "🔧 Partial Setup";
            statusType = SystemMessageType.Info;
        }
        else
        {
            systemStatusMessage = "✅ System Ready";
            statusType = SystemMessageType.Success;
        }
        
        lastScanTime = System.DateTime.Now;
        Repaint();
    }
    
    /// <summary>
    /// 🛠️ แก้ไขปัญหาอัตโนมัติ
    /// </summary>
    void PerformAutoFix()
    {
        Debug.Log("🛠️ Performing Auto-Fix...");
        
        int fixesApplied = 0;
        
        // Fix Enemy Layer
        if (analysisData.enemyLayersFound == 0)
        {
            CreateEnemyLayer();
            fixesApplied++;
        }
        
        // Fix missing components connections
        CombatManager[] combatManagers = FindObjectsOfType<CombatManager>();
        foreach (CombatManager manager in combatManagers)
        {
            if (manager.targetManager == null)
            {
                TargetManager targetManager = FindObjectOfType<TargetManager>();
                if (targetManager != null)
                {
                    var serializedObject = new SerializedObject(manager);
                    var targetManagerProperty = serializedObject.FindProperty("targetManager");
                    if (targetManagerProperty != null)
                    {
                        targetManagerProperty.objectReferenceValue = targetManager;
                        serializedObject.ApplyModifiedProperties();
                        fixesApplied++;
                    }
                }
            }
        }
        
        // Fix CombatUI connections
        CombatUI[] combatUIs = FindObjectsOfType<CombatUI>();
        foreach (CombatUI ui in combatUIs)
        {
            if (ui.damageNumberParent == null)
            {
                Transform damageParent = ui.transform.Find("DamageNumberParent");
                if (damageParent == null)
                {
                    GameObject damageParentObj = new GameObject("DamageNumberParent");
                    damageParentObj.transform.SetParent(ui.transform);
                    damageParent = damageParentObj.transform;
                }
                
                var serializedObject = new SerializedObject(ui);
                var damageParentProperty = serializedObject.FindProperty("damageNumberParent");
                if (damageParentProperty != null)
                {
                    damageParentProperty.objectReferenceValue = damageParent;
                    serializedObject.ApplyModifiedProperties();
                    fixesApplied++;
                }
            }
        }
        
        Debug.Log($"✅ Auto-Fix completed! Applied {fixesApplied} fixes.");
        PerformSystemScan();
        ShowNotification(new GUIContent($"🛠️ Auto-Fix: {fixesApplied} fixes applied"));
    }
    
    /// <summary>
    /// 🔄 Smart Reset อัตโนมัติ
    /// </summary>
    void PerformSmartReset()
    {
        PerformReset(resetMode);
    }
    
    /// <summary>
    /// 🔄 ทำการ Reset ตามโหมดที่เลือก
    /// </summary>
    void PerformReset(ResetMode mode)
    {
        Debug.Log($"🔄 Performing {mode} Reset...");
        
        switch (mode)
        {
            case ResetMode.Quick:
                ResetProblematicComponents();
                break;
            case ResetMode.Full:
                ResetAllComponents();
                break;
            case ResetMode.Selective:
                // This will be handled by dialog
                break;
            case ResetMode.Safe:
                ResetWithBackup();
                break;
        }
        
        PerformSystemScan();
        ShowNotification(new GUIContent($"🔄 {mode} Reset completed"));
    }
    
    /// <summary>
    /// 🔄 Reset เฉพาะส่วนที่มีปัญหา
    /// </summary>
    void ResetProblematicComponents()
    {
        // Find and remove duplicate components
        var targetManagers = FindObjectsOfType<TargetManager>();
        if (targetManagers.Length > 1)
        {
            for (int i = 1; i < targetManagers.Length; i++)
            {
                DestroyImmediate(targetManagers[i].gameObject);
            }
        }
        
        var combatManagers = FindObjectsOfType<CombatManager>();
        if (combatManagers.Length > 1)
        {
            for (int i = 1; i < combatManagers.Length; i++)
            {
                DestroyImmediate(combatManagers[i].gameObject);
            }
        }
        
        var combatUIs = FindObjectsOfType<CombatUI>();
        if (combatUIs.Length > 1)
        {
            for (int i = 1; i < combatUIs.Length; i++)
            {
                DestroyImmediate(combatUIs[i].gameObject);
            }
        }
    }
    
    /// <summary>
    /// 💥 Reset ทั้งหมด
    /// </summary>
    void ResetAllComponents()
    {
        // Remove all combat-related objects
        var targetManagers = FindObjectsOfType<TargetManager>();
        foreach (var manager in targetManagers)
        {
            DestroyImmediate(manager.gameObject);
        }
        
        var combatManagers = FindObjectsOfType<CombatManager>();
        foreach (var manager in combatManagers)
        {
            DestroyImmediate(manager.gameObject);
        }
        
        var combatUIs = FindObjectsOfType<CombatUI>();
        foreach (var ui in combatUIs)
        {
            DestroyImmediate(ui.gameObject);
        }
        
        // Remove enemy prefabs from Assets/Prefabs/Enemies
        string enemyPrefabsPath = "Assets/Prefabs/Enemies";
        if (AssetDatabase.IsValidFolder(enemyPrefabsPath))
        {
            string[] enemyPrefabs = AssetDatabase.FindAssets("t:Prefab", new[] { enemyPrefabsPath });
            foreach (string guid in enemyPrefabs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AssetDatabase.DeleteAsset(path);
            }
        }
        
        // Remove enemy data from Assets/Items/EnemyData
        string enemyDataPath = "Assets/Items/EnemyData";
        if (AssetDatabase.IsValidFolder(enemyDataPath))
        {
            string[] enemyData = AssetDatabase.FindAssets("t:EnemyData", new[] { enemyDataPath });
            foreach (string guid in enemyData)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AssetDatabase.DeleteAsset(path);
            }
        }
        
        AssetDatabase.Refresh();
    }
    
    /// <summary>
    /// 🛡️ Reset พร้อมสร้าง Backup
    /// </summary>
    void ResetWithBackup()
    {
        // Create backup of current scene
        string scenePath = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;
        if (!string.IsNullOrEmpty(scenePath))
        {
            string backupPath = scenePath.Replace(".unity", "_backup_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".unity");
            AssetDatabase.CopyAsset(scenePath, backupPath);
            Debug.Log($"💾 Scene backup created: {backupPath}");
        }
        
        ResetAllComponents();
    }
    
    /// <summary>
    /// 🎯 แสดง Dialog สำหรับ Selective Reset
    /// </summary>
    void ShowSelectiveResetDialog()
    {
        // Create options for selective reset
        bool resetTargetManager = true;
        bool resetCombatManager = true;
        bool resetCombatUI = true;
        bool resetEnemyPrefabs = false;
        bool resetEnemyData = false;
        
        // Show dialog (simplified version - in real implementation would use custom window)
        if (EditorUtility.DisplayDialog("Selective Reset", 
            "Select what to reset:\n\n" +
            "• TargetManager: " + (resetTargetManager ? "✓" : "✗") + "\n" +
            "• CombatManager: " + (resetCombatManager ? "✓" : "✗") + "\n" +
            "• CombatUI: " + (resetCombatUI ? "✓" : "✗") + "\n" +
            "• Enemy Prefabs: " + (resetEnemyPrefabs ? "✓" : "✗") + "\n" +
            "• Enemy Data: " + (resetEnemyData ? "✓" : "✗") + "\n\n" +
            "This will reset selected components only.", 
            "Reset Selected", "Cancel"))
        {
            if (resetTargetManager)
            {
                var targetManagers = FindObjectsOfType<TargetManager>();
                foreach (var manager in targetManagers)
                {
                    DestroyImmediate(manager.gameObject);
                }
            }
            
            if (resetCombatManager)
            {
                var combatManagers = FindObjectsOfType<CombatManager>();
                foreach (var manager in combatManagers)
                {
                    DestroyImmediate(manager.gameObject);
                }
            }
            
            if (resetCombatUI)
            {
                var combatUIs = FindObjectsOfType<CombatUI>();
                foreach (var ui in combatUIs)
                {
                    DestroyImmediate(ui.gameObject);
                }
            }
            
            if (resetEnemyPrefabs)
            {
                string enemyPrefabsPath = "Assets/Prefabs/Enemies";
                if (AssetDatabase.IsValidFolder(enemyPrefabsPath))
                {
                    string[] enemyPrefabs = AssetDatabase.FindAssets("t:Prefab", new[] { enemyPrefabsPath });
                    foreach (string guid in enemyPrefabs)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        AssetDatabase.DeleteAsset(path);
                    }
                }
            }
            
            if (resetEnemyData)
            {
                string enemyDataPath = "Assets/Items/EnemyData";
                if (AssetDatabase.IsValidFolder(enemyDataPath))
                {
                    string[] enemyData = AssetDatabase.FindAssets("t:EnemyData", new[] { enemyDataPath });
                    foreach (string guid in enemyData)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        AssetDatabase.DeleteAsset(path);
                    }
                }
            }
            
            AssetDatabase.Refresh();
            PerformSystemScan();
        }
    }
    
    /// <summary>
    /// 🏷️ สร้าง Enemy Layer
    /// </summary>
    void CreateEnemyLayer()
    {
        // This is a simplified version - in real implementation would need to use SerializedObject to modify layers
        Debug.LogWarning("⚠️ Please create 'Enemy' layer manually in Project Settings > Tags and Layers");
        Debug.LogWarning("⚠️ Or use the Layer Fix tool to create it automatically");
    }
    
    /// <summary>
    /// สร้าง Combat System ทั้งหมด
    /// </summary>
    void GenerateCombatSystem()
    {
        Debug.Log("🚀 Starting Combat System generation...");
        
        // 🔍 Smart Validation - ตรวจสอบคลาสที่จำเป็น
        if (!ValidateRequiredClasses())
        {
            Debug.LogError("❌ Cannot proceed with generation - missing required classes!");
            return;
        }
        
        // สร้าง Parent Object ถ้ายังไม่มี
        if (combatSystemParent == null)
        {
            combatSystemParent = new GameObject("CombatSystem");
            combatSystemParent.transform.position = Vector3.zero;
        }
        
        // 📊 ติดตามสถานะการสร้าง
        System.Text.StringBuilder statusReport = new System.Text.StringBuilder();
        statusReport.AppendLine("📋 Generation Status Report:");
        
        // สร้าง TargetManager
        if (createTargetManager)
        {
            if (CreateTargetManager())
                statusReport.AppendLine("✅ TargetManager: SUCCESS");
            else
                statusReport.AppendLine("❌ TargetManager: FAILED");
        }
        
        // สร้าง CombatManager
        if (createCombatManager)
        {
            if (CreateCombatManager())
                statusReport.AppendLine("✅ CombatManager: SUCCESS");
            else
                statusReport.AppendLine("❌ CombatManager: FAILED");
        }
        
        // สร้าง CombatUI
        if (createCombatUI)
        {
            if (CreateCombatUI())
                statusReport.AppendLine("✅ CombatUI: SUCCESS");
            else
                statusReport.AppendLine("❌ CombatUI: FAILED");
        }
        
        // สร้าง Enemy Prefabs
        if (createEnemyPrefabs)
        {
            if (CreateEnemyPrefabs())
                statusReport.AppendLine("✅ Enemy Prefabs: SUCCESS");
            else
                statusReport.AppendLine("❌ Enemy Prefabs: FAILED");
        }
        
        // สร้าง Enemy Data Assets
        if (createEnemyDataAssets)
        {
            if (CreateEnemyDataAssets())
                statusReport.AppendLine("✅ Enemy Data Assets: SUCCESS");
            else
                statusReport.AppendLine("❌ Enemy Data Assets: FAILED");
        }
        
        // เชื่อมต่อ Components ทั้งหมด
        if (ConnectComponents())
            statusReport.AppendLine("✅ Component Connection: SUCCESS");
        else
            statusReport.AppendLine("❌ Component Connection: FAILED");
        
        // 📄 แสดงรายงาน
        Debug.Log("✅ Combat System generation completed!");
        Debug.Log(statusReport.ToString());
        
        // เลือก Parent Object ใน Hierarchy
        Selection.activeGameObject = combatSystemParent;
        
        // สแกนระบบใหม่
        PerformSystemScan();
    }
    
    /// <summary>
    /// 🔍 Smart Validation - ตรวจสอบคลาสที่จำเป็น
    /// </summary>
    bool ValidateRequiredClasses()
    {
        bool allValid = true;
        
        // ตรวจสอบคลาสที่จำเป็น
        if (!ClassExists("TargetManager"))
        {
            Debug.LogWarning("⚠️ TargetManager class not found! Creating fallback...");
            allValid = false;
        }
        
        if (!ClassExists("CombatManager"))
        {
            Debug.LogWarning("⚠️ CombatManager class not found! Creating fallback...");
            allValid = false;
        }
        
        if (!ClassExists("CombatUI"))
        {
            Debug.LogWarning("⚠️ CombatUI class not found! Creating fallback...");
            allValid = false;
        }
        
        if (!ClassExists("EnemyStats"))
        {
            Debug.LogWarning("⚠️ EnemyStats class not found! Creating fallback...");
            allValid = false;
        }
        
        if (!ClassExists("EnemyController"))
        {
            Debug.LogWarning("⚠️ EnemyController class not found! Creating fallback...");
            allValid = false;
        }
        
        if (!ClassExists("EnemyData"))
        {
            Debug.LogWarning("⚠️ EnemyData class not found! Creating fallback...");
            allValid = false;
        }
        
        return allValid;
    }
    
    /// <summary>
    /// ตรวจสอบว่ามีคลาสอยู่หรือไม่
    /// </summary>
    bool ClassExists(string className)
    {
        try
        {
            System.Type type = System.Type.GetType(className);
            if (type != null) return true;
            
            // ตรวจสอบใน assemblies ทั้งหมด
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(className);
                if (type != null) return true;
            }
            
            return false;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// สร้าง TargetManager
    /// </summary>
    bool CreateTargetManager()
    {
        try
        {
            GameObject targetManagerObj = new GameObject("TargetManager");
            targetManagerObj.transform.SetParent(combatSystemParent.transform);
            
            TargetManager targetManager = targetManagerObj.AddComponent<TargetManager>();
            
            // ตั้งค่า TargetManager
            var serializedObject = new SerializedObject(targetManager);
            var enemyLayerProperty = serializedObject.FindProperty("enemyLayer");
            
            if (enemyLayerProperty == null)
            {
                Debug.LogError("❌ TargetManager missing 'enemyLayer' property!");
                DestroyImmediate(targetManagerObj);
                return false;
            }
            
            // ตรวจสอบว่ามี Layer "Enemy" หรือไม่
            int enemyLayerValue = LayerMask.NameToLayer("Enemy");
            if (enemyLayerValue == -1)
            {
                Debug.LogWarning("⚠️ Layer 'Enemy' not found! Please create it in Project Settings > Tags and Layers");
                Debug.LogWarning("⚠️ Using Default layer (0) for now. Please fix this manually.");
                enemyLayerValue = 0; // Default layer
            }
            
            // แปลงเป็น LayerMask
            LayerMask enemyLayerMask = 1 << enemyLayerValue;
            enemyLayerProperty.intValue = enemyLayerMask.value;
            serializedObject.ApplyModifiedProperties();
            
            Debug.Log("✅ TargetManager created");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to create TargetManager: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// สร้าง CombatManager
    /// </summary>
    bool CreateCombatManager()
    {
        try
        {
            GameObject combatManagerObj = new GameObject("CombatManager");
            combatManagerObj.transform.SetParent(combatSystemParent.transform);
            
            CombatManager combatManager = combatManagerObj.AddComponent<CombatManager>();
            
            // ตั้งค่า CombatManager
            var serializedObject = new SerializedObject(combatManager);
            var attackRangeProperty = serializedObject.FindProperty("attackRange");
            var attackSpeedProperty = serializedObject.FindProperty("attackSpeed");
            var criticalChanceProperty = serializedObject.FindProperty("criticalChance");
            var criticalMultiplierProperty = serializedObject.FindProperty("criticalMultiplier");
            
            if (attackRangeProperty == null || attackSpeedProperty == null || 
                criticalChanceProperty == null || criticalMultiplierProperty == null)
            {
                Debug.LogError("❌ CombatManager missing required properties!");
                DestroyImmediate(combatManagerObj);
                return false;
            }
            
            attackRangeProperty.floatValue = defaultAttackRange;
            attackSpeedProperty.floatValue = defaultAttackSpeed;
            criticalChanceProperty.floatValue = defaultCriticalChance;
            criticalMultiplierProperty.floatValue = defaultCriticalMultiplier;
            
            serializedObject.ApplyModifiedProperties();
            
            Debug.Log("✅ CombatManager created");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to create CombatManager: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// สร้าง CombatUI
    /// </summary>
    bool CreateCombatUI()
    {
        try
        {
            // หา Canvas ที่มีอยู่แล้วใน scene
            Canvas[] existingCanvases = FindObjectsOfType<Canvas>();
            Canvas targetCanvas = null;
            
            if (existingCanvases.Length > 0)
            {
                // ใช้ Canvas แรกที่พบ (ควรเป็น Canvas หลัก)
                targetCanvas = existingCanvases[0];
                Debug.Log($"📋 Using existing Canvas: {targetCanvas.gameObject.name}");
            }
            else
            {
                // ถ้าไม่มี Canvas เลย ให้สร้างใหม่
                Debug.LogWarning("⚠️ No existing Canvas found! Creating new Canvas...");
                GameObject canvasObj = new GameObject("Canvas");
                targetCanvas = canvasObj.AddComponent<Canvas>();
                CanvasScaler canvasScaler = canvasObj.AddComponent<CanvasScaler>();
                GraphicRaycaster graphicRaycaster = canvasObj.AddComponent<GraphicRaycaster>();
                
                // ตั้งค่า Canvas
                targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                targetCanvas.sortingOrder = 10;
                
                // ตั้งค่า Canvas Scaler
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1920, 1080);
            }
            
            // สร้าง CombatUI เป็นลูกของ Canvas ที่มีอยู่แล้ว
            GameObject combatUIObj = new GameObject(combatUIName);
            combatUIObj.transform.SetParent(targetCanvas.transform);
            
            RectTransform rectTransform = combatUIObj.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            // สร้าง CombatUI component
            CombatUI combatUI = combatUIObj.AddComponent<CombatUI>();
            
            // 🎨 สร้าง UI Elements ทั้งหมด
            CreateTargetInfoPanel(combatUIObj, combatUI);
            CreateCombatStatusPanel(combatUIObj, combatUI);
            CreateSkillPanel(combatUIObj, combatUI);
            CreateDamageNumberPrefab(combatUIObj, combatUI);
            
            // สร้าง Damage Number Parent
            GameObject damageParentObj = new GameObject("DamageNumberParent");
            damageParentObj.transform.SetParent(combatUIObj.transform);
            
            RectTransform damageParentRect = damageParentObj.AddComponent<RectTransform>();
            damageParentRect.anchorMin = Vector2.zero;
            damageParentRect.anchorMax = Vector2.one;
            damageParentRect.offsetMin = Vector2.zero;
            damageParentRect.offsetMax = Vector2.zero;
            
            // ตั้งค่า CombatUI ด้วย property ที่ถูกต้อง
            var serializedObject = new SerializedObject(combatUI);
            var damageParentProperty = serializedObject.FindProperty("damageNumberParent");
            
            if (damageParentProperty == null)
            {
                Debug.LogWarning("⚠️ CombatUI missing 'damageNumberParent' property - skipping auto-setup");
            }
            else
            {
                damageParentProperty.objectReferenceValue = damageParentObj.transform;
                serializedObject.ApplyModifiedProperties();
            }
            
            Debug.Log("✅ CombatUI created with complete UI elements");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to create CombatUI: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 🎯 สร้าง Target Info Panel
    /// </summary>
    void CreateTargetInfoPanel(GameObject parent, CombatUI combatUI)
    {
        // สร้าง Panel หลัก
        GameObject panelObj = new GameObject("TargetInfoPanel");
        panelObj.transform.SetParent(parent.transform);
        
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 0.8f);
        panelRect.anchorMax = new Vector2(0.3f, 1f);
        panelRect.offsetMin = new Vector2(10, 10);
        panelRect.offsetMax = new Vector2(-10, -10);
        
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.7f);
        
        // สร้างชื่อเป้าหมาย
        GameObject nameObj = new GameObject("TargetName");
        nameObj.transform.SetParent(panelObj.transform);
        
        RectTransform nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.7f);
        nameRect.anchorMax = new Vector2(1, 0.9f);
        nameRect.offsetMin = Vector2.zero;
        nameRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
        nameText.text = "Target Name";
        nameText.fontSize = 16;
        nameText.color = Color.white;
        nameText.alignment = TextAlignmentOptions.Center;
        
        // สร้างเลเวล
        GameObject levelObj = new GameObject("TargetLevel");
        levelObj.transform.SetParent(panelObj.transform);
        
        RectTransform levelRect = levelObj.AddComponent<RectTransform>();
        levelRect.anchorMin = new Vector2(0, 0.5f);
        levelRect.anchorMax = new Vector2(0.4f, 0.7f);
        levelRect.offsetMin = Vector2.zero;
        levelRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI levelText = levelObj.AddComponent<TextMeshProUGUI>();
        levelText.text = "Lv.1";
        levelText.fontSize = 14;
        levelText.color = Color.yellow;
        levelText.alignment = TextAlignmentOptions.Left;
        
        // สร้าง Health Bar
        GameObject healthBarObj = new GameObject("TargetHealthBar");
        healthBarObj.transform.SetParent(panelObj.transform);
        
        RectTransform healthBarRect = healthBarObj.AddComponent<RectTransform>();
        healthBarRect.anchorMin = new Vector2(0.1f, 0.1f);
        healthBarRect.anchorMax = new Vector2(0.9f, 0.3f);
        healthBarRect.offsetMin = Vector2.zero;
        healthBarRect.offsetMax = Vector2.zero;
        
        Slider healthBar = healthBarObj.AddComponent<Slider>();
        healthBar.minValue = 0;
        healthBar.maxValue = 100;
        healthBar.value = 100;
        
        // Background ของ Health Bar
        GameObject healthBgObj = new GameObject("Background");
        healthBgObj.transform.SetParent(healthBarObj.transform);
        
        RectTransform healthBgRect = healthBgObj.AddComponent<RectTransform>();
        healthBgRect.anchorMin = Vector2.zero;
        healthBgRect.anchorMax = Vector2.one;
        healthBgRect.offsetMin = Vector2.zero;
        healthBgRect.offsetMax = Vector2.zero;
        
        Image healthBg = healthBgObj.AddComponent<Image>();
        healthBg.color = Color.gray;
        
        // Fill ของ Health Bar
        GameObject healthFillObj = new GameObject("Fill");
        healthFillObj.transform.SetParent(healthBarObj.transform);
        
        RectTransform healthFillRect = healthFillObj.AddComponent<RectTransform>();
        healthFillRect.anchorMin = Vector2.zero;
        healthFillRect.anchorMax = Vector2.one;
        healthFillRect.offsetMin = Vector2.zero;
        healthFillRect.offsetMax = Vector2.zero;
        
        Image healthFill = healthFillObj.AddComponent<Image>();
        healthFill.color = Color.red;
        
        healthBar.fillRect = healthFillRect;
        healthBar.targetGraphic = healthFill;
        
        // สร้าง HP Text
        GameObject healthTextObj = new GameObject("TargetHealthText");
        healthTextObj.transform.SetParent(panelObj.transform);
        
        RectTransform healthTextRect = healthTextObj.AddComponent<RectTransform>();
        healthTextRect.anchorMin = new Vector2(0, 0.3f);
        healthTextRect.anchorMax = new Vector2(1, 0.5f);
        healthTextRect.offsetMin = Vector2.zero;
        healthTextRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI healthText = healthTextObj.AddComponent<TextMeshProUGUI>();
        healthText.text = "100/100";
        healthText.fontSize = 12;
        healthText.color = Color.white;
        healthText.alignment = TextAlignmentOptions.Center;
        
        // กำหนดค่าให้ CombatUI
        var serializedObject = new SerializedObject(combatUI);
        serializedObject.FindProperty("targetInfoPanel").objectReferenceValue = panelObj;
        serializedObject.FindProperty("targetNameText").objectReferenceValue = nameText;
        serializedObject.FindProperty("targetLevelText").objectReferenceValue = levelText;
        serializedObject.FindProperty("targetHealthBar").objectReferenceValue = healthBar;
        serializedObject.FindProperty("targetHealthText").objectReferenceValue = healthText;
        serializedObject.ApplyModifiedProperties();
        
        Debug.Log("✅ Target Info Panel created");
    }
    
    /// <summary>
    /// ⚔️ สร้าง Combat Status Panel
    /// </summary>
    void CreateCombatStatusPanel(GameObject parent, CombatUI combatUI)
    {
        // สร้าง Panel
        GameObject panelObj = new GameObject("CombatStatusPanel");
        panelObj.transform.SetParent(parent.transform);
        
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.35f, 0.9f);
        panelRect.anchorMax = new Vector2(0.65f, 1f);
        panelRect.offsetMin = new Vector2(0, 5);
        panelRect.offsetMax = new Vector2(0, -5);
        
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0.2f, 0, 0, 0.8f);
        
        // สร้าง Status Text
        GameObject statusTextObj = new GameObject("CombatStatusText");
        statusTextObj.transform.SetParent(panelObj.transform);
        
        RectTransform statusTextRect = statusTextObj.AddComponent<RectTransform>();
        statusTextRect.anchorMin = Vector2.zero;
        statusTextRect.anchorMax = Vector2.one;
        statusTextRect.offsetMin = new Vector2(10, 0);
        statusTextRect.offsetMax = new Vector2(-10, 0);
        
        TextMeshProUGUI statusText = statusTextObj.AddComponent<TextMeshProUGUI>();
        statusText.text = "READY";
        statusText.fontSize = 18;
        statusText.color = Color.white;
        statusText.fontStyle = FontStyles.Bold;
        statusText.alignment = TextAlignmentOptions.Center;
        
        // สร้าง Status Icon
        GameObject statusIconObj = new GameObject("CombatStatusIcon");
        statusIconObj.transform.SetParent(panelObj.transform);
        
        RectTransform statusIconRect = statusIconObj.AddComponent<RectTransform>();
        statusIconRect.anchorMin = new Vector2(0, 0.2f);
        statusIconRect.anchorMax = new Vector2(0.2f, 0.8f);
        statusIconRect.offsetMin = Vector2.zero;
        statusIconRect.offsetMax = Vector2.zero;
        
        Image statusIcon = statusIconObj.AddComponent<Image>();
        statusIcon.color = Color.green;
        
        // กำหนดค่าให้ CombatUI
        var serializedObject = new SerializedObject(combatUI);
        serializedObject.FindProperty("combatStatusPanel").objectReferenceValue = panelObj;
        serializedObject.FindProperty("combatStatusText").objectReferenceValue = statusText;
        serializedObject.FindProperty("combatStatusIcon").objectReferenceValue = statusIcon;
        serializedObject.ApplyModifiedProperties();
        
        Debug.Log("✅ Combat Status Panel created");
    }
    
    /// <summary>
    /// 🎮 สร้าง Skill Panel
    /// </summary>
    void CreateSkillPanel(GameObject parent, CombatUI combatUI)
    {
        // สร้าง Panel
        GameObject panelObj = new GameObject("SkillPanel");
        panelObj.transform.SetParent(parent.transform);
        
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.7f, 0.7f);
        panelRect.anchorMax = new Vector2(1f, 0.9f);
        panelRect.offsetMin = new Vector2(10, 0);
        panelRect.offsetMax = new Vector2(-10, 0);
        
        // สร้าง Skill 1
        GameObject skill1Obj = new GameObject("Skill1");
        skill1Obj.transform.SetParent(panelObj.transform);
        
        RectTransform skill1Rect = skill1Obj.AddComponent<RectTransform>();
        skill1Rect.anchorMin = new Vector2(0, 0);
        skill1Rect.anchorMax = new Vector2(0.4f, 1f);
        skill1Rect.offsetMin = new Vector2(5, 5);
        skill1Rect.offsetMax = new Vector2(-5, -5);
        
        Image skill1Icon = skill1Obj.AddComponent<Image>();
        skill1Icon.color = Color.blue;
        
        Button skill1Btn = skill1Obj.AddComponent<Button>();
        
        // Skill 1 Cooldown Text
        GameObject skill1CooldownObj = new GameObject("Skill1Cooldown");
        skill1CooldownObj.transform.SetParent(skill1Obj.transform);
        
        RectTransform skill1CooldownRect = skill1CooldownObj.AddComponent<RectTransform>();
        skill1CooldownRect.anchorMin = Vector2.zero;
        skill1CooldownRect.anchorMax = Vector2.one;
        skill1CooldownRect.offsetMin = Vector2.zero;
        skill1CooldownRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI skill1CooldownText = skill1CooldownObj.AddComponent<TextMeshProUGUI>();
        skill1CooldownText.text = "";
        skill1CooldownText.fontSize = 14;
        skill1CooldownText.color = Color.white;
        skill1CooldownText.alignment = TextAlignmentOptions.Center;
        skill1CooldownText.gameObject.SetActive(false);
        
        // สร้าง Skill 2
        GameObject skill2Obj = new GameObject("Skill2");
        skill2Obj.transform.SetParent(panelObj.transform);
        
        RectTransform skill2Rect = skill2Obj.AddComponent<RectTransform>();
        skill2Rect.anchorMin = new Vector2(0.6f, 0);
        skill2Rect.anchorMax = new Vector2(1f, 1f);
        skill2Rect.offsetMin = new Vector2(5, 5);
        skill2Rect.offsetMax = new Vector2(-5, -5);
        
        Image skill2Icon = skill2Obj.AddComponent<Image>();
        skill2Icon.color = Color.red;
        
        Button skill2Btn = skill2Obj.AddComponent<Button>();
        
        // Skill 2 Cooldown Text
        GameObject skill2CooldownObj = new GameObject("Skill2Cooldown");
        skill2CooldownObj.transform.SetParent(skill2Obj.transform);
        
        RectTransform skill2CooldownRect = skill2CooldownObj.AddComponent<RectTransform>();
        skill2CooldownRect.anchorMin = Vector2.zero;
        skill2CooldownRect.anchorMax = Vector2.one;
        skill2CooldownRect.offsetMin = Vector2.zero;
        skill2CooldownRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI skill2CooldownText = skill2CooldownObj.AddComponent<TextMeshProUGUI>();
        skill2CooldownText.text = "";
        skill2CooldownText.fontSize = 14;
        skill2CooldownText.color = Color.white;
        skill2CooldownText.alignment = TextAlignmentOptions.Center;
        skill2CooldownText.gameObject.SetActive(false);
        
        // กำหนดค่าให้ CombatUI
        var serializedObject = new SerializedObject(combatUI);
        serializedObject.FindProperty("skillPanel").objectReferenceValue = panelObj;
        serializedObject.FindProperty("skill1Icon").objectReferenceValue = skill1Icon;
        serializedObject.FindProperty("skill2Icon").objectReferenceValue = skill2Icon;
        serializedObject.FindProperty("skill1CooldownText").objectReferenceValue = skill1CooldownText;
        serializedObject.FindProperty("skill2CooldownText").objectReferenceValue = skill2CooldownText;
        serializedObject.ApplyModifiedProperties();
        
        Debug.Log("✅ Skill Panel created");
    }
    
    /// <summary>
    /// 💥 สร้าง Damage Number Prefab
    /// </summary>
    void CreateDamageNumberPrefab(GameObject parent, CombatUI combatUI)
    {
        // สร้าง Damage Number Prefab
        GameObject damagePrefabObj = new GameObject("DamageNumberPrefab");
        damagePrefabObj.transform.SetParent(parent.transform);
        
        RectTransform damageRect = damagePrefabObj.AddComponent<RectTransform>();
        damageRect.sizeDelta = new Vector2(100, 30);
        
        TextMeshProUGUI damageText = damagePrefabObj.AddComponent<TextMeshProUGUI>();
        damageText.text = "0";
        damageText.fontSize = 20;
        damageText.color = Color.white;
        damageText.fontStyle = FontStyles.Bold;
        damageText.alignment = TextAlignmentOptions.Center;
        
        // ทำให้เป็น Prefab
        string prefabPath = "Assets/Prefabs/DamageNumber.prefab";
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        PrefabUtility.SaveAsPrefabAsset(damagePrefabObj, prefabPath);
        
        // โหลด Prefab กลับมา
        GameObject damagePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        // ทำลาย GameObject ชั่วคราว
        DestroyImmediate(damagePrefabObj);
        
        // กำหนดค่าให้ CombatUI
        var serializedObject = new SerializedObject(combatUI);
        serializedObject.FindProperty("damageNumberPrefab").objectReferenceValue = damagePrefab;
        serializedObject.ApplyModifiedProperties();
        
        Debug.Log("✅ Damage Number Prefab created");
    }
    
    /// <summary>
    /// สร้าง Enemy Prefabs
    /// </summary>
    bool CreateEnemyPrefabs()
    {
        try
        {
            string prefabsPath = "Assets/Prefabs/Enemies";
            
            if (!AssetDatabase.IsValidFolder(prefabsPath))
            {
                AssetDatabase.CreateFolder("Assets/Prefabs", "Enemies");
            }
            
            for (int i = 0; i < numberOfEnemyTypes; i++)
            {
                string enemyName = string.IsNullOrEmpty(enemyNames[i]) ? $"Enemy_{i + 1}" : enemyNames[i];
                GameObject enemyObj = new GameObject(enemyName);
                
                // เพิ่ม Components พื้นฐาน
                SpriteRenderer spriteRenderer = enemyObj.AddComponent<SpriteRenderer>();
                Rigidbody2D rb = enemyObj.AddComponent<Rigidbody2D>();
                Collider2D collider = enemyObj.AddComponent<BoxCollider2D>();
                EnemyStats enemyStats = enemyObj.AddComponent<EnemyStats>();
                EnemyController enemyController = enemyObj.AddComponent<EnemyController>();
                
                // ตั้งค่า Rigidbody2D
                rb.gravityScale = 0f;
                rb.freezeRotation = true;
                
                // ตั้งค่า Collider
                ((BoxCollider2D)collider).size = Vector2.one;
                
                // ตั้งค่า Layer
                int enemyLayer = LayerMask.NameToLayer("Enemy");
                if (enemyLayer == -1)
                {
                    enemyLayer = 0; // Default layer
                }
                enemyObj.layer = enemyLayer;
                
                // สร้าง Prefab
                string prefabPath = $"{prefabsPath}/{enemyName}.prefab";
                PrefabUtility.SaveAsPrefabAsset(enemyObj, prefabPath);
                
                // ทำลาย GameObject ชั่วคราว
                Object.DestroyImmediate(enemyObj);
            }
            
            Debug.Log($"✅ Created {numberOfEnemyTypes} enemy prefabs");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to create Enemy Prefabs: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// สร้าง Enemy Data Assets
    /// </summary>
    bool CreateEnemyDataAssets()
    {
        try
        {
            string dataPath = "Assets/Items/EnemyData";
            
            if (!AssetDatabase.IsValidFolder(dataPath))
            {
                AssetDatabase.CreateFolder("Assets/Items", "EnemyData");
            }
            
            for (int i = 0; i < numberOfEnemyTypes; i++)
            {
                string enemyName = string.IsNullOrEmpty(enemyNames[i]) ? $"Enemy_{i + 1}" : enemyNames[i];
                
                // สร้าง EnemyData ScriptableObject
                EnemyData enemyData = ScriptableObject.CreateInstance<EnemyData>();
                
                // ตั้งค่าข้อมูลพื้นฐาน
                enemyData.enemyName = enemyName;
                enemyData.level = 1 + i;
                enemyData.maxHealth = 50 + (i * 25);
                // currentHealth will be set at runtime when enemy is spawned
                enemyData.attack = 5 + (i * 3);
                enemyData.defense = 2 + (i * 2);
                enemyData.speed = (int)(2 + (i * 0.5f));
                enemyData.experienceReward = 10 + (i * 5);
                enemyData.goldReward = 5 + (i * 3);
                enemyData.detectionRange = 5f + (i * 1f);
                enemyData.attackRange = 1.5f;
                enemyData.moveSpeed = 2f + (i * 0.5f);
                enemyData.attackSpeed = 1f / (2f - (i * 0.1f));
                
                // บันทึก Asset
                string assetPath = $"{dataPath}/{enemyName}Data.asset";
                AssetDatabase.CreateAsset(enemyData, assetPath);
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log($"✅ Created {numberOfEnemyTypes} enemy data assets");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to create Enemy Data Assets: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// เชื่อมต่อ Components ทั้งหมด
    /// </summary>
    bool ConnectComponents()
    {
        try
        {
            // หา Components
            TargetManager targetManager = combatSystemParent.GetComponentInChildren<TargetManager>();
            CombatManager combatManager = combatSystemParent.GetComponentInChildren<CombatManager>();
            CombatUI combatUI = combatSystemParent.GetComponentInChildren<CombatUI>();
            
            // เชื่อมต่อ CombatManager
            if (combatManager != null)
            {
                var serializedObject = new SerializedObject(combatManager);
                
                if (targetManager != null)
                {
                    var targetManagerProperty = serializedObject.FindProperty("targetManager");
                    if (targetManagerProperty != null)
                        targetManagerProperty.objectReferenceValue = targetManager;
                }
                
                if (combatUI != null)
                {
                    var combatUIProperty = serializedObject.FindProperty("combatUI");
                    if (combatUIProperty != null)
                        combatUIProperty.objectReferenceValue = combatUI;
                }
                
                serializedObject.ApplyModifiedProperties();
            }
            
            Debug.Log("✅ Components connected");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to connect components: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// แก้ไข Combat System ที่มีอยู่แล้ว
    /// </summary>
    void FixExistingCombatSystem()
    {
        Debug.Log("🔧 Fixing existing Combat System...");
        
        // หา Components ที่มีอยู่แล้ว
        TargetManager targetManager = FindObjectOfType<TargetManager>();
        CombatManager combatManager = FindObjectOfType<CombatManager>();
        CombatUI combatUI = FindObjectOfType<CombatUI>();
        PlayerStatsManager playerStatsManager = FindObjectOfType<PlayerStatsManager>();
        
        // แก้ไข CombatManager
        if (combatManager != null)
        {
            var serializedObject = new SerializedObject(combatManager);
            
            if (targetManager != null)
            {
                var targetManagerProperty = serializedObject.FindProperty("targetManager");
                if (targetManagerProperty.objectReferenceValue == null)
                {
                    targetManagerProperty.objectReferenceValue = targetManager;
                }
            }
            
            if (playerStatsManager != null)
            {
                var playerStatsManagerProperty = serializedObject.FindProperty("playerStatsManager");
                if (playerStatsManagerProperty.objectReferenceValue == null)
                {
                    playerStatsManagerProperty.objectReferenceValue = playerStatsManager;
                }
            }
            
            if (combatUI != null)
            {
                var combatUIProperty = serializedObject.FindProperty("combatUI");
                if (combatUIProperty.objectReferenceValue == null)
                {
                    combatUIProperty.objectReferenceValue = combatUI;
                }
            }
            
            serializedObject.ApplyModifiedProperties();
            Debug.Log("✅ CombatManager fixed");
        }
        
        // แก้ไข TargetManager
        if (targetManager != null)
        {
            var serializedObject = new SerializedObject(targetManager);
            var enemyLayerProperty = serializedObject.FindProperty("enemyLayer");
            if (enemyLayerProperty.intValue == 0)
            {
                int enemyLayerValue = LayerMask.NameToLayer("Enemy");
                if (enemyLayerValue == -1)
                {
                    enemyLayerValue = 0; // Default layer
                }
                // แปลงเป็น LayerMask
                LayerMask enemyLayerMask = 1 << enemyLayerValue;
                enemyLayerProperty.intValue = enemyLayerMask.value;
            }
            serializedObject.ApplyModifiedProperties();
            Debug.Log("✅ TargetManager fixed");
        }
        
        // แก้ไข CombatUI
        if (combatUI != null)
        {
            var serializedObject = new SerializedObject(combatUI);
            var damageParentProperty = serializedObject.FindProperty("damageNumberParent");
            if (damageParentProperty.objectReferenceValue == null)
            {
                // หา DamageNumberParent
                Transform damageParent = combatUI.transform.Find("DamageNumberParent");
                if (damageParent != null)
                {
                    damageParentProperty.objectReferenceValue = damageParent;
                }
                else
                {
                    // สร้างใหม่ถ้าไม่มี
                    GameObject damageParentObj = new GameObject("DamageNumberParent");
                    damageParentObj.transform.SetParent(combatUI.transform);
                    damageParentProperty.objectReferenceValue = damageParentObj.transform;
                    Debug.Log("✅ Created missing DamageNumberParent");
                }
            }
            serializedObject.ApplyModifiedProperties();
            Debug.Log("✅ CombatUI fixed");
        }
        
        Debug.Log("✅ Combat System fix completed!");
    }
}
