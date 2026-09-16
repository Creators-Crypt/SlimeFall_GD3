using UnityEngine;

public class RarityGlow : MonoBehaviour
{
    
    [Header("Rarity")]
    [SerializeField] private ItemRarity rarity;

    [Header("Glow Settings")]
    [SerializeField] private Light glowLight;
    [SerializeField] private float intensity = 1f;
    [SerializeField] private float range = 3f;

    private RaritySystem raritySystem;

    private void Awake()
    {
        raritySystem = FindFirstObjectByType<RaritySystem>();

        SetupLight();
    }

    private void Start()
    {
        UpdateGlow();
    }

    public void SetRarity(ItemRarity newRarity)
    {
        rarity = newRarity;
        UpdateGlow();
    }

    private void SetupLight()
    {
        if (glowLight != null)
            return;

        GameObject LightObject = new GameObject("Rarity Glow Light");
        LightObject.transform.SetParent(transform);
        LightObject.transform.localPosition = Vector3.zero;

        glowLight = LightObject.AddComponent<Light>();

        glowLight.type = LightType.Point;
        glowLight.shadows = LightShadows.None;
        glowLight.intensity = intensity;
        glowLight.range = range;
    }

    private void UpdateGlow()
    {
      if(raritySystem == null)
        {
            Debug.LogWarning("RarityGlow could not find RaritySystem.", this);
            return;
        }

        if (glowLight == null)
            return;

        RarityDef rarityDef = raritySystem.GetDef(rarity);

        if (rarityDef == null)
            return;

        glowLight.color = rarityDef.displayColor;
        glowLight.intensity = intensity;
        glowLight.range = range;
    }
}
