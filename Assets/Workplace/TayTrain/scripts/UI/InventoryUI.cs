using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;


public class InventoryUI : MonoBehaviour
{
    [Header("Inventory UI")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private TextMeshProUGUI questItemsText;
    [SerializeField] private TextMeshProUGUI equipmentText;
    [SerializeField] private TextMeshProUGUI weaponsText;

    [Header("Input")]
    [SerializeField] private InputAction inventoryAction;

    [Header("Equipment Controls")]
    [SerializeField] private TMP_Dropdown equipmentDropdown;
    [SerializeField] private Button equipButton;
    [SerializeField] private TMP_Dropdown equippedDropdown;
    [SerializeField] private Button unequipButton;
    [SerializeField] private Button dropButton;

    [Header("Weapon Controls")]
    [SerializeField] private TMP_Dropdown weaponDropdown;
    [SerializeField] private Button equipWeaponButton;
    [SerializeField] private SpellWeaponManager weaponManager;

    [Header("Rarity Colors")]
    [SerializeField] private Color uncommonColor = Color.white;
    [SerializeField] private Color rareColor = Color.blue;
    [SerializeField] private Color uniqueColor = Color.red;
    [SerializeField] private Color legendaryColor = Color.yellow;

    [SerializeField] private EquipmentManager equipmentManager;

    [SerializeField] private CameraController cameraController;

    private bool isOpen;

    private void Awake()
    {
        FindUIReferences();
    }
    private void OnEnable()
    {
        inventoryAction.Enable();
    }

    private void OnDisable()
    {
        inventoryAction.Disable();
    }
    private void Start()
    {
        if(inventoryPanel != null)
            inventoryPanel.SetActive(false);

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if(player != null)
        {
            equipmentManager = player.GetComponent<EquipmentManager>();
            weaponManager = player.GetComponent<SpellWeaponManager>();
        }

        cameraController = FindFirstObjectByType<CameraController>(FindObjectsInactive.Include);

        if(equipButton != null)
        {
            equipButton.onClick.AddListener(EquipSelectedItem);
        }

        if(unequipButton != null)
        {
            unequipButton.onClick.AddListener(UnequipSelectedItem);
        }

        if(dropButton != null)
        {
            dropButton.onClick.AddListener(DropSelectedItem);
        }

        if(equipWeaponButton != null)
        {
            equipWeaponButton.onClick.AddListener(EquipSelectedWeapon);
        }
    }

    private void Update()
    {
        if(inventoryAction.WasPressedThisFrame())
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (inventoryPanel == null)
            return;

        isOpen = !isOpen;

        inventoryPanel.SetActive(isOpen);

        if(isOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (cameraController != null)
                cameraController.enabled = false;

            Time.timeScale = 0f;

            RefreshUI();
        }
        else
        {
            Cursor.lockState= CursorLockMode.Locked;
            Cursor.visible = false;

            if (cameraController != null)
                cameraController.enabled = true;

            Time.timeScale = 1f;
        }
    }
    public void RefreshUI()
    {
        if (InventorySystem.Instance == null)
            return;
        UpdateQuestItems();
        UpdateEquipment();
        UpdateWeapons();
        UpdateEquipmentDropdown();
        UpdateEquippedDropdown();
        UpdateWeaponDropdown();
    }

    private void UpdateQuestItems()
    {
        if (questItemsText == null)
            return;

        questItemsText.text = "QUEST ITEMS\n";

        foreach (string item in InventorySystem.Instance.QuestItems)
        {
            questItemsText.text += $"- {item}\n";
        }
    }
    private void UpdateEquipment()
    {
        if (equipmentText == null)
            return;

        equipmentText.text = "EQUIPMENT\n";

        foreach (EquipmentData item in InventorySystem.Instance.EquipmentItems)
        {
            Color rarityColor = GetRarityColor(item.defaultRarity);
            string colorHex = ColorUtility.ToHtmlStringRGB(rarityColor);

            equipmentText.text += "- <color=#" + colorHex + ">" + item.itemName + "</color>\n";
        }
    }
    private void UpdateWeapons()
    {
        if (weaponsText == null)
            return;

        weaponsText.text = "WEAPONS\n";

        //Update the weapons when we have the weapon rarity stored somewhere to match the equipment above

        foreach (SpellWeaponData item in InventorySystem.Instance.WeaponItems)
        {
            weaponsText.text += $"- {item.name}\n";
        }
    }

    private void FindUIReferences()
    {
        Transform[] allChildren = transform.root.GetComponentsInChildren<Transform>(true);

        foreach(Transform child in allChildren)
        {
            switch(child.name)
            {
                case "InventoryPanel":
                   inventoryPanel = child.gameObject;
                    break;

                case "QuestItemsText":
                    questItemsText = child.GetComponent<TextMeshProUGUI>();
                    break;

                case "EquipmentText":
                    equipmentText = child.GetComponent<TextMeshProUGUI>();
                    break;

                case "WeaponsText":
                   weaponsText = child.GetComponent<TextMeshProUGUI>();
                    break;

                case "EquipmentDropdown":
                    equipmentDropdown = child.GetComponent<TMP_Dropdown>();
                    break;

                case "EquipButton":
                    equipButton = child.GetComponent<Button>();
                    break;

                case "EquippedDropdown":
                    equippedDropdown = child.GetComponent<TMP_Dropdown>();
                    break;

                case "UnequipButton":
                    unequipButton = child.GetComponent<Button>();
                    break;

                case "DropButton":
                    dropButton = child.GetComponent<Button>();
                    break;

                case "WeaponDropdown":
                    weaponDropdown = child.GetComponent<TMP_Dropdown>();
                    break;

                case "EquipWeaponButton":
                    equipWeaponButton = child.GetComponent<Button>();
                    break;
            }
        }
    }

    private void UpdateEquipmentDropdown()
    {
        if (equipmentDropdown == null)
            return;

        equipmentDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();

        foreach (EquipmentData item in InventorySystem.Instance.EquipmentItems)
        {
            options.Add(item.itemName);
        }

        equipmentDropdown.AddOptions(options);
    }

    private void EquipSelectedItem()
    {
        if (InventorySystem.Instance == null)
            return;

        if (equipmentManager == null)
            return;

        if (InventorySystem.Instance.EquipmentItems.Count == 0)
            return;

        int index = equipmentDropdown.value;

        if (index < 0 || index >= InventorySystem.Instance.EquipmentItems.Count)
            return;

        EquipmentData selectedEquipment = InventorySystem.Instance.EquipmentItems[index];

        bool removed = InventorySystem.Instance.RemoveEquipment(selectedEquipment);

        if(!removed)
        {
            Debug.LogWarning("Could not remove selected equipment from inventory");
            return;
        }

        EquipmentData oldEquipment = equipmentManager.GetEquipment(selectedEquipment);

        if(oldEquipment != null)
        {
            InventorySystem.Instance.AddEquipment(oldEquipment);
        }

        RefreshUI();
    }

    private void UpdateEquippedDropdown()
    {
        if (equippedDropdown == null || equipmentManager == null)
            return;

        equippedDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();

        options.Add(equipmentManager.GetHelmet() != null ? "Helmet: " + equipmentManager.GetHelmet().itemName : "Helmet: Empty");
        options.Add(equipmentManager.GetAmulet() != null ? "Amulet: " + equipmentManager.GetAmulet().itemName : "Amulet: Empty");
        options.Add(equipmentManager.GetArmor() != null ? "Armor: " + equipmentManager.GetArmor().itemName : "Armor: Empty");
        options.Add(equipmentManager.GetBoots() != null ? "Boots: " + equipmentManager.GetBoots().itemName : "Boots: Empty");

        equippedDropdown.AddOptions(options);
    }

    private void UnequipSelectedItem()
    {
        if (InventorySystem.Instance == null || equipmentManager == null)
            return;

        EquipmentManager.EquipmentSlot selectedSlot;

        switch(equippedDropdown.value)
        {
            case 0:
                selectedSlot = EquipmentManager.EquipmentSlot.Helmet;
                break;

            case 1:
                selectedSlot = EquipmentManager.EquipmentSlot.Amulet;
                break;

            case 2:
                selectedSlot = EquipmentManager.EquipmentSlot.Armor;
                break;

            case 3:
                selectedSlot = EquipmentManager.EquipmentSlot.Boots;
                break;

            default:
                return;
        }

        EquipmentData unequippedItem = equipmentManager.UnequipEquipment(selectedSlot);

        if(unequippedItem != null)
        {
            InventorySystem.Instance.AddEquipment(unequippedItem);
        }

        RefreshUI();
    }

    private void DropSelectedItem()
    {
        if(InventorySystem.Instance == null)
            return;

        if (InventorySystem.Instance.EquipmentItems.Count == 0)
            return;

        int index = equipmentDropdown.value;

        if (index < 0 || index >= InventorySystem.Instance.EquipmentItems.Count)
            return;
        
        EquipmentData selectedEquipment = InventorySystem.Instance.EquipmentItems[index];

        if(selectedEquipment.pickupPrefabs == null)
        {
            Debug.LogWarning(selectedEquipment.itemName + " does not have a pickup prefab.");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            return;

        Vector3 dropPosition = player.transform.position + player.transform.forward * 3f + Vector3.up * 0.5f;

        bool removed = InventorySystem.Instance.RemoveEquipment(selectedEquipment);

        if (!removed)
            return;

        Instantiate(selectedEquipment.pickupPrefabs, dropPosition, Quaternion.identity);

        Debug.Log("Dropped: " + selectedEquipment.itemName);

        RefreshUI();
    }

    private Color GetRarityColor(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Uncommon: return uncommonColor;

            case ItemRarity.Rare: return rareColor;

            case ItemRarity.Unique: return uniqueColor;

            case ItemRarity.Legendary: return legendaryColor;

            default: return Color.white;
        }
    }

    private void EquipSelectedWeapon()
    {
        if(InventorySystem.Instance == null || weaponManager == null)
            return;

        if (InventorySystem.Instance.WeaponItems.Count == 0)
            return;

        int index = weaponDropdown.value;

        if (index < 0 || index >= InventorySystem.Instance.WeaponItems.Count)
            return;

        SpellWeaponData selectedWeapon = InventorySystem.Instance.WeaponItems[index];

        weaponManager.EquipWeapon(selectedWeapon);

        Debug.Log("Equipped weapon: " + selectedWeapon.weaponName);
        
    }

    private void UpdateWeaponDropdown()
    {
        if (weaponDropdown == null || InventorySystem.Instance == null)
            return;

        weaponDropdown.ClearOptions();

        List<string> options = new List<string>();

        foreach (SpellWeaponData weapon in InventorySystem.Instance.WeaponItems)
        {
            options.Add(weapon.weaponName);
        }

        weaponDropdown.AddOptions(options);
    }
}
