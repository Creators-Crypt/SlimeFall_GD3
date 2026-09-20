using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static EnemyStatsSO;

public class EnemyAI : MonoBehaviour, IDamageable, IHealth
{
    
    [Header("Data from Scriptiabl object")]
    [SerializeField] public EnemyStatsSO stats;
    

    [SerializeField] public Transform firePoint;
    [SerializeField] public Transform mortarFirePoint;
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] public Transform playerTarget;
    [SerializeField] public Renderer model;
    
    [Header("Layers")]
    public LayerMask playerLayer;    
    public LayerMask groundMask = ~0;
    public LayerMask damageableMask = 0; 
    public LayerMask otherEnemyMask = 0;

    [Header("Friendly Fire - hurt other slimes")]
    public bool mortarFriendlyFire = false;
    public bool landingShockFriendlyFire = false;
    public bool meleeFriendlyFire = false;
    public bool aoeWaveFriendlyFire = false;
    public bool detonationFriendlyFire = true;


    public Vector3 spawnPostion;    
    public float faceTargetRotSpeed = 10;
    public float timeSinceLastSawPlayer;
    public float lastAttackTime;
    public Color origColor;
    public Vector3 playerVelocity;
    public Vector3 lastPlayerPostion;

    public EnemyStateMachine stateMachine;
    public EnemyIdleState idleState;
    public EnemyPatrolState patrolState;
    public EnemyChaseState chaseState;
    public EnemyAttackState attackState;  
    public EnemyJumpState jumpState;

 
   
    public bool jumpLanded;
    private bool jumpMoving;
    private bool savedUpdatePoS;
    private Vector3 safeJumpPos;

    public event Action OnDeath;
    public event Action<float, float> OnHealthChanged;

    public float CurrentHealth {  get; protected set; }
    public float MaxHealth { get; private set; }
    public bool IsDead { get; protected set; }

    public virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if(playerObj != null )
        {
            playerTarget = playerObj.transform;
            lastPlayerPostion = playerTarget.position;
        }
        else
        {
            Debug.LogWarning("Be sure the player is TAGED AS PLAYER");
        }

        origColor = model.material.GetColor("_BaseColor");

        if (firePoint == null) firePoint = transform;
        spawnPostion = transform.position;
        InitializeEnemyHealth();


        stateMachine = new EnemyStateMachine();
        idleState = new EnemyIdleState(this);
        patrolState = new EnemyPatrolState(this);
        chaseState = new EnemyChaseState(this);
        attackState = new EnemyAttackState(this);
        jumpState = new EnemyJumpState(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        stateMachine.Initialize(patrolState);
    }
    // Update is called once per frame
   public virtual void Update()
    {
        MeasurePlayerSpeed();
        if (IsDead ) return;
        stateMachine.Tick();
       
    }

    protected void InitializeEnemyHealth()
    {
        if (stats != null)
        {
            MaxHealth = stats.maxHealth;
        }
        CurrentHealth = MaxHealth;
        IsDead = false;
    }

    protected void NotifyHealthChanged()
    {
        if (OnHealthChanged != null)
        {
            OnHealthChanged(CurrentHealth, MaxHealth);
        }
    }

    public bool CanSeePlayer()
    {
        if(playerTarget == null) return false;

        Vector3 playerDist = playerTarget.position - firePoint.position;
        float distance = playerDist.magnitude;

        if(distance > stats.detectionRadius) return false;

        float angle = Vector3.Angle(transform.forward, playerDist.normalized);
        if(angle > stats.detectionAngle * .5f) return false;
        if(Physics.Raycast(firePoint.position, playerDist.normalized,distance,stats.obstacleMask)) return false;

        timeSinceLastSawPlayer = 0f;
        return true;
    }

    public void AddSightLossTime()
    {
        timeSinceLastSawPlayer += Time.deltaTime;
    }

    public bool IsPlayerInAttackRange()
    {
        if(playerTarget == null) return false;

        return Vector3.Distance(transform.position, playerTarget.position) <= stats.attackRange;
    }

    public void FacePlayer()
    {
        if(playerTarget ==null) return;

        Vector3 dir = playerTarget.position - transform.position;
        dir.y = 0f;

        Quaternion targetRot = Quaternion.LookRotation(dir); 
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, faceTargetRotSpeed * Time.deltaTime);
    }

    public bool CanAttack()
    {
        if (IsDead) return false;
        return Time.time - lastAttackTime >= stats.attackCooldown;
    }

    public virtual void PreformAttack()
    {
        if (IsDead) return;
        Debug.Log("Enter Perform Attack");
        lastAttackTime = Time.time;

        switch(stats.enemyType)
        {
            case EnemyType.Melee:
                PerformMeleeAttack();
                break;
            case EnemyType.Ranged:
                PreformRangedAttack();
                break;
            case EnemyType.Bomber:
                PreforeBomberAttack();
                break;
            case EnemyType.Mortar:
                PreformMortarAttack(stats.attackDamage);
                break;
            case EnemyType.Jump:
                stateMachine.ChangeState(jumpState);
                break;
            case EnemyType.Wave:
                StartCoroutine(WaveAttack(stats.attackDamage));
                break;
            case EnemyType.JumpNdWave:
                stateMachine.ChangeState(jumpState);
                break;

        }
    }

    public virtual void PlayVFXandSFX(AttackFeedback _feedback, Vector3 _position)
    {
        if(_feedback != null)
        {
            _feedback.Play(_position);
        }
    }

    public virtual void PreformMortarAttack(float _damage)
    {
        if(IsDead || playerTarget == null || stats == null) return;
        if(stats.mortarPrefab == null) return;

        StartCoroutine(MortarAttack(_damage));
    }
   
    public virtual LayerMask GetAttackMask(bool _friendlyFire)
    {
        int mask = damageableMask;
        if (_friendlyFire)
        {
            mask = mask | otherEnemyMask;
        }
        return mask;
    }
    public virtual Vector3 PickImpactPoint(int _shellNumber)
    {
        Vector3 lead = playerVelocity;
        lead.y = 0f;

        Vector3 aimPoint = playerTarget.position + playerVelocity * stats.mortarAimAheadOfPlayer;

        if (_shellNumber > 0)
        {
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * stats.mortarScatter;
            aimPoint = aimPoint + new Vector3(randomCircle.x, 0f, randomCircle.y);
        }

        Vector3 groundPoint = GetGroundPoint(aimPoint);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(groundPoint, out hit, 3f, NavMesh.AllAreas))
        {
            groundPoint = hit.position;
        }
        return groundPoint;
    }
    public virtual Vector3 GetGroundPoint(Vector3 _point)
    {
        Vector3 start = _point + Vector3.up * 5f;

        RaycastHit hit;
        if (Physics.Raycast(start, Vector3.down, out hit, 35f, stats.mortarGroundMask, QueryTriggerInteraction.Ignore))
        {
            return hit.point;
        }
        return _point;
    }
   public virtual void SpawnTelegraph(Vector3 _impactPoint, float _flightTime)
    {
        if (stats.mortarHitPosDisplayPrefab == null) return;

        Vector3 spawnPoint = _impactPoint + Vector3.up * .05f;

        GameObject marker = Instantiate(stats.mortarHitPosDisplayPrefab, spawnPoint, Quaternion.identity);

        BossTelegraph telegraph = marker.GetComponentInParent<BossTelegraph>();
        if (telegraph != null)
        {
            telegraph.Play(stats.mortarSplashRadius, _flightTime);
        }
        else
        {
            Destroy(marker, _flightTime + .1f);
        }
    }
    public virtual void MeasurePlayerSpeed()
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



    public virtual void PreformRangedAttack()
    {
        if(playerTarget == null || stats.projectilePrefab == null) return;

        Vector3 dir = playerTarget.position - firePoint.position;
        dir.Normalize();

        GameObject projectileObj = Instantiate(stats.projectilePrefab,firePoint.position, Quaternion.LookRotation(dir));

        PlayVFXandSFX(stats.projectileLaunch, firePoint.position);

        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if(projectile != null)
        {
            projectile.Fire(dir, stats.projectileSpeed, stats.attackDamage,this);
        }
        else
        {
            Debug.LogWarning("PLEASE CHECK THE PROJECTILE PREFAB IN THE ENEMY SCRIPTED OBJECT IS NOT EMPTY");
        }
       
    }
    public virtual void PreforeBomberAttack()
    {
        Debug.Log("Enter the attack");
        Collider[] hits = Physics.OverlapSphere(transform.position, stats.explosionRadius, playerLayer);

        foreach (Collider hit in hits)
        {
            //if (hit.gameObject != gameObject)
            //{
            //    IDamageable damageable = hit.GetComponent<IDamageable>();

            //    if (damageable != null)
            //    {
            //        damageable.OnDamage(stats.attackDamage);
            //    }
            //}
            if (hit.TryGetComponent<HealthSystem>(out var playerHealth)) {
                playerHealth.OnDamage(stats.attackDamage);
            }

        }
        if(stats.bomberBlast != null)
        {
            PlayVFXandSFX(stats.bomberBlast, new Vector3(transform.position.x, transform.position.y + .03f, transform.position.z));
        }
        Die();
    }
    public virtual void PerformMeleeAttack()
    {
        if (playerTarget == null) {
            Debug.LogError($"[{gameObject.name}] Melee Attack failed: playerTarget is NULL!");
            return;
        }

        Debug.Log($"[{gameObject.name}] Attempting melee attack on object named: '{playerTarget.name}'");

        if (playerTarget.TryGetComponent<HealthSystem>(out var damageable)) {
            Debug.Log($"[{gameObject.name}] SUCCESS: Found HealthSystem on '{playerTarget.name}'. Dealing {stats.attackDamage} damage.");
            damageable.OnDamage(stats.attackDamage);
        } else {
            Debug.LogError($"[{gameObject.name}] CRITICAL: Checked '{playerTarget.name}' but it does NOT have a HealthSystem! " +
                       $"Is this the correct root object?");
        }
    }

    public virtual void OnDamage(float amount)
    {
        if (IsDead) return;       

        CurrentHealth -= amount;
        StartCoroutine(FlashRed());
        //if(stats.projectilePrefab != null) { Destroy(stats.projectilePrefab, .01f); }
        if (CurrentHealth <= 0f)
        {
            Die();
        }
        else
        {
            NotifyHealthChanged();
        }
    }
  

    private void SplitSlime()
    {
       
        for(int i =0; i < stats.splitCount; i++)
        {
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * stats.splitRadius;
            Vector3 spawnPOS = transform.position + new Vector3(randomOffset.x, 0 , randomOffset.y);

            if (stats.splitVFXPrefab != null)
            {
                Instantiate(stats.splitVFXPrefab, spawnPOS, Quaternion.identity);
            }

            GameObject newEnemy = Instantiate(stats.splitPrefab, spawnPOS, Quaternion.identity);

            if (stats.splitGrowthSpeed > 0f)
            {
                EnemyAI newEnemyAI = newEnemy.GetComponent<EnemyAI>();
                if(newEnemyAI != null)
                {
                    newEnemyAI.StartCoroutine(newEnemyAI.GrowSpawn(stats.splitGrowthSpeed));
                }
            }

        }
    }
    public void RestoreJumpMovement()
    {
        if (jumpMoving == false) return;
        jumpMoving = false;
        transform.position = safeJumpPos;
        if (agent == null) return;
        if (agent.enabled && agent.gameObject.activeInHierarchy) agent.Warp(safeJumpPos);
        agent.updatePosition = savedUpdatePoS;
    }
    protected virtual void OnDisable()
    {
        if (jumpState != null) jumpState.Exit();
    }
    public virtual void Die()
    {
        if (IsDead) return;
        IsDead = true;
        if (jumpState != null) jumpState.Exit();
        CurrentHealth = 0f;

        NotifyHealthChanged();
        if(OnDeath != null) OnDeath();
        
        if(stats.splitPrefab != null && UnityEngine.Random.value <= stats.splitChance)
        {
            SplitSlime();
        }

        GameManager.Instance.PlayerPerformAction("TargetDefeated");
        Destroy(gameObject, .01f);
    }
    
    
    public virtual IEnumerator JumpAttack(float _damage)
    {
        jumpLanded = false;
        if(playerTarget == null|| agent == null) yield break;
        if (agent.isOnNavMesh == false) yield break;

        PlayVFXandSFX(stats.jumpWindup, transform.position);
        yield return new WaitForSeconds(stats.jumpWindupTime);
        if(IsDead || playerTarget == null) yield break;

        Vector3 start = transform.position;
        Vector3 aimPoint = playerTarget.position;
        CapsuleCollider  playerCollider = playerTarget.GetComponentInChildren<CapsuleCollider>();
        if(playerCollider != null)
        {
            aimPoint = playerCollider.bounds.center;
        }
        Vector3 direction = aimPoint - start;
        direction.y = 0f;
        Vector3 target = start + Vector3.ClampMagnitude(direction, stats.jumpMaxDistance);
        NavMeshHit landing;
        if(NavMesh.SamplePosition(target, out landing,1, agent.areaMask)==false)yield break;
        NavMeshHit edge;
        if(agent.Raycast(landing.position, out edge))yield break;
        Vector3 end = landing.position;

        safeJumpPos = agent.nextPosition;
        savedUpdatePoS = agent.updatePosition;
        jumpMoving = true;
        agent.updatePosition = false;
        PlayVFXandSFX(stats.jumpTakeOff, start);   
        

        List<IDamageable> alreadyHit = new List<IDamageable>();
        float timer = 0f;
        while (timer < stats.jumpDuration)
        {
            if (IsDead) yield break;
            timer += Time.deltaTime;
            float amount = Mathf.Clamp01(timer/stats.jumpDuration);
            Vector3 next = Vector3.Lerp(start,end,amount);
            next.y += stats.jumpHeight * 4f * amount * (1f - amount);

            float rad = stats.jumpHitRadius;
            Vector3 center = transform.position +Vector3.up * rad;
            Vector3 nextCenter = next + Vector3.up * rad;

            if(Physics.CheckCapsule(center, nextCenter, rad, stats.jumpObstacleMask, QueryTriggerInteraction.Ignore))
            {
                Debug.Log("Jump canclled by obstacle check");
                RestoreJumpMovement();
                yield break;
            }

            transform.position = next;
            Collider[] hits = Physics.OverlapCapsule(center,nextCenter,rad,GetAttackMask(meleeFriendlyFire));
            Debug.Log("Jump damage check: " + hits.Length + " collider");
            foreach (Collider hit in hits)
            {
                Debug.Log("Jump overlap: " + hit.name + " |Layer: " + LayerMask.LayerToName(hit.gameObject.layer));
                if (hit.transform.IsChildOf(transform)) continue;
                Debug.Log("Jum detected: " + hit.name + " | Layer: " + LayerMask.LayerToName(hit.gameObject.layer));
                IDamageable health = hit.GetComponent<IDamageable>();
                if (health == null || alreadyHit.Contains(health)) continue;
                alreadyHit.Add(health);
                Debug.Log("Jump doing: " + _damage);
                health.OnDamage(_damage);
                PlayVFXandSFX(stats.jumpHit, hit.ClosestPoint(nextCenter));
            }
            yield return null;
        }

        safeJumpPos = landing.position;
        RestoreJumpMovement();
        jumpLanded = true;
        PlayVFXandSFX(stats.jumpLanding, end);
    }

    public virtual IEnumerator WaveAttack(float _damage)
    {
        if(IsDead || stats.aoeWavePrefab == null)yield break;
        PlayVFXandSFX(stats.waveWindup,transform.position);
        yield return new WaitForSeconds(stats.aoeWaveWaringTime);
        if(IsDead)yield break;

        GameObject waveObj = Instantiate(stats.aoeWavePrefab, transform.position,Quaternion.identity);
        BossAoEWave wave = waveObj.GetComponent<BossAoEWave>();
        if(wave == null)
        {
            Debug.LogWarning("The wave prefab need the BossAoEWave",this);
            Destroy(waveObj);
            yield break;
        }
        PlayVFXandSFX(stats.waveRelease,transform.position);
        wave.Play(this, stats.aoeWaveRadius, stats.aoeWaveSpeed, _damage, GetAttackMask(aoeWaveFriendlyFire));
    }

    public virtual IEnumerator JumpNdWaveAttack()
    {
        yield return JumpAttack(stats.jumpdamage);
        if (IsDead || jumpLanded == false) yield break;
        yield return new WaitForSeconds(stats.jumpToWaveDelay);
        if(IsDead ) yield break;
        yield return WaveAttack(stats.jumpWaveDamage);
            
    }
    public virtual IEnumerator JumpNdWaveAttack(float _jumpDmg, float _waveDmg, float _jumpToWaveDelay)
    {
        yield return JumpAttack(_jumpDmg);
        if (IsDead || jumpLanded == false) yield break;
        yield return new WaitForSeconds(_jumpToWaveDelay);
        if (IsDead) yield break;
        yield return WaveAttack(_waveDmg);

    }
    public virtual IEnumerator FlashRed()
    {
        model.material.SetColor("_BaseColor", Color.red);
        yield return new WaitForSeconds(0.1f);
        model.material.SetColor("_BaseColor", origColor);
    }
    private IEnumerator MortarAttack(float _damage)
    {
        Transform muzzle = mortarFirePoint;
        if (muzzle == null) muzzle = transform;

        for (int i = 0; i < stats.mortarShellsPreSalvo; i++)
        {
            if (playerTarget == null) break;

            Vector3 impactPoint = PickImpactPoint(i);

            Vector3 horizontalOffset = impactPoint - muzzle.position;
            horizontalOffset.y = 0;
            float flightTime = horizontalOffset.magnitude / stats.mortarSpeed;

            SpawnTelegraph(impactPoint, flightTime);

            GameObject mortarShellObj = Instantiate(stats.mortarPrefab, muzzle.position, Quaternion.identity);
            PlayVFXandSFX(stats.mortarLaunch, muzzle.position);
            BossMortarProjectile mortarShell = mortarShellObj.GetComponent<BossMortarProjectile>();
            if (mortarShell != null)
            {
                LayerMask splashHits = GetAttackMask(mortarFriendlyFire);

                mortarShell.Launch(impactPoint, stats.mortarSpeed, stats.mortarArcHeight, _damage, stats.mortarSplashRadius, stats.mortarGroundMask, splashHits);
            }
            else
            {
                Debug.LogWarning("Check the mortarPrefab anb make sure it has the BossMortarProjectile script :)");
                Destroy(mortarShellObj);
            }
            lastAttackTime = Time.time;

            yield return new WaitForSeconds(stats.mortarTimeBetweenShells);
        }
    }
    public virtual IEnumerator GrowSpawn (float _duration)
    {
        Vector3 fullScale = transform.localScale;
        transform.localScale = Vector3.zero;
        float elapsed = 0f;
        while(elapsed < _duration)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, fullScale, elapsed /  _duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = fullScale;
    }

    public void OnHeal(float healAmount)
    {
        //enemies don't heal
    }

    public void HealMax()
    {
        //enemies don't heal
    }

    public IEnumerator HealOverTime(float duration)
    {
        //Enemies don't heal with time.....Unless????
        yield break;
    }

    public  void OnDrawGizmosSelected()
    {
        if(stats == null)return;

        Vector3 origin = transform.position;
        if(firePoint != null )
        {
            origin = firePoint.position;
        }
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(origin, stats.detectionRadius);

        Gizmos.color = Color.darkRed;
        Gizmos.DrawWireSphere(origin, stats.attackRange);

        float halfAngle = stats.detectionAngle / 2f;
        Vector3 left = Quaternion.AngleAxis(-halfAngle, Vector3.up) * transform.forward;
        Vector3 right = Quaternion.AngleAxis(halfAngle, Vector3.up) * transform.forward;

        Gizmos.color = Color.crimson;
        Gizmos.DrawRay(origin, left * stats.detectionRadius);
        Gizmos.DrawRay(origin, right * stats.detectionRadius);

    }
}
