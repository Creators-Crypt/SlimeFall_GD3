using UnityEngine;

/// <summary>
/// 
/// Ensures the GameObject always faces the main camera, creating a billboard effect.
/// 
/// </summary>
public class Billboard : MonoBehaviour {

    private Transform cameraTransform;

    private void Start() => cameraTransform = Camera.main.transform;
    private void LateUpdate() { transform.LookAt(transform.position + cameraTransform.forward); }
}