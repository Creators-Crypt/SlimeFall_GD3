using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Equipment/Equipment Item")]
public class EquipmentData : ScriptableObject 
{
    [Header("Equipment Info")]
    public string itemName;

    [TextArea]
    public string itemDescription;

    public Sprite icon;

    public EquipmentManager.EquipmentSlot slot;

    [Header("World Pickup")]
    public GameObject pickupPrefabs;
    public Material pickupMaterial;

    //Boots
    [Header("Boots Bonuses")]
    public int bonusJumps;
    public float speedBonus;
    public float gravityReduction;

    //Amulets
    [Header("Amulet Bonuses")]
    public float staminaRegenBonus;
    public float concentrationTimeReduction;
    public float healthRegenBonus;

    //Helmets
    [Header("Helmet Bonuses")]
    public float healthMaxBonus;
    public float concentrationMaxBonus;
    public float staminaMaxBonus;

    //Armor
    [Header("Armor Bonuses")]
    public float teleportCooldownReduction;
    public float teleportDistanceBonus;
    public float dodgeCooldownReduction;
    public float dodgeSpeedBonus;

    public void Equip(EquipStatsMods stats)
    {
        switch(slot)
        {
            case EquipmentManager.EquipmentSlot.Helmet:
                stats.addHealthMax(healthMaxBonus);
                stats.addConcentrationMax(concentrationMaxBonus);
                stats.addStaminaMax(staminaMaxBonus);
                break;

            case EquipmentManager.EquipmentSlot.Amulet:
                stats.increaseStaminaRegen(staminaRegenBonus);
                stats.increaseConcentrationSpeedMult(concentrationTimeReduction);
                stats.increaseHealthRegen(healthRegenBonus);
                break;

            case EquipmentManager.EquipmentSlot.Armor:
                stats.decreaseTeleportCooldown(teleportCooldownReduction);
                stats.increaseTeleportDistance(teleportDistanceBonus);
                stats.decreaseDodgeCooldown(dodgeCooldownReduction);
                stats.increaseDodgeSpeed(dodgeSpeedBonus);
                break;

            case EquipmentManager.EquipmentSlot.Boots:
                stats.addJumps(bonusJumps);
                stats.addSpeed(speedBonus);
                stats.lowerGravity(gravityReduction);
                break;

            
        }
    }

    public void Unequip(EquipStatsMods stats)
    {
        switch(slot)
        {
            case EquipmentManager.EquipmentSlot.Helmet:
                stats.normalHealthMax(healthMaxBonus);
                stats.normalConcentrationMax(concentrationMaxBonus);
                stats.normalStaminaMax(staminaMaxBonus);
                break;

            case EquipmentManager.EquipmentSlot.Amulet:
                stats.normalStaminaRegen(staminaRegenBonus);
                stats.normalConcentrationSpeedMult(concentrationTimeReduction);
                stats.normalHealthRegen(healthRegenBonus);
                break;

            case EquipmentManager.EquipmentSlot.Armor:
                stats.normalTeleportCooldown(teleportCooldownReduction);
                stats.normalTeleportDistance(teleportDistanceBonus);
                stats.normalDodgeCooldown(dodgeCooldownReduction);
                stats.normalDodgeSpeed(dodgeSpeedBonus);
                break;

            case EquipmentManager.EquipmentSlot.Boots:
                stats.removeJumps(bonusJumps);
                stats.removeSpeed(speedBonus);
                stats.restoreGravity(gravityReduction);
                break;

           
        }
    }
}