using UnityEngine;

public class ObjectiveTrigger : MonoBehaviour {

    [SerializeField] private string newObjective;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ObjectiveManager.Instance.SetObjective(newObjective);
            Destroy(gameObject); //Destroy the trigger after it's activated
        }
    }
}