using UnityEngine;

public enum StatusEffectKind { None, Burn, Freeze, Shock }

public abstract class StatusEffect {

    public abstract StatusEffectKind Kind { get; }
    public float DurationRemaining { get; protected set; }
    public bool IsExpired => DurationRemaining <= 0f;

    public StatusEffect(float duration) => DurationRemaining = duration;
    public virtual void Tick(GameObject target, float deltaTime) { DurationRemaining -= deltaTime; }
    public abstract void OnApply(GameObject target);
    public abstract void OnRemove(GameObject target);
    public virtual void Refresh(float additionalDuration) {
        DurationRemaining = Mathf.Max(DurationRemaining, additionalDuration);
    }
}