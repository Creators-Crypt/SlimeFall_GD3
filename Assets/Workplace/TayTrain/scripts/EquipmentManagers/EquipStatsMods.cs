using System;
using UnityEngine;

public class EquipStatsMods : MonoBehaviour
{
    public static event Action<float> OnHealthMaxChanged;

    [Header("References")]
    [SerializeField] PlayerController playerController;
    [SerializeField] StaminaController staminaController;
    [SerializeField] ConcentrationController concentrationController;

    [Header("Equipment Bonuses")]

    [Header("Helmet")]
    [SerializeField] float healthMaxBonus;
    [SerializeField] float concentrationMaxBonus;
    [SerializeField] float staminaMaxBonus;

    [Header("Amulets")]
    [SerializeField] float staminaRegenMult = 1f;
    [SerializeField] float concentrationSpeedMult = 1f;
    [SerializeField] float healthRegenMult = 1f;

    [Header("Armor")]
    [SerializeField] float teleportCooldownReduction;
    [SerializeField] float teleportDistanceBonus;
    [SerializeField] float dodgeCooldownReduction;
    [SerializeField] float dodgeSpeedBonus;

    [Header("Boots")]
    [SerializeField] int bonusJumps;
    [SerializeField] float speedMult = 1f;
    [SerializeField] float gravityMult = 1f;

    //Helmet
    public float HealthMaxBonus
    {
        get { return healthMaxBonus; }
    }
    public float ConcentrationMaxBonus
    {
        get { return concentrationMaxBonus; }
    }
    public float StaminaMaxBonus
    {
        get { return staminaMaxBonus; }
    }
    public void addHealthMax(float amount)
    {
        healthMaxBonus += amount;

        OnHealthMaxChanged?.Invoke(amount);
    }
    public void normalHealthMax(float amount)
    {
        healthMaxBonus -= amount;

        if (healthMaxBonus < 0)
        {
            healthMaxBonus = 0;
        }

        OnHealthMaxChanged?.Invoke(amount);
    }
    public void addConcentrationMax(float amount)
    {
        concentrationMaxBonus += amount;

        concentrationController.SetMaxBonus(concentrationMaxBonus);
    }
    public void normalConcentrationMax(float amount)
    {
        concentrationMaxBonus -= amount;

        if(concentrationMaxBonus < 0)
        {
            concentrationMaxBonus = 0;
        }

       concentrationController.SetMaxBonus(concentrationMaxBonus);
    }
    public void addStaminaMax(float amount)
    {
        staminaMaxBonus += amount;
       // will connect to stamina system
    }
    public void normalStaminaMax(float amount)
    {
        staminaMaxBonus -= amount;

        if (staminaMaxBonus < 0)
        {
            staminaMaxBonus = 0;
        }
        // will connect to stamina system
    }
    //Amulets
    public float StaminaRegenMult
    {
        get { return staminaRegenMult; }
    }
    public float ConcentrationSpeedMult
    {
        get { return concentrationSpeedMult; }
    }
    public float HealthRegenMult
    {
        get { return healthRegenMult; }
    }
    public void increaseStaminaRegen(float amount)
    {
        staminaRegenMult += amount;
        staminaController.setRegenMult(staminaRegenMult);
    }
    public void normalStaminaRegen(float amount)
    {
        staminaRegenMult -= amount;
        if (staminaRegenMult < 1f)
        {
            staminaRegenMult = 1f;
        }

        staminaController.setRegenMult(staminaRegenMult);
    }
    public void increaseConcentrationSpeedMult(float amount)
    {
        concentrationSpeedMult += amount;
        playerController.setConcentrationSpeedMult(concentrationSpeedMult);
    }
    public void normalConcentrationSpeedMult(float amount)
    {
        concentrationSpeedMult -= amount;
        if (concentrationSpeedMult < 1f)
        {
            concentrationSpeedMult = 1f;
        }
        playerController.setConcentrationSpeedMult(concentrationSpeedMult);
    }
    public void increaseHealthRegen(float amount)
    {
        healthRegenMult += amount;
        playerController.setHealthRegenMult(healthRegenMult);
    }
    public void normalHealthRegen(float amount)
    {
        healthRegenMult -= amount;
        if (healthRegenMult < 1f)
        {
            healthRegenMult = 1f;
        }
        playerController.setHealthRegenMult(healthRegenMult);
    }

