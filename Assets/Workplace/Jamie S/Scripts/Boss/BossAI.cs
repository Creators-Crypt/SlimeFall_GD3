using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;


public enum BossPhase
{
    Phase1,
    Transition,
    Phase2,
    Stunned,
    Phase3,
    Dead
}


public class BossAI : EnemyAI
{
    public Transform mortarFirePoint;

    [Header("Layers")]
    public LayerMask groundMask = ~0;
    public LayerMask damageableMask = ~0; //Player Layer
    public LayerMask otherEnemyMask = 0;

    [Header("Friendly Fire - hurt other slimes")]
    public bool mortarFriendlyFire = true;
    public bool landingShockFriendlyFire = false;
    public bool meleeFriendlyFire = false;
    public bool aoeWaveFriendlyFire = false;
    public bool detonationFriendlyFire = true;

    [Header("Info/Stats")]
    public BossStatsSO bossStats;
    public BossPhase currentPhase = BossPhase.Phase1;
    public bool isInvulnerable = false;
    public bool isStunned = false;
    public float stunTimeleft = 0f;
    public float detonationTimeLeft = 0f;
    public int eyeHitCount = 0;

    public BossPhase1State phase1State;
    public BossPhase2State phase2State;
    public BossPhase3State phase3State;
    public BossStunState stunState;
    public BossTransitionState transitionState;



    public Vector3 playerVelocity;

    private Vector3 lastPlayerPostion;

    private Color currentColor;
    private Color targetColor;
    [SerializeField] private float colorBlendSpeed;
    private bool isBlendingColor = false;

    [SerializeField] private float eyeHitExpireTime = 0f;
    [SerializeField] private float stunLockout = 0f;


    public override void Awake()
    {
        if (bossStats == null)
        {
            Debug.LogError("The boss needs his Scripted Object dropped into the field");
            enabled = false;
            return;
        }

        stats = bossStats;

        base.Awake();

        if (mortarFirePoint == null) mortarFirePoint = firePoint;

        SetPhaseColor(bossStats.phase1Material);

        phase1State = new BossPhase1State(this);
        phase2State = new BossPhase2State(this);
        phase3State = new BossPhase3State(this);
        transitionState = new BossTransitionState(this);
        stunState = new BossStunState(this);

        BossDamageZone[] zones = GetComponentsInChildren<BossDamageZone>();
        foreach (BossDamageZone zone in zones)
        {
            zone.SetBoss(this);
        }
    }

    public override void Start()
    {
        stateMachine.Initialize(phase1State);
        currentPhase = BossPhase.Phase1;
    }

    public override void Update()
    {
        MeasurePlayerSpeed();
        ClearEyeHIts();

        base.Update();

    }

    private void MeasurePlayerSpeed()
    {
        if (playerTarget == null)
        {
            playerVelocity = Vector3.zero;
            return;
        }

        if (Time.deltaTime > 0f)
        {
            Vector3 movedThisFrame = playerTarget.position - lastPlayerPostion;
            Vector3 speed = movedThisFrame / Time.deltaTime;

            playerVelocity = Vector3.Lerp(playerVelocity, speed, .25f);
        }
        lastPlayerPostion = playerTarget.position;
    }

    private void StartStun()
    {
        if (currentPhase != BossPhase.Phase2) return;

        currentPhase = BossPhase.Stunned;
        isStunned = true;
        stateMachine.ChangeState(stunState);
    }

    public void EndStun()
    {
        isStunned = false;
        stunTimeleft = 0f;
        stunLockout = Time.time + bossStats.stunCooldown;
        eyeHitCount = 0;

        if (GetHealthPercent() <= bossStats.phase2HealthPercent)
        {
            StartTransition(BossPhase.Phase3);
            return;
        }

        currentPhase = BossPhase.Phase2;
        stateMachine.ChangeState(phase2State);
    }

    public void SetPhaseColor(Material _phaseMaterial)
    {
        if (_phaseMaterial == null)
        {
            Debug.LogWarning("Check the Phase Material Slots and make sure they are filled");
            return;
        }

        model.material = _phaseMaterial;

        currentColor = model.material.color;
    }

