using UnityEngine;
using System.Collections.Generic;
using System.Drawing;


public class BossMortarProjectile : Projectile
{
    [Header("Effects")]
    public GameObject explosionVfxPrefab;
    public AudioClip explosionSound;
    public TrailRenderer trail;

    [Header("Mortar")]
    [SerializeField] public float arcHeight = 6f;
    [SerializeField] public float splachRadius = 4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask damagebleMask;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float totalFlatDist;
    private float travelled;
    private bool launched;
    private bool exploded;
    // Update is called once per frame
    public override void Update()
    {
        if (launched == false) return;
        if (exploded) return;

        travelled = travelled + speed * Time.deltaTime;

        float t = 1f;
        if(totalFlatDist > 0.01f)
        {
            t = travelled / totalFlatDist;
        }
        if (t > 1f) t = 1f;

        Vector3 nextPos = Vector3.Lerp(startPos, targetPos, t);

        nextPos.y = nextPos.y + arcHeight * 4f * t *(1f - t);

        RaycastHit hit;
        if (Physics.Linecast(transform.position, nextPos, out hit, groundMask)) 
        {
            Explode(hit.point);
            return;
        }

        transform.position = nextPos;
        if(t >= 1f)
        {
            Explode(transform.position);
        }
        
    }

    public void Launch(Vector3 _target, float _flySpeed, float _height, float _damageAmount, float _splash, LayerMask _ground, LayerMask __targetLayers)
    {
        startPos = transform.position;
        targetPos = _target;
        speed = _flySpeed;
        damage = _damageAmount;
        arcHeight = _height;
        splachRadius = _splash;
        groundMask = _ground;
        damagebleMask = __targetLayers;

        direction = Vector3.zero;

        Vector3 flat = targetPos - startPos;
        flat.y = 0;
        totalFlatDist = flat.magnitude;

        travelled = 0;
        launched = true;
    }

    private void Explode(Vector3 _point)
    {
        if (exploded) return;
        exploded = true;

        Collider[] hits = Physics.OverlapSphere(_point, splachRadius, damagebleMask);

        List<IDamageable> alreadyHit = new List<IDamageable>();

        foreach (Collider hit in hits)
        {
            if (hit == null) continue;

            IDamageable target = hit.GetComponent<IDamageable>();
            if (target == null) continue;
            if (alreadyHit.Contains(target)) continue;

            alreadyHit.Add(target);

            float distance = Vector3.Distance(hit.ClosestPoint(_point), _point);
            float closeness = 1f - (distance / splachRadius);
            closeness = Mathf.Clamp01(closeness);

            float finalDamage = damage * Mathf.Lerp(0.4f, 1f, closeness);

            target.OnDamage(finalDamage);
        }

        if (explosionVfxPrefab != null)
        {
            GameObject vfx = Instantiate(explosionVfxPrefab, _point, Quaternion.identity);
            Destroy(vfx, 5f);
        }

        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, _point);
        }

        if (trail != null)
        {
            trail.transform.SetParent(null);
            Destroy(trail.gameObject, trail.time + .1f);
        }

        Destroy(gameObject);
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (exploded) return;
        Explode(other.ClosestPoint(transform.position));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.violetRed;
        Gizmos.DrawWireSphere(transform.position, splachRadius);
    }
}
