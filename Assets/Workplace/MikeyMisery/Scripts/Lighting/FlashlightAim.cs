using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightAim : MonoBehaviour {

    [SerializeField] private InputAction flashlightSwitch;
    
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float aimDistance = 50f;

    [SerializeField] private Light flashlight;

    private bool isOn = false;

    private void OnEnable() { flashlightSwitch.Enable(); }
    private void OnDisable() { flashlightSwitch.Disable(); }
    private void LateUpdate() {

        if (playerCamera == null) return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 aimPoint = ray.GetPoint(aimDistance);
        Vector3 direction = aimPoint - transform.position;

        transform.rotation = Quaternion.LookRotation(direction);

        if (flashlightSwitch.WasPerformedThisFrame()) {

            isOn = !isOn;
            flashlight.enabled = isOn;

            if(flashlight.enabled)
            {
                GameManager.Instance.PlayerPerformAction("Flashlight");
            }
        }
    }
}
