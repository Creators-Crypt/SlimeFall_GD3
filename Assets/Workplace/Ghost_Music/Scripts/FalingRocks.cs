using UnityEngine;

public class FallingRocks : MonoBehaviour
{
    [Header("Boulder Settings")]
    [SerializeField] float rollForce = 15f;
    [SerializeField] int damage = 25;
    [SerializeField] float resetDelay = 3f;

    Vector3 startPos;
    Quaternion startRot;

    Rigidbody rb;

    bool isRolling;
    bool hasHitPlayer;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;

        rb = GetComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false; 
    }

    void Update()
    {
        // controlled by physics
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!isRolling && other.CompareTag("Player"))
        {
            startRolling(); 
        }
    }

    public void startRolling()
    {
        isRolling = true;

        rb.isKinematic = false;
        rb.useGravity = true;

        rb.AddForce(transform.forward * rollForce, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(!hasHitPlayer && collision.gameObject.CompareTag("Player"))
        {
            hasHitPlayer = true;

            HealthSystem healthSystem = collision.gameObject.GetComponent<HealthSystem>();

            if(healthSystem != null)
            {
                healthSystem.OnDamage(damage); 
            }

            Invoke(nameof(ResetBoulder), resetDelay);
        }
    }

    void ResetBoulder()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;
        rb.useGravity = false;

        transform.position = startPos;
        transform.rotation = startRot;

        isRolling = false;
        hasHitPlayer = false; 
    }
}
