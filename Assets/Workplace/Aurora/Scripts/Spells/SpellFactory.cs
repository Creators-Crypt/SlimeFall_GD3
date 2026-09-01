using UnityEngine;

/// <summary>
/// 
/// FACTORY pattern: maps a SpellDeliveryKind to its strategy instance.
/// Strategies are stateless, so one shared instance per kind is enough.
/// This is the single place to register new delivery shapes.
/// 
/// </summary>
public static class SpellFactory {

    private static readonly ProjectileDelivery projectile = new();
    private static readonly ArcProjectileDelivery arcProjectile = new();
    private static readonly RayDelivery ray = new();
    private static readonly AoeDelivery aoe = new();
    private static readonly HandDelivery hand = new();

    private static ElementalVfxDatabase vfxDatabase;

    public static void Initialize(ElementalVfxDatabase database) { vfxDatabase = database; }

    public static ElementVisualSettings GetVFX(SpellElement element) {

        if (vfxDatabase == null) {
            vfxDatabase = Resources.Load<ElementalVfxDatabase>("ElementalVfxDatabase");
        }
        return vfxDatabase != null ? vfxDatabase.GetSettings(element) : default;
    }

    public static ISpellDeliveryStrategy GetDelivery(SpellDeliveryKind kind) {

        switch (kind) {

            case SpellDeliveryKind.Projectile:    return projectile;
            case SpellDeliveryKind.ArcProjectile: return arcProjectile;
            case SpellDeliveryKind.Ray:           return ray;
            case SpellDeliveryKind.Aoe:           return aoe;
            case SpellDeliveryKind.Hand:          return hand;
            default:
                Debug.LogWarning($"[SpellFactory] Unknown delivery kind {kind}, defaulting to Projectile.");
                return projectile;
        }
    }

    /// <summary> Create a runtime Spell straight from its SO with no overrides. </summary>
    public static Spell Create(SpellData data) { return new SpellBuilder(data).Build(); }
}

/// <summary>
/// 
/// BUILDER pattern: fluent construction of a runtime Spell from a SpellData SO,
/// with optional per-instance overrides (delivery shape, element, damage, multi-spawn...).
///
/// var spell = new SpellBuilder(fireballData)
///     .WithDelivery(SpellDeliveryKind.ArcProjectile)
///     .WithSpawnCount(3, spreadAngle: 45f)
///     .WithElement(SpellElement.Fire)
///     .Build();
///     
/// </summary>
public class SpellBuilder {

    private readonly SpellData source;

    private SpellDeliveryKind? delivery;
    private SpellElement? element;

    private float? damage;
    private float? staminaCost;
    private float? concentrationCost;
    private float? cooldown;
    private int? spawnCount;
    private float? spreadAngle;
    private float? spawnInterval;

    public SpellBuilder(SpellData source) { this.source = source; }

    public SpellBuilder WithDelivery(SpellDeliveryKind kind)       { delivery = kind; return this; }
    public SpellBuilder WithElement(SpellElement elementType)      { element = elementType; return this; }
    public SpellBuilder WithDamage(float dmg)                      { damage = dmg; return this; }
    public SpellBuilder WithStaminaCost(float stamina)             { staminaCost = stamina; return this; }
    public SpellBuilder WithConcentrationCost(float concentration) { concentrationCost = concentration; return this; }
    public SpellBuilder WithCooldown(float waitTime)               { cooldown = waitTime; return this; }

    public SpellBuilder WithSpawnCount(int count, float? spreadAngle = null, float? interval = null) {

        spawnCount = Mathf.Max(1, count);
        if (spreadAngle.HasValue) this.spreadAngle = spreadAngle.Value;
        if (interval.HasValue) spawnInterval = interval.Value;
        return this;
    }

    public Spell Build() {

        if (source == null) {

            Debug.LogError("[SpellBuilder] Cannot build a spell from null SpellData.");
            return null;
        }

        // Runtime copy so overrides never touch the shared SO asset.
        var runtimeDelivery = delivery ?? source.delivery;
        var strategy = SpellFactory.GetDelivery(runtimeDelivery);

        var spell = new Spell(source, strategy);

        if (damage.HasValue)            spell.DamageOverride = damage.Value;
        if (staminaCost.HasValue)       spell.StaminaCostOverride = staminaCost.Value;
        if (concentrationCost.HasValue) spell.ConcentrationCostOverride = concentrationCost.Value;
        if (cooldown.HasValue)          spell.CooldownOverride = cooldown.Value;
        if (spawnCount.HasValue)        spell.SpawnCountOverride = spawnCount.Value;
        if (spreadAngle.HasValue)       spell.SpreadAngleOverride = spreadAngle.Value;
        if (spawnInterval.HasValue)     spell.SpawnIntervalOverride = spawnInterval.Value;
        if (delivery.HasValue)          spell.SetDelivery(delivery.Value);
        if (element.HasValue)           spell.Element = element.Value;

        return spell;
    }
}

