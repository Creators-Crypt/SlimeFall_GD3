using UnityEngine;

//Creating menu to make it easier to adjust later
[CreateAssetMenu( fileName = "NewRarityDefinition", menuName = "Items/Rarity Definition")]

public class RarityDef : ScriptableObject
{
    [Header("Name")]
    public ItemRarity rarity;

    [Header("Visuals")]
    public Color displayColor = Color.white;

    [Header("Loot")]
    [Min(0f)]
    public float dropWeight = 1f;

    [Header("Random Stats")]
    [Min(0)]
    public int minStatRolls = 1;

    [Min(0)]
    public int maxStatRolls = 1;

    [Min(0f)]
    public float minStatMultiplier = 1f;

    [Min(0f)]
    public float maxStatMultiplier = 1f;
}
