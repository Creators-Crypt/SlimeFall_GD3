using UnityEngine;

public class FreezeEffect : StatusEffect {
    public override StatusEffectKind Kind => StatusEffectKind.Freeze;
    private readonly float speedReductionPercent;

    public FreezeEffect(float duration, float slowPercent) : base(duration) {
        speedReductionPercent = slowPercent;
    }
    public override void OnApply(GameObject target) {
        Debug.Log($"[Status] {target.name} chilled! Slowed by {speedReductionPercent * 100}%.");
        // HOOK: Alter the enemy speed variable here (e.g. NavMeshAgent.speed)
    }

    public override void OnRemove(GameObject target) {
        Debug.Log($"[Status] {target.name} thawed out.");
        // HOOK: Restore enemy movement speed back to default
    }
}