    public void WarpToNavMesh(Vector3 _position)
    {
        if (agent == null) return;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(_position, out hit, 4f, NavMesh.AllAreas))
        {
            agent.enabled = true;
            agent.Warp(hit.position);
        }
        else
        {

            agent.enabled = true;
        }
    }
    public void StartTransition(BossPhase _nextPhase)
    {
        if (currentPhase == BossPhase.Transition) return;
        if (currentPhase == BossPhase.Dead) return;

        isStunned = false;
        stunTimeleft = 0f;

        transitionState.targetPhase = _nextPhase;
        currentPhase = BossPhase.Transition;
        stateMachine.ChangeState(transitionState);

    }

    public void FinishTransition(BossPhase _nextPhase)
    {
        currentPhase = _nextPhase;

        switch (_nextPhase)
        {
            case BossPhase.Phase2:
                stateMachine.ChangeState(phase2State);
                break;
            case BossPhase.Phase3:
                stateMachine.ChangeState(phase3State);
                break;
            default:
                stateMachine.ChangeState(phase1State);
                break;
        }
    }

    public void MoveTo(Vector3 _destination)
    {
        if (agent == null) return;
        if (agent.enabled == false) return;
        if (agent.isOnNavMesh == false) return;

        agent.SetDestination(_destination);

    }
    public void SetMovementEnabled(bool _canMove)
    {
        if (agent == null) return;
        if (agent.enabled == false) return;
        if (agent.isOnNavMesh == false) return;

        agent.isStopped = !_canMove;

        if (_canMove == false)
        {
            agent.velocity = Vector3.zero;
            agent.ResetPath();
        }

    }

    public override void OnDamage(float amount)
    {
        ApplyZoneDamage(amount, 1f, false);
    }

    public void ApplyZoneDamage(float _amount, float _zoneMultiplier, bool _isWeakPoint)
    {
        if (currentPhase == BossPhase.Dead) return;

        if (isInvulnerable) return;
        float finalDamage = _amount * _zoneMultiplier;

        if (isStunned)
        {
            finalDamage = finalDamage * bossStats.stunDamageMultiplier;
        }

        float bottom = GetPhaseHealthBottom();
        currentHealth = Mathf.Max(currentHealth - finalDamage, bottom);

        StartCoroutine(FlashRed());

        if (_isWeakPoint)
        {
            RegisterEyeHits();
        }
        CheckPhaseChange();
    }



    public void DealRadialDamage(Vector3 _center, float _radius, float _damage, LayerMask _whoCanBeHit)
    {
        Collider[] hits = Physics.OverlapSphere(_center, _radius, _whoCanBeHit);

        List<IDamageable> alreadyHit = new List<IDamageable>();

        foreach (Collider hit in hits)
        {
            if (hit == null) continue;

            if (hit.transform.IsChildOf(transform)) continue;

            IDamageable target = hit.GetComponent<IDamageable>();
            if (target == null) continue;

            if (alreadyHit.Contains(target)) continue;

            alreadyHit.Add(target);
            target.OnDamage(_damage);
        }
    }
    private void CheckPhaseChange()
    {
        float percent = GetHealthPercent();
        if(currentPhase == BossPhase.Phase1 && percent <= bossStats.phase2HealthPercent)
        {
            StartTransition(BossPhase.Phase2);
            return;
        }

        if (currentPhase == BossPhase.Phase2 || currentPhase == BossPhase.Stunned)
        {
           if(percent <= bossStats.phase3HealthPercent)
            {
                StartTransition(BossPhase.Phase3);
            }
        }
    }

    private void RegisterEyeHits()
    {
        if (currentPhase != BossPhase.Phase2) return;
        if (Time.time < stunLockout) return;

        if (Time.time > eyeHitExpireTime)
        {
            eyeHitCount = 0;
        }

        eyeHitCount++;
        eyeHitExpireTime = Time.time + bossStats.eyeHitTime;

        if (eyeHitCount >= bossStats.eyeHitsToStun)
        {
            eyeHitCount = 0;
            StartStun();
        }
    }

    private void ClearEyeHIts()
    {
        if (eyeHitCount > 0 && Time.time > eyeHitExpireTime)
        {
            eyeHitCount = 0;
        }
    }
    public Color GetCurrentColor() { return currentColor; }
    public LayerMask GetAttackMask(bool _friendlyFire)
    {
        int mask = damageableMask;
        if (_friendlyFire)
        {
            mask = mask | otherEnemyMask;
        }
        return mask;
    }
    private float GetPhaseHealthBottom()
    {
        if (currentPhase == BossPhase.Phase1)
        {
            return GetMaxHealth() * bossStats.phase2HealthPercent;
        }

        if (currentPhase == BossPhase.Phase2 || currentPhase == BossPhase.Stunned)
        {
            return GetMaxHealth() * bossStats.phase3HealthPercent;
        }
        return 0f;
    }
    public float GetMaxHealth()
    {
        if (stats == null) return 1f;
        return stats.maxHealth;
    }
    public float GetHealthPercent()
    {
        if (GetMaxHealth() <= 0f) return 0f;
        return Mathf.Clamp01(currentHealth / GetMaxHealth());
    }

    public override void Die()
    {
        if (currentPhase == BossPhase.Dead) return;

        currentPhase = BossPhase.Dead;

        if(stateMachine != null && stateMachine.currentState != null)
        {
            stateMachine.currentState.Exit();
            stateMachine.currentState = null;
        }

        StopAllCoroutines();
        SetMovementEnabled(false);

        GameManager.Instance.SetWin();
        base.Die();
    }
}

