using System.Runtime.CompilerServices;
using UnityEngine;

public class VoidFloor : MonoBehaviour
{
    [SerializeField] private GameObject target;

    private void OnTriggerEnter(Collider other)
    {
        Transform player = other.transform.root;
        
        if (!other.CompareTag("Player"))
            return;

        CharacterController controller = player.GetComponent<CharacterController>();

        if(controller != null)
        {
            controller.enabled = false;
        }

        player.position = target.transform.position;
        player.rotation = target.transform.rotation;

        if(controller != null)
        {
            controller.enabled = true;
        }

        Debug.Log("Player entered the void.");
    }
}
