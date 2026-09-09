using UnityEngine;
using System.Collections.Generic;

public class Raritysystem : MonoBehaviour
{
    [Header("Rarity Definitions")]
    [SerializeField] private List<RarityDef> rarityDef;

    public RarityDef GetDef(ItemRarity rarity)
    {
        foreach (RarityDef def in rarityDef)
        {
            if (def != null && def.rarity == rarity)
            {
                return def;
            }
        }

        Debug.LogWarning("No rarity def found");
        return null;
    }

    public RarityDef RollRarity()
    {
        float totalWeight = 0f;

        foreach (RarityDef def in rarityDef)
        {
            if (def != null)
            {
                totalWeight += def.dropWeight;
            }
        }

        if(totalWeight <= 0f)
        {
            Debug.LogWarning("RaritySystem has no valid drop weight");
            return null;
        }

        float roll = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach(RarityDef def in rarityDef)
        {
            if(def == null)
            {
                continue;
            }

            currentWeight += def.dropWeight;

            if(roll <= currentWeight)
            {
                return def;
            }
        }
        return null;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            RarityDef rolledRarity = RollRarity();

            if(rolledRarity != null)
            {
                Debug.Log($"Rolled Rarity: {rolledRarity.rarity}");
            }
        }
    }
}
