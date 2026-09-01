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

        staminaController = GameObject.FindGameObjectWithTag("Player").GetComponent<StaminaController>();

        concentrationController = GameObject.FindGameObjectWithTag("Player").GetComponent<ConcentrationController>();

        staminaFill.fillAmount = staminaController.Ratio * 0.5f; // Start with half stamina
        manaFill.fillAmount = concentrationController.Ratio * 0.5f; // Start with half mana
    }

    private void Update()
    {
        
        staminaFill.fillAmount = staminaController.Ratio * 0.5f; // Update stamina fill amount
        manaFill.fillAmount = concentrationController.Ratio * 0.5f;
    }
    private void UpdateHealthBar(float currentHealth, float maxHealth) {

        if (healthFill == null) return;
        
        healthFill.fillAmount = currentHealth / maxHealth;
    }
}