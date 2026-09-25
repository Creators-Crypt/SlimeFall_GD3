using UnityEngine;

public class Respawn : MonoBehaviour {

    [SerializeField] private GameObject target;

    private void OnTriggerEnter(Collider other) {
        
        if (other.CompareTag("Player")) {

            if (other.TryGetComponent(out CharacterController character)) {

                character.enabled = false;

                if (other.TryGetComponent(out PlayerController player)) {
                    player.ResetVelocity();
                }
                other.transform.position = target.transform.position;

                character.enabled = true;
            } else {
                other.transform.position = target.transform.position;
            }
            Physics.SyncTransforms();
        } else {
            Destroy(other.gameObject);
        }
    }
}