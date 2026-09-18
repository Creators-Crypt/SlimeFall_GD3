using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class MummyAI : EnemyAI
{
    [Header("Stats")]
    public MummStatsSO mummyStats;

    [SerializeField] private List<Renderer> bodyRenderers = new List<Renderer>();
    [SerializeField] private List<Collider> bodyColliders = new List<Collider>();

    public MummyBandageState bandageState;
    public MummyQuicksandState quicksandState;
    public MummyBurrowState burrowState;

    public bool isBurrowed;

    private float nextQuicksandTime;
    private float nextBurrowTime;
    private bool initialized;
    private bool agentWasEnabled;
    private GameObject burrowMound;


    public override void Awake()
    {
        if (mummyStats == null || model == null)
        {
            Debug.LogError("Assign Mummy stats and model before enabling", this);
            enabled = false;
            return;
        }
        stats = mummyStats;

        base.Awake();

        if (bodyRenderers == null || bodyRenderers.Count == 0) 
            bodyRenderers = new List<Renderer>(GetComponentsInChildren<Renderer>(true));
        if (bodyColliders == null || bodyColliders.Count == 0)
            bodyColliders = new List<Collider>(GetComponentsInChildren<Collider>(true));

        bandageState = new MummyBandageState(this);
        quicksandState = new MummyQuicksandState(this);
        burrowState = new MummyBurrowState(this);

        nextQuicksandTime = Time.time + mummyStats.quicksandCooldow * .5f;
        nextBurrowTime = Time.time + mummyStats.burrowCooldown * .75f;

        initialized = true;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void  Start()
    {
        base.Start();
    }

    // Update is called once per frame
   public override void Update()
    {
        if (!initialized) return;
        base.Update();
    }

    public override void PreformAttack()
    {
        if (IsDead || initialized == false) return;
        
        lastAttackTime = Time.time;

        float dist = DistanceToPlayer();

        if(CanBurrow() && dist >= mummyStats.burrowMinDistance)
        {
            stateMachine.ChangeState(burrowState);
            return;
        }
        if (CanQuicksand())
        {
            stateMachine.ChangeState(quicksandState);
            return;
        }

        stateMachine.ChangeState(bandageState);
    }
    public bool CanQuicksand()
    {
        if(mummyStats.quicksandPoolPrefab == null)return false;

        return Time.time >= nextQuicksandTime;
    }
    public bool CanBurrow()
    {
        return Time.time >= nextBurrowTime;
    }
    public void MarkQuicksandUsed()
    {
        nextQuicksandTime = Time.time + mummyStats.burrowCooldown;
    }
    public void MarkBurrowUsed()
    {
        nextBurrowTime = Time.time + mummyStats.burrowCooldown;
    }
    public float DistanceToPlayer()
    {
        if (playerTarget == null)return Mathf.Infinity;
        return Vector3.Distance(transform.position, playerTarget.position);
    }
    public Vector3 GetPlayerAimPoint()
    {
        if (playerTarget == null) return transform.position + transform.forward;

        Collider playerCollider = playerTarget.GetComponentInChildren<Collider>();
        if (playerCollider != null) return playerCollider.bounds.center;
        return playerTarget.position + Vector3.up;
    }
    public void ReturnToChase()
    {
        if(playerTarget == null)
        {
            stateMachine.ChangeState(patrolState);
            return;
        }
        stateMachine.ChangeState(chaseState);
    }

    public void FireBandage()
    {
        if (playerTarget == null || mummyStats.bandagePrefab == null)
        {
            Debug.LogWarning("No Bandage Prefab assigned on the stats SO", this); return;
        }
            Vector3 origin = firePoint.position;
            Vector3 direction = GetPlayerAimPoint() - origin;

            direction.Normalize();

            GameObject spawned = Instantiate(mummyStats.bandagePrefab, origin, Quaternion.LookRotation(direction));

            MummyBandage bandage = spawned.GetComponent<MummyBandage>();

            if(bandage != null)
            {
                bandage.Launch(direction, transform, mummyStats, GetAttackMask(meleeFriendlyFire));
            }
            else
            {
                Debug.LogWarning("Bandage Prefab is missing",this);
                Destroy(spawned);
                return;
            }
            PlayVFXandSFX(mummyStats.bandageLaunch, origin);
         
    }
    public Vector3 PickQuicksandPoint()
    {
        if (playerTarget == null) return transform.position;
        Vector3 lead = playerVelocity;
        lead.y =0f; 

        Vector3 aim = playerTarget.position + lead * mummyStats.quicksandAimAhead;
        Vector3 ground = MummySlimeEffects.GroundPoint(aim, mummyStats.sandGroundMask);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(ground, out hit, 3f, NavMesh.AllAreas)) return hit.position;

        return ground;
    }

    public void SpawnQuicksandTelegraph(Vector3 _point)
    {
        if(mummyStats.quicksandTelegraphPrefab == null) return;

        GameObject marker = Instantiate(mummyStats.quicksandTelegraphPrefab,_point +Vector3.up *.05f,Quaternion.identity);
        
        BossTelegraph telegraph = marker.GetComponentInParent<BossTelegraph>();
        if (telegraph != null) telegraph.Play(mummyStats.quicksandRadius, mummyStats.quicksandWarningTime);
        else Destroy(marker,mummyStats.quicksandWarningTime);      
    }

    public void SpawnQuicksandPool(Vector3 _point)
    {
        if(mummyStats.quicksandPoolPrefab == null) return;

        GameObject spawned = Instantiate(mummyStats.quicksandPoolPrefab, _point, Quaternion.identity);
        

        MummyQuicksandPool pool = spawned.GetComponent<MummyQuicksandPool>();

        if (pool != null) pool.Play(mummyStats, GetAttackMask(mortarFriendlyFire));
        else
        {
            Debug.LogWarning("Quicksand Pool prefab is missing the MummyQuicksandPool script", this);
            Destroy(spawned);
            return;
        }

        PlayVFXandSFX(mummyStats.quicksandSpawn, _point);
        PlayVFXandSFX(mummyStats.quicksandPool, _point);
    }

    public Vector3 PickBurrowExit()
    {
        if (playerTarget == null) return transform.position;

        Vector3 forward = playerTarget.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 wanted = playerTarget.position - forward * mummyStats.burrowEmergeDistance;

        NavMeshHit hit;
        if(NavMesh.SamplePosition(wanted,out hit, 3f,NavMesh.AllAreas)) return hit.position;

        if(NavMesh.SamplePosition(playerTarget.position,out hit, mummyStats.burrowEmergeDistance +2, NavMesh.AllAreas)) return hit.position;
        return transform.position;
    }

    public void SpawnBurrowMound()
    {
        if (mummyStats.burrowMountPrefab == null || burrowMound != null) return;

        burrowMound = Instantiate(mummyStats.burrowMountPrefab, transform);
        burrowMound.transform.localPosition = new Vector3(0f,mummyStats.burrowMoundHeightOffset,0f);
        burrowMound.transform.localRotation = Quaternion.identity;
    }

    public void ClearBurrowMound()
    {
        if(burrowMound == null) return;
        Destroy(burrowMound); burrowMound = null; 
    }

    public void DoEmergeBurst(Vector3 _point)
    {
        PlayVFXandSFX(mummyStats.burrowUp,_point);

        Collider[] hits = Physics.OverlapSphere(_point, mummyStats.burrowEmergeRadius, GetAttackMask(meleeFriendlyFire));
        List<IDamageable> alreadyHit = new List<IDamageable>();

        foreach(Collider hit  in hits)
        {
            if(hit == null || hit.transform.IsChildOf(transform)) continue;

            IDamageable target = hit.GetComponent<IDamageable>();
            if(target != null)
            {
                if(alreadyHit.Contains(target)) continue;
                alreadyHit.Add(target);
            }
            MummySlimeEffects.DealDamage(hit,mummyStats.burrowEmergeDamage);

        }
    }
    public void SetBodyVisible(bool _visible)
    {
        foreach(Renderer body in bodyRenderers)
        {
            if(body != null)
            {
                body.enabled = _visible;
            }
        }
        foreach(Collider body in bodyColliders)
        {
            if (body != null)
            {
                body.enabled = _visible; 
            }
        }
    }

    public void SetBurrowed(bool _value)
    {
        isBurrowed = _value;
        SetBodyVisible(_value == false);
    }

    public void StopAgent()
    {
        if (agent == null || agent.isActiveAndEnabled == false || agent.isOnNavMesh == false) return;

        agent.isStopped = true;
        agent.ResetPath();
        agent.velocity = Vector3.zero;
    }

    public void SetAgentEnabled(bool _value)
    {
        if (agent == null) return;

        if(_value == false)
        {
            if (agent.enabled)
            {
                agentWasEnabled = true;
                StopAgent();
                agent.enabled = false;
            }
            return;
        }

        if (agentWasEnabled)
        {
            agent.enabled = true;
            agentWasEnabled = false;
        }
    }

    public void WarpTo(Vector3 _position)
    {
        if(agent!=null && agent.enabled)
        {
            NavMeshHit hit;
            if(NavMesh.SamplePosition(_position, out hit,3f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
                return;
            }
        }
        transform.position = _position;
    }

    public void QuickFace (Vector3 _position)
    {
        Vector3 direction = _position - transform.position;
        direction.y = 0f;

        transform.rotation = Quaternion.LookRotation(direction);
    }

    public override void OnDamage(float _amount)
    {
        if(initialized == false || IsDead) return;

        if (isBurrowed && mummyStats.burrowInvulnerable) return;

        base.OnDamage(_amount);
    }

    public override void Die()
    {
        if(IsDead) return;

        isBurrowed = false;
        ClearBurrowMound();
        SetBodyVisible(true);
        SetAgentEnabled(true);
        StopAgent();

        base.Die();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (initialized == false) return;

        isBurrowed = false;
        ClearBurrowMound();
        SetBodyVisible(true);
    }
    public new void OnDrawGizmosSelected()
    {
        if (stats == null) return;

        Vector3 origin = transform.position;
        if (firePoint != null)
        {
            origin = firePoint.position;
        }
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(origin, mummyStats.detectionRadius);

        Gizmos.color = Color.darkRed;
        Gizmos.DrawWireSphere(origin, mummyStats.attackRange);

        float halfAngle = mummyStats.detectionAngle / 2f;
        Vector3 left = Quaternion.AngleAxis(-halfAngle, Vector3.up) * transform.forward;
        Vector3 right = Quaternion.AngleAxis(halfAngle, Vector3.up) * transform.forward;

        Gizmos.color = Color.crimson;
        Gizmos.DrawRay(origin, left * mummyStats.detectionRadius);
        Gizmos.DrawRay(origin, right * mummyStats.detectionRadius);

    }

}
