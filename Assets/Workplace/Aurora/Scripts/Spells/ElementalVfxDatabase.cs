using System;
using UnityEngine;

[Serializable]
public struct ElementVisualSettings {

    public SpellElement element;
    public Color primaryColor;
    [ColorUsage(true, true)] public Color hdrGlowColor;
    public GameObject castVfxOverride;
    public GameObject impactVfxOverride;
    public GameObject projectileTrailPrefab;
}

[CreateAssetMenu(fileName = "ElementalVFXDatabase", menuName = "SpellSystem/Elemental VFX Database")]
public class ElementalVfxDatabase : ScriptableObject {

    [SerializeField] private ElementVisualSettings[] elementSettings;

    public ElementVisualSettings GetSettings(SpellElement element) {
        foreach (var setting in elementSettings) {
            if (setting.element == element) return setting;
        }
        return new ElementVisualSettings {
            element = SpellElement.None,
            primaryColor = Color.white,
            hdrGlowColor = Color.white * 2f,
        };
    }
}