using UnityEngine;

public class cdDarkZone : MonoBehaviour
{

    [Header("Player References")]
    [SerializeField] private GameObject flashlight;
    [SerializeField] private Light concentrationLight;

    [Header("Dungeon Light Settings")]
    [SerializeField] private float dungeonIntensity = 3000f;
    [SerializeField] private float dungeonRange = 50f;

    private float normalIntensity;
    private float normalRange;
    private bool flashlightWasActive;
    private bool playerInside;

    private void Start()
    {
        if(concentrationLight != null)
        {
            normalIntensity = concentrationLight.intensity;
            normalRange = concentrationLight.range;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (playerInside || other.GetComponentInParent<PlayerController>() == null)
            return;

        playerInside = true;

        if(flashlight != null)
        {
            flashlightWasActive = flashlight.activeSelf;
            flashlight.SetActive(false);
        }

        if(concentrationLight != null)
        {
            concentrationLight.intensity = dungeonIntensity;
            concentrationLight.range = dungeonRange;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!playerInside || other.GetComponentInParent<PlayerController>() == null)
            return;

        playerInside = false;

        if (flashlight != null)
            flashlight.SetActive(flashlightWasActive);

        if(concentrationLight != null)
        {
            concentrationLight.intensity = normalIntensity;
            concentrationLight.range = normalRange;
        }
    }

}
