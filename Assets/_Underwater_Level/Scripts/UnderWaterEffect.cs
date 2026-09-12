using UnityEngine;

public class UnderWaterEffect : MonoBehaviour {
    [Header("Ocean Surface Settings")]
    public float baseWaterLevel = 0f;
    public float waveSpeed = 1.5f;
    public float waveAmplitude = 0.3f;
    public float waveWavelength = 0.5f;

    [Header("Camera Settings")]
    private Camera mainCamera;
    private int defaultMask;
    private int underwaterMask;

    void Start() {
        mainCamera = GetComponent<Camera>();

        defaultMask = mainCamera.cullingMask;

        underwaterMask = defaultMask | (1 << LayerMask.NameToLayer("Underwater"));
    }

    void Update() {

        float currentWaveHeight = baseWaterLevel +
            (Mathf.Sin(transform.position.x * waveWavelength + Time.time * waveSpeed) * waveAmplitude);

        if (transform.position.y < currentWaveHeight) {

            mainCamera.cullingMask = underwaterMask;

            mainCamera.backgroundColor = new Color(0.05f, 0.15f, 0.25f, 1f);
            mainCamera.clearFlags = CameraClearFlags.Color;
        } else {

            mainCamera.cullingMask = defaultMask;
            mainCamera.clearFlags = CameraClearFlags.Skybox;
        }
    }
}