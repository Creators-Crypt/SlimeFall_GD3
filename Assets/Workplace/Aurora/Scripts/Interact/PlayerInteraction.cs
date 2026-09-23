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
    [SerializeField, Range(3f, 50f)] private float maxCameraAimDistance = 10f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask exclusionLayer;
    [SerializeField] private InputAction interact;

    [Tooltip("How many seconds the button must be held down to trigger a swap interaction.")]
    [SerializeField] private float holdDurationThreshold = 0.4f;
    private float holdTimer = 0f;
    private bool isHoldingInteraction = false;

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

        LayerMask cameraMask = interactableLayer | exclusionLayer;

        if (Physics.Raycast(ray, out RaycastHit camerahit, maxCameraAimDistance, cameraMask, QueryTriggerInteraction.Ignore)) {
            targetPoint = camerahit.point;
        } else {
            targetPoint = ray.GetPoint(maxCameraAimDistance);
        }

        Vector3 interactionDirection = (targetPoint - playerOrigin.position).normalized;

        if (Physics.Raycast(playerOrigin.position, interactionDirection, out RaycastHit hit, interactionRange, cameraMask)) {

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
        if (current != null) {
            if (interact.WasPressedThisFrame()) {
                isHoldingInteraction = true;
                holdTimer = 0f;
            }
            if (isHoldingInteraction) {
                if (interact.IsPressed()) {
                    holdTimer += Time.deltaTime;

                    if (holdTimer >= holdDurationThreshold) {
                        isHoldingInteraction = false;
                        current.Interact();
                        ClearCurrentTarget();
                    }
                } else if (interact.WasReleasedThisFrame()) {
                    isHoldingInteraction = false;
                    current.Interact();
                    ClearCurrentTarget();
                }
            }
        } else { isHoldingInteraction = false; }
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