using System.Runtime.CompilerServices;
using UnityEngine;

public class VoidFloor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("Player entered the void.");
    }
}
