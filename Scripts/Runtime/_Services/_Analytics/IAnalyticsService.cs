using System.Collections.Generic;

namespace _Infrastructure._Analytics {
    /// <summary>
    /// Абстракция для отправки аналитических событий.
    /// Позволяет легко менять провайдера (GamePush, Unity Analytics, AppMetrica и т.д.)
    /// </summary>
    public interface IAnalyticsService {
        /// <summary>
        /// Отправляет простое событие по имени.
        /// </summary>
        void LogEvent(string eventName);

        /// <summary>
        /// Отправляет событие с одним строковым значением (согласно API GamePush Goal).
        /// </summary>
        void LogEvent(string eventName, string value);
        void LogEvent(string eventName, int value);

        /// <summary>
        /// Отправляет событие с набором параметров (для более сложных систем аналитики).
        /// </summary>
        void LogEvent(string eventName, Dictionary<string, object> parameters);
    }
}