    //Armor
    public float TeleportCooldownReduction
    {
        get { return teleportCooldownReduction; }
    }
    public float TeleportDistanceBonus
    {
        get { return teleportDistanceBonus; }
    }
    public float DodgeCooldownReduction
    {
        get { return dodgeCooldownReduction; }
    }
    public float DodgeSpeedBonus
    {
        get { return dodgeSpeedBonus; }
    }
    public void decreaseTeleportCooldown(float amount)
    {
        teleportCooldownReduction += amount;
        playerController.setTeleportCooldownReduction(teleportCooldownReduction);
    }
    public void normalTeleportCooldown(float amount)
    {
        teleportCooldownReduction -= amount;

        if(teleportCooldownReduction < 0f)
        {
            teleportCooldownReduction = 0f;
        }
        playerController.setTeleportCooldownReduction(teleportCooldownReduction);
    }

    public void increaseTeleportDistance(float amount)
    {
        teleportDistanceBonus += amount;
        playerController.setTeleportDistanceBonus(teleportDistanceBonus);
    }
    public void normalTeleportDistance(float amount)
    {
        teleportDistanceBonus -= amount;

        if (teleportDistanceBonus < 0f)
        {
            teleportDistanceBonus = 0f;
        }
        playerController.setTeleportDistanceBonus(teleportDistanceBonus);
    }
    public void decreaseDodgeCooldown(float amount)
    {
        dodgeCooldownReduction += amount;
        playerController.setDodgeCooldownReduction(dodgeCooldownReduction);
    }
    public void normalDodgeCooldown(float amount)
    {
        dodgeCooldownReduction -= amount;
        if(dodgeCooldownReduction < 0f)
        {
            dodgeCooldownReduction = 0f;
        }
        playerController.setDodgeCooldownReduction(dodgeCooldownReduction);
    }
    public void increaseDodgeSpeed(float amount)
    {
        dodgeSpeedBonus += amount;
        playerController.setDodgeSpeedBonus(dodgeSpeedBonus);
    }
    public void normalDodgeSpeed(float amount)
    {
        dodgeSpeedBonus -= amount;
        if(dodgeSpeedBonus < 0f)
        {
            dodgeSpeedBonus = 0f;
        }
        playerController.setDodgeSpeedBonus(dodgeSpeedBonus);
    }
    //Boots
    public int BonusJumps
    {
        get { return bonusJumps;}
    }
    public float SpeedMult
    {
        get { return speedMult; }
    }
    public float GravityMult
    {
        get { return gravityMult; }
    }
    public void addJumps(int amount)
    {
        bonusJumps += amount;
        playerController.setBonusJumps(bonusJumps);
    }
    public void removeJumps(int amount)
    {
        bonusJumps -= amount;
        if(bonusJumps < 0)
        {
            bonusJumps = 0;
        }
        playerController.setBonusJumps(bonusJumps);
    }
    public void addSpeed(float amount)
    {
        speedMult += amount;
        playerController.setSpeedMult(speedMult);
    }
    public void removeSpeed(float amount)
    {
        speedMult -= amount;
        if(speedMult < 1f)
        {
            speedMult = 1f;
        }
        playerController.setSpeedMult(speedMult);
    }
    public void lowerGravity(float amount)
    {
        gravityMult -= amount;
        gravityMult = Mathf.Max(0.1f, gravityMult);
        playerController.setGravityMult(gravityMult);
    }
    public void restoreGravity(float amount)
    {
        gravityMult += amount;
        
        if(gravityMult > 1f)
        {
            gravityMult = 1f;
        }
        playerController.setGravityMult(gravityMult);
        
    }
}
