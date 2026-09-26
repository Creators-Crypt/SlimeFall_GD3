using UnityEngine;

public class BossDoorLock : MonoBehaviour
{
    [SerializeField] private GameObject doorLock;

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activated)
            return;

        if(other.CompareTag("Player"))
        {
            activated = true;

            doorLock.SetActive(true); 
        }
    }
}
