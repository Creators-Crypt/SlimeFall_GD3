using UnityEngine;

public class EquipmentPickup : MonoBehaviour
{
    [SerializeField] EquipmentData equipment;

    private bool pickedUp = false;
   
    private void OnTriggerEnter(Collider other)
    {
        if (pickedUp)
            return;

        if (!other.CompareTag("Player"))
            return;

        if(InventorySystem.Instance == null)
        {
            Debug.LogWarning("No InventroySystem found. ");
            return;
        }

        pickedUp = true;

        InventorySystem.Instance.AddEquipment(equipment);

        Debug.Log($"Picked up equipment: {equipment.itemName}");

        gameObject.SetActive(false);
    }
    public string GetEquipmentName()
    {
        return equipment.itemName;
    }
  

}
