using System;
using UnityEngine;

namespace _Services._BoosterInventory {
    /// <summary>
    /// Abstract contract for playing booster visual effects and juice.
    /// Portable to any game template.
    /// </summary>
    public interface IBoosterVisualService<TBoosterId> where TBoosterId : Enum {
        void PlayProjectile(Vector3 startScreenOrWorldPos, Vector3 targetWorldPos, Action onArrived = null);
        void PlayImpact(Vector3 targetWorldPos, Action onImpact = null);
        void PlayTargetPunch(Transform target, Action onComplete = null);
        void PlayBoosterVisual(TBoosterId id, Vector3 fromPos, Vector3? targetPos = null, Action onComplete = null);
    }
}
