using UnityEngine;

public class LevelKeyPickup : MonoBehaviour
{

    [Header("Key")]
    [SerializeField] private string keyName = "Desert Key";

    private bool collected;

    private void OnTriggerEnter(Collider other)
    {
        if (collected || !other.CompareTag("Player"))
            return;

        if(InventorySystem.Instance == null)
        {
            return;
        }

        collected = true;

        if (!InventorySystem.Instance.HasQuestItem(keyName))
        {
            InventorySystem.Instance.AddQuestItem(keyName);
        }

        gameObject.SetActive(false);
    }

   

}
