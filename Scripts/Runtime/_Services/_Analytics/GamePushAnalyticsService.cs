using System.Collections.Generic;
using System.Linq;
using GamePush;

namespace _Infrastructure._Analytics {
    /// <summary>
    /// Реализация аналитики для GamePush.
    /// Использует метод Goal согласно документации.
    /// </summary>
    public class GamePushAnalyticsService : IAnalyticsService {
        public void LogEvent(string eventName) {
            GP_Analytics.Goal(eventName, "");
        }

        public void LogEvent(string eventName, string value) {
            GP_Analytics.Goal(eventName, value);
        }

        public void LogEvent(string eventName, Dictionary<string, object> parameters) {
            // Так как GamePush Goal принимает только одно значение, 
            // мы можем объединить параметры в строку или взять самый важный.
            // В индустрии часто сериализуют в JSON или просто джойнят ключи-значения.
            string serializedParams = parameters != null 
                ? string.Join("; ", parameters.Select(kvp => $"{kvp.Key}:{kvp.Value}")) 
                : "";
            
            GP_Analytics.Goal(eventName, serializedParams);
        }
    }
}