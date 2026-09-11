using UnityEngine;

public class Respawn : MonoBehaviour {

    [SerializeField] private GameObject target;

    private void OnTriggerEnter(Collider other) {
        
        if (other.CompareTag("Player")) {

            other.transform.position = target.transform.position;
        } else {
            Destroy(other.gameObject);
        }
    }
}