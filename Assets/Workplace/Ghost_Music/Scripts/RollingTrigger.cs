using UnityEngine;

public class RollingTrigger : MonoBehaviour
{
    [SerializeField] FallingRocks rocks;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            rocks.startRolling(); 
        }
    }
}
