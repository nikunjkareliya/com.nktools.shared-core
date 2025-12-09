/// <summary>
/// UI Setup Wizard - Part of NK Tools Shared Core Package
/// Generates MainUI prefabs with PanelNavigator system and aspect ratio support
/// </summary>

using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Shared.Core;

namespace Shared.Core.Editor
{
    #region Data Classes

    [System.Serializable]
    public class PanelConfig
    {
        public string panelName = "NewPanel";
        public PanelType panelType = PanelType.Custom;
        public string customScriptName = "";
        public GameState gameState = GameState.None;
        public bool isOverlay = false;
    }

    public enum PanelType
    {
        HomePanel,
        GameplayPanel,
        LevelCompletedPanel,
        LevelFailedPanel,
        LevelSelectPanel,
        CharacterSelectPanel,
        Custom
    }

    public enum AspectRatioMode
    {
        Portrait,
        Landscape,
        Custom
    }

    public enum PortraitPreset
    {
        Phone_9_16,      // 1080×1920
        TallPhone_9_20,  // 1080×2400
        Tablet_3_4,      // 768×1024
        Custom
    }

    public enum LandscapePreset
    {
        Standard_16_9,   // 1920×1080
        Wide_20_9,       // 2400×1080
        Tablet_4_3,      // 1024×768
        Custom
    }

    [System.Serializable]
    public class CanvasSettings
    {
        public AspectRatioMode aspectMode = AspectRatioMode.Portrait;
        public PortraitPreset portraitPreset = PortraitPreset.Phone_9_16;
        public LandscapePreset landscapePreset = LandscapePreset.Standard_16_9;

        public Vector2 customResolution = new Vector2(1080, 1920);
        public float matchWidthOrHeight = 0.2f;
        public RenderMode renderMode = RenderMode.ScreenSpaceCamera;

        public Vector2 GetResolution()
        {
            switch (aspectMode)
            {
                case AspectRatioMode.Portrait:
                    return GetPortraitResolution();
                case AspectRatioMode.Landscape:
                    return GetLandscapeResolution();
                case AspectRatioMode.Custom:
                    return customResolution;
                default:
                    return new Vector2(1080, 1920);
            }
        }

        private Vector2 GetPortraitResolution()
        {
            switch (portraitPreset)
            {
                case PortraitPreset.Phone_9_16:
                    return new Vector2(1080, 1920);
                case PortraitPreset.TallPhone_9_20:
                    return new Vector2(1080, 2400);
                case PortraitPreset.Tablet_3_4:
                    return new Vector2(768, 1024);
                case PortraitPreset.Custom:
                    return customResolution;
                default:
                    return new Vector2(1080, 1920);
            }
        }

        private Vector2 GetLandscapeResolution()
        {
            switch (landscapePreset)
            {
                case LandscapePreset.Standard_16_9:
                    return new Vector2(1920, 1080);
                case LandscapePreset.Wide_20_9:
                    return new Vector2(2400, 1080);
                case LandscapePreset.Tablet_4_3:
                    return new Vector2(1024, 768);
                case LandscapePreset.Custom:
                    return customResolution;
                default:
                    return new Vector2(1920, 1080);
            }
        }

        public float GetAutoMatch()
        {
            Vector2 res = GetResolution();
            float aspect = res.x / res.y;

            if (aspect < 1.0f) return 0.2f;  // Portrait
            if (aspect > 1.0f) return 0.5f;  // Landscape
            return 0.5f;  // Square
        }
    }

    [System.Serializable]
    public class LayerSettings
    {
        public int backgroundSortOrder = -1;
        public int panelsSortOrder = 0;
        public int foregroundSortOrder = 1;
    }

    public enum GenerationMode
    {
        GenerateNew,
        UpdateExisting,
        TemplateOnly
    }

