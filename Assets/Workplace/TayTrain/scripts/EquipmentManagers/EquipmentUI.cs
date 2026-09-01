using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] EquipmentManager equipmentManager;

    [Header("Helmet")]
    [SerializeField] Image helmetIcon;
    [SerializeField] TMP_Text helmetName;
    [SerializeField] TMP_Text helmetStats;

    [Header("Amulet")]
    [SerializeField] Image amuletIcon;
    [SerializeField] TMP_Text amuletName;
    [SerializeField] TMP_Text amuletStats;

    [Header("Armor")]
    [SerializeField] Image armorIcon;
    [SerializeField] TMP_Text armorName;
    [SerializeField] TMP_Text armorStats;

    [Header("Boots")]
    [SerializeField] Image bootsIcon;
    [SerializeField] TMP_Text bootsName;
    [SerializeField] TMP_Text bootsStats;

    private void Start() {
        equipmentManager = GameObject.FindGameObjectWithTag("Player").GetComponent<EquipmentManager>();
    }

    void Update()
    {
        EquipmentData helmet = equipmentManager.GetHelmet();
        EquipmentData amulet = equipmentManager.GetAmulet();
        EquipmentData armor = equipmentManager.GetArmor();
        EquipmentData boots = equipmentManager.GetBoots();

        updateHelmet();
        updateAmulet();
        updateArmor();
        updateBoots();
    }
/*    private void UpdateGear(EquipmentData equipment) {
        
        if (equipment != null) {
            equipment.text = equipment.itemName;

            if (equipment.icon != null) {
                helmetIcon.sprite = helmet.icon;
            }

            string statsText = "";

            if (equipment.healthMaxBonus != 0)
                statsText += "Health Max: +" + helmet.healthMaxBonus + "\n";

            if (equipment.concentrationMaxBonus != 0)
                statsText += "Concentration Max: +" + equipment.concentrationMaxBonus + "\n";

            if (helmet.staminaMaxBonus != 0)
                statsText += "Stamina Max: +" + equipment.staminaMaxBonus + "\n";

            helmetStats.text = statsText;
        }
    }*/
    void updateHelmet()
    {
        EquipmentData helmet = equipmentManager.GetHelmet();

        if (helmet != null)
        {
            helmetName.text = helmet.itemName;

            if (helmet.icon != null)
            {
                helmetIcon.sprite = helmet.icon;
            }

            string statsText = "";

            if (helmet.healthMaxBonus != 0)
                statsText += "Health Max: +" + helmet.healthMaxBonus + "\n";

            if (helmet.concentrationMaxBonus != 0)
                statsText += "Concentration Max: +" + helmet.concentrationMaxBonus + "\n";

            if (helmet.staminaMaxBonus != 0)
                statsText += "Stamina Max: +" + helmet.staminaMaxBonus + "\n";

            helmetStats.text = statsText;
        }
        else
        {
            helmetName.text = "Helmet";
            helmetStats.text = "";
        }
    }

    void updateAmulet()
    {
        EquipmentData amulet = equipmentManager.GetAmulet();

        if (amulet != null)
        {
            amuletName.text = amulet.itemName;

            if (amulet.icon != null)
            {
                amuletIcon.sprite = amulet.icon;
            }

            string statsText = "";

            if (amulet.staminaRegenBonus != 0)
                statsText += "Stamina Regen: +" + amulet.staminaRegenBonus + "\n";

            if (amulet.concentrationTimeReduction != 0)
                statsText += "Concentration Time: -" + amulet.concentrationTimeReduction + "\n";

            if (amulet.healthRegenBonus != 0)
                statsText += "Health Regen: +" + amulet.healthRegenBonus + "\n";

            amuletStats.text = statsText;
        }
        else
        {
            amuletName.text = "Amulet";
            amuletStats.text = "";
        }
    }

    void updateArmor()
    {
        EquipmentData armor = equipmentManager.GetArmor();

        if (armor != null)
        {
            armorName.text = armor.itemName;

            if (armor.icon != null)
            {
                armorIcon.sprite = armor.icon;
            }

            string statsText = "";

            if (armor.teleportCooldownReduction != 0)
                statsText += "Teleport Cooldown: -" + armor.teleportCooldownReduction + "\n";

            if (armor.teleportDistanceBonus != 0)
                statsText += "Teleport Distance: +" + armor.teleportDistanceBonus + "\n";

            if (armor.dodgeCooldownReduction != 0)
                statsText += "Dodge Cooldown: -" + armor.dodgeCooldownReduction + "\n";

            if (armor.dodgeSpeedBonus != 0)
                statsText += "Dodge Speed: +" + armor.dodgeSpeedBonus + "\n";

            armorStats.text = statsText;
        }
        else
        {
            armorName.text = "Armor";
            armorStats.text = "";
        }
    }

    void updateBoots()
    {
        EquipmentData boots = equipmentManager.GetBoots();

        if (boots != null)
        {
            bootsName.text = boots.itemName;

            if(boots.icon != null)
            {
                bootsIcon.sprite = boots.icon;
            }

            string statsText = "";

            if (boots.bonusJumps != 0)
                statsText += "Bonus Jumps: " + boots.bonusJumps + "\n";

            if (boots.speedBonus != 0)
                statsText += "Speed Bonus: " + boots.speedBonus + "\n";

            if (boots.gravityReduction != 0)
                statsText += "Gravity Reduction: " + boots.gravityReduction + "\n";

            bootsStats.text = statsText;
        }
        else
        {
            bootsName.text = "Boots";
            bootsStats.text = "";
        }
    }
}
