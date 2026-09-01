using UnityEngine;


/// <summary>
/// 
/// Generic spell projectile. Works for straight shots gravity = false
/// and arcing shots gravity = true. Prefab needs: Rigidbody & Collider
/// 
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class SpellProjectile : MonoBehaviour {

    [Tooltip("Optional VFX spawned on impact.")]
    [SerializeField] private GameObject impactVfxPrefab;
    [Tooltip("Destroy on the first thing hit, or pierce through everything.")]
    [SerializeField] private bool destroyOnHit = true;

    private Rigidbody rb;
    private LayerMask targetLayers;
    private float baseDamage;
    private float calculatedDamage;
    private float custonGravityScale = 1f;
    private bool useCustomGravity = false;

    private GameObject instantiatedTrail;

    private void Awake() {

        rb = GetComponent<Rigidbody>();
        // start the rb with dynamic
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    /// <summary>Called by the delivery strategy right after Instantiate.</summary>
    public void Launch(SpellData spellData, SpellElement spellElement, Vector3 direction , float damageMultiplier) {

        var vfxSettings = SpellFactory.GetVFX(spellElement);
        
        targetLayers = spellData.hitLayers;
        baseDamage = spellData.damage;
        calculatedDamage = damageMultiplier;
        useCustomGravity = false;
        rb.useGravity = false;

        if (vfxSettings.impactVfxOverride != null) { impactVfxPrefab = vfxSettings.impactVfxOverride; } 
        else { impactVfxPrefab = spellData.impactVfxPrefab; }

        if (TryGetComponent(out Renderer renderer)) {
            renderer.material.SetColor("_Color", vfxSettings.primaryColor);
            renderer.material.SetColor("_EmissionColor", vfxSettings.hdrGlowColor);
        }
        if (vfxSettings.projectileTrailPrefab != null) {
            instantiatedTrail = Instantiate(vfxSettings.projectileTrailPrefab, transform);
            instantiatedTrail.transform.localPosition = Vector3.zero;
        }

        rb.linearVelocity = direction.normalized * spellData.projectileSpeed;
        // Face travel direction; arcing shots keep re-facing in Update.
        FaceVelocity(rb.linearVelocity);
        Destroy(gameObject, spellData.projectileLifetime);
    }
    public void Launch(SpellData spellData, SpellElement spellElement, Vector3 direction, bool gravityState, float damageMultiplier) { 
        
        targetLayers = spellData.hitLayers;
        calculatedDamage = damageMultiplier;

        if (gravityState) {
            
            useCustomGravity = true;
            rb.useGravity = false;
            custonGravityScale = spellData.arcGravityScale;
        }
        rb.linearVelocity = direction.normalized * spellData.projectileSpeed;
        // Face travel direction; arcing shots keep re-facing in Update.
        FaceVelocity(rb.linearVelocity);
        Destroy(gameObject, spellData.projectileLifetime);
    }
    private void FixedUpdate() {

        if (useCustomGravity) {
            rb.linearVelocity += custonGravityScale * Time.fixedDeltaTime * Physics.gravity;
        }
        if (rb.useGravity && rb.linearVelocity.sqrMagnitude > 0.001f) FaceVelocity(rb.linearVelocity);
    }
    private void FaceVelocity(Vector3 velocity) {

        if (velocity.sqrMagnitude > 0.001f) {
            Quaternion targetRotation = Quaternion.LookRotation(velocity, Vector3.up);
            rb.MoveRotation(targetRotation);
        }
    }

    private void OnTriggerEnter(Collider other) {

        // Only react to layers this spell is allowed to hit.
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0) return;

        if (other.TryGetComponent(out IDamageable dmg)) {
            float totalDamage = baseDamage * calculatedDamage;
            dmg.OnDamage(totalDamage);
        }

        if (impactVfxPrefab != null)
        {
            var vfx = Instantiate(impactVfxPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, 2f);
        }
        if (destroyOnHit) Destroy(gameObject);
    }
}