using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;


public class BossPhase1State : IEnemyState
{
    private BossAI boss;
    private BossStatsSO stats;
    private Coroutine attackRoutine;

    public BossPhase1State(BossAI _boss)
    {
        boss = _boss;
    }

    public void Enter()
    {
        stats = boss.bossStats;

        boss.isInvulnerable = false;
        boss.SetMovementEnabled(false);
        boss.SetPhaseColor(stats.phase1Material);

        attackRoutine = boss.StartCoroutine(AttackLoop());
    }

    public void Tick()
    {
        if (boss.playerTarget != null)
        {
            boss.FacePlayer();
        }
    }
    public void Exit()
    {
        if (attackRoutine != null)
        {
            boss.StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
        boss.SetMovementEnabled(true);
    }

    private IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(stats.p1StartDelay);

        float nextMortarTime = Time.time + stats.mortarCooldown;

        while (true)
        {
            if (boss.playerTarget == null)
            {
                yield return null;
                continue;
            }

            if (Time.time >= nextMortarTime)
            {
                yield return MortarAttack();
                nextMortarTime = Time.time + stats.mortarCooldown;
            }
            else
            {
                yield return NormalAttack();
                yield return new WaitForSeconds(stats.p1TimeBetweenVolley);
            }
        }

    }

    private IEnumerator NormalAttack()
    {
        if (stats.projectilePrefab == null)
        {
            Debug.LogWarning("Please put a projectile prefab on the bossSO");
            yield return new WaitForSeconds(.2f);
        }
        else
        {
            Transform muzzle = boss.firePoint;
            if (muzzle == null) muzzle = boss.transform;

            for (int i = 0; i < stats.p1ShotsPerVolley; i++)
            {
                if (boss.playerTarget == null) break;

                Vector3 aimPoint = boss.playerTarget.position;

                float travelTime = Vector3.Distance(muzzle.position, aimPoint) / stats.p1ProjectileSpeed;
                aimPoint = aimPoint + boss.playerVelocity * (travelTime * stats.bulletAimAheadOfPlayer);

                Vector3 dir = (aimPoint - muzzle.position).normalized;
                dir = AddSpread(dir, stats.p1SpreadDegrees);

                GameObject shot = Object.Instantiate(stats.projectilePrefab, muzzle.position, Quaternion.LookRotation(dir));
                Projectile bullet = shot.GetComponent<Projectile>();

                if (bullet != null)
                {
                    bullet.Fire(dir, stats.p1ProjectileSpeed, stats.p1ProjectileDamage);
                }

                boss.lastAttackTime = Time.time;

                yield return new WaitForSeconds(stats.p1TimeBetweenShots);


            }
        }
    }

    private Vector3 AddSpread(Vector3 _dir, float _degrees)
    {
        if (_degrees <= 0) return _dir;

        float randomX = Random.Range(-_degrees, _degrees);
        float randomY = Random.Range(-_degrees, _degrees);

        return Quaternion.Euler(randomX, randomY, 0) * _dir;
    }

    private IEnumerator MortarAttack()
    {
        Transform muzzle = boss.mortarFirePoint;
        if (muzzle == null) muzzle = boss.transform;

        for (int i = 0; i < stats.mortarShellsPreSalvo; i++)
        {
            if (boss.playerTarget == null) break;

            Vector3 impactPoint = PickImpactPoint(i);

            Vector3 horizontalOffset = impactPoint - muzzle.position;
            horizontalOffset.y = 0;
            float flightTime = horizontalOffset.magnitude / stats.mortarSpeed;

            SpawnTelegraph(impactPoint, flightTime);

            GameObject mortarShellObj = Object.Instantiate(stats.mortarPrefab, muzzle.position, Quaternion.identity);

            BossMortarProjectile mortarShell = mortarShellObj.GetComponent<BossMortarProjectile>();
            if (mortarShell != null)
            {
                LayerMask splashHits = boss.GetAttackMask(boss.mortarFriendlyFire);

                mortarShell.Launch(impactPoint, stats.mortarSpeed, stats.mortarArcHeight, stats.mortarDamage, stats.mortarSplashRadius, boss.groundMask, splashHits);
            }
            else
            {
                Debug.LogWarning("Check the mortarPrefab anb make sure it has the BossMortarProjectile script :)");
                Object.Destroy(mortarShellObj);
            }
            boss.lastAttackTime = Time.time;

            yield return new WaitForSeconds(stats.mortarTimeBetweenShells);
        }
    }

