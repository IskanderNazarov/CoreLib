using System;
using System.Linq;
using _Services._BoosterInventory;
using _Services._Currency;
using UnityEditor;
using UnityEngine;

namespace Core.Editor {
    public static class BoostersConfigEditorUtility {
        public static void DrawScenarioInspector(SerializedObject serializedObject, SerializedProperty activeScenarioProp, SerializedProperty boostersProp) {
            serializedObject.Update();

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Booster A/B Scenario Configuration", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(activeScenarioProp, new GUIContent("Active Scenario"));

            var scenario = (BoosterScenarioType)activeScenarioProp.enumValueIndex;
            DrawScenarioDescription(scenario);

            EditorGUILayout.Space(6);
            ValidateCurrencyAndPricing(scenario, boostersProp);

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox(
                "Cap Rule: If Cap <= 0, the booster capacity is treated as infinite (UI shows only count, e.g. '2'). " +
                "If Cap > 0, the UI button displays '{count}/{cap}' (e.g. '2/5').",
                MessageType.Info
            );

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Booster Definitions", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(boostersProp, true);

            serializedObject.ApplyModifiedProperties();
        }

        public static void DrawScenarioDescription(BoosterScenarioType scenario) {
            string description = scenario switch {
                BoosterScenarioType.Classic =>
                    "Scenario 1 (Classic):\n" +
                    "• Shows booster count on button.\n" +
                    "• When count > 0: Tapping immediately applies the booster.\n" +
                    "• When count == 0: Button displays [+] plus icon. Tapping opens Market.",

                BoosterScenarioType.ModalDialog =>
                    "Scenario 2 (Modal Dialog - Unpuzzle Lines style):\n" +
                    "• Shows booster count on button.\n" +
                    "• When count > 0: Tapping immediately applies the booster.\n" +
                    "• When count == 0: Button displays [+] plus icon. Tapping opens BuyBoosterDialog (Buy for Coins vs Get for Ad).\n" +
                    "• If player has insufficient coins when clicking 'Buy for Coins', automatically redirects to Market.",

                BoosterScenarioType.InlineCoinBadge =>
                    "Scenario 3 (Inline Coin Price Badge):\n" +
                    "• Shows booster count on button.\n" +
                    "• When count > 0: Tapping immediately applies the booster.\n" +
                    "• When count == 0: Button displays a coin price badge directly on the button.\n" +
                    "• Tapping with enough coins: immediately purchases booster and updates UI (no extra dialog).\n" +
                    "• Tapping with insufficient coins: redirects to Market.",

                _ => "Unknown scenario."
            };

            EditorGUILayout.HelpBox(description, MessageType.None);
        }

        public static void ValidateCurrencyAndPricing(BoosterScenarioType scenario, SerializedProperty boostersProp) {
            bool scenarioRequiresCoins = scenario == BoosterScenarioType.ModalDialog || 
                                          scenario == BoosterScenarioType.InlineCoinBadge;

            // 1. Check if ICoinsService is available in the project assembly
            bool hasCurrencyImplementation = HasCoinsServiceImplementation();
            if (!hasCurrencyImplementation && scenarioRequiresCoins) {
                EditorGUILayout.HelpBox(
                    "DEVELOPER WARNING: Coins currency is not detected or implemented in this project!\n" +
                    "The selected scenario requires coins, but no active ICoinsService implementation was found. " +
                    "Coin purchases will fallback to Market.",
                    MessageType.Warning
                );
            }

            // 2. Check booster prices in property
            if (scenarioRequiresCoins && boostersProp != null && boostersProp.isArray) {
                for (int i = 0; i < boostersProp.arraySize; i++) {
                    var element = boostersProp.GetArrayElementAtIndex(i);
                    var priceProp = element.FindPropertyRelative("PriceInCoins");
                    var idProp = element.FindPropertyRelative("BoosterId");
                    if (priceProp != null && priceProp.intValue <= 0) {
                        string idName = idProp != null ? idProp.enumDisplayNames.ElementAtOrDefault(idProp.enumValueIndex) ?? idProp.intValue.ToString() : $"Index {i}";
                        EditorGUILayout.HelpBox(
                            $"DEVELOPER WARNING: Booster '{idName}' has PriceInCoins = {priceProp.intValue}! " +
                            $"Scenario '{scenario}' requires PriceInCoins to be greater than 0.",
                            MessageType.Error
                        );
                    }
                }
            }
        }

        private static bool HasCoinsServiceImplementation() {
            try {
                var targetType = typeof(ICoinsService);
                return AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => {
                        try { return a.GetTypes(); }
                        catch { return Array.Empty<Type>(); }
                    })
                    .Any(t => targetType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract && t != typeof(NullCoinsService));
            } catch {
                return false;
            }
        }
    }
}
