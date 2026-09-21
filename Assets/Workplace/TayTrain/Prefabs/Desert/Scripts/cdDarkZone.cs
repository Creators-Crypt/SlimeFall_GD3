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
        if (playerInside || other.GetComponent<PlayerController>() == null)
            return;

        playerInside = true;

        if (other.TryGetComponent(out PlayerController player)) {

            FlashlightAim flashlightAim = player.GetComponentInChildren<FlashlightAim>(true);
            if (flashlightAim != null) {
                flashlight = flashlightAim.gameObject;
                flashlightWasActive = flashlight.activeSelf;
                flashlight.SetActive(false);
            }

            player.GetConcentrationObject.TryGetComponent(out concentrationLight);

            normalIntensity = concentrationLight.intensity;
            normalRange = concentrationLight.range;

            concentrationLight.intensity = dungeonIntensity;
            concentrationLight.range = dungeonRange;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!playerInside || other.GetComponentInParent<PlayerController>() == null)
            return;

        playerInside = false;

        if (flashlight != null) {
            flashlight.SetActive(flashlightWasActive);
            flashlight = null;
        }

        if(concentrationLight != null)
        {
            concentrationLight.intensity = normalIntensity;
            concentrationLight.range = normalRange;
            concentrationLight = null;
        }
    }
}