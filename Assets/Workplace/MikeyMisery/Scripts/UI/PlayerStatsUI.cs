using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private Image healthFill;
    [SerializeField] private Image staminaFill;
    [SerializeField] private Image manaFill;

    [SerializeField] private StaminaController staminaController;
    [SerializeField] private ConcentrationController concentrationController;

    private void OnEnable() {
        HealthSystem.OnHealthChangedUI += UpdateHealthBar;
    }
    private void OnDisable() {
        HealthSystem.OnHealthChangedUI -= UpdateHealthBar;
    }
    private void Start() {

        if(staminaController == null)
        staminaController = FindFirstObjectByType<StaminaController>();

        if(concentrationController == null)
        concentrationController = FindFirstObjectByType<ConcentrationController>();

        if(staminaController == null || concentrationController == null)
        {
            Debug.LogWarning("PlayerstatsUI could not find StaminaConroller or Concentration Controller");
            enabled = false;
            return;
        }

        staminaFill.fillAmount = staminaController.Ratio * 0.5f; // Start with half stamina
        manaFill.fillAmount = concentrationController.Ratio * 0.5f; // Start with half mana
    }

    private void Update()
    {
        if (staminaController != null && staminaFill != null)
            staminaFill.fillAmount = staminaController.Ratio * 0.5f;

        if (concentrationController != null && manaFill != null)
            manaFill.fillAmount = concentrationController.Ratio * 0.5f;
    }
    private void UpdateHealthBar(float currentHealth, float maxHealth) {

        if (healthFill == null) return;
        
        healthFill.fillAmount = currentHealth / maxHealth;
    }
}