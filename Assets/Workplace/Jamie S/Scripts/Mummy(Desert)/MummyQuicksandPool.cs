using UnityEngine;
using UnityEngine.VFX;
using System.Collections.Generic;

public class MummyQuicksandPool : MonoBehaviour
{
    public Transform visual;
    public float visualScale;

    private float radius;
    private float duration;
    private float tickInterval;
    private float damagePerTick;
    private float pullPerSecond;
    private float slowMultiplier;
    private LayerMask hitMask;
    private float endTime;
    private float nextTickTime;
    private bool playing;  
    // Update is called once per frame
    void Update()
    {
        if (playing == false) return;
        if(Time.time >= endTime)
        {
            playing = false;
            return;
        }

        Collider[] inside = Physics.OverlapSphere(transform.position, radius, hitMask);
        bool doDamage = Time.time >= nextTickTime;

        List<Transform> alreadyPulled = new List<Transform>();
        List<IDamageable> alreadyHurt = new List<IDamageable>();

        foreach(Collider hit in inside)
        {
            if(hit == null) continue;

            Transform root = hit.transform;

            if(alreadyPulled.Contains(root)==false)
            {
                alreadyPulled.Add(root);

                MummySlimeEffects.PullToward(root,transform.position,pullPerSecond *  Time.deltaTime);

                MummySlimeEffects.ApplySlow(root, slowMultiplier, tickInterval);
            }

            if (doDamage == false) continue;

            IDamageable target = hit.GetComponent<IDamageable>();
            
            if(target != null)
            {
                if (alreadyHurt.Contains(target)) continue;
                alreadyHurt.Add(target);
            }
            MummySlimeEffects.DealDamage(hit, damagePerTick);
        }
        if (doDamage)
        {
            nextTickTime = Time.time + tickInterval;
        }
    }

    public void Play(MummStatsSO _stats, LayerMask _hitMask)
    {
        radius =_stats.quicksandRadius;
        duration = _stats.quicksandDuration;
        tickInterval = _stats.quicksandTickInterval;
        damagePerTick = _stats.quicksandDamagePerTick;
        pullPerSecond = _stats.quicksandPullPerSecond;
        slowMultiplier = _stats.quicksandSlowMultiplier;
        hitMask = _hitMask;

        endTime = Time.time + duration;
        nextTickTime = Time.time + tickInterval;
        playing = true;

        if(visual != null)
        {
            float size = radius * visualScale;
            visual.localScale = new Vector3(size,visual.localScale.y,size);

        }

        Destroy(gameObject, duration);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.azure;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
