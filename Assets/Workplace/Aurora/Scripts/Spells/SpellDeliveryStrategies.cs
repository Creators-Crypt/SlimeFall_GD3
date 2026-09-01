using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 
/// Everything a delivery strategy needs to execute one cast.
/// Built once per cast by Spell and handed to the strategy.
/// 
/// </summary>
public struct SpellCastContext {

    public SpellData data;
    public SpellElement element;      // May differ from data.element (runtime override)
    public float damage;
    public float multiplier;
    public Transform caster;
    public Vector3 origin;
    public Vector3 direction;         // Normalized aim direction
    public MonoBehaviour runner;      // Coroutine host for staggered spawns / beam fade

    public int spawnCount;
    public float spreadAngle;
    public float spawnInterval;
}

/// <summary>
/// 
/// STRATEGY pattern: one implementation per delivery shape.
/// A Spell holds a strategy and can swap it at runtime to change its type.
/// 
/// </summary>
public interface ISpellDeliveryStrategy {

    SpellDeliveryKind Kind { get; }
    void Cast(SpellCastContext context);
}

/// <summary>
/// 
/// Shared multi-spawn plumbing: fires Execute() spawnCount times,
/// fanned across spreadAngle, optionally staggered by spawnInterval.
/// 
/// </summary>
public abstract class SpellDeliveryStrategyBase : ISpellDeliveryStrategy {

    public abstract SpellDeliveryKind Kind { get; }
    public void Cast(SpellCastContext context) {

        int count = Mathf.Max(1, context.spawnCount);

        if (count == 1) {

            Execute(context, context.direction);
            return;
        }

        if (context.spawnInterval > 0f && context.runner != null)
            context.runner.StartCoroutine(StaggeredCast(context, count));
        else
            for (int i = 0; i < count; i++)
                Execute(context, FanDirection(context, i, count));
    }
    private IEnumerator StaggeredCast(SpellCastContext context, int count) {

        for (int i = 0; i < count; i++) {
            Execute(context, FanDirection(context, i, count));
            yield return new WaitForSeconds(context.spawnInterval);
        }
    }
    private static Vector3 FanDirection(SpellCastContext context, int index, int count) {

        float spread = context.spreadAngle;
        float t = count > 1 ? (float)index / (count - 1) : 0.5f;
        float angle = Mathf.Lerp(-spread * 0.5f, spread * 0.5f, t);
        Vector3 localRight = Vector3.Cross(context.direction, Vector3.up).normalized;
        Vector3 localUp = Vector3.Cross(localRight, context.direction).normalized;

        return Quaternion.AngleAxis(angle, localUp) * context.direction;
    }
    /// <summary>Fire one instance of this delivery in the given direction. </summary>
    protected abstract void Execute(SpellCastContext context, Vector3 direction);
    /// <summary>Damage every IDamageable in a circle. Shared by AOE and Hand. </summary>
    protected static void DamageCircle(SpellCastContext context, Vector3 center, float radius) {

        var hits = Physics.OverlapSphere(center, radius, context.data.hitLayers);
        foreach (var hit in hits) {
            var dmg = hit.GetComponentInParent<IDamageable>();
            if (dmg != null) {
                Debug.Log($"Enter here for damage, {context.damage}");
                dmg.OnDamage(context.damage * context.multiplier);
            }
        }
    }
}

// ------------------------------------------------------------------
// Concrete strategies
// ------------------------------------------------------------------

/// <summary> Straight-flying projectile. </summary>
public class ProjectileDelivery : SpellDeliveryStrategyBase {

    public override SpellDeliveryKind Kind => SpellDeliveryKind.Projectile;
    protected override void Execute(SpellCastContext context, Vector3 direction) {
        
        if (context.data.projectilePrefab == null) {
            Debug.LogWarning($"[Spell] '{context.data.spellName}' has no projectilePrefab assigned.");
            return;
        }


        Quaternion spawnRotation = Quaternion.LookRotation(direction, Vector3.up);

        var proj = Object.Instantiate(context.data.projectilePrefab, context.origin, Quaternion.identity);
        proj.Launch(context.data, context.element, direction, context.multiplier);

    }
}

