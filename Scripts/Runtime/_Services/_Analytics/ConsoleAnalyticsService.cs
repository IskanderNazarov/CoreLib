using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Infrastructure._Analytics {
    /// <summary>
    /// Логирует события в консоль Unity. Полезно для разработки.
    /// </summary>
    public class ConsoleAnalyticsService : IAnalyticsService {
        private readonly string _prefix = "[Analytics]";

        public void LogEvent(string eventName) {
            Debug.Log($"{_prefix} Event: {eventName}");
        }

        public void LogEvent(string eventName, string value) {
            Debug.Log($"{_prefix} string Event: {eventName} | Value: {value}");
        }
        public void LogEvent(string eventName, int value) {
            Debug.Log($"{_prefix} int Event: {eventName} | Value: {value}");
        }

        public void LogEvent(string eventName, Dictionary<string, object> parameters) {
            string p = parameters != null 
                ? string.Join(", ", parameters.Select(kvp => $"{kvp.Key}={kvp.Value}")) 
                : "null";
            Debug.Log($"{_prefix} Event: {eventName} | Params: {p}");
        }
    }
}