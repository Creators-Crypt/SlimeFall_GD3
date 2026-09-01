using UnityEngine;

public class EquipmentManager : MonoBehaviour, IEquipmentPickup
{
    [Header("Equipment Slots")]
    [SerializeField] EquipmentData helmet;
    [SerializeField] EquipmentData amulet;
    [SerializeField] EquipmentData armor;
    [SerializeField] EquipmentData boots;


    [Header("Player")]
    [SerializeField] EquipStatsMods stats;

    //Equipment types
    public enum EquipmentSlot
    {
        Helmet,
        Amulet,
        Armor,
        Boots
    }

    public EquipmentData GetEquipment(EquipmentData newEquipment)
    {
        if (newEquipment == null)
            return null;

        EquipmentData oldEquipment = null;

      switch (newEquipment.slot)
        {
            case EquipmentSlot.Helmet:
                oldEquipment = helmet;
                break;

            case EquipmentSlot.Amulet:
                oldEquipment = amulet;
                break;

            case EquipmentSlot.Armor:
                oldEquipment = armor;
                break;

            case EquipmentSlot.Boots:
                oldEquipment = boots;
                break;
        }

        if(oldEquipment == newEquipment)
        {
            return null;
        }

        if (oldEquipment != null)
        {
            oldEquipment.Unequip(stats);
        }
            

        switch (newEquipment.slot)
        {
            case EquipmentSlot.Helmet:
                helmet = newEquipment;
                break;

            case EquipmentSlot.Amulet:
                amulet = newEquipment;
                break;

            case EquipmentSlot.Armor:
                armor = newEquipment;
                break;

            case EquipmentSlot.Boots:
                boots = newEquipment;
                break;
        }

        newEquipment.Equip(stats);
        Debug.Log("Equipped: " + newEquipment.itemName);
        return oldEquipment;
    }

    public EquipmentData GetHelmet()
    {
        return helmet;
    }

    public EquipmentData GetAmulet()
    {
        return amulet;
    }

    public EquipmentData GetArmor()
    {
        return armor;
    }

    public EquipmentData GetBoots()
    {
        return boots;
    }
}
