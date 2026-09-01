public interface IConcentration 
{
    float Current { get; }
    float Ratio { get; }

    
    void spend(float cost);

    void refill();
    public float getDamageMultiplier();
    
  
}
