using UnityEngine;

public class SpellWeaponPickup : MonoBehaviour, IInteractable {

    private SpellWeaponManager spellManager;

    [Header("Weapon Settings")]
    [SerializeField] private SpellWeaponData weaponData;

    [Header("Visual Display")]
    [SerializeField] private Transform visualAnchor;

    [Header("Interaction Settings")]
    [SerializeField] private string promptMessage = "Press E to equip ";

    public string InteractionPrompt => weaponData != null ? $"{promptMessage}{weaponData.weaponName}" : "Interact";

    private void Awake() {
        if (TryGetComponent(out Collider collider)) {
            collider.isTrigger = true;
        }
        SpawnWorldDisplayMesh();
    }

    /// <summary> Spawns a floating preview of the weapon model in the world scene. </summary>
    private void SpawnWorldDisplayMesh() {
        if (weaponData == null || weaponData.weaponModelPrefab == null || visualAnchor == null) return;

        var worldMesh = Instantiate(weaponData.weaponModelPrefab, visualAnchor);
        worldMesh.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
    public void Interact() {
        if (weaponData == null) return;

        var player = GameObject.FindWithTag("Player");
        if (player == null || !player.TryGetComponent(out SpellWeaponManager weaponManager)) {
            Debug.LogError($"[Pickup] Failed to find a valid SpellWeaponManager on the object tagged 'Player'!");
            return;
        }
        int targetSlot = DetermineBestPlacementSlot(weaponManager);

        if (targetSlot == -1) {

            int activeSlot = GetActiveSlotIndexFromManager(weaponManager);
            weaponManager.EquipWeapon(weaponData);
            Debug.Log($"[Interaction] Inventory full. Overwrote active slot {activeSlot} with '{weaponData.weaponName}'.");
        } 
        else { weaponManager.EquipWeapon(weaponData); }

        Destroy(gameObject);
    }
    /// <summary> 
    /// Logic evaluation matrix: Checks if slot 0 is empty, then slot 1. 
    /// If both are full, returns -1.
    /// </summary>
    private int DetermineBestPlacementSlot(SpellWeaponManager manager) {

        if (manager.GetWeaponInSlot(0) == null) return 0;
        if (manager.GetWeaponInSlot(1) == null) return 1;
        return -1;
    }
    private int GetActiveSlotIndexFromManager(SpellWeaponManager manager) {
        return (manager.ActiveWeapon == manager.GetWeaponInSlot(1) && manager.GetWeaponInSlot(1) != null) ? 1 : 0;
    }
}