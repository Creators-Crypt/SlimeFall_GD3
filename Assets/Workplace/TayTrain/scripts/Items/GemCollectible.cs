using UnityEngine;

public class GemCollectible : MonoBehaviour
{
    [Header("Quest Item")]
    [SerializeField] private string questItemName = "Desert Gem";

    private bool collected = false;

    //public string InteractionPrompt => "Collect Gem";

   private void OnTriggerEnter(Collider other)
    {
        if (collected)
            return;

        if (!other.CompareTag("Player"))
            return;

        if(GemCollectionManager.Instance == null || InventorySystem.Instance == null)
        {
            Debug.LogWarning("GemCollectible has no GemCollectionManager assigned.");
            return;
        }

        collected = true;

        InventorySystem.Instance.AddQuestItem(questItemName);
        GemCollectionManager.Instance.CollectGem();

        gameObject.SetActive(false);
    }
}
