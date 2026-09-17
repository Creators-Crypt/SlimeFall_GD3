using UnityEngine;

public class BuoyancyController : MonoBehaviour {

    [Header("Buoyancy Physics")]
    [Tooltip("How fast the player sinks naturally in the water")]
    public float baseSinkSpeed = 2f;

    [Tooltip("1 = Tier 1 Gear (Heavy Sink), 0.2 = Tier 2 Gear (Near weightless floating)")]
    public float currentBuoyancyModifier = 1f;

    private Rigidbody rb;

    private void Start() {
        
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
    }
    private void FixedUpdate() {

        // Apply a gentle, controlled downward or upward velocity to simulate water density
        float targetSinkSpeed = baseSinkSpeed * currentBuoyancyModifier;

        // Keep current X and Z movement, but override Y for water physics
        Vector3 currentVelocity = rb.linearVelocity;
        rb.linearVelocity = new Vector3(currentVelocity.x, -targetSinkSpeed, currentVelocity.z);
    }
    public void UpgradeToTierTwo() {

        currentBuoyancyModifier = 0.3f;
        Debug.Log("Equipped Tier 2 Gear: Buoyancy balanced.");
    }
}