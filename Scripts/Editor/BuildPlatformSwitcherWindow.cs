using System.IO;
using _Data;
using UnityEditor;
using UnityEngine;

namespace Core.Editor {
    public class BuildPlatformSwitcherWindow : EditorWindow {
        private ProjectSettings _cachedSettings;
        private Vector2 _scrollPos;

        [MenuItem("Tools/NazoCore/SDK Platform Switcher", priority = 100)]
        public static void Open() {
            var window = GetWindow<BuildPlatformSwitcherWindow>("SDK Switcher");
            window.minSize = new Vector2(380, 260);
            window.Show();
        }

        [MenuItem("Tools/NazoCore/Switch SDK/PlayGama", priority = 110)]
        public static void MenuSwitchToPlaygama() {
            if (EditorUtility.DisplayDialog("Switch SDK to PlayGama",
                    "Are you sure you want to switch SDK to PlayGama?\n\nThis will:\n- Set DEFINE to PLAYGAMA\n- Set ProjectSettings.SDKType to Playgama\n- Set WebGL template to PROJECT:Bridge",
                    "Switch", "Cancel")) {
                BuildPlatformSwitcher.SwitchToPlaygama();
            }
        }

        [MenuItem("Tools/NazoCore/Switch SDK/GamePush", priority = 111)]
        public static void MenuSwitchToGamePush() {
            if (EditorUtility.DisplayDialog("Switch SDK to GamePush",
                    "Are you sure you want to switch SDK to GamePush?\n\nThis will:\n- Set DEFINE to GAMEPUSH\n- Set ProjectSettings.SDKType to GamePush\n- Set WebGL template to PROJECT:GamePush",
                    "Switch", "Cancel")) {
                BuildPlatformSwitcher.SwitchToGamePush();
            }
        }

        private void OnEnable() {
            RefreshSettings();
        }

        private void RefreshSettings() {
            _cachedSettings = Resources.Load<ProjectSettings>("ProjectSettings");
            if (_cachedSettings == null) {
                string[] guids = AssetDatabase.FindAssets("t:ProjectSettings");
                if (guids.Length > 0) {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    _cachedSettings = AssetDatabase.LoadAssetAtPath<ProjectSettings>(path);
                }
            }
        }

        private void OnGUI() {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("NazoCore Build & SDK Switcher", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Switch between PlayGama and GamePush SDK targets. This tool automatically synchronizes Scripting Defines, ProjectSettings.asset, and WebGL Template.", MessageType.Info);

            EditorGUILayout.Space(10);

            // Display Current State
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Current Project State", EditorStyles.boldLabel);
            
            // Defines
            var activeTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            PlayerSettings.GetScriptingDefineSymbols(activeTarget, out string[] currentDefines);
            bool hasPlaygamaDefine = currentDefines != null && System.Array.Exists(currentDefines, d => d.Trim() == BuildPlatformSwitcher.DEFINE_PLAYGAMA);
            bool hasGamepushDefine = currentDefines != null && System.Array.Exists(currentDefines, d => d.Trim() == BuildPlatformSwitcher.DEFINE_GAMEPUSH);

            string defineState = hasPlaygamaDefine ? "PLAYGAMA" : (hasGamepushDefine ? "GAMEPUSH" : "None / Other");
            EditorGUILayout.LabelField("Active Define:", defineState);

            // ProjectSettings
            if (_cachedSettings == null) {
                RefreshSettings();
            }
            string settingsState = _cachedSettings != null ? _cachedSettings.SDKType.ToString() : "Not Found";
            EditorGUILayout.LabelField("ProjectSettings.SDKType:", settingsState);

            // WebGL Template
            EditorGUILayout.LabelField("WebGL Template:", PlayerSettings.WebGL.template);

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(15);

            // Action Buttons
            EditorGUILayout.LabelField("Switch Platform", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            GUI.backgroundColor = hasPlaygamaDefine ? new Color(0.6f, 1f, 0.6f) : Color.white;
            if (GUILayout.Button("Switch to PlayGama", GUILayout.Height(38))) {
                BuildPlatformSwitcher.SwitchToPlaygama();
                RefreshSettings();
            }

            GUI.backgroundColor = hasGamepushDefine ? new Color(0.6f, 1f, 0.6f) : Color.white;
            if (GUILayout.Button("Switch to GamePush", GUILayout.Height(38))) {
                BuildPlatformSwitcher.SwitchToGamePush();
                RefreshSettings();
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(15);

            // Extensibility Area
            EditorGUILayout.LabelField("Configured Defaults", EditorStyles.miniBoldLabel);
            EditorGUILayout.LabelField("PlayGama Template:", BuildPlatformSwitcher.TEMPLATE_PLAYGAMA, EditorStyles.miniLabel);
            EditorGUILayout.LabelField("GamePush Template:", BuildPlatformSwitcher.TEMPLATE_GAMEPUSH, EditorStyles.miniLabel);

            EditorGUILayout.EndScrollView();
        }
    }
}
