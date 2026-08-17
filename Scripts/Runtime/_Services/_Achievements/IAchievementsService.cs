// Файл: Core/Services/Achievements/IAchievementsService.cs
namespace core.achievements {
    public interface IAchievementsService {
        /// <summary>
        /// Поддерживает ли текущая площадка функционал достижений.
        /// </summary>
        bool IsSupported { get; }

        /// <summary>
        /// Разблокировать достижение полностью.
        /// </summary>
        void Unlock(string achievementId);

        /// <summary>
        /// Установить частичный прогресс достижения.
        /// </summary>
        void SetProgress(string achievementId, int progress);

        /// <summary>
        /// Проверить, разблокировано ли достижение (Внимание: лучше проверять через локальный SaveManager!).
        /// </summary>
        bool Has(string achievementId);

        /// <summary>
        /// Получить текущий прогресс (Внимание: лучше проверять через локальный SaveManager!).
        /// </summary>
        int GetProgress(string achievementId);

        /// <summary>
        /// Открыть нативное окно/оверлей достижений площадки (если поддерживается).
        /// </summary>
        void ShowUI();
    }
}
