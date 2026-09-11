using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// Tracks and manages active status effects on a GameObject.
/// Please add this on the enemies and the Player if we want the player to be affected by statuses.
/// 
/// </summary>
/// <remarks>
/// 
/// Handles the application, updating, and removal of status effects, ensuring that expired effects are
/// cleaned up safely.
/// 
/// </remarks>

public class StatusEffectTracker : MonoBehaviour {

    private readonly List<StatusEffect> activeEffects = new();
    private readonly List<StatusEffect> removalQueue = new();

    private void Update() {
        float dt = Time.deltaTime;

        // Tick all active effects safely
        for (int i = 0; i < activeEffects.Count; i++) {
            var effect = activeEffects[i];
            effect.Tick(gameObject, dt);

            if (effect.IsExpired) {
                removalQueue.Add(effect);
            }
        }

        // Clean up expired items outside the main loop to avoid collection modification errors
        if (removalQueue.Count > 0) {
            foreach (var expiredEffect in removalQueue) {
                expiredEffect.OnRemove(gameObject);
                activeEffects.Remove(expiredEffect);
            }
            removalQueue.Clear();
        }
    }

    public void ApplyEffect(StatusEffect newEffect) {
        if (newEffect == null) return;

        // Check if the enemy already has this specific type of status applied
        StatusEffect existing = activeEffects.Find(e => e.Kind == newEffect.Kind);

        if (existing != null) {
            // Already affected: Refresh the duration instead of adding a double copy
            existing.Refresh(newEffect.DurationRemaining);
        } else {
            // New effect: Add and execute its starting behavior
            activeEffects.Add(newEffect);
            newEffect.OnApply(gameObject);
        }
    }
}
