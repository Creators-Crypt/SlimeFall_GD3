using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 
/// Player-facing spellcasting component.
/// Holds a loadout of SpellData SOs, builds runtime Spells via SpellBuilder,
/// and casts toward the mouse. Number keys 1-4 switch spells,
/// Q cycles the equipped spell's delivery type at runtime.
/// 
/// </summary>
public class SpellCaster : MonoBehaviour {

    [Header("Loadout")]
    [Tooltip("Spell definitions available to the player. Slot 0 is equipped by default.")]
    [SerializeField] private SpellData[] spellLoadout;

    [Header("Casting")]
    [Tooltip("Where spells originate. Defaults to this transform if unassigned.")]
    [SerializeField] private Transform castPoint;

    private IStamina stamina;
    private IConcentration concentration;
    private SpellWeaponManager weaponManager;
    private Spell[] spells;
    private int equippedIndex;

    [SerializeField] private SpellWeaponData equippedWeapon;
    public SpellWeaponData EquippedWeapon => equippedWeapon;

    public Spell EquippedSpell =>
        (spells != null && spells.Length > 0) ? spells[equippedIndex] : null;

    private void Awake() {

        if (castPoint == null) castPoint = transform;

        // Grab whatever IStamina implementation lives on this GameObject.
        // Interface-typed, so swapping StaminaController for another system needs no code change here.
        stamina = GetComponent<IStamina>();
        concentration = GetComponent<IConcentration>();
        weaponManager = GetComponent<SpellWeaponManager>();

        BuildLoadout();
    }

    /// <summary> Builds every runtime Spell from its SO. Called once at startup. </summary>
    private void BuildLoadout() {

        if (spellLoadout == null || spellLoadout.Length == 0) {

            spells = new Spell[0];
            return;
        }

        spells = new Spell[spellLoadout.Length];

        for (int i = 0; i < spellLoadout.Length; i++) {

            if (spellLoadout[i] == null) continue;
            // Plain build - SO values as-is. Use builder overrides here for
            // pickups/talents, e.g. .WithSpawnCount(3, 45f) for a "split shot" upgrade.
            spells[i] = new SpellBuilder(spellLoadout[i]).Build();
        }
    }
    private void Update() {

        // Tick every cooldown, not just the equipped spell.
        if (spells != null) { foreach (var spell in spells) spell?.Tick(Time.deltaTime); }

        HandleInput();
    }
    private void HandleInput() {

        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        if (keyboard == null || mouse == null) return;

        // 1-4: switch equipped spell.
        if (keyboard.digit1Key.wasPressedThisFrame) Equip(0);
        if (keyboard.digit2Key.wasPressedThisFrame) Equip(1);
        if (keyboard.digit3Key.wasPressedThisFrame) Equip(2);
        if (keyboard.digit4Key.wasPressedThisFrame) Equip(3);

        // Q: cycle the equipped spell's delivery type at runtime.
        if (keyboard.qKey.wasPressedThisFrame) CycleDelivery();

        // Right mouse: cast toward cursor.
        if (mouse.rightButton.wasPressedThisFrame) TryCast();
    }
    public void Equip(int index) {

        if (spells == null || index < 0 || index >= spells.Length || spells[index] == null) return;

        equippedIndex = index;
    }

    /// <summary> Cycles Hand -> Projectile -> ArcProjectile -> Ray -> Aoe -> Hand. </summary>
    public void CycleDelivery() {

        var spell = EquippedSpell;
        if (spell == null) return;

        int totalDelivery = System.Enum.GetValues(typeof(SpellDeliveryKind)).Length;
        int next = ((int)spell.Delivery.Kind + 1) % totalDelivery;

        spell.SetDelivery((SpellDeliveryKind)next);
    }
    public void TryCast() {

        if (!weaponManager.CanSwap) return;

        var spell = EquippedSpell;

        if (spell == null || !spell.IsReady) return;

        if (EquippedWeapon == null) {
            spell.SetDelivery(spell.AssetData.delivery);
        } else {
            spell.SetDelivery(EquippedWeapon.delivery);
        }

            float requiredStamina = spell.StaminaCostOverride;
        if (stamina != null && !stamina.TrySpend(requiredStamina)) return;

        float multiplier = 1f;

        if(concentration != null)
        {
            multiplier = concentration.getDamageMultiplier();
            concentration.spend(spell.ConcentrationCostOverride);
        }

        Vector3 origin = castPoint.position;
        Vector3 aim = GetAimDirection(origin);

        spell.Cast(this, EquippedWeapon, transform, origin, aim, multiplier);

    }
    private Vector3 GetAimDirection(Vector3 origin) {

        var mouse = Mouse.current;
        var cam = Camera.main;

        if (mouse == null || cam == null) return transform.forward;

        Vector3 mouseScreenPosition = mouse.position.ReadValue();

        Ray ray = cam.ScreenPointToRay(mouseScreenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            Vector3 direction = hit.point - origin;
            
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f) return direction.normalized;
        }
        return transform.forward;
    }
    public void SetWeapon(SpellWeaponData newWeapon) { equippedWeapon = newWeapon; }
}