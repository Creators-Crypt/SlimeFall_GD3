using UnityEngine;

public class EquipmentInstanceTester : MonoBehaviour
{
    [SerializeField] private EquipmentData testEquipment;
    [SerializeField] private RaritySystem raritySystem;

    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            RarityDef rolledRarity = raritySystem.RollRarity();

            if (rolledRarity == null || testEquipment == null)
                continue;

            EquipmentInstance item =
                new EquipmentInstance(testEquipment, rolledRarity);

            Debug.Log(
                $"{item.baseData.itemName} | " +
                $"Rarity: {item.rarity} | " +
                $"Jumps: {item.bonusJumps} | " +
                $"Speed: {item.speedBonus:F2} | " +
                $"Gravity Reduction: {item.gravityReduction:F2}"
            );
        }
    }
}
