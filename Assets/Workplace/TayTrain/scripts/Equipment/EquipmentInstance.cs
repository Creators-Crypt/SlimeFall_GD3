using UnityEngine;

[System.Serializable]
public class EquipmentInstance 
{
    [Header("Base Item")]
    public EquipmentData baseData;
    public ItemRarity rarity;

    [Header("Rolled Boots Stats")]
    public int bonusJumps;
    public float speedBonus;
    public float gravityReduction;

    [Header("Rolled Amulet Stats")]
    public float staminaRegenBonus;
    public float concentrationTimeReduction;
    public float healthRegenBonus;

    [Header("Rolled Helmet Stats")]
    public float healthMaxBonus;
    public float concentrationMaxBonus;
    public float staminaMaxBonus;

    [Header("Rolled Armor Stats")]
    public float teleportCooldownReduction;
    public float teleportDistanceBonus;
    public float dodgeCooldownReduction;
    public float dodgeSpeedBonus;

    public EquipmentInstance(EquipmentData data, RarityDef rarityDef)
    {
        baseData = data;
        rarity = rarityDef.rarity;

        RollStats(rarityDef);
    }

    private void RollStats(RarityDef rarityDef)
    {
        switch(baseData.slot)
        {
            case EquipmentManager.EquipmentSlot.Helmet:
                healthMaxBonus = RollStat(baseData.healthMaxBonus, rarityDef);
                concentrationMaxBonus = RollStat(baseData.concentrationMaxBonus, rarityDef);
                staminaMaxBonus = RollStat(baseData.staminaMaxBonus, rarityDef);

                break;

            case EquipmentManager.EquipmentSlot.Amulet:
               staminaRegenBonus  = RollStat(baseData.staminaRegenBonus, rarityDef);
               concentrationTimeReduction  = RollStat(baseData.concentrationTimeReduction, rarityDef);
               healthRegenBonus  = RollStat(baseData.healthRegenBonus, rarityDef);

                break;

            case EquipmentManager.EquipmentSlot.Armor:
                teleportCooldownReduction = RollStat(baseData.teleportCooldownReduction, rarityDef);
                teleportDistanceBonus = RollStat(baseData.teleportDistanceBonus, rarityDef);
                dodgeCooldownReduction = RollStat(baseData.dodgeCooldownReduction, rarityDef);
                dodgeSpeedBonus = RollStat(baseData.dodgeSpeedBonus, rarityDef);

                break;

            case EquipmentManager.EquipmentSlot.Boots:
                bonusJumps = Mathf.RoundToInt(RollStat(baseData.bonusJumps, rarityDef));
                speedBonus = RollStat(baseData.speedBonus, rarityDef);
                gravityReduction = RollStat(baseData.gravityReduction, rarityDef);

                break;
        }
    }
    private float RollStat(float baseStat, RarityDef rarityDef)
    {
        if(baseStat == 0f)
        {
            return 0f;
        }

        float multiplier = Random.Range(rarityDef.minStatMultiplier, rarityDef.maxStatMultiplier);
        return baseStat * multiplier;
    }

    public void Equip(EquipStatsMods stats)
    {
        switch(baseData.slot)
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
        switch(baseData.slot)
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
