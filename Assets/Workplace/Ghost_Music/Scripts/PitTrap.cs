using UnityEngine;

public class PitTrap : MonoBehaviour
{
    [SerializeField] private int damage = 100;
    [SerializeField] private Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        /*HealthSystem health = other.GetComponent<HealthSystem>(); 

        if(health != null)
        {

        }*/
        if (other.TryGetComponent(out HealthSystem health)) {
            health.OnDamage(damage);
        }

        if(respawnPoint != null)
        {
            CharacterController controller = other.GetComponent<CharacterController>();

            if (controller != null)
                controller.enabled = false;

            other.transform.position = respawnPoint.position;

            if (controller != null)
                controller.enabled = true; 
        }
    }
}
