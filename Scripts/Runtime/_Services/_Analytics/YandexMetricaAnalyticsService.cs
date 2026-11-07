using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace _Services._Analytics {
    /// <summary>
    /// Sends analytics events directly to Yandex Metrika via WebGL jslib bridge.
    /// Supports rich dictionary parameters (e.g. level, coins, etc.) serialized as JSON.
    /// </summary>
    public class YandexMetricaAnalyticsService : IAnalyticsService {
        private readonly long _counterId;

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void YandexMetricaBridge_ReachGoal(double counterId, string eventName, string paramsJson);
#endif

        public YandexMetricaAnalyticsService(long counterId = 0) {
            _counterId = counterId;
        }

        public void LogEvent(string eventName) {
            LogEvent(eventName, (string)null);
        }

        public void LogEvent(string eventName, string value) {
            if (string.IsNullOrEmpty(value)) {
                SendGoal(eventName, null);
            } else {
                var dict = new Dictionary<string, object> { { "value", value } };
                SendGoal(eventName, SerializeParams(dict));
            }
        }

        public void LogEvent(string eventName, int value) {
            var dict = new Dictionary<string, object> { { "value", value } };
            SendGoal(eventName, SerializeParams(dict));
        }

        public void LogEvent(string eventName, Dictionary<string, object> parameters) {
            string json = SerializeParams(parameters);
            SendGoal(eventName, json);
        }

        private void SendGoal(string eventName, string paramsJson) {
#if !UNITY_EDITOR && UNITY_WEBGL
            try {
                YandexMetricaBridge_ReachGoal(_counterId, eventName, paramsJson ?? string.Empty);
            } catch (System.Exception ex) {
                Debug.LogError($"[YandexMetricaAnalyticsService] Failed to send goal '{eventName}': {ex.Message}");
            }
#else
            Debug.Log($"[YandexMetrica (Simulated)] Goal: {eventName} | Params: {paramsJson ?? "none"} (CounterId: {_counterId})");
#endif
        }

        private string SerializeParams(Dictionary<string, object> parameters) {
            if (parameters == null || parameters.Count == 0) return null;

            var sb = new StringBuilder();
            sb.Append("{");
            bool first = true;
            foreach (var kvp in parameters) {
                if (!first) sb.Append(",");
                first = false;

                sb.Append("\"").Append(EscapeJson(kvp.Key)).Append("\":");
                if (kvp.Value == null) {
                    sb.Append("null");
                } else if (kvp.Value is string s) {
                    sb.Append("\"").Append(EscapeJson(s)).Append("\"");
                } else if (kvp.Value is bool b) {
                    sb.Append(b ? "true" : "false");
                } else if (kvp.Value is int || kvp.Value is long || kvp.Value is short || kvp.Value is byte ||
                           kvp.Value is float || kvp.Value is double || kvp.Value is decimal) {
                    sb.Append(System.Convert.ToString(kvp.Value, System.Globalization.CultureInfo.InvariantCulture));
                } else {
                    sb.Append("\"").Append(EscapeJson(kvp.Value.ToString())).Append("\"");
                }
            }
            sb.Append("}");
            return sb.ToString();
        }

        private static string EscapeJson(string str) {
            if (string.IsNullOrEmpty(str)) return "";
            return str
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }
    }
}