    [System.Serializable]
    public class UISetupConfig
    {
        public CanvasSettings canvasSettings = new CanvasSettings();
        public LayerSettings layerSettings = new LayerSettings();
        public List<PanelConfig> panels = new List<PanelConfig>();
        public GenerationMode mode = GenerationMode.GenerateNew;
        public string outputPath = "Assets/Prefabs/UI/MainUIPrefab.prefab";
    }

    #endregion

    public class UISetupWizard : EditorWindow
    {
        private CanvasSettings _canvasSettings = new CanvasSettings();
        private LayerSettings _layerSettings = new LayerSettings();
        private List<PanelConfig> _panels = new List<PanelConfig>();
        private GenerationMode _mode = GenerationMode.GenerateNew;
        private string _outputPath = "Assets/Prefabs/UI/MainUIPrefab.prefab";
        private bool _syncModeEnabled = false;

        private Vector2 _scrollPosition;
        private bool _showLayerSettings = false;

        [MenuItem("Tools/NK Tools/UI Setup Wizard")]
        public static void ShowWindow()
        {
            UISetupWizard window = GetWindow<UISetupWizard>("UI Setup Wizard");
            window.minSize = new Vector2(450, 600);
        }

        private void OnEnable()
        {
            if (_panels.Count == 0)
            {
                AddDefaultPanels();
            }
        }

        private void AddDefaultPanels()
        {
            _panels.Add(new PanelConfig
            {
                panelName = "HomePanel",
                panelType = PanelType.HomePanel,
                gameState = GameState.Home
            });

            _panels.Add(new PanelConfig
            {
                panelName = "GameplayPanel",
                panelType = PanelType.GameplayPanel,
                gameState = GameState.Gameplay
            });

            _panels.Add(new PanelConfig
            {
                panelName = "LevelCompletedPanel",
                panelType = PanelType.LevelCompletedPanel,
                gameState = GameState.LevelCompleted
            });

            _panels.Add(new PanelConfig
            {
                panelName = "LevelFailedPanel",
                panelType = PanelType.LevelFailedPanel,
                gameState = GameState.LevelFailed
            });
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("UI Setup Wizard", EditorStyles.largeLabel);
            EditorGUILayout.Space(10);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            DrawCanvasSettings();
            EditorGUILayout.Space(10);

            DrawLayerSettings();
            EditorGUILayout.Space(10);

            DrawPanelsList();
            EditorGUILayout.Space(10);

            DrawGenerationOptions();
            EditorGUILayout.Space(10);

            DrawValidation();
            EditorGUILayout.Space(10);

            DrawActionButtons();

            EditorGUILayout.EndScrollView();
        }

