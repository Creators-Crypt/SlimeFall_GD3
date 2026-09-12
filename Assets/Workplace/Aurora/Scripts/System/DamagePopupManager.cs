using UnityEngine;

public class DamagePopupManager : Singleton<DamagePopupManager> {

    [SerializeField] private DamagePopup damagePopup;

    public static void SpawnPopup(Vector3 worldPosition, float damageAmount, SpellElement element) {

        if (Instance == null || Instance.damagePopup == null) return;

        DamagePopup popup = Instantiate(Instance.damagePopup, worldPosition, Quaternion.identity);

        string damageText = $"{Mathf.RoundToInt(damageAmount)}";
        Color fontColor = SpellFactory.GetElementColor(element);

        popup.Setup(damageText, fontColor);
    }
}