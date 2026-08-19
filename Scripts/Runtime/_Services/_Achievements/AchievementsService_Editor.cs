namespace core.achievements {
    public class AchievementsService_Editor: IAchievementsService {
        public bool IsSupported { get; }
        public void Unlock(string achievementId) {
        }

        public void SetProgress(string achievementId, int progress) {
        }

        public bool Has(string achievementId) {
            return false;
        }

        public int GetProgress(string achievementId) {
            return 0;
        }

        public void ShowUI() {
            
        }
    }
}