        private void DrawCanvasSettings()
        {
            EditorGUILayout.LabelField("Canvas Settings", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Aspect Ratio:", GUILayout.Width(100));
            _canvasSettings.aspectMode = (AspectRatioMode)GUILayout.Toolbar(
                (int)_canvasSettings.aspectMode,
                new string[] { "Portrait", "Landscape", "Custom" }
            );
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            if (_canvasSettings.aspectMode == AspectRatioMode.Portrait)
            {
                _canvasSettings.portraitPreset = (PortraitPreset)EditorGUILayout.EnumPopup(
                    "Resolution Preset",
                    _canvasSettings.portraitPreset
                );

                if (_canvasSettings.portraitPreset == PortraitPreset.Custom)
                {
                    _canvasSettings.customResolution = EditorGUILayout.Vector2Field(
                        "Custom Resolution",
                        _canvasSettings.customResolution
                    );
                }
            }
            else if (_canvasSettings.aspectMode == AspectRatioMode.Landscape)
            {
                _canvasSettings.landscapePreset = (LandscapePreset)EditorGUILayout.EnumPopup(
                    "Resolution Preset",
                    _canvasSettings.landscapePreset
                );

                if (_canvasSettings.landscapePreset == LandscapePreset.Custom)
                {
                    _canvasSettings.customResolution = EditorGUILayout.Vector2Field(
                        "Custom Resolution",
                        _canvasSettings.customResolution
                    );
                }
            }
            else
            {
                _canvasSettings.customResolution = EditorGUILayout.Vector2Field(
                    "Resolution",
                    _canvasSettings.customResolution
                );
            }

            Vector2 finalRes = _canvasSettings.GetResolution();
            EditorGUILayout.LabelField(
                $"Final Resolution: {finalRes.x}×{finalRes.y}",
                EditorStyles.miniLabel
            );

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            _canvasSettings.matchWidthOrHeight = EditorGUILayout.Slider(
                "Match W/H",
                _canvasSettings.matchWidthOrHeight,
                0f,
                1f
            );

            if (GUILayout.Button("Auto", GUILayout.Width(50)))
            {
                _canvasSettings.matchWidthOrHeight = _canvasSettings.GetAutoMatch();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox(
                _canvasSettings.matchWidthOrHeight < 0.5f
                    ? "Favoring Height (better for portrait)"
                    : "Favoring Width (better for landscape)",
                MessageType.Info
            );

            _canvasSettings.renderMode = (RenderMode)EditorGUILayout.EnumPopup(
                "Render Mode",
                _canvasSettings.renderMode
            );

            EditorGUILayout.EndVertical();
        }

        private void DrawLayerSettings()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Layer Settings (Advanced)", EditorStyles.boldLabel);
            _showLayerSettings = EditorGUILayout.Toggle(_showLayerSettings, GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();

            if (_showLayerSettings)
            {
                EditorGUILayout.BeginVertical("box");

                _layerSettings.backgroundSortOrder = EditorGUILayout.IntField(
                    "Background Sort Order",
                    _layerSettings.backgroundSortOrder
                );

                _layerSettings.panelsSortOrder = EditorGUILayout.IntField(
                    "Panels Sort Order",
                    _layerSettings.panelsSortOrder
                );

                _layerSettings.foregroundSortOrder = EditorGUILayout.IntField(
                    "Foreground Sort Order",
                    _layerSettings.foregroundSortOrder
                );

                EditorGUILayout.EndVertical();
            }
        }

        private void DrawPanelsList()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Panels Configuration", EditorStyles.boldLabel);

            if (GUILayout.Button("+ Add Panel", GUILayout.Width(100)))
            {
                _panels.Add(new PanelConfig());
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical("box");

            for (int i = 0; i < _panels.Count; i++)
            {
                EditorGUILayout.BeginVertical("helpBox");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Panel {i + 1}", EditorStyles.boldLabel, GUILayout.Width(60));

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("×", GUILayout.Width(20)))
                {
                    _panels.RemoveAt(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();

                _panels[i].panelName = EditorGUILayout.TextField("Name", _panels[i].panelName);
                _panels[i].panelType = (PanelType)EditorGUILayout.EnumPopup("Type", _panels[i].panelType);

                if (_panels[i].panelType == PanelType.Custom)
                {
                    EditorGUILayout.BeginHorizontal();
                    _panels[i].customScriptName = EditorGUILayout.TextField("Script Name", _panels[i].customScriptName);

                    if (!string.IsNullOrEmpty(_panels[i].customScriptName))
                    {
                        if (GetPanelScriptType(_panels[i].customScriptName) == null)
                        {
                            EditorGUILayout.LabelField("⚠️", GUILayout.Width(20));
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }

                _panels[i].gameState = (GameState)EditorGUILayout.EnumPopup("State", _panels[i].gameState);
                _panels[i].isOverlay = EditorGUILayout.Toggle("Is Overlay", _panels[i].isOverlay);

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawGenerationOptions()
        {
            EditorGUILayout.LabelField("Generation Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");

            _mode = (GenerationMode)EditorGUILayout.EnumPopup("Mode", _mode);

            if (_mode == GenerationMode.UpdateExisting)
            {
                EditorGUILayout.Space(5);
                _syncModeEnabled = EditorGUILayout.Toggle("Sync Mode (Remove Missing Panels)", _syncModeEnabled);

                if (_syncModeEnabled)
                {
                    EditorGUILayout.HelpBox(
                        "⚠️ Sync Mode: Panels in prefab but NOT in wizard will be DELETED. Use with caution!",
                        MessageType.Warning
                    );
                }
                else
                {
                    EditorGUILayout.HelpBox(
                        "Safe Mode: Only adds new panels, never removes existing ones.",
                        MessageType.Info
                    );
                }
            }

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            _outputPath = EditorGUILayout.TextField("Output Path", _outputPath);

            if (GUILayout.Button("📁", GUILayout.Width(30)))
            {
                string path = EditorUtility.SaveFilePanelInProject(
                    "Save MainUI Prefab",
                    "MainUIPrefab",
                    "prefab",
                    "Select location for MainUI prefab"
                );

                if (!string.IsNullOrEmpty(path))
                {
                    _outputPath = path;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void DrawValidation()
        {
            List<string> warnings = ValidateConfiguration();

            EditorGUILayout.LabelField("Validation:", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");

            if (warnings.Count == 0)
            {
                EditorGUILayout.HelpBox("✅ Configuration valid", MessageType.Info);
            }
            else
            {
                foreach (var warning in warnings)
                {
                    EditorGUILayout.HelpBox(warning, MessageType.Warning);
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawActionButtons()
        {
            EditorGUILayout.BeginHorizontal();

            GUI.enabled = ValidateConfiguration().Count == 0;

            if (GUILayout.Button(_mode == GenerationMode.UpdateExisting ? "Update Prefab" : "Generate Prefab", GUILayout.Height(30)))
            {
                if (_mode == GenerationMode.UpdateExisting)
                {
                    UpdateExistingPrefab();
                }
                else
                {
                    GeneratePrefab();
                }
            }

            GUI.enabled = true;

            if (GUILayout.Button("Save Preset", GUILayout.Height(30)))
            {
                SavePreset();
            }

            if (GUILayout.Button("Load Preset", GUILayout.Height(30)))
            {
                LoadPreset();
            }

            EditorGUILayout.EndHorizontal();
        }

        private List<string> ValidateConfiguration()
        {
            List<string> warnings = new List<string>();

            HashSet<string> names = new HashSet<string>();
            foreach (var panel in _panels)
            {
                if (!names.Add(panel.panelName))
                {
                    warnings.Add($"Duplicate panel name: {panel.panelName}");
                }
            }

            HashSet<GameState> states = new HashSet<GameState>();
            foreach (var panel in _panels)
            {
                if (panel.gameState != GameState.None && !states.Add(panel.gameState))
                {
                    warnings.Add($"Duplicate GameState: {panel.gameState}");
                }
            }

            foreach (var panel in _panels)
            {
                if (panel.panelType == PanelType.Custom && !string.IsNullOrEmpty(panel.customScriptName))
                {
                    if (GetPanelScriptType(panel.customScriptName) == null)
                    {
                        warnings.Add($"Custom script '{panel.customScriptName}' not found");
                    }
                }
            }

            Vector2 res = _canvasSettings.GetResolution();
            if (res.x < 320 || res.y < 320 || res.x > 4096 || res.y > 4096)
            {
                warnings.Add($"Resolution {res.x}×{res.y} outside recommended range (320-4096)");
            }

            if (_mode == GenerationMode.UpdateExisting && !System.IO.File.Exists(_outputPath))
            {
                warnings.Add($"Prefab not found at {_outputPath} for update mode");
            }

            return warnings;
        }

        private void GeneratePrefab()
        {
            GameObject mainUI = new GameObject("MainUIPrefab");
            Canvas canvas = mainUI.AddComponent<Canvas>();
            canvas.renderMode = _canvasSettings.renderMode;

            CanvasScaler scaler = mainUI.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = _canvasSettings.GetResolution();
            scaler.matchWidthOrHeight = _canvasSettings.matchWidthOrHeight;

            mainUI.AddComponent<GraphicRaycaster>();

            CreateCanvasLayer("BackgroundPanels", mainUI.transform, _layerSettings.backgroundSortOrder);

            GameObject panelsNav = CreatePanelsNavigator(mainUI.transform);

            CreateCanvasLayer("ForgroundPanels", mainUI.transform, _layerSettings.foregroundSortOrder);

            Transform container = panelsNav.transform.Find("Container");
            foreach (var panelConfig in _panels)
            {
                if (_mode != GenerationMode.TemplateOnly)
                {
                    CreatePanel(panelConfig, container);
                }
            }

            string directory = System.IO.Path.GetDirectoryName(_outputPath);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            PrefabUtility.SaveAsPrefabAsset(mainUI, _outputPath);
            DestroyImmediate(mainUI);

            AssetDatabase.Refresh();

            Debug.Log($"<color=green>✅ MainUIPrefab generated at {_outputPath}</color>");
            Debug.Log($"<color=cyan>Resolution: {_canvasSettings.GetResolution()}, Match: {_canvasSettings.matchWidthOrHeight}</color>");
        }

        private void UpdateExistingPrefab()
        {
            GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(_outputPath);

            if (existingPrefab == null)
            {
                Debug.LogError($"Prefab not found at {_outputPath}");
                return;
            }

            GameObject instance = PrefabUtility.InstantiatePrefab(existingPrefab) as GameObject;
            Transform container = instance.transform.Find("PanelsNavigator/Container");

            if (container == null)
            {
                Debug.LogError("Container not found in existing prefab!");
                DestroyImmediate(instance);
                return;
            }

            // Add new panels
            foreach (var panelConfig in _panels)
            {
                Transform existingPanel = container.Find(panelConfig.panelName);

                if (existingPanel == null)
                {
                    CreatePanel(panelConfig, container);
                    Debug.Log($"<color=cyan>➕ Added {panelConfig.panelName}</color>");
                }
            }

            // Remove missing panels if Sync Mode is enabled
            if (_syncModeEnabled)
            {
                HashSet<string> wizardPanelNames = new HashSet<string>(_panels.Select(p => p.panelName));
                List<Transform> panelsToRemove = new List<Transform>();

                foreach (Transform child in container)
                {
                    if (!wizardPanelNames.Contains(child.name))
                    {
                        panelsToRemove.Add(child);
                    }
                }

                foreach (var panelToRemove in panelsToRemove)
                {
                    Debug.Log($"<color=red>➖ Removed {panelToRemove.name}</color>");
                    DestroyImmediate(panelToRemove.gameObject);
                }
            }

            PrefabUtility.SaveAsPrefabAsset(instance, _outputPath);
            DestroyImmediate(instance);

            AssetDatabase.Refresh();

            Debug.Log($"<color=green>✅ Updated prefab at {_outputPath}</color>");
        }

        private GameObject CreateCanvasLayer(string name, Transform parent, int sortOrder)
        {
            GameObject layer = new GameObject(name);
            layer.transform.SetParent(parent, false);

            RectTransform rect = layer.AddComponent<RectTransform>();
            SetFullScreen(rect);

            Canvas canvas = layer.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = sortOrder;

            return layer;
        }

        private GameObject CreatePanelsNavigator(Transform parent)
        {
            GameObject panelsNav = new GameObject("PanelsNavigator");
            panelsNav.transform.SetParent(parent, false);
            RectTransform navRect = panelsNav.AddComponent<RectTransform>();
            SetFullScreen(navRect);
            panelsNav.AddComponent<PanelNavigatorController>();

            GameObject container = new GameObject("Container");
            container.transform.SetParent(panelsNav.transform, false);
            RectTransform containerRect = container.AddComponent<RectTransform>();
            SetFullScreen(containerRect);

            return panelsNav;
        }

        private void CreatePanel(PanelConfig config, Transform parent)
        {
            GameObject panel = new GameObject(config.panelName);
            panel.transform.SetParent(parent, false);

            RectTransform rect = panel.AddComponent<RectTransform>();
            SetFullScreen(rect);

            Canvas canvas = panel.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = _layerSettings.panelsSortOrder;

            panel.AddComponent<GraphicRaycaster>();
            CanvasGroup canvasGroup = panel.AddComponent<CanvasGroup>();

            BasePanel panelScript = null;

            if (config.panelType == PanelType.Custom)
            {
                System.Type scriptType = GetPanelScriptType(config.customScriptName);

                if (scriptType != null && typeof(BasePanel).IsAssignableFrom(scriptType))
                {
                    panelScript = panel.AddComponent(scriptType) as BasePanel;
                }
            }
            else
            {
                panelScript = AddPanelScriptByType(panel, config.panelType);
            }

            if (panelScript != null)
            {
                SerializedObject so = new SerializedObject(panelScript);
                so.FindProperty("_canvasGroup").objectReferenceValue = canvasGroup;
                so.FindProperty("State").enumValueIndex = (int)config.gameState;
                so.FindProperty("IsOverlay").boolValue = config.isOverlay;
                so.ApplyModifiedProperties();
            }
        }

        private BasePanel AddPanelScriptByType(GameObject panel, PanelType type)
        {
            switch (type)
            {
                case PanelType.HomePanel:
                    return panel.AddComponent<HomePanel>();
                case PanelType.GameplayPanel:
                    return panel.AddComponent<GameplayPanel>();
                case PanelType.LevelCompletedPanel:
                    return panel.AddComponent<LevelCompletedPanel>();
                case PanelType.LevelFailedPanel:
                    return panel.AddComponent<LevelFailedPanel>();
                case PanelType.LevelSelectPanel:
                    return panel.AddComponent<LevelSelectPanel>();
                case PanelType.CharacterSelectPanel:
                    return panel.AddComponent<CharacterSelectPanel>();
                default:
                    return null;
            }
        }

        private System.Type GetPanelScriptType(string scriptName)
        {
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(scriptName);
                if (type != null) return type;

                type = assembly.GetType($"Shared.Core.{scriptName}");
                if (type != null) return type;

                type = assembly.GetType($"MergeBlocks.{scriptName}");
                if (type != null) return type;
            }

            return null;
        }

        private void SetFullScreen(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
        }

        private void SavePreset()
        {
            UISetupConfig config = new UISetupConfig
            {
                canvasSettings = _canvasSettings,
                layerSettings = _layerSettings,
                panels = _panels,
                mode = _mode,
                outputPath = _outputPath
            };

            string json = JsonUtility.ToJson(config, true);
            string path = EditorUtility.SaveFilePanel("Save Preset", "Assets", "UISetup", "json");

            if (!string.IsNullOrEmpty(path))
            {
                System.IO.File.WriteAllText(path, json);
                AssetDatabase.Refresh();
                Debug.Log($"<color=green>Preset saved to {path}</color>");
            }
        }

        private void LoadPreset()
        {
            string path = EditorUtility.OpenFilePanel("Load Preset", "Assets", "json");

            if (!string.IsNullOrEmpty(path))
            {
                string json = System.IO.File.ReadAllText(path);
                UISetupConfig config = JsonUtility.FromJson<UISetupConfig>(json);

                _canvasSettings = config.canvasSettings;
                _layerSettings = config.layerSettings;
                _panels = config.panels;
                _mode = config.mode;
                _outputPath = config.outputPath;

                Debug.Log($"<color=green>Preset loaded from {path}</color>");
            }
        }
    }
}
