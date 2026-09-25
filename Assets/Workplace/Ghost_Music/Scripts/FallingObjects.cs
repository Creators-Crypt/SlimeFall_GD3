using System;
using UnityEngine;

public class FallingObjects : MonoBehaviour
{
    [SerializeField] float fallSpeed = 15f;
    [SerializeField] int damage = 25;
    [SerializeField] float resetDelay = 2f;
    private float fallStartTime;
    private const float hitGracePeriod = 0.08f;
    [SerializeField] private float detectionDistance = 20f;

    Vector3 StartPos;
    Rigidbody rb;

    bool isFalling;
    bool hasHitPlayer;
    bool hasHitGround;

    void Start()
    {
        StartPos = transform.position;
        rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void Update()
    {
        if (!isFalling & !hasHitPlayer) {
            CheckForPlayer();
        }
        if (isFalling)
        {
            transform.Translate(fallSpeed * Time.deltaTime * Vector3.down, Space.World);
        }
    }

    private void CheckForPlayer() {

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, detectionDistance)) {
            if (hit.collider.CompareTag("Player")) {
                Debug.Log($"[Falling Object] '{gameObject.name}' spotted the player underneath! Dropping trap.");
                isFalling = true;
                hasHitGround = false;
                fallStartTime = Time.time;
            }
        }
    }
    void OnTriggerEnter(Collider other) {

        if (!other.CompareTag("Player")) return;
        if (hasHitPlayer) return;

        if ((Time.time - fallStartTime) < hitGracePeriod) return;

        hasHitPlayer = true;
        isFalling = false;

        if (other.TryGetComponent(out HealthSystem health)) {
            Debug.Log($"[Trap Object] Hit Player! HealthSystem found: {health != null}");
            health.OnDamage(damage);
            Debug.Log($"[Trap Object] Damaged the Player for {damage} points.");
            return;
        }
        if (!hasHitGround && !other.CompareTag("Player") && !other.CompareTag("MainCamera")) {
            Debug.Log($"[Falling Object] '{gameObject.name}' hit the ground structure: '{other.name}'. Freezing movement layout.");

            hasHitGround = true;
            isFalling = false;

            // Trigger the automated reset sequence now that it has fully completed its visual drop
            Invoke(nameof(ResetObject), resetDelay);
        }

    }
    void ResetObject()
    {
        transform.position = StartPos;

        isFalling = false;
        hasHitPlayer = false;
        hasHitGround = false;
    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * detectionDistance));
    }
}