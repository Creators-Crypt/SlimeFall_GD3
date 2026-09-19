using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpellWeaponManager : MonoBehaviour {

    [SerializeField] private SpellCaster spellCaster;

    [Header("Weapon Slots")]
    [Tooltip("Max amount of weapons is 2!")]
    [SerializeField] private List<SpellWeaponData> carriedWeapons = new();
    private int maxWeapons = 2;

    [Header("Visual Hand Anchors")]
    [SerializeField] private Transform handAnchor;
    [Tooltip("Where dropped weapons are physically instantiated in 3D space.")]
    [SerializeField] private Transform dropPoint;
    private GameObject spawnedModel;

    [Header("Swap Settings")]
    [Tooltip("How many seconds the Player needs to wait for swap.")]
    [SerializeField] private float swapCooldown = 0.5f;

    [Header("Input")]
    [SerializeField] private InputAction scrollAction;

    private int activeWeaponIndex = 0;
    private float swapTimer = 0f;

    private string lastTrackedWeaponID = "";
    private bool isFirstFrameInitialized = false;

    private Transform runtimeCastPoint;
    public Transform ActiveCastPoint => runtimeCastPoint != null ? runtimeCastPoint : handAnchor;
    //private SpellWeaponData lastWeapon;
    public SpellWeaponData ActiveWeapon => (carriedWeapons != null && activeWeaponIndex < carriedWeapons.Count) ? carriedWeapons[activeWeaponIndex] : null;
    public bool CanSwap => swapTimer <= 0f;

    private void Awake() {

        if (spellCaster == null) spellCaster = GetComponent<SpellCaster>();
        carriedWeapons ??= new();
        if (carriedWeapons.Count == 0 && spellCaster != null && spellCaster.EquippedWeapon != null) {
            carriedWeapons.Add(spellCaster.EquippedWeapon);
            activeWeaponIndex = 0;
            Debug.Log($"[SpellWeaponManager] found weapon successfully for {spellCaster.EquippedWeapon.weaponName} from Caster.");
        }
        if (handAnchor == null) handAnchor = transform;
    }
    private void OnEnable() { scrollAction.Enable(); }
    private void OnDisable() { scrollAction.Disable(); }
    private void Start() {
        if (carriedWeapons.Count == 0) {
            if (spellCaster != null) spellCaster.SetWeapon(null);
            return;
        }
        isFirstFrameInitialized = false;
        lastTrackedWeaponID = "Force_Reset";

        //Debug.Log($"[SpellWeaponManager] failed to find weapon {spellCaster.EquippedWeapon.weaponName} from Caster.");

        UpdateCasterWeapon();
    }
    private void Update() { 
        
        if (swapTimer > 0f) {
            swapTimer -= Time.deltaTime;
        }
        HandleWeaponInput(); 
    }
    private void HandleWeaponInput() {

        Vector2 scrollValue = scrollAction.ReadValue<Vector2>();

        if (Mathf.Abs(scrollValue.y) <= 0.1f) return;

        int validWeaponCount = 0;
        foreach (var slot in carriedWeapons) {
            if (slot != null) validWeaponCount++;
        }
        if (validWeaponCount <= 1) return;

        CycleWeapon();
    }
    public void ProcessIncomingPickup(SpellWeaponData weaponData) {
        
        if (weaponData == null) return;

        if (carriedWeapons.Count < maxWeapons) {
            carriedWeapons.Add(weaponData);
            activeWeaponIndex = carriedWeapons.Count - 1;

            if (InventorySystem.Instance != null) {
                InventorySystem.Instance.AddWeapon(weaponData);
            }
            UpdateCasterWeapon();
            return;
        }
        //Add an intentional capacity check hook to the Inventory System class next!
        if (InventorySystem.Instance != null && !InventorySystem.Instance.IsBackpackFull()) {
            InventorySystem.Instance.AddWeapon(weaponData);
            Debug.Log($"[Slots Full] Stored '{weaponData.weaponName}' directly inside global inventory backpack storage!");
            return;
        }
        if (ActiveWeapon != null) {

            SpellWeaponData weaponToDiscard = ActiveWeapon;

            if (InventorySystem.Instance != null) {
                InventorySystem.Instance.RemoveWeapon(weaponToDiscard);
            }
            DropWeaponInstance(weaponToDiscard);

            carriedWeapons[activeWeaponIndex] = weaponData;

            if (InventorySystem.Instance != null) {
                InventorySystem.Instance.AddWeapon(weaponData);
            }
            UpdateCasterWeapon();
        }
    }
    private void CycleWeapon() {

        if (!CanSwap) return;

        activeWeaponIndex = (activeWeaponIndex == 1) ? 0 : 1;

        swapTimer = swapCooldown;
        GameManager.Instance.PlayerPerformAction("WeaponCycle");
        UpdateCasterWeapon();
    }

    private void UpdateCasterWeapon() {

        if (spellCaster != null) spellCaster.SetWeapon(ActiveWeapon);


        string currentID = (ActiveWeapon != null) ? ActiveWeapon.name : "Unarmed";
        
        if (!isFirstFrameInitialized || currentID != lastTrackedWeaponID) {
            isFirstFrameInitialized = true;
            lastTrackedWeaponID = currentID;
            UpdateWeaponVisuals();
        }
    }
    private void UpdateWeaponVisuals() {

        if (spawnedModel != null) Destroy(spawnedModel);

        SpellWeaponData currentWeapon = ActiveWeapon;
        if (currentWeapon == null || currentWeapon.weaponModelPrefab == null) {
            runtimeCastPoint = null;
            return;
        }
        if (handAnchor == null) return;

        spawnedModel = Instantiate(currentWeapon.weaponModelPrefab);
        spawnedModel.transform.SetParent(handAnchor, false);
        spawnedModel.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        spawnedModel.transform.localScale = Vector3.one;

        var weaponTip = spawnedModel.transform.Find("CastPoint");
        runtimeCastPoint = weaponTip != null ? weaponTip : spawnedModel.transform;
    }
    public void EquipOrSwapWeapon(SpellWeaponData newWeapon) {

        if (newWeapon == null) return;

        int existingIndex = carriedWeapons.IndexOf(newWeapon);

        if(existingIndex >= 0)
        {
            activeWeaponIndex = existingIndex;
            UpdateCasterWeapon();
            return;
        }

        if (carriedWeapons.Count < maxWeapons) {
            carriedWeapons.Add(newWeapon);
            activeWeaponIndex = carriedWeapons.Count - 1;
            UpdateCasterWeapon();
        } else {
            //This drop weapon might be what is causing our duplicates. If you equip while both slots are full
            // it will drop without removing it from the inventory
            DropWeaponInstance(ActiveWeapon);
            Debug.Log($"[Inventory Full] Replacing '{carriedWeapons[activeWeaponIndex].weaponName}' with '{newWeapon.weaponName}'.");
            carriedWeapons[activeWeaponIndex] = newWeapon;
            UpdateCasterWeapon();
        }
    }
    public void DropActiveWeaponFromInventory() {
        if (carriedWeapons.Count == 0 || ActiveWeapon == null) return;

        SpellWeaponData weaponToDrop = ActiveWeapon;

        //Evict from global backpack framework tracker lists
        if (InventorySystem.Instance != null) {
            InventorySystem.Instance.RemoveWeapon(weaponToDrop);
        }

        //Physical ground instantiation
        DropWeaponInstance(weaponToDrop);

        //Clear local slots
        carriedWeapons.RemoveAt(activeWeaponIndex);

        if (activeWeaponIndex >= carriedWeapons.Count) {
            activeWeaponIndex = Mathf.Max(0, carriedWeapons.Count - 1);
        }
        UpdateCasterWeapon();
    }
    private void DropWeaponInstance(SpellWeaponData weaponToDrop) {
        if (weaponToDrop == null) return;

        GameObject prefabToSpawn = weaponToDrop.worldPickupPrefab != null ? weaponToDrop.worldPickupPrefab : weaponToDrop.weaponModelPrefab;
        GameObject droppedItem = Instantiate(prefabToSpawn, dropPoint.position, Quaternion.identity);

        if (droppedItem.TryGetComponent(out SpellWeaponPickup pickupScript)) {
            pickupScript.SetWeaponData(weaponToDrop);
        }
        // Apply a gentle physical pop outward so drops don't stack perfectly flat inside character collision fields
        if (droppedItem.TryGetComponent(out Rigidbody rb)) {
            Vector3 randomTossVector = (transform.forward + transform.up * 0.5f + UnityEngine.Random.insideUnitSphere * 0.2f).normalized;
            rb.AddForce(randomTossVector * 4f, ForceMode.Impulse);
        }
    }
    public SpellWeaponData GetWeaponInSlot(int index) {
        if (carriedWeapons == null || index < 0 || index >= carriedWeapons.Count) return null;
        return carriedWeapons[index];
    }
}