var YandexMetricaBridgePlugin = {
    $yandexMetricaBridge: {
        findCounterId: function () {
            if (typeof window === 'undefined') return 0;
            if (window.yandexMetricaCounterId) {
                return window.yandexMetricaCounterId;
            }
            if (window.metricaCounterId) {
                return window.metricaCounterId;
            }
            if (typeof window.Ya !== 'undefined' && window.Ya._metrika && window.Ya._metrika.counters) {
                var keys = Object.keys(window.Ya._metrika.counters);
                if (keys.length > 0) return parseInt(keys[0], 10) || 0;
            }
            return 0;
        },

        sendGoal: function (counterId, eventName, paramsJson) {
            if (typeof window === 'undefined') return;

            var targetCounterId = counterId || yandexMetricaBridge.findCounterId();

            var parsedParams = undefined;
            if (paramsJson && paramsJson.length > 0) {
                try {
                    parsedParams = JSON.parse(paramsJson);
                } catch (e) {
                    console.warn('[YandexMetricaBridge] Failed to parse params JSON:', e);
                }
            }

            // 1. Try window.ym standard function
            if (typeof window.ym === 'function') {
                try {
                    if (targetCounterId) {
                        if (parsedParams !== undefined) {
                            window.ym(targetCounterId, 'reachGoal', eventName, parsedParams);
                        } else {
                            window.ym(targetCounterId, 'reachGoal', eventName);
                        }
                        return;
                    } else {
                        // In case counterId is omitted in ym call
                        if (parsedParams !== undefined) {
                            window.ym('reachGoal', eventName, parsedParams);
                        } else {
                            window.ym('reachGoal', eventName);
                        }
                        return;
                    }
                } catch (e) {
                    console.error('[YandexMetricaBridge] Error calling window.ym:', e);
                }
            }

            // 2. Fallback to Ya.Metrika / yaCounterXXXXX
            if (targetCounterId) {
                var yaCounterName = 'yaCounter' + targetCounterId;
                if (typeof window[yaCounterName] !== 'undefined' && typeof window[yaCounterName].reachGoal === 'function') {
                    try {
                        if (parsedParams !== undefined) {
                            window[yaCounterName].reachGoal(eventName, parsedParams);
                        } else {
                            window[yaCounterName].reachGoal(eventName);
                        }
                        return;
                    } catch (e) {
                        console.error('[YandexMetricaBridge] Error calling ' + yaCounterName + ':', e);
                    }
                }
            }

            console.warn('[YandexMetricaBridge] Yandex Metrika is not initialized (ym or counter not found). Event:', eventName, parsedParams);
        }
    },

    YandexMetricaBridge_ReachGoal: function (counterId, eventNamePtr, paramsJsonPtr) {
        var eventName = UTF8ToString(eventNamePtr);
        var paramsJson = paramsJsonPtr ? UTF8ToString(paramsJsonPtr) : '';
        try {
            yandexMetricaBridge.sendGoal(counterId, eventName, paramsJson);
        } catch (e) {
            console.error('[YandexMetricaBridge] ReachGoal error:', e);
        }
    }
};

autoAddDeps(YandexMetricaBridgePlugin, '$yandexMetricaBridge');
mergeInto(LibraryManager.library, YandexMetricaBridgePlugin);
