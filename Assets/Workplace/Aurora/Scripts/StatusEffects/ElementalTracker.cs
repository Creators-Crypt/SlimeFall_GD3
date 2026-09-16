using UnityEngine;

public class ElementalTracker : MonoBehaviour {

    [Header("Elemental Affinities")]
    [Tooltip("Assign an asset configuration to set weaknesses and resistances.")]
    [SerializeField] private ElementalResistanceProfile resistanceProfile;

    private IDamageable localDamageable;

    private void Awake() { localDamageable = GetComponent<IDamageable>(); }

    /// <summary> 
    /// Intercepts incoming elemental attacks, calculates resistance math, 
    /// displays popups, and forwards flat numbers to teammates' scripts.
    /// </summary>
    public void ProcessIncomingDamage(float baseAmount, SpellElement element) {

        float dynamicMultiplier = 1.0f;
        if (resistanceProfile != null) {
            dynamicMultiplier = resistanceProfile.GetMultiplier(element);
        }

        float finalCalculatedDamage = baseAmount * dynamicMultiplier;

        Vector3 popupPosition = transform.position + Vector3.up * 2f;

        if (dynamicMultiplier >= 1.9f) {
            DamagePopupManager.SpawnPopup(popupPosition, finalCalculatedDamage, element);
            Debug.Log($"<color=cyan>[WEAKNESS]</color> {gameObject.name} took amplified {element} damage!");
        } else if (dynamicMultiplier > 0f && dynamicMultiplier <= 0.51f) {
            DamagePopupManager.SpawnPopup(popupPosition, finalCalculatedDamage, element);
            Debug.Log($"<color=grey>[RESISTED]</color> {gameObject.name} diminished incoming {element} damage.");
        } else if (dynamicMultiplier == 0f) {
            Debug.Log($"<color=red>[IMMUNE]</color> {gameObject.name} is completely immune to {element} magic!");
            return;
        } else {
            DamagePopupManager.SpawnPopup(popupPosition, finalCalculatedDamage, element);
        }
        localDamageable?.OnDamage(finalCalculatedDamage);
    }
}