using UnityEngine;

public class WorldIteraction : MonoBehaviour {

    [SerializeField] private GameObject target;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) target.SetActive(false);
    }
}