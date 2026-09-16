using UnityEngine;

public class MummyBandage : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float damage;
    private float pullDistance;
    private float slowMultiplier;
    private float slowDuration;
    private float vfxLivetime;
    private float hitRad = .35f;
    private bool launched;
    private bool spent;
    private GameObject onPlayerVFX;
    private AttackFeedback hitFeedback;
    private LayerMask hitMask;
    private LayerMask obstacleMask;
    private Transform puller;     

    // Update is called once per frame
    void Update()
    {
        if(launched == false || spent)
        {
            return;
        }

        float step = speed * Time.deltaTime;
        Vector3 start = transform.position;
        Vector3 next = start + direction *step;

        if(Physics.Raycast(start, direction, step + hitRad, obstacleMask))
        {
            spent = true;
            if(hitFeedback != null)
            {
                hitFeedback.Play(transform.position);
            }
            Destroy(gameObject);
            return;
        }

        Collider[] hits = Physics.OverlapCapsule(start, next, hitRad, hitMask);
        foreach(Collider hit in hits)
        {
            if(hit == null) continue;
            if (puller != null && hit.transform.IsChildOf(puller)) continue;
            Strike(hit);
            return;
        }
        transform.position = next;
        transform.rotation = Quaternion.LookRotation(direction);
    }
    public void Launch(Vector3 _direction, Transform _puller, MummStatsSO _stats, LayerMask _hitMask)
    {
        direction = _direction.normalized;
        puller = _puller;
        hitMask = _hitMask;
        obstacleMask = _stats.obstacleMask;

        speed = _stats.bandageSpeed;
        damage = _stats.bandageDamage;
        pullDistance = _stats.bandagePullDist;
        slowMultiplier = _stats.bandageSlowMultiplier;
        slowDuration = _stats.bandageSlowDuration;
        onPlayerVFX = _stats.bandageOnPlayerVFX;
        vfxLivetime = _stats.bandageVfxLifetime;
        hitFeedback = _stats.bandageHit;

        launched = true;
        Destroy(gameObject, _stats.bandageLifetime);
    }
    private void Strike(Collider _hit)
    {
        spent = true;
        bool connected = MummySlimeEffects.DealDamage(_hit, damage);

        MummySlimeEffects.ApplySlow(_hit.transform, slowMultiplier, slowDuration);
        MummySlimeEffects.AttachBandageVFX(_hit.transform, onPlayerVFX,vfxLivetime);

        if(puller != null)
        {
            MummySlimeEffects.PullToward(_hit.transform, puller.position, pullDistance);
        }
        if(hitFeedback != null)
        {
            hitFeedback.Play(transform.position);
        }
        if(connected == false)
        {
            Debug.Log("the attack did not find healthsystem or IDamageable on the target");
        }
        Destroy(gameObject);
    }
}
