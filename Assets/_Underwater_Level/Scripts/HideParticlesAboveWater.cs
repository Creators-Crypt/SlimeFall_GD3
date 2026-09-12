using UnityEngine;
using UnityEngine.VFX;

public class HideParticlesAboveWater : MonoBehaviour {
    private VisualEffect vfx;
    public float waterLevel = 0f;

    void Start() {
        vfx = GetComponent<VisualEffect>();
    }

    void Update() {
        // Dynamically pass the water height threshold to the VFX graph
        // (Ensure you create a Float property named "MaxHeight" in your VFX graph blackboard)
        if (vfx != null) {
            vfx.SetFloat("MaxHeight", waterLevel);
        }
    }
}