using UnityEngine;

public class TutorialItemPickup : MonoBehaviour, IInteractable {
    public string InteractionPrompt => $"Press Z to pickup {gameObject.name}";

    [SerializeField] private bool isWeapon;

    public void Interact() {
        if (isWeapon) {
            GameManager.Instance.PlayerPerformAction("WeaponPickedUp");
        } else {
            GameManager.Instance.PlayerPerformAction("EquipmentPickedUp");
        }

        // 2. Play pickup effects, give player the item, etc.

        // 3. Destroy the pickup object from the scene
        Destroy(gameObject);
    }
}