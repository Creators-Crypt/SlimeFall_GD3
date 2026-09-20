using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class DesNarTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    [Tooltip("These lines will play in order.")]
    [SerializeField] private List<DesNarLine> lines = new();

    [Header("Settings")]
    [SerializeField] private bool triggerOnlyOnce = true;

    private bool hasTriggered;

    private void Reset()
    {
        Collider triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (triggerOnlyOnce && hasTriggered)
            return;

        if(DesNarManager.Instance == null)
        { Debug.LogWarning("No Desert narrator manager found.");
            return;
        }

        hasTriggered = true;

        foreach(DesNarLine line in lines)
        {
            DesNarManager.Instance.PlayLine(line);
        }
    }
}
