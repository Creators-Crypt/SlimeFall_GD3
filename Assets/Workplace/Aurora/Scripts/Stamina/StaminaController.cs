using UnityEngine;

public class StaminaController : MonoBehaviour, IStamina {
    
    [SerializeField] private PlayerStats stats;
    [SerializeField] private float currentStamina;
    [SerializeField, Range(3f, 25f)] private float fallBackRegenRate = 5f;
    [SerializeField] float regenMult = 1f;

    public float Current => currentStamina;
    public float Ratio => stats ? currentStamina / stats.maxStamina : 0f;
    public bool IsConsuming {  get; set; }
    private void Awake() {
        currentStamina = stats.maxStamina;
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

            currentStamina = Mathf.Clamp(currentStamina + (regenRate * regenMult * Time.deltaTime), 0f, stats.maxStamina);

        }
    }

    /// <summary>
    /// 
    /// Alternative regen for sprint (slower, can be called from a locomotion script).
    /// 
    /// </summary>
    public void Regen(float amount) {
        
        //if (stats == null) return;
        
        currentStamina = Mathf.Clamp(currentStamina + amount * Time.deltaTime, 0f, stats.maxStamina);
        
    }

    public void ContinousSpent(float amount) {

       
        currentStamina = Mathf.Clamp(currentStamina - amount * Time.deltaTime, 0f, stats.maxStamina);
    }

    
}