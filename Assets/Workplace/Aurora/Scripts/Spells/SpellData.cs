using UnityEngine;

/// <summary> How the spell is physically delivered into the world. </summary>
public enum SpellDeliveryKind {

    Hand,           // Melee-range touch burst in front of the caster
    Projectile,     // Straight-flying projectile
    ArcProjectile,  // Gravity-affected lobbed projectile
    Ray,            // Instant hitscan laser/beam
    Aoe             // Area burst at caster or at a point along aim
}

/// <summary> Elemental/thematic school. Swap freely at runtime via Spell.SetElement. </summary>
public enum SpellElement { None, Fire, Ice, Void, Wind }

/// <summary>
/// 
/// Base data for one spell. Pure data - no behaviour.
/// Behaviour comes from delivery strategies chosen by SpellFactory/SpellBuilder.
/// 
/// </summary>
[CreateAssetMenu(fileName = "SpellData", menuName = "SpellSystem/Spell Data")]
public class SpellData : ScriptableObject {

    [Header("Identity")]
    public string spellName = "New Spell";
    public Sprite icon;
    public SpellElement element = SpellElement.None;

    [Header("Delivery")]
    [Tooltip("Default delivery shape. Can be overridden at runtime through SpellBuilder or Spell.SetDelivery.")]
    public SpellDeliveryKind delivery = SpellDeliveryKind.Projectile;

    [Header("Core Stats")]
    public float damage = 15f;
    public float staminaCost = 10f;
    public float concentrationCost = 25f;
    public float cooldown = 0.75f;
    [Tooltip("Layers this spell can hit (enemies, props...). Exclude the Player layer.")]
    public LayerMask hitLayers;

    [Header("Projectile")]
    public SpellProjectile projectilePrefab;
    public float projectileSpeed = 12f;
    public float projectileLifetime = 4f;
    [Tooltip("Gravity scale applied when delivery is ArcProjectile.")]
    public float arcGravityScale = 2f;
    [Tooltip("Upward launch bias in degrees for arcing shots.")]
    public float arcLaunchAngle = 30f;

    [Header("Ray / Laser")]
    public float rayDistance = 10f;
    public float rayWidth = 0.15f;
    public float rayVisualDuration = 0.12f;
    [Tooltip("Optional material for the beam LineRenderer. Falls back to Sprites/Default.")]
    public Material rayMaterial;
    public Color rayColor = Color.cyan;

    [Header("AOE")]
    public float aoeRadius = 2.5f;
    [Tooltip("0 = burst centered on caster. Otherwise placed this far along the aim direction.")]
    public float aoeCastDistance = 0f;
    public GameObject aoeVfxPrefab;

    [Header("Hand / Touch")]
    public float handRange = 1.4f;
    public float handRadius = 0.8f;
    public GameObject handVfxPrefab;

    [Header("Multi-Spawn")]
    [Min(1)] [Tooltip("How many instances of the delivery fire per cast. 1 = single.")]
    public int spawnCount = 1;
    [Tooltip("Total angular spread in degrees across all spawned instances.")]
    public float spreadAngle = 30f;
    [Tooltip("Seconds between successive spawns. 0 = all at once.")]
    public float spawnInterval = 0f;

    public GameObject impactVfxPrefab;
}