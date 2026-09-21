using UnityEngine;

public class VoidFloor : MonoBehaviour
{
    [SerializeField] private GameObject target;

    private void OnTriggerEnter(Collider other)
    {
        Transform player = other.transform;
        
        if (!other.CompareTag("Player")) return;

        if(player.TryGetComponent<CharacterController>(out var controller))
        {
            controller.enabled = false;
            player.SetPositionAndRotation(target.transform.position, target.transform.rotation);
            controller.enabled = true;
        }
        Debug.Log("Player entered the void.");
    }
}