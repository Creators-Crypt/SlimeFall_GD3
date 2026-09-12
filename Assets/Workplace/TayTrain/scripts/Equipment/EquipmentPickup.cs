using UnityEngine;

public class EquipmentPickup : MonoBehaviour, IInteractable
{
    [SerializeField] EquipmentData equipment;

    private bool pickedUp = false;
   
    public string InteractionPrompt
    {
        get
        {
            if (equipment == null)
                return "Pick up Equipment";

            return "Pick up " + equipment.itemName;
        }
    }

    public void Interact()
    {
        if (pickedUp)
            return;

        if(equipment == null)
        {
            Debug.LogWarning("No equipmentData assigned to pickup.");
            return;
        }
        if(InventorySystem.Instance == null)
        {
            Debug.LogWarning("No InventorySystem found.");
            return;
        }
        pickedUp = true;
        InventorySystem.Instance.AddEquipment(equipment);
        GameManager.Instance.PlayerPerformAction("EquipmentPickedUp");
        Debug.Log("Picked up equipment: " + equipment.itemName);
        gameObject.SetActive(false);
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (pickedUp)
    //        return;

    //    if (!other.CompareTag("Player"))
    //        return;

    //    if(InventorySystem.Instance == null)
    //    {
    //        Debug.LogWarning("No InventroySystem found. ");
    //       return;
    //    }

    //    pickedUp = true;

    //    InventorySystem.Instance.AddEquipment(equipment);

    //    Debug.Log($"Picked up equipment: {equipment.itemName}");

    //    gameObject.SetActive(false);
    //}
    public string GetEquipmentName()
    {
        if (equipment == null)
            return "";

        return equipment.itemName;
    }


}