/// <summary>
/// 
/// Runtime spell instance: data + current delivery strategy + current element.
/// SetDelivery / SetElement change the spell's type on the fly.
/// 
/// </summary>
public class Spell {

    public SpellData AssetData { get; }
    public ISpellDeliveryStrategy Delivery { get; private set; }
    public SpellElement Element { get; set; }
    public float CooldownRemaining { get; private set; }
    public bool IsReady => CooldownRemaining <= 0f;

    private float damageOverride;
    public float DamageOverride {
        get => damageOverride > 0f ? damageOverride : AssetData.damage;
        set => damageOverride = value;
    }
    private float staminaCostOverride;
    public float StaminaCostOverride {
        get => staminaCostOverride > 0f ? staminaCostOverride : AssetData.staminaCost;
        set => staminaCostOverride = value;
    }

    private float concentrationCostOverride;
    public float ConcentrationCostOverride {
        get => concentrationCostOverride > 0f ? concentrationCostOverride : AssetData.concentrationCost;
        set => concentrationCostOverride = value;
    }

    private float cooldownOverride;
    public float CooldownOverride {
        get => cooldownOverride > 0f ? cooldownOverride : AssetData.cooldown;
        set => cooldownOverride = value;
    }
    public int SpawnCountOverride { get; set; }
    public float SpreadAngleOverride { get; set; }
    public float SpawnIntervalOverride { get; set; }

    public Spell(SpellData data, ISpellDeliveryStrategy delivery) {

        AssetData = data;
        Delivery = delivery;
        Element = data.element;

        DamageOverride = data.damage;
        StaminaCostOverride = data.staminaCost;
        ConcentrationCostOverride = data.concentrationCost;
        CooldownOverride = data.cooldown;
        SpawnCountOverride = data.spawnCount;
        SpreadAngleOverride = data.spreadAngle;
        SpawnIntervalOverride = data.spawnInterval;
    }

    /// <summary> Change the delivery shape at runtime (projectile -> ray, etc.). </summary>
    public void SetDelivery(SpellDeliveryKind kind) {

        Delivery = SpellFactory.GetDelivery(kind);
    }

    /// <summary> Change the element at runtime without rebuilding. </summary>
    public void SetElement(SpellElement element) => Element = element;

    /// <summary>Tick from the owning MonoBehaviour's Update.</summary>
    public void Tick(float deltaTime) { CooldownRemaining = Mathf.Max(0f, CooldownRemaining - deltaTime); }

    /// <summary>
    /// 
    /// Cast toward a direction. Returns false if on cooldown or misconfigured.
    /// Caller is responsible for paying stamina first (check Data.staminaCost).
    /// 
    /// </summary>
    public bool Cast(MonoBehaviour runner, SpellWeaponData weapon, Transform caster, Vector3 origin, Vector3 direction, float multiplier)
    {
        if (!IsReady || AssetData == null) return false;

        if (direction.sqrMagnitude < 0.0001f) direction = Vector3.forward;

        SpellDeliveryKind activeDelivery = (weapon != null) ? weapon.delivery : Delivery.Kind;
        ISpellDeliveryStrategy activeStrategy = SpellFactory.GetDelivery(activeDelivery);

        int finalSpawnCount = (weapon != null && weapon.overrideSpawnCount) ? weapon.weaponSpawnCount : SpawnCountOverride;
        float finalSpreadAngle = (weapon != null && weapon.overrideSpawnCount) ? weapon.weaponSpreadAngle : SpreadAngleOverride;

        var context = new SpellCastContext {
            data = AssetData,
            element = Element,
            damage = this.DamageOverride,
            caster = caster,
            origin = origin,
            direction = direction.normalized,
            runner = runner,
            multiplier = multiplier,

            spawnCount = finalSpawnCount,
            spreadAngle = finalSpreadAngle,
            spawnInterval = SpawnIntervalOverride
        };

        activeStrategy.Cast(context);
        CooldownRemaining = CooldownOverride;
        return true;
    }
}