using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

public class EquipmentManager : MonoBehaviour, IEquipmentPickup
{
    [Header("Equipment Slots")]
    [SerializeField] EquipmentData helmet;
    [SerializeField] EquipmentData amulet;
    [SerializeField] EquipmentData armor;
    [SerializeField] EquipmentData boots;
    
    [Header("Player")]
    [SerializeField] EquipStatsMods stats;

    [UnitHeaderInspectable("Equipment Visuals")]
    [SerializeField] private Transform helmetAnchor;
    [SerializeField] private Transform amuletAnchor;
    [SerializeField] private Transform armorAnchor;
    [SerializeField] private Transform bootsAnchor;
    [SerializeField] private GameObject wizardHat;

    private GameObject currentHelmetModel;
    private GameObject currentAmuletModel;
    private GameObject currentArmorModel;
    private GameObject currentBootsModel;

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

        if(InventorySystem.Instance != null)
        {
            InventorySystem.Instance.SetEquippedItem(newEquipment.slot, newEquipment);
        }

        UpdateEquipmentVisual(newEquipment);
        Debug.Log("Equipped: " + newEquipment.itemName);
        return oldEquipment;
    }

    public EquipmentData UnequipEquipment(EquipmentSlot slot)
    {
        EquipmentData equipmentToRemove = null;

        switch(slot)
        {
            case EquipmentSlot.Helmet:
                equipmentToRemove = helmet;
                helmet = null;
                break;

            case EquipmentSlot.Amulet:
                equipmentToRemove = amulet;
                 amulet = null;
                break;

            case EquipmentSlot.Armor:
                equipmentToRemove = armor;
                armor = null;
                break;

            case EquipmentSlot.Boots:
                equipmentToRemove = boots;
                boots = null;
                break;

        }
        if(equipmentToRemove != null)
        {
            equipmentToRemove.Unequip(stats);
            RemoveEquipmentVisual(slot);

            if(InventorySystem.Instance != null)
            {
                InventorySystem.Instance.SetEquippedItem(slot, null);
            }
        }
        
        return equipmentToRemove;
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

    private void UpdateEquipmentVisual(EquipmentData equipment)
    {
        if (equipment == null)
            return;
        switch(equipment.slot)
        {
            case EquipmentSlot.Helmet:

                if (currentHelmetModel != null)
                     Destroy(currentHelmetModel);

                if(equipment.equippedModelPrefab != null && helmetAnchor != null)
                {
                    currentHelmetModel = Instantiate(equipment.equippedModelPrefab, helmetAnchor);

                    currentHelmetModel.transform.localPosition = Vector3.zero;
                    currentHelmetModel.transform.localRotation = Quaternion.identity;

                    if (wizardHat != null)
                        wizardHat.SetActive(false);
                }
                    break;

            case EquipmentSlot.Amulet:

                if (currentAmuletModel != null)
                    Destroy(currentAmuletModel);

                if (equipment.equippedModelPrefab != null && amuletAnchor != null)
                {
                    currentAmuletModel = Instantiate(equipment.equippedModelPrefab, amuletAnchor);

                    currentAmuletModel.transform.localPosition = Vector3.zero;
                    currentAmuletModel.transform.localRotation = Quaternion.identity;

                }
                break;

            case EquipmentSlot.Armor:

                if (currentArmorModel != null)
                    Destroy(currentArmorModel);

                if (equipment.equippedModelPrefab != null && armorAnchor != null)
                {
                    currentArmorModel = Instantiate(equipment.equippedModelPrefab, armorAnchor);

                    currentArmorModel.transform.localPosition = Vector3.zero;
                    currentArmorModel.transform.localRotation = Quaternion.identity;

                }
                break;

            case EquipmentSlot.Boots:

                if (currentBootsModel != null)
                    Destroy(currentBootsModel);

                if (equipment.equippedModelPrefab != null && bootsAnchor != null)
                {
                    currentBootsModel = Instantiate(equipment.equippedModelPrefab, bootsAnchor);

                    currentBootsModel.transform.localPosition = Vector3.zero;
                    currentBootsModel.transform.localRotation = Quaternion.identity;

                }
                break;

        }

}

    private void RemoveEquipmentVisual(EquipmentSlot slot)
    {
        switch (slot)
        {
            case EquipmentSlot.Helmet:


                if (currentHelmetModel != null)
                {
                    Destroy(currentHelmetModel);
                    currentHelmetModel = null;
                }

                if (wizardHat != null)
                    wizardHat.SetActive(true);

                break;

            case EquipmentSlot.Amulet:


                if (currentAmuletModel != null)
                {
                    Destroy(currentAmuletModel);
                    currentAmuletModel = null;
                }
                break;

            case EquipmentSlot.Armor:


                if (currentArmorModel != null)
                {
                    Destroy(currentArmorModel);
                    currentArmorModel = null;
                }
                break;

            case EquipmentSlot.Boots:


                if (currentBootsModel != null)
                {
                    Destroy(currentBootsModel);
                    currentBootsModel = null;
                }
                break;
        }
    }

    private void Start()
    {
        RestoreEquipment();
    }

    private void RestoreEquipment()
    {
        if (InventorySystem.Instance == null)
            return;

        RestoreSlot(InventorySystem.Instance.GetEquippedItem(EquipmentSlot.Helmet));

        RestoreSlot(InventorySystem.Instance.GetEquippedItem(EquipmentSlot.Amulet));

        RestoreSlot(InventorySystem.Instance.GetEquippedItem(EquipmentSlot.Armor));

        RestoreSlot(InventorySystem.Instance.GetEquippedItem(EquipmentSlot.Boots));
    }

    private void RestoreSlot(EquipmentData equipment)
    {
        if (equipment == null)
            return;

        switch(equipment.slot)
        {
            case EquipmentSlot.Helmet: 
                helmet = equipment;
                break;

            case EquipmentSlot.Amulet:
                amulet = equipment;
                break;

            case EquipmentSlot.Armor:
                armor = equipment;
                break;

            case EquipmentSlot.Boots:
               boots  = equipment;
                break;
        }

        equipment.Equip(stats);
        UpdateEquipmentVisual(equipment);
    }
}
