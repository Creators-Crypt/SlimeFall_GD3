using UnityEngine;

public interface IEquipment
{
    //Item parts that will connect to UI
    string ItemName { get; }
    string ItemDescription { get; }
    Sprite Icon { get; }
    EquipmentManager.EquipmentSlot Slot { get; }

    //Methods for changing out Equipment
    void equip(EquipStatsMods stats);

    void unequip(EquipStatsMods stats);
}
