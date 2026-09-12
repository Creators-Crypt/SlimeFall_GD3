using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WaterFloat : MonoBehaviour {
    [Header("Buoyancy Settings")]
    public float waterLevel = 0f;
    public float floatThreshold = 2f;
    public float buoyancyForce = 15f;
    public float waterDrag = 1f;
    public float waterAngularDrag = 0.5f;

    [Header("Wave Settings (Match your Shader)")]
    public float waveSpeed = 1.5f;
    public float waveAmplitude = 0.3f;
    public float waveWavelength = 0.5f;

    private Rigidbody rb;

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {

        float waveOffset = Mathf.Sin(transform.position.x * waveWavelength + Time.time * waveSpeed) * waveAmplitude;
        float currentWaterHeight = waterLevel + waveOffset;


        if (transform.position.y < currentWaterHeight) {

            float displacementMultiplier = Mathf.Clamp01((currentWaterHeight - transform.position.y) / floatThreshold);


            Vector3 lift = buoyancyForce * displacementMultiplier * Vector3.up;
            rb.AddForce(lift, ForceMode.Acceleration);


            rb.linearDamping = waterDrag;
            rb.angularDamping = waterAngularDrag;
        } else {

            rb.linearDamping = 0f;
            rb.angularDamping = 0.05f;
        }
    }
}