using UnityEngine;

public class FlashlightAim : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float aimDistance = 50f;

    [SerializeField] private Light flashlight;

    private void LateUpdate()
    {
        if (playerCamera == null)
            return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 aimPoint = ray.GetPoint(aimDistance);
        Vector3 direction = aimPoint - transform.position;

        transform.rotation = Quaternion.LookRotation(direction);

        if (Input.GetKeyDown(KeyCode.F))
        {
            flashlight.enabled = !flashlight.enabled;

            if(flashlight.enabled)
            {
                GameManager.Instance.PlayerPerformAction("Flashlight");
            }
        }
    }
}
