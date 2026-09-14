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

    [SerializeField] private Transform handAnchor;
    private GameObject spawnedModel;

    [Header("Swap Settings")]
    [Tooltip("How many seconds the Player needs to wait for swap.")]
    [SerializeField] private float swapCooldown = 0.5f;

    [Header("Input")]
    [SerializeField] private InputAction scrollAction;

    private int activeSlotIndex = 0;
    private float swapTimer = 0f;

    private string lastTrackedWeaponID = "";
    private bool isFirstFrameInitialized = false;

    private Transform runtimeCastPoint;
    public Transform ActiveCastPoint => runtimeCastPoint != null ? runtimeCastPoint : handAnchor;
    //private SpellWeaponData lastWeapon;
    public SpellWeaponData ActiveWeapon => (carriedWeapons != null && activeSlotIndex < carriedWeapons.Count) ? carriedWeapons[activeSlotIndex] : null;
    public bool CanSwap => swapTimer <= 0f;

    private void Awake() {

        if (spellCaster == null) spellCaster = GetComponent<SpellCaster>();
        carriedWeapons ??= new();
        if (carriedWeapons.Count == 0 && spellCaster != null && spellCaster.EquippedWeapon != null) {
            carriedWeapons.Add(spellCaster.EquippedWeapon);
            activeSlotIndex = 0;
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
    private void CycleWeapon() {

        if (!CanSwap) return;

        activeSlotIndex = (activeSlotIndex == 1) ? 0 : 1;

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