    private Vector3 PickImpactPoint(int _shellNumber)
    {
        Vector3 aimPoint = boss.playerTarget.position + boss.playerVelocity * stats.mortarAimAheadOfPlayer;

        if (_shellNumber > 0)
        {
            Vector2 randomCircle = Random.insideUnitCircle * stats.mortarScatter;
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

    private Vector3 GetGroundPoint(Vector3 _point)
    {
        Vector3 start = _point + Vector3.up * 5f;

        RaycastHit hit;
        if (Physics.Raycast(start, Vector3.down, out hit, 35f, boss.groundMask, QueryTriggerInteraction.Ignore))
        {
            return hit.point;
        }
        return _point;
    }

    private void SpawnTelegraph(Vector3 _impactPoint, float _flightTime)
    {
        if (stats.mortarHitPosDisplayPrefab == null) return;

        Vector3 spawnPoint = _impactPoint + Vector3.up * .05f;

        GameObject marker = Object.Instantiate(stats.mortarHitPosDisplayPrefab, spawnPoint, Quaternion.identity);

        BossTelegraph telegraph = marker.GetComponentInParent<BossTelegraph>();
        if (telegraph != null)
        {
            telegraph.Play(stats.mortarSplashRadius, _flightTime);
        }
        else
        {
            Object.Destroy(marker, _flightTime + .5f);
        }
    }
}

public class BossTransitionState : IEnemyState
{
    private BossAI boss;
    private BossStatsSO stats;
    private Coroutine routine;

    public BossPhase targetPhase = BossPhase.Phase2;

    public BossTransitionState(BossAI _boss)
    {
        boss = _boss;
    }

    public void Enter()
    {
        stats = boss.bossStats;

        boss.isInvulnerable = true;
        boss.SetMovementEnabled(false);

        switch (targetPhase)
        {
            case BossPhase.Phase2:
                boss.SetPhaseColor(stats.phase2Material);
                routine = boss.StartCoroutine(LeapDownToPlayer());
                break;
            case BossPhase.Phase3:
                boss.SetPhaseColor(stats.phase3Material);
                routine = boss.StartCoroutine(Transition());
                break;

            default:
                boss.SetPhaseColor(stats.phase2Material);
                routine = boss.StartCoroutine(Transition());
                break;
        }

    }

    public void Tick()
    {
        if (boss.playerTarget != null)
        {
            boss.FacePlayer();
        }
    }

    public void Exit()
    {
        if (routine != null)
        {
            boss.StopCoroutine(routine);
            routine = null;
        }
    }

    private IEnumerator LeapDownToPlayer()
    {
        yield return new WaitForSeconds(stats.transitionWindup);

        Vector3 startPos = boss.transform.position;
        Vector3 landingPos = PickLandingSpot();

        if (boss.agent != null)
        {
            boss.agent.enabled = false;
        }

        float timer = 0f;
        float jumpTime = stats.leapTime;

        while (timer < jumpTime)
        {
            timer += Time.deltaTime;

            float amout = timer / jumpTime;

            Vector3 newPos = Vector3.Lerp(startPos, landingPos, amout);

            newPos.y = newPos.y + stats.leapHeight * Mathf.Sin(amout * Mathf.PI);

            boss.transform.position = newPos;

            yield return null;
        }

        boss.transform.position = landingPos;
        boss.WarpToNavMesh(landingPos);

        LayerMask shotHits = boss.GetAttackMask(boss.landingShockFriendlyFire);
        boss.DealRadialDamage(boss.transform.position, stats.landingShockRad, stats.landingShockDmg, shotHits);

        if (stats.landingShockVfxPrefab != null)
        {
            GameObject vfx = Object.Instantiate(stats.landingShockVfxPrefab, boss.transform.position, Quaternion.identity);
            Object.Destroy(vfx, 5f);
        }

        yield return new WaitForSeconds(.75f);

        boss.isInvulnerable = false;
        boss.FinishTransition(targetPhase);
    }
    private IEnumerator Transition()
    {
        yield return new WaitForSeconds(stats.transitionWindup);

        boss.FinishTransition(targetPhase);
    }
    private Vector3 PickLandingSpot()
    {
        if (boss.playerTarget == null) return boss.transform.position;

        Vector3 playerPos = boss.playerTarget.position;

        Vector3 randomPosAwayFromPlayer = boss.transform.position - playerPos;
        randomPosAwayFromPlayer.y = 0f;

        Vector3 spot = playerPos + randomPosAwayFromPlayer.normalized * stats.leapLandingDist;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(spot, out hit, 6f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        if (NavMesh.SamplePosition(playerPos, out hit, 6f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return spot;
    }
}


public class BossPhase2State : IEnemyState
{
    private BossAI boss;
    private BossStatsSO stats;
    private Coroutine routine;

    private float nextMeleeTime;
    private float nextWaveTime;
    private bool busy;

    public BossPhase2State(BossAI _boss)
    {
        boss = _boss;
    }

    public void Enter()
    {
        stats = boss.bossStats;

        boss.isInvulnerable = false;
        boss.SetPhaseColor(stats.phase2Material);

        if (boss.agent != null && boss.agent.enabled)
        {
            boss.agent.speed = stats.p2ChaseSpeed;
            boss.agent.stoppingDistance = stats.p2MeleeRange;

            boss.SetMovementEnabled(true);

            busy = false;
            nextWaveTime = Time.time + stats.aoeWaveCooldown;
            nextMeleeTime = Time.time + .5f;
        }
    }

    public void Tick()
    {
        if (boss.playerTarget == null) return;
        if (busy)
        {
            boss.FacePlayer();
            return;
        }

        float distance = Vector3.Distance(boss.transform.position, boss.playerTarget.position);

        if (Time.time >= nextWaveTime)
        {
            StartRoutine(AoEWave());
            return;
        }
        if (distance <= stats.p2MeleeRange && Time.time >= nextMeleeTime)
        {
            StartRoutine(MeleeAttack());
            return;
        }

        boss.SetMovementEnabled(true);
        boss.MoveTo(boss.playerTarget.position);

        if (distance <= stats.p2MeleeRange)
        {
            boss.FacePlayer();
        }

    }
    public void Exit()
    {
        if (routine != null)
        {
            boss.StopCoroutine(routine);
            routine = null;
        }
        busy = false;
    }

    private void StartRoutine(IEnumerator _routineToRun)
    {
        if (routine != null)
        {
            boss.StopCoroutine(routine);
        }
        routine = boss.StartCoroutine(_routineToRun);
    }

    private IEnumerator MeleeAttack()
    {
        busy = true;
        boss.SetMovementEnabled(false);

        float timer = 0f;
        while (timer < stats.p2MeleeWindup)
        {
            timer += Time.deltaTime;
            boss.FacePlayer();
            yield return null;
        }

        HitAllInFront();

        boss.lastAttackTime = Time.time;
        nextMeleeTime = Time.time + stats.p2MeleeCooldown;

        yield return new WaitForSeconds(.3f);

        boss.SetMovementEnabled(true);
        busy = false;
        routine = null;
    }

    private void HitAllInFront()
    {
        LayerMask meleeHits = boss.GetAttackMask(boss.meleeFriendlyFire);

        Collider[] hits = Physics.OverlapSphere(boss.transform.position, stats.p2MeleeRange, meleeHits);

        List<IDamageable> alreadyHit = new List<IDamageable>();

        foreach (Collider hit in hits)
        {
            if (hit == null) continue;
            if (hit.transform.IsChildOf(boss.transform)) continue;

            Vector3 dist = hit.transform.position - boss.transform.position;

            float angle = Vector3.Angle(boss.transform.forward, dist);
            if (angle > stats.p2MeleeRange) continue;

            IDamageable target = hit.GetComponent<IDamageable>();
            if (target == null) continue;
            if (alreadyHit.Contains(target)) continue;

            alreadyHit.Add(target);
            target.OnDamage(stats.p2MelleDmg);
        }
    }

    private IEnumerator AoEWave()
    {
        busy = true;
        boss.SetMovementEnabled(false);

        yield return new WaitForSeconds(stats.aoeWaveWarnintTime);

        Vector3 center = boss.transform.position;
        LayerMask waveHits = boss.GetAttackMask(boss.aoeWaveFriendlyFire);

        if (stats.aoeWavePrefab != null)
        {
            GameObject waveObject = Object.Instantiate(stats.aoeWavePrefab, center, Quaternion.identity);

            BossAoEWave wave = waveObject.GetComponent<BossAoEWave>();
            if (wave != null)
            {
                wave.Play(boss, stats.aoeWaveRadius, stats.aoeWaveSpeed, stats.aoeWaveDmg, waveHits);

            }
            else
            {
                Debug.LogWarning("The AoE prefab is missing the BossAoEWave script please fill it in 'instant damage delt'");
                boss.DealRadialDamage(center, stats.aoeWaveRadius, stats.aoeWaveDmg, waveHits);
                Object.Destroy(waveObject, 3f);
            }

        }
        else
        {
            Debug.LogWarning("The AoE prefab is empty on the BossStatsSO please fill it in 'instant damage delt'");
            boss.DealRadialDamage(center, stats.aoeWaveRadius, stats.aoeWaveDmg, waveHits);
        }

        boss.lastAttackTime = Time.time;
        nextWaveTime = Time.time + stats.aoeWaveCooldown;

        yield return new WaitForSeconds(.5f);
        busy = false;
        routine = null;
    }
}

public class BossStunState : IEnemyState
{
    private BossAI boss;
    private BossStatsSO stats;
    private float timer;

    public BossStunState(BossAI _bossAI)
    {
        boss = _bossAI;
    }

    public void Enter()
    {
        stats = boss.bossStats;

        boss.isInvulnerable = false;
        boss.isStunned = true;
        boss.SetMovementEnabled(false);

        timer = stats.stunDuration;
        boss.stunTimeleft = timer;

        boss.SetPhaseColor(stats.stunMaterial);
    }
    public void Tick()
    {
        timer -= Time.deltaTime;
        boss.stunTimeleft = timer;

        if (timer <= 0f)
        {
            boss.EndStun();
        }
    }
    public void Exit()
    {
        boss.isStunned = false;
        boss.stunTimeleft = 0f;
        boss.SetPhaseColor(stats.phase2Material);
        boss.SetMovementEnabled(true);
    }
}

public class BossPhase3State : IEnemyState
{
    private BossAI boss;
    private BossStatsSO stats;
    private Coroutine routine;
    private GameObject telegraph;
    private float timer;
    [SerializeField] private float pullEndTime;
    [SerializeField] public float pullDuration = 1f;

    [SerializeField] private float pullMinDist = 3f;

    public BossPhase3State(BossAI _bossAI)
    {
        boss = _bossAI;
    }
    public void Enter()
    {
        stats = boss.bossStats;

        boss.isInvulnerable = true;
        boss.SetMovementEnabled(false);
        boss.SetPhaseColor(stats.phase3Material);

        timer = stats.detonationTime;
        boss.detonationTimeLeft = timer;
        pullEndTime = Time.time + pullDuration;

        if (stats.detonationTelegraphPrefab != null)
        {
            telegraph = Object.Instantiate(stats.detonationTelegraphPrefab, boss.transform.position, Quaternion.identity);
            telegraph.transform.SetParent(boss.transform);

            BossTelegraph marker = telegraph.GetComponent<BossTelegraph>();

            if (marker != null)
            {
                marker.Play(stats.detonationKillRad, stats.detonationTime);
            }
        }

        routine = boss.StartCoroutine(Countdow());
    }
    public void Tick()
    {
        if (boss.playerTarget == null) return;

        if (Time.time < pullEndTime)
        {
            PullPlayerIn();
        }

        boss.FacePlayer();

    }
    public void Exit()
    {
        if (routine != null)
        {
            boss.StopCoroutine(routine);
            routine = null;
        }
        if (telegraph != null)
        {
            Object.Destroy(telegraph);
            telegraph = null;
        }
    }
    private void PullPlayerIn()
    {
        Transform player = boss.playerTarget;

        Vector3 towardBoss = boss.transform.position - player.position;
        towardBoss.y = 0f;
        float dist = towardBoss.magnitude;

        if (dist > stats.p3PullRadius) return;
        if (dist < pullMinDist) return;

        float howClose = dist / stats.p3PullRadius;
        float force = Mathf.Lerp(stats.p3PullForce, stats.p3PullForce, howClose);

        Vector3 pull = towardBoss.normalized * force * Time.deltaTime;

        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.Move(pull);
        }

        PlayerSlimedEffect slime = player.GetComponent<PlayerSlimedEffect>();
        if (slime != null)
        {
            slime.ApplySlime(stats.slimedSlowAmount, stats.slimedTime);
        }

    }

    private IEnumerator Countdow()
    {
        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            boss.detonationTimeLeft = timer;
            if (boss.detonationTimeLeft < 0f) boss.detonationTimeLeft = 0f;

            yield return null;
        }
        Detonate();
    }

    private void Detonate()
    {
        Vector3 center = boss.transform.position;

        if (stats.detonationVFXPrefab != null)
        {
            GameObject vfx = Object.Instantiate(stats.detonationVFXPrefab, center, Quaternion.identity);
            Object.Destroy(vfx, 8f);
        }

        LayerMask blastHits = boss.GetAttackMask(boss.detonationFriendlyFire);
        boss.DealRadialDamage(center, stats.detonationKillRad, stats.detonationDmg, blastHits);

        boss.Die();
    }
}