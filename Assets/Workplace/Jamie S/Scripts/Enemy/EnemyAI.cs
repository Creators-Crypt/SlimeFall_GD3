using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static EnemyStatsSO;

public class EnemyAI : MonoBehaviour, IDamageable
{
    
    [Header("Data from Scriptiabl object")]
    [SerializeField] public EnemyStatsSO stats;
    

    [SerializeField] public Transform firePoint;
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] public Transform playerTarget;
    [SerializeField] public Renderer model;

    [SerializeField] private LayerMask playerLayer; //ADDED Line to script.

    public Vector3 spawnPostion;   
    public float currentHealth;
    public float faceTargetRotSpeed = 10;
    public float timeSinceLastSawPlayer;
    public float lastAttackTime;
    public Color origColor;


    public EnemyStateMachine stateMachine;
    public EnemyIdleState idleState;
    public EnemyPatrolState patrolState;
    public EnemyChaseState chaseState;
    public EnemyAttackState attackState;


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

        origColor = model.material.color;

        if (firePoint == null) firePoint = transform;
        spawnPostion = transform.position;
        currentHealth = stats.maxHealth;

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
            case EnemyType.Boss:
                break;
        }
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

        currentHealth -= amount;
        StartCoroutine(FlashRed());
        //if(stats.projectilePrefab != null) { Destroy(stats.projectilePrefab, .01f); }
        if (currentHealth <= 0f)
        {
            Die();
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
        //GameManager.instance.EnemyAIKilled();
        if(stats.splitPrefab != null && UnityEngine.Random.value <= stats.splitChance)
        {
            SplitSlime();
        }

        GameManager.Instance.PlayerPerformAction("SlimeKilled");
        Destroy(gameObject, .01f);
    }


    public virtual IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = origColor;
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
}
