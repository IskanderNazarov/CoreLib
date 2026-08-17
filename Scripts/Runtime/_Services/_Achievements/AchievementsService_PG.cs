// Файл: Core/Services/Achievements/AchievementsService_PG.cs
using Playgama;
using UnityEngine;

namespace core.achievements {
    public class AchievementsService_PG : IAchievementsService {

        public bool IsSupported => true; //Bridge.achievements.isSupported;

        public void Unlock(string achievementId) {
            if (!IsSupported) return;
            
            try {
                // Вызываем стандартный анлок Bridge SDK
                Bridge.achievements.Unlock(achievementId);
            } catch (System.Exception e) {
                Debug.LogError($"[AchievementsService_PG] Ошибка анлока: {e.Message}");
            }
        }

        public void SetProgress(string achievementId, int progress) {
            if (!IsSupported) return;

            // Многие веб-площадки в Playgama не поддерживают частичный SetProgress.
            // Если прогресс достиг 100%, мы просто триггерим Unlock.
            if (progress >= 100) {
                Unlock(achievementId);
            } else {
                Debug.LogWarning("[AchievementsService_PG] Установка частичного прогресса может не поддерживаться текущей площадкой. Логика должна обрабатываться локально.");
            }
        }

        public bool Has(string achievementId) {
            Debug.LogWarning("[AchievementsService_PG] Синхронный метод Has() недоступен в Bridge SDK. Проверяйте статус в локальном PlayerData.");
            return false; 
        }

        public int GetProgress(string achievementId) {
            Debug.LogWarning("[AchievementsService_PG] Синхронный метод GetProgress() недоступен. Проверяйте статус в локальном PlayerData.");
            return 0;
        }

        public void ShowUI() {
            if (!IsSupported) return;
            // Пытаемся показать нативный UI площадки, если он есть
            try {
                //Bridge.achievements.ShowNativePopup();
            } catch (System.Exception e) {
                Debug.LogWarning($"[AchievementsService_PG] Нативный UI достижений недоступен: {e.Message}");
            }
        }
    }
}