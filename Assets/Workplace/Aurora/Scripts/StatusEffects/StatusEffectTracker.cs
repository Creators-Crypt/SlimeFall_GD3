using System;
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

    public event Action<ElementalReactionKind> OnElementReaction;
    
    private readonly List<StatusEffect> activeEffects = new();
    private readonly List<StatusEffect> removalQueue = new();

    private void Update() {
        float dt = Time.deltaTime;

        // Tick all active effects safely
        for (int i = 0; i < activeEffects.Count; i++) {
            var effect = activeEffects[i];
            effect.Tick(gameObject, dt);

            if (effect.IsExpired) removalQueue.Add(effect);
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
    public bool ProcessIncomingElement(SpellElement incomingElement) {
        StatusEffectKind activeKind = GetActiveEffectKind();

        // 1. CHILL MATTERS (ICE)
        if (activeKind == StatusEffectKind.Freeze) {
            if (incomingElement == SpellElement.Fire) {
                TriggerReaction(ElementalReactionKind.Vaporize, StatusEffectKind.Freeze);
                return true;
            }
            if (incomingElement == SpellElement.Wind) {
                TriggerReaction(ElementalReactionKind.Supercharge, StatusEffectKind.Freeze);
                return true;
            }
        }

        // 2. IGNITE MATTERS (FIRE)
        if (activeKind == StatusEffectKind.Burn) {
            if (incomingElement == SpellElement.Ice) {
                TriggerReaction(ElementalReactionKind.Vaporize, StatusEffectKind.Burn);
                return true;
            }
        }

        // 3. Fallthrough: If no reaction triggered, apply the baseline tracking status normally
        ApplyBaselineStatus(incomingElement);
        return false;
    }
    private void TriggerReaction(ElementalReactionKind reaction, StatusEffectKind statusToRemove) {
        // Cleanse the conflicting active status instantly
        StatusEffect existing = activeEffects.Find(e => e.Kind == statusToRemove);
        if (existing != null) {
            existing.OnRemove(gameObject);
            activeEffects.Remove(existing);
        }

        Debug.Log($"<color=yellow>[REACTION]</color> {gameObject.name} triggered {reaction}!");

        // Execute dynamic visual changes or extra damage loops
        ExecuteReactionImpact(reaction);

        // Fire the event pipeline to update nearby puzzles or scripting containers
        OnElementReaction?.Invoke(reaction);
    }
    private void ExecuteReactionImpact(ElementalReactionKind reaction) {
        Vector3 headPosition = transform.position + Vector3.up * 2.2f;

        switch (reaction) {
            case ElementalReactionKind.Vaporize:
                // Spawn a big custom steam burst popup number!
                DamagePopupManager.SpawnPopup(headPosition, 40f, SpellElement.None); // High burst
                if (TryGetComponent(out IDamageable dmg)) dmg.OnDamage(40f);
                break;

            case ElementalReactionKind.Supercharge:
                // Stun the enemy or break their guard structure
                Debug.Log($"{gameObject.name} guard shattered by Supercharge!");
                break;
        }
    }
    private void ApplyBaselineStatus(SpellElement element) {
        switch (element) {
            case SpellElement.Fire: ApplyEffect(new BurnEffect(5.0f, 3f)); break;
            case SpellElement.Ice: ApplyEffect(new FreezeEffect(4.0f, 0.5f)); break;
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
    private StatusEffectKind GetActiveEffectKind() {
        if (activeEffects.Count > 0) return activeEffects[0].Kind;
        return StatusEffectKind.None;
    }
    public SpellWeaponData GetWeaponInSlot(int index) => null; // Kept for interface compliance
}