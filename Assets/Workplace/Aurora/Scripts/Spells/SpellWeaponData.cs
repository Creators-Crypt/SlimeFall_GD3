using UnityEngine;

/// <summary>
/// Specifies the available types of weapons.
/// </summary>
public enum WeaponKind { Gun, Wand, Book, Staff, Stave }

[CreateAssetMenu(fileName = "Spell Weapon Data", menuName = "SpellSystem/Spell Weapon Data")]
public class SpellWeaponData : ScriptableObject {

    public string weaponName;
    public WeaponKind weaponKind;

    [Header("Spell System Bridge")]
    [Tooltip("The delivery shape this weapon forces onto any casted spell.")]
    public SpellDeliveryKind delivery;

    public GameObject weaponModelPrefab;

    [Header("Power Scales")]
    [Tooltip("Global impact scaling modifier applied to spell parameters")]
    public float damageMultiplier = 1.0f;
    public float cooldownMultiplier = 1.0f;

    [Tooltip("Optional: Modifies the spell's spawn count (Example: Wand now instantiates 3 projectiles)")]
    public bool overrideSpawnCount = false;
    public int weaponSpawnCount = 3;
    public float weaponSpreadAngle = 30f;
}