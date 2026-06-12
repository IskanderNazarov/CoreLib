// Файл: Core/Services/Rating/IRatingService.cs
using System;

namespace core.rating {
    public interface IRatingService {
        /// <summary>
        /// Проверяет, можно ли прямо сейчас вызвать нативное окно оценки.
        /// </summary>
        bool IsReviewSupported();

        /// <summary>
        /// Вызывает нативное окно оценки площадки/магазина.
        /// </summary>
        /// <param name="onComplete">Коллбэк, возвращающий true, если окно было успешно показано (или игрок поставил оценку).</param>
        void RequestNativeReview(Action<bool> onComplete = null);
    }
}
