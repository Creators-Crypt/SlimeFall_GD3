using UnityEngine;

public class ConcentrationController : MonoBehaviour ,IConcentration
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] private float currentConcentration;
    private float maxConcentrationBonus = 0f;
    public float Current => currentConcentration;
    private float MaxConcentration => stats.maxConcentration + maxConcentrationBonus;

    public float MaxConcentrationBonus
    { get { return maxConcentrationBonus; } }
    
    public float Ratio => stats ? currentConcentration / MaxConcentration : 0f;
    private void Awake()
    {
        currentConcentration = MaxConcentration;
    }
    public void spend(float cost)
    {
        currentConcentration = Mathf.Clamp(currentConcentration -  cost, 0f, MaxConcentration);
        //currentConcentration -= Spell.concentrationCost;
    }
    public float getDamageMultiplier()
    {
        if (currentConcentration >= 400f)
            return 2f;
        if (currentConcentration >= 300f)
            return 1.75f;
        if (currentConcentration >= 200f)
            return 1.5f;
        if (currentConcentration >= 100f)
            return 1.25f;

        return 1f;
    }
    public void refill()
    {
        currentConcentration = MaxConcentration;
    }
  
    public void SetMaxBonus(float amount)

    {
        maxConcentrationBonus = amount;

        if(currentConcentration > MaxConcentration)
        {
            currentConcentration = MaxConcentration;
        }
    }
}
