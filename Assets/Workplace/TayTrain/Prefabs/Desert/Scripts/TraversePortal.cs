using UnityEngine;

public class TraversePortal : MonoBehaviour
{
    [Header("Destination")]
    [SerializeField] private Transform destination;

    private bool teleporting = false;

    private void OnTriggerEnter(Collider other)
    {
        if (teleporting)
            return;

        if (!other.CompareTag("Player"))
            return;

        if(destination == null)
        {
            Debug.LogWarning("TraversalPortal has no destination assigned.");
            return;
        }

        teleporting = true;

        CharacterController controller = other.GetComponent<CharacterController>();

        if (controller != null) 
        {
            controller.enabled = false;
        }

        other.transform.position = destination.position;
        other.transform.rotation = destination.rotation;

        if (controller != null)
        {
            controller.enabled = true;
        }

        Invoke(nameof(ResetTeleport), 0.5f);
    }

    private void ResetTeleport()
    {
        teleporting = false;
    }
}
