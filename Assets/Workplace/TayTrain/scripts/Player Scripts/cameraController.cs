using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Camera Collision")]
    [SerializeField] float cameraCollisionOffset = 0.2f;
    [SerializeField] float cameraRayStartOffset = 0.75f;
    [SerializeField] LayerMask ignoreCameraLayer;

    [Header("Camera Controlls")]
    [SerializeField] int sens;
    [SerializeField] int lockVertMin, lockVertMax; // stops over rotate

    [Header("References")]
    [SerializeField] Transform player;
    [SerializeField] Transform cameraTarget;
    [SerializeField] float playerRotationSpeed = 10f;

    float camRotX; // camera x axis rotation
    float camRotY;
    Vector3 cameraOriginalLocalPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() 
    {
        cameraOriginalLocalPosition = transform.localPosition;
        //control the start view of the camera
        camRotX = cameraTarget.localEulerAngles.x;
        camRotY = cameraTarget.localEulerAngles.y;
        if (camRotX > 180) {
            camRotX -= 360f;
        }

        if(camRotY > 180)
        {
            camRotY -= 360f;
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update() {

        //only need x and y because mouse only moves in two directions
        float mouseX = Input.GetAxisRaw("Mouse X") * sens;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sens;

        camRotY += mouseX;
        camRotX -= mouseY;
        camRotX = Mathf.Clamp(camRotX, lockVertMin, lockVertMax);

        cameraTarget.localRotation = Quaternion.Euler(camRotX, camRotY, 0f);

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            float turnAmount = camRotY * playerRotationSpeed * Time.deltaTime;

            player.Rotate(Vector3.up * turnAmount);

            camRotY -= turnAmount;
        }
        cameraTarget.localRotation = Quaternion.Euler(camRotX, camRotY, 0f);
        cameraCollision();
    }

    private void LateUpdate()
    {
        ////  only need x and y because mouse only moves in two directions
        //float mouseX = Input.GetAxisRaw("Mouse X") * sens;
        //float mouseY = Input.GetAxisRaw("Mouse Y") * sens;

        ////Left / right
        //player.Rotate(Vector3.up * mouseX);
        //// Up / Down
        //camRotX -= mouseY;
        //// need to clamp the rotation max so we dont go to far down or back
        //camRotX = Mathf.Clamp(camRotX, lockVertMin, lockVertMax);

        //cameraTarget.localRotation = Quaternion.Euler(camRotX, 0f, 0f);
        
        //cameraCollision();
    }

    void cameraCollision()
    {
        
        Vector3 directionToCamera = cameraOriginalLocalPosition.normalized;

        float distanceToCamera = cameraOriginalLocalPosition.magnitude;

        Vector3 worldDirection = cameraTarget.TransformDirection(directionToCamera);

        Vector3 rayStart = cameraTarget.position + worldDirection * cameraRayStartOffset;

        RaycastHit hit;


        if(Physics.Raycast(rayStart, worldDirection, out hit, distanceToCamera, ~ignoreCameraLayer))
        {
            Vector3 collisionPosition = hit.point - worldDirection * cameraCollisionOffset;

            transform.position = Vector3.Lerp(transform.position, collisionPosition, 10f * Time.deltaTime);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, cameraOriginalLocalPosition, 10f * Time.deltaTime);
        }
    }
}

