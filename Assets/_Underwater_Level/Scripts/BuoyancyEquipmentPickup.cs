using UnityEngine;

public class BuoyancyEquipmentPickup : MonoBehaviour, IInteractable {

    [Header("Interaction Settings")]
    [SerializeField] private string promptText = "Press E to equip Buoyancy Gear.";

    [Header("Gear Settings")]
    [SerializeField] private bool isTierTwoUpgrade = false;

    public string InteractionPrompt => promptText;

    public void Interact() {

        PlayerInteraction player = FindFirstObjectByType<PlayerInteraction>();

        if (player == null) return;

        GameObject playerObj = player.gameObject;

        if (isTierTwoUpgrade) {
            if (playerObj.TryGetComponent<BuoyancyController>(out var existingController)) {
                existingController.UpgradeToTierTwo();
            } else {
                BuoyancyController newController = playerObj.AddComponent<BuoyancyController>();
                newController.UpgradeToTierTwo();
            }
        } else {
            if (playerObj.GetComponent<BuoyancyController>() == null) {
                BuoyancyController newController = playerObj.AddComponent<BuoyancyController>();
                newController.currentBuoyancyModifier = 1.0f; // Default Tier 1 weight
                Debug.Log("Tier 1 Buoyancy Equipment successfully attached via interaction.");
            }
        }
        Destroy(gameObject);
        
    }
}