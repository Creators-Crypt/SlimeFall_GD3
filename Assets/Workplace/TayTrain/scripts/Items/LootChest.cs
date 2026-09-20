using UnityEngine;

public class LootChest : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private string interactionPrompt = "Open Chest";

    [Header("Loot")]
    [SerializeField] private GameObject equipmentPickupPrefab;
    [SerializeField] private GameObject weaponPickupPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private Transform LootSpawnPoint;
    [SerializeField] private float upwardForce = 2f;
    [SerializeField] private float forwardForce = 1f;

    [Header("Narrator")]
    [SerializeField] private bool playNarratorOnOpen;
    [SerializeField] private DesNarLine narratorLineOnOpen;

    private bool opened = false;

    public string InteractionPrompt
    {
        get
        {
            if (opened)
                return "Chest Empty";
            return interactionPrompt;
        }
    }

    public void Interact()
    {
        if (opened)
            return;

        if(equipmentPickupPrefab == null && weaponPickupPrefab == null)
        {
            Debug.LogWarning("No equipment reward assigned to chest.");
            return;
        }

        opened = true;

        if(playNarratorOnOpen && DesNarManager.Instance != null)
        {
            DesNarManager.Instance.PlayLine(narratorLineOnOpen);
        }

        if (equipmentPickupPrefab != null)
            SpawnLoot(equipmentPickupPrefab);

        if (weaponPickupPrefab != null)
            SpawnLoot(weaponPickupPrefab);

        Debug.Log("Chest opened.");
    }

    private void SpawnLoot(GameObject lootPrefab)
    {
        Vector3 spawnPosition = LootSpawnPoint != null ? LootSpawnPoint.position : transform.position + Vector3.up;

        GameObject spawnedLoot = Instantiate(lootPrefab, spawnPosition, Quaternion.identity);

        Rigidbody rb = spawnedLoot.GetComponent<Rigidbody>();

        if(rb != null && !rb.isKinematic)
        {
            Vector3 force = Vector3.up * upwardForce + transform.forward * forwardForce;

            rb.AddForce(force, ForceMode.Impulse);
        }
    }
}
