using System.Collections.Generic;

namespace _Infrastructure._Analytics {
    /// <summary>
    /// Позволяет отправлять события сразу нескольким сервисам.
    /// Например: в GamePush и в Консоль одновременно.
    /// </summary>
    public class CompositeAnalyticsService : IAnalyticsService {
        private readonly List<IAnalyticsService> _services;

        public CompositeAnalyticsService(params IAnalyticsService[] services) {
            _services = new List<IAnalyticsService>(services);
        }

        public void LogEvent(string eventName) {
            foreach (var service in _services) service.LogEvent(eventName);
        }

        public void LogEvent(string eventName, string value) {
            foreach (var service in _services) service.LogEvent(eventName, value);
        }
        public void LogEvent(string eventName, int value) {
            foreach (var service in _services) service.LogEvent(eventName, value);
        }

        public void LogEvent(string eventName, Dictionary<string, object> parameters) {
            foreach (var service in _services) service.LogEvent(eventName, parameters);
        }
    }
}