using UnityEngine;

public class BurnEffect : StatusEffect {

    public override StatusEffectKind Kind => StatusEffectKind.Burn;
    private readonly float damagePerSecond;
    private float damageTimer;

    public BurnEffect(float duration, float dps) : base(duration) {
        damagePerSecond = dps;
    }
    public override void OnApply(GameObject target) {
        Debug.Log($"[Status] {target.name} ignited! Burning for {DurationRemaining}s.");
    }
    public override void Tick(GameObject target, float deltaTime) {
        base.Tick(target, deltaTime);

        damageTimer += deltaTime;
        if (damageTimer >= 1f) {
            damageTimer -= 1f;
            if (target.TryGetComponent<IDamageable>(out IDamageable damage)) {
                damage.OnDamage(damagePerSecond);

                Vector3 dotSpawnPos = target.transform.position + Vector3.up * 1.8f;
                DamagePopupManager.SpawnPopup(dotSpawnPos, damagePerSecond, SpellElement.Fire);
            }
        }
    }
    public override void OnRemove(GameObject target) {
        Debug.Log($"[Status] Burn extinguished on {target.name}.");
    }
}