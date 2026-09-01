using UnityEngine;

public class EquipmentPickup : MonoBehaviour
{
    [SerializeField] EquipmentData equipment;
    [SerializeField] MeshRenderer modelRenderer;

    [Header("Swap Drop")]
    [SerializeField] float dropForce = 6f;
    [SerializeField] float upwardForce = 5f;
    [SerializeField] float pickupCooldown = 0.5f;

    Rigidbody rb;
    bool canPickup = true;
    float pickupTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        UpdateVisual();
    }

    private void Update()
    {
        if(!canPickup)
        {
            pickupTimer -= Time.deltaTime;

            if(pickupTimer <= 0 )
            {
                canPickup = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canPickup)
            return;

        IEquipmentPickup pickup = other.GetComponentInParent<IEquipmentPickup>();

        if (pickup != null)
        {
            EquipmentData oldEquipment = pickup.GetEquipment(equipment);

            if (oldEquipment != null)
            {
                equipment = oldEquipment;

                UpdateVisual();

                canPickup = false;
                pickupTimer = pickupCooldown;

                //To make the old object fall back to the ground
                rb.isKinematic = false;
                rb.useGravity = true;

                //Reset the old movement to throw after every swap
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                Debug.Log("Throwing old Equipment");

                rb.AddForce(other.transform.forward * dropForce + Vector3.up * upwardForce, ForceMode.Impulse);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
    public string GetEquipmentName()
    {
        return equipment.itemName;
    }
  
    void UpdateVisual()
    {
        if(modelRenderer != null && equipment != null && equipment.pickupMaterial != null)
        {
            modelRenderer.material = equipment.pickupMaterial;
        }
    }
}
