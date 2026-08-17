// Файл: Core/Services/Achievements/AchievementsService_GP.cs
using GamePush;
using UnityEngine;

namespace core.achievements {
    public class AchievementsService_GP : IAchievementsService {
        
        // GamePush практически всегда поддерживает достижения через свой оверлей
        public bool IsSupported => true; 

        public void Unlock(string achievementId) {
            if (!GP_Init.isReady) return;
            GP_Achievements.Unlock(achievementId);
        }

        public void SetProgress(string achievementId, int progress) {
            if (!GP_Init.isReady) return;
            GP_Achievements.SetProgress(achievementId, progress);
        }

        public bool Has(string achievementId) {
            if (!GP_Init.isReady) return false;
            return GP_Achievements.Has(achievementId);
        }

        public int GetProgress(string achievementId) {
            if (!GP_Init.isReady) return 0;
            return GP_Achievements.GetProgress(achievementId);
        }

        public void ShowUI() {
            if (!GP_Init.isReady) return;
            GP_Achievements.Open();
        }
    }
}
