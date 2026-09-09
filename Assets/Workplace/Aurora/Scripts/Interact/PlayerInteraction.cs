using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour {

    public static Action OnHide;
    public static Action<string> OnInteract;

    [Header("References")]
    [SerializeField] private Transform playerOrigin;

    [Header("Settings")]
    [SerializeField, Range(1.5f, 5f)] private float interactionRange = 3f;
    [SerializeField] private float maxCameraAimDistance = 100f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask exclusionLayer;
    [SerializeField] private InputAction interact;

    [SerializeField] private IInteractable current;
    private Collider lastCheckedCollider;

    private void OnEnable() => interact.Enable();
    private void OnDisable() => interact.Disable();

    private void Start() {

        if (playerOrigin == null) playerOrigin = this.transform;
    }
    private void Update() {

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit camerahit, maxCameraAimDistance, ~exclusionLayer, QueryTriggerInteraction.Ignore)) {
            targetPoint = camerahit.point;
        } else {
            targetPoint = ray.GetPoint(maxCameraAimDistance);
        }

        Vector3 interactionDirection = (targetPoint - playerOrigin.position).normalized;
        LayerMask combinedMask = interactableLayer & ~exclusionLayer;

        if (Physics.Raycast(playerOrigin.position, interactionDirection, out RaycastHit hit, interactableLayer, combinedMask)) {

            if (hit.collider != lastCheckedCollider) {

                lastCheckedCollider = hit.collider;

                if (hit.collider.TryGetComponent<IInteractable>(out var interactable)) {

                    current = interactable;
                    OnInteract?.Invoke(current.InteractionPrompt);
                } else { ClearCurrentTarget(); }
            }
        } else {

            if (lastCheckedCollider != null) ClearCurrentTarget();
        }
        if (interact.WasPressedThisFrame() && current != null) {

            current.Interact();

            ClearCurrentTarget();
        }
    }
    private void ClearCurrentTarget() {
        current = null;
        lastCheckedCollider = null;
        OnHide?.Invoke();
    }
    private void OnDrawGizmos() {
        if (Camera.main == null || playerOrigin == null) return;
        // Visualizes Step 1 (Camera Aiming)
        Ray cameraRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(cameraRay.origin, cameraRay.direction * 15f);

        // Visualizes Step 3 (True Player Interaction Range)
        Vector3 targetPoint = Physics.Raycast(cameraRay, out RaycastHit cameraHit, maxCameraAimDistance, ~exclusionLayer)
            ? cameraHit.point
            : cameraRay.GetPoint(maxCameraAimDistance);

        Vector3 interactionDirection = (targetPoint - playerOrigin.position).normalized;

        Gizmos.color = current != null ? Color.green : Color.red;
        Gizmos.DrawRay(playerOrigin.position, interactionDirection * interactionRange);
    }
}