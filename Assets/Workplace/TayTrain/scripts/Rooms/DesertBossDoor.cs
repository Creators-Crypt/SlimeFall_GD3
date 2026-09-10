using UnityEngine;

public class DesertBossDoor : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] private GameObject doorObject;

    private void OnEnable()
    {
        GemCollectionManager.OnAllGemsCollected += OpenDoor;
    }
    private void OnDisabe()
    {
        GemCollectionManager.OnAllGemsCollected -= OpenDoor;
    }

    private void OpenDoor()
    {
        Debug.Log("All gems collected - opening boss door.");

        if(doorObject != null)
        {
            doorObject.SetActive(false);
        }
    }
}
