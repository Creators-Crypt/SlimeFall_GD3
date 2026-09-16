using System;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : Singleton<InventorySystem>
{
    [Header("Inventory")]
    [SerializeField] private List<string> questItems = new List<string>();
    [SerializeField] private List<EquipmentData> equipmentItems = new List<EquipmentData>();
    [SerializeField] private List<SpellWeaponData> weaponItems = new List<SpellWeaponData>();

    [SerializeField] private int maxBackpackCapacity = 20;

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

        weaponItems.Add(weapon);

        Debug.Log($"Weapon Added: {weapon.name}");
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
}