using UnityEngine;

public class Horror_Objective : MonoBehaviour
{
    private void Start()
    {
        ObjectiveManager.Instance.SetObjective("Explore. PS: I wouldn't go near the castle without some gear.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            ObjectiveManager.Instance.SetObjective("DEFEAT THE BOSS!");

            Destroy(gameObject); 
        }
    }
}
