using System;
using System.Collections;
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

    public event Action OnDeath;
    public event Action<float, float> OnHealthChanged;

    public float CurrentHealth {  get; protected set; }
    public float MaxHealth { get; private set; }
    public bool IsDead { get; private set; }

    public virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if(playerObj != null )
        {
            playerTarget = playerObj.transform;
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
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        stateMachine.Initialize(patrolState);
    }
    // Update is called once per frame
   public virtual void Update()
    {
        stateMachine.Tick();
        MeasurePlayerSpeed();
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

        Quaternion targetRot = Quaternion.LookRotation(dir); 
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, faceTargetRotSpeed * Time.deltaTime);
    }

    public bool CanAttack()
    {
        return Time.time - lastAttackTime >= stats.attackCooldown;
    }

    public virtual void PreformAttack()
    {
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
                PreforeBomberAttack();
                break;
            case EnemyType.Boss:
                break;
        }
    }

    public virtual void PreformMortarAttack(float _damage)
    {
        if(IsDead || playerTarget == null || stats == null) return;
        if(stats.mortarPrefab == null) return;

        StartCoroutine(MortarAttack(_damage));
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

            BossMortarProjectile mortarShell = mortarShellObj.GetComponent<BossMortarProjectile>();
            if (mortarShell != null)
            {
                LayerMask splashHits = GetAttackMask(mortarFriendlyFire);

                mortarShell.Launch(impactPoint, stats.mortarSpeed, stats.mortarArcHeight,_damage, stats.mortarSplashRadius, stats.mortarGroundMask, splashHits);
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

        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if(projectile != null)
        {
            projectile.Fire(dir, stats.projectileSpeed, stats.attackDamage);
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
/*            if (hit.gameObject != gameObject)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();

                if (damageable != null)
                {
                    damageable.OnDamage(stats.attackDamage);
                }
            }*/
            if (hit.TryGetComponent<HealthSystem>(out var playerHealth)) {
                playerHealth.OnDamage(stats.attackDamage);
            }

        }
        if(stats.explosionVFX != null)
        {
            Instantiate(stats.explosionVFX, transform.position + new Vector3(0,.5f,0), Quaternion.identity);
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
                    newEnemyAI.StartCoroutine(GrowSpawn(stats.splitGrowthSpeed));
                }
            }

        }
    }
    public virtual void Die()
    {
        if (IsDead) return;
        CurrentHealth = 0f;

        NotifyHealthChanged();
        if(OnDeath != null) OnDeath();
        
        if(stats.splitPrefab != null && UnityEngine.Random.value <= stats.splitChance)
        {
            SplitSlime();
        }

        GameManager.Instance.PlayerPerformAction("SlimeKilled");
        Destroy(gameObject, .01f);
    }


    public virtual IEnumerator FlashRed()
    {
        model.material.SetColor("_BaseColor", Color.red);
        yield return new WaitForSeconds(0.1f);
        model.material.SetColor("_BaseColor", origColor);
    }

    IEnumerator GrowSpawn (float _duration)
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
}
