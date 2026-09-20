using UnityEngine;

public class StaminaController : MonoBehaviour, IStamina {
    
    [SerializeField] private PlayerStats stats;
    [SerializeField] private float currentStamina;
    [SerializeField, Range(3f, 25f)] private float fallBackRegenRate = 5f;
    [SerializeField] float regenMult = 1f;

    private float maxStamina;
    public float Current => currentStamina;
    public float Max => maxStamina;
    public float Ratio => maxStamina > 0f ? currentStamina / maxStamina : 0f;
    public bool IsConsuming {  get; set; }
    private void Awake() {
        maxStamina = stats.maxStamina;
        currentStamina = maxStamina;
    }
    public void setRegenMult(float amount)
    {

        regenMult = amount;
    }
    public bool TrySpend(float cost) {
        
        //if (stats == null) return false;
        
        if (currentStamina >= cost) {
            currentStamina -= cost;
            return true;
        }
        return false;
    }
    private void Update() {

        //if (stats == null) return;
        float regenRate = (stats != null && stats.staminaRegenRate > 0) ? stats.staminaRegenRate : fallBackRegenRate;
        if (!IsConsuming) {

            currentStamina = Mathf.Clamp(currentStamina + (regenRate * regenMult * Time.deltaTime), 0f, maxStamina);

        }
    }

    /// <summary>
    /// 
    /// Alternative regen for sprint (slower, can be called from a locomotion script).
    /// 
    /// </summary>
    public void Regen(float amount) {
        
        //if (stats == null) return;
        
        currentStamina = Mathf.Clamp(currentStamina + amount * Time.deltaTime, 0f, maxStamina);
        
    }

    public void ContinousSpent(float amount) {

       
        currentStamina = Mathf.Clamp(currentStamina - amount * Time.deltaTime, 0f, maxStamina);
    }

    public void SetStamina(float stamina)
    {
        currentStamina = Mathf.Clamp(
            stamina,
            0f,
            maxStamina
        ); 
    }
    public void SetMaxBonus(float bonus)
    {
        float oldMax = maxStamina;

        float staminaPercentage = oldMax > 0f ? currentStamina / oldMax : 1f;
        maxStamina = stats.maxStamina + Mathf.Max(0f, bonus);
        currentStamina = staminaPercentage * maxStamina;
    }
}