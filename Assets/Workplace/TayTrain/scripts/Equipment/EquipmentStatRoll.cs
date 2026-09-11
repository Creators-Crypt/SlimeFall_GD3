using System;
using UnityEngine;

[Serializable]
public class EquipmentStatRoll 
{
    public EquipmentStatType statType;
    public float value;

    public EquipmentStatRoll(EquipmentStatType type, float statValue)
    {
        statType = type;
        value = statValue;
    }
}
