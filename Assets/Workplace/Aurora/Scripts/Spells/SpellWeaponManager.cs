using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpellWeaponManager : MonoBehaviour {

    [SerializeField] private SpellCaster spellCaster;

    [Header("Weapon Slots")]
    [Tooltip("Max amount of weapons is 2!")]
    [SerializeField] private List<SpellWeaponData> carriedWeapons = new();
    private int maxWeapons = 2;

    [SerializeField] private Transform handAnchor;
    private GameObject spawnedModel;

    [Header("Swap Settings")]
    [Tooltip("How many seconds the Player needs to wait for swap.")]
    [SerializeField] private float swapCooldown = 0.5f;

    [Header("Input")]
    [SerializeField] private InputAction scrollAction;

    private int activeSlotIndex = 0;
    private float swapTimer = 0f;

    public SpellWeaponData ActiveWeapon => (carriedWeapons != null && activeSlotIndex < carriedWeapons.Count) ? carriedWeapons[activeSlotIndex] : null;
    public bool CanSwap => swapTimer <= 0f;

    private void Awake() {
        spellCaster = GetComponent<SpellCaster>();
        if (handAnchor == null) handAnchor = transform;
    }
    private void OnEnable() { scrollAction.Enable(); }
    private void OnDisable() { scrollAction.Disable(); }
    private void Start() { 
        
        if (carriedWeapons.Count == 0) {
            spellCaster.SetWeapon(null);
            return;
        }
        
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
    private void CycleWeapon() {

        if (!CanSwap) return;

        activeSlotIndex = (activeSlotIndex == 1) ? 0 : 1;

        swapTimer = swapCooldown;
        UpdateCasterWeapon();
    }

    private void UpdateCasterWeapon() {
        spellCaster.SetWeapon(ActiveWeapon);

        UpdateWeaponVisuals();
    }
    private void UpdateWeaponVisuals() {

        if (spawnedModel != null) Destroy(spawnedModel);

        SpellWeaponData currentWeapon = ActiveWeapon;
        //if (currentWeapon != null || currentWeapon.weaponPrefab == null) return;

        if (currentWeapon == null) {
            Debug.LogWarning($"[WEAPON DEBUG] ActiveWeapon is NULL at slot index {activeSlotIndex}. Check your weaponSlots array assignment.");
            return;
        }
        if (currentWeapon.weaponModelPrefab == null) {
            Debug.LogError($"[SpellWeaponManager] Missing Visual Prefab! '{currentWeapon.weaponName}' has no weaponModelPrefab assigned in its ScriptableObject asset asset file.", currentWeapon);
            return;
        }
        if (handAnchor == null) {
            Debug.LogError($"[SpellWeaponManager] Hand Anchor transform is unassigned on the {gameObject.name} GameObject!", this);
            return;
        }
        spawnedModel = Instantiate(currentWeapon.weaponModelPrefab, handAnchor);

        spawnedModel.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        spawnedModel.transform.localScale = Vector3.one;
    }
    public void EquipWeapon(SpellWeaponData newWeapon) {

        if (newWeapon == null) return;

        if (carriedWeapons.Count < maxWeapons) {
            carriedWeapons.Add(newWeapon);
            activeSlotIndex = carriedWeapons.Count - 1;
        } else {
            Debug.Log($"[Inventory Full] Replacing '{carriedWeapons[activeSlotIndex].weaponName}' with '{newWeapon.weaponName}'.");
            carriedWeapons[activeSlotIndex] = newWeapon;
        }

        UpdateCasterWeapon();
    }
    public SpellWeaponData GetWeaponInSlot(int index) {
        if (carriedWeapons == null || index < 0 || index >= carriedWeapons.Count) return null;
        return carriedWeapons[index];
    }
}