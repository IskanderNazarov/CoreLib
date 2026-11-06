using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _Data;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Core.Editor {
    /// <summary>
    /// Utility for switching SDK configuration across platforms (Defines, ProjectSettings, WebGL template, etc.).
    /// Designed to be extensible for future build automation steps.
    /// </summary>
    public static class BuildPlatformSwitcher {
        public const string DEFINE_PLAYGAMA = "PLAYGAMA";
        public const string DEFINE_GAMEPUSH = "GAMEPUSH";

        public const string TEMPLATE_PLAYGAMA = "PROJECT:Bridge";
        public const string TEMPLATE_GAMEPUSH = "PROJECT:GamePush";

        public static readonly string[] ManagedDefines = {
            DEFINE_PLAYGAMA,
            DEFINE_GAMEPUSH
        };

        public static void SwitchToPlaygama() {
            SwitchTo(SDK_Type.Playgama);
        }

        public static void SwitchToGamePush() {
            SwitchTo(SDK_Type.GamePush);
        }

        public static void SwitchTo(SDK_Type sdkType) {
            Debug.Log($"[BuildPlatformSwitcher] Starting switch to {sdkType}...");

            string targetDefine = sdkType switch {
                SDK_Type.Playgama => DEFINE_PLAYGAMA,
                SDK_Type.GamePush => DEFINE_GAMEPUSH,
                _ => throw new ArgumentOutOfRangeException(nameof(sdkType), sdkType, null)
            };

            string targetTemplate = sdkType switch {
                SDK_Type.Playgama => TEMPLATE_PLAYGAMA,
                SDK_Type.GamePush => TEMPLATE_GAMEPUSH,
                _ => throw new ArgumentOutOfRangeException(nameof(sdkType), sdkType, null)
            };

            // 1. Update Scripting Define Symbols
            UpdateScriptingDefines(targetDefine, ManagedDefines);

            // 2. Update ProjectSettings asset (SDKType)
            UpdateProjectSettingsAsset(sdkType);

            // 3. Update WebGL Template in PlayerSettings
            UpdateWebGLTemplate(targetTemplate);

            AssetDatabase.SaveAssets();
            Debug.Log($"[BuildPlatformSwitcher] Successfully switched to {sdkType}!");
        }

        /// <summary>
        /// Updates scripting define symbols for the active build target and WebGL.
        /// Replaces any existing managed defines with the specified target define.
        /// </summary>
        public static void UpdateScriptingDefines(string targetDefine, IEnumerable<string> definesToRemove) {
            var removeSet = new HashSet<string>(definesToRemove ?? Array.Empty<string>());

            var targets = new List<NamedBuildTarget> {
                NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup),
                NamedBuildTarget.WebGL
            };

            var processedGroups = new HashSet<string>();

            foreach (var namedTarget in targets) {
                if (!processedGroups.Add(namedTarget.TargetName)) {
                    continue;
                }

                PlayerSettings.GetScriptingDefineSymbols(namedTarget, out string[] defines);
                var defineList = defines != null ? defines.ToList() : new List<string>();

                bool changed = false;

                // Remove competing defines
                for (int i = defineList.Count - 1; i >= 0; i--) {
                    string def = defineList[i].Trim();
                    if (removeSet.Contains(def) && def != targetDefine) {
                        defineList.RemoveAt(i);
                        changed = true;
                    }
                }

                // Add target define if missing
                if (!defineList.Contains(targetDefine)) {
                    defineList.Add(targetDefine);
                    changed = true;
                }

                if (changed) {
                    PlayerSettings.SetScriptingDefineSymbols(namedTarget, defineList.ToArray());
                    Debug.Log($"[BuildPlatformSwitcher] Scripting defines updated for {namedTarget.TargetName}: {string.Join(";", defineList)}");
                } else {
                    Debug.Log($"[BuildPlatformSwitcher] Scripting defines already up-to-date for {namedTarget.TargetName}.");
                }
            }
        }

        /// <summary>
        /// Finds and updates the ProjectSettings ScriptableObject located in Resources.
        /// </summary>
        public static void UpdateProjectSettingsAsset(SDK_Type sdkType) {
            ProjectSettings settings = Resources.Load<ProjectSettings>("ProjectSettings");

            if (settings == null) {
                string[] guids = AssetDatabase.FindAssets("t:ProjectSettings");
                foreach (string guid in guids) {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    var asset = AssetDatabase.LoadAssetAtPath<ProjectSettings>(path);
                    if (asset != null) {
                        settings = asset;
                        break;
                    }
                }
            }

            if (settings != null) {
                if (settings.SDKType != sdkType) {
                    Undo.RecordObject(settings, "Change SDK Type");
                    settings.SDKType = sdkType;
                    EditorUtility.SetDirty(settings);
                    Debug.Log($"[BuildPlatformSwitcher] ProjectSettings.asset updated: SDKType = {sdkType} at '{AssetDatabase.GetAssetPath(settings)}'");
                } else {
                    Debug.Log($"[BuildPlatformSwitcher] ProjectSettings.asset already has SDKType = {sdkType}.");
                }
            } else {
                Debug.LogWarning("[BuildPlatformSwitcher] Could not find ProjectSettings.asset in Resources or project! Please make sure it exists.");
            }
        }

        /// <summary>
        /// Updates PlayerSettings.WebGL.template to the target template name.
        /// </summary>
        public static void UpdateWebGLTemplate(string targetTemplate) {
            if (PlayerSettings.WebGL.template != targetTemplate) {
                string previousTemplate = PlayerSettings.WebGL.template;
                PlayerSettings.WebGL.template = targetTemplate;
                Debug.Log($"[BuildPlatformSwitcher] WebGL template changed from '{previousTemplate}' to '{targetTemplate}'");
            } else {
                Debug.Log($"[BuildPlatformSwitcher] WebGL template is already set to '{targetTemplate}'.");
            }
        }
    }
}