/// <summary> Gravity-affected lobbed projectile. </summary>
public class ArcProjectileDelivery : SpellDeliveryStrategyBase {

    public override SpellDeliveryKind Kind => SpellDeliveryKind.ArcProjectile;
    protected override void Execute(SpellCastContext context, Vector3 direction) {
        
        if (context.data.projectilePrefab == null) {
            Debug.LogWarning($"[Spell] '{context.data.spellName}' has no projectilePrefab assigned.");
            return;
        }

        Vector3 tiltAxis = Vector3.Cross(direction, Vector3.up);
        Vector3 arced = Quaternion.AngleAxis(context.data.arcLaunchAngle, tiltAxis) * direction;

        Quaternion spawnRotation = Quaternion.LookRotation(arced, Vector3.up);

        var proj = Object.Instantiate(context.data.projectilePrefab, context.origin, Quaternion.identity);
        proj.Launch(context.data, context.element, arced, true, context.multiplier);

    }
}

/// <summary> Instant hitscan ray/laser with a brief LineRenderer visual. </summary>
public class RayDelivery : SpellDeliveryStrategyBase {

    public override SpellDeliveryKind Kind => SpellDeliveryKind.Ray;

    private static readonly RaycastHit[] hitBuffer = new RaycastHit[16];

    protected override void Execute(SpellCastContext context, Vector3 direction) {

        var vfxSettings = SpellFactory.GetVFX(context.element);
        
        Vector3 endPosition = context.origin + direction * context.data.rayDistance;

        int hitCount = Physics.RaycastNonAlloc(
            
            context.origin,
            direction,
            hitBuffer,
            context.data.rayDistance,
            context.data.hitLayers
        );

        if (hitCount > 0) {
            
            Array.Sort(hitBuffer, 0, hitCount, Comparer<RaycastHit>.Create((a, b) => a.distance.CompareTo(b.distance)));

            endPosition = hitBuffer[0].point;

            for (int i = 0; i < hitCount; i++) {
                var dmg = hitBuffer[i].collider.GetComponentInParent<IDamageable>();
                if (dmg != null) {
                    dmg.OnDamage(context.damage * context.multiplier);
                }
            }
        }
        // Beam visual - short-lived LineRenderer.
        var beam = new GameObject();
        var lineRenderer = beam.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, context.origin);
        lineRenderer.SetPosition(1, endPosition);
        lineRenderer.startWidth = context.data.rayWidth;
        lineRenderer.endWidth = context.data.rayWidth;
        lineRenderer.sharedMaterial = context.data.rayMaterial != null
            ? context.data.rayMaterial
            : new Material(Shader.Find("Sprites/Default"));

        lineRenderer.startColor = vfxSettings.primaryColor;
        lineRenderer.endColor = vfxSettings.primaryColor;
        lineRenderer.sortingOrder = 50;

        Object.Destroy(beam, Mathf.Max(0.02f, context.data.rayVisualDuration));
    }
}

/// <summary> Area burst at the caster or a point along the aim direction. </summary>
public class AoeDelivery : SpellDeliveryStrategyBase {
    public override SpellDeliveryKind Kind => SpellDeliveryKind.Aoe;

    protected override void Execute(SpellCastContext context, Vector3 direction) {

        Vector3 center = context.origin + direction * context.data.aoeCastDistance;
        DamageCircle(context, center, context.data.aoeRadius);

        if (context.data.aoeVfxPrefab != null) {

            var vfx = Object.Instantiate(context.data.aoeVfxPrefab, center, Quaternion.identity);
            Object.Destroy(vfx, 3f);
        }
    }
}

/// <summary> Close-range touch burst in front of the caster. </summary>
public class HandDelivery : SpellDeliveryStrategyBase {

    public override SpellDeliveryKind Kind => SpellDeliveryKind.Hand;

    protected override void Execute(SpellCastContext context, Vector3 direction) {

        Vector3 center = context.origin + direction * context.data.handRange;
        DamageCircle(context, center, context.data.handRadius);

        if (context.data.handVfxPrefab != null) {
            var vfx = Object.Instantiate(context.data.handVfxPrefab, center, Quaternion.identity);
            Object.Destroy(vfx, 2f);
        }
    }
}