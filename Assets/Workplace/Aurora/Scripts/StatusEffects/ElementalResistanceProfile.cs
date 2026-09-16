using UnityEngine;

[System.Serializable]
public struct ElementMultiplier {
    public SpellElement element;
    [Tooltip("1.0 = Normal. 2.0 = Weakness (Double Damage). 0.5 = Resistant (Half Damage). 0.0 = Immune.")]
    public float multiplier;
}

[CreateAssetMenu(fileName = "NewResistanceProfile", menuName = "SpellSystem/Resistance Profile")]
public class ElementalResistanceProfile : ScriptableObject {

    [SerializeField] private ElementMultiplier[] modifiers;

    [Header("Default Fallbacks")]
    [Tooltip("If an element isn't explicitly listed in the array above, what multiplier does it use?")]
    public float defaultMultiplier = 1.0f;

    /// <summary> Returns the damage modifier for the incoming element. </summary>
    public float GetMultiplier(SpellElement element) {
        if (modifiers == null) return defaultMultiplier;

        foreach (var modifier in modifiers) {
            if (modifier.element == element) return modifier.multiplier;
        }
        return defaultMultiplier;
    }
}