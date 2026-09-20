using UnityEngine;

public class WarmLightFollower : MonoBehaviour {
    
    [Header("Movement Settings")]
    [SerializeField] private float travelSpeed = 3f;

    private Transform targetWaypoint;
    private bool isMoving = false;

    void Update() {
        
        if (!isMoving || targetWaypoint == null) return;

        // Smoothly interpolate the light down to the lower level marker
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, travelSpeed * Time.deltaTime);

        // Check if the light arrived at the lower level room
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f) {
            isMoving = false;
            targetWaypoint = null;
            Debug.Log("Warm Light has successfully descended to the next floor.");

            // Optional: Call a level manager to slide the entry doors to Room 2 open!
        }
    }
    public void DescendToWaypoint(Transform newWaypoint) {
        
        targetWaypoint = newWaypoint;
        isMoving = true;
    }
}
