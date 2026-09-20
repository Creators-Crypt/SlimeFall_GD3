using UnityEngine;

public class IceStructure : MonoBehaviour {
    [Header("Melting Settings")]
    [Tooltip("How many seconds of continuous heat exposure required to melt the ice.")]
    [SerializeField] private float meltDuration = 3f;
    [SerializeField] private ParticleSystem meltParticles;

    [Header("Puzzle Rewards")]
    [Tooltip("The Warm Light GameObject that will be freed to descend.")]
    [SerializeField] private WarmLightFollower warmLight;
    [Tooltip("The destination marker where the light should move to after the ice melts.")]
    [SerializeField] private Transform lightNextStopMarker;

    private float meltTimer = 0f;
    private bool isMelted = false;

    public void ApplyHeat(float deltaTime) {
        
        if (isMelted) return;

        meltTimer += deltaTime;

        if (meltParticles != null && !meltParticles.isPlaying) { meltParticles.Play(); }
        if (meltTimer >= meltDuration) { MeltIce(); }
    }
    public void StopHeat() {
        if (meltParticles != null && meltParticles.isPlaying) { meltParticles.Stop(); }
    }
    private void MeltIce() {
        
        isMelted = true;
        Debug.Log("The ancient ice has melted!");

        // Free the light and command it to descend through the floor grate
        if (warmLight != null && lightNextStopMarker != null) {
            warmLight.DescendToWaypoint(lightNextStopMarker);
        }
        // Play a nice underwater ice-shattering effect or sound here
        Destroy(gameObject);
    }
}