using System;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : Singleton<InventorySystem>
{
    protected override void Awake()
    {
        //Remove inventorySystem from prefab at runtime so singleton persists between scenes.
        transform.SetParent(null);
        base.Awake();
    }

    [Header("Inventory")]
    [SerializeField] private List<string> questItems = new List<string>();
    [SerializeField] private List<EquipmentData> equipmentItems = new List<EquipmentData>();
    [SerializeField] private List<SpellWeaponData> weaponItems = new List<SpellWeaponData>();

    [SerializeField] private int maxBackpackCapacity = 20;


    private EquipmentData equippedHelmet;
    private EquipmentData equippedAmulet;
    private EquipmentData equippedArmor;
    private EquipmentData equippedBoots;

    public IReadOnlyList<string> QuestItems => questItems;
    public IReadOnlyList<EquipmentData> EquipmentItems => equipmentItems;
    public IReadOnlyList<SpellWeaponData> WeaponItems => weaponItems;

    public void AddQuestItem(string itemName)
    {
        if (string.IsNullOrWhiteSpace(itemName))
            return;

        questItems.Add(itemName);

        Debug.Log($"Quest Item Added: {itemName}");
    }

    public void AddEquipment(EquipmentData equipment)
    {
        if (equipment == null)
            return;

        if(equipmentItems.Contains(equipment))
        {
            Debug.LogWarning(equipment.itemName + " is already in the inventroy.");
            return;
        }

        equipmentItems.Add(equipment);

        Debug.Log($"Equipment Added: {equipment.itemName}");
    }

    public bool HasQuestItem(string itemName)
    {
        return questItems.Contains(itemName);
    }

    public void AddWeapon(SpellWeaponData weapon)
    {
        if (weapon == null)
            return;

      if(weaponItems.Contains(weapon))
        {
            return;
        }

        weaponItems.Add(weapon);

    }

    public bool RemoveEquipment(EquipmentData equipment)
    {
        return equipmentItems.Remove(equipment);
    }

    public bool RemoveWeapon(SpellWeaponData weapon)
    {
        return weaponItems.Remove(weapon);
    }

    public bool IsBackpackFull() {
        return weaponItems.Count >= maxBackpackCapacity;
    }

    public void SetEquippedItem(EquipmentManager.EquipmentSlot slot, EquipmentData equipment)
    {
        switch (slot)
        {
            case EquipmentManager.EquipmentSlot.Helmet:
                equippedHelmet = equipment;
                break;

            case EquipmentManager.EquipmentSlot.Amulet:
                equippedAmulet = equipment;
                break;

            case EquipmentManager.EquipmentSlot.Armor:
                equippedArmor = equipment;
                break;

            case EquipmentManager.EquipmentSlot.Boots:
                equippedBoots = equipment;
                break;
        }
    }

    public EquipmentData GetEquippedItem(EquipmentManager.EquipmentSlot slot)
    { 
        switch(slot)
        {
            case EquipmentManager.EquipmentSlot.Helmet:
                return equippedHelmet;

            case EquipmentManager.EquipmentSlot.Amulet:
                return equippedAmulet;

            case EquipmentManager.EquipmentSlot.Armor:
                return equippedArmor;

            case EquipmentManager.EquipmentSlot.Boots:
                return equippedBoots;

            default:
                return null;

        }

    }
}