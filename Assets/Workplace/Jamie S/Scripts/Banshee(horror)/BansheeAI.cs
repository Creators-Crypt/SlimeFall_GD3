using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class BansheeAI : EnemyAI
{
    [Header("Banshee sight reference")]
    public Transform sightOrigin;
    public Vector3 targetSightOffset = new Vector3(0,1,0);

    [Header("Apperance")]
    [Tooltip("The renderers that should disappear.")]
    public Renderer[] visibleRenderers;
    [Tooltip("Only thie hit colliders.")]
    public Collider[] hitColliders;

    public BansheeHiddenState hiddenState;
    public BansheeAppearState appearState;
    public BansheeScreamState screamState;
    public BansheeFleeState fleeState;
    public BansheeApproachState approachState;
    public BansheeDefeatedState defeatedState;

    private AudioSource voice;
    private AudioClip lastVoiceClip;
    private List<AudioClip> screamChoices = new List<AudioClip>(); 
    private bool initialized;
    private bool visible;


    public override void Awake()
    {
        if(stats == null)
        {
            Debug.LogError("Assign the Banshee a filled out EnemyAI Scriptable Object. Her settings are near the bottom");
            enabled = false;
            return;
        }
        
        base.Awake();
        
        voice = GetComponent<AudioSource>();
        voice.spatialBlend = 1f; //3D for more 2D set to 0
        voice.rolloffMode = AudioRolloffMode.Linear;
        voice.minDistance = 1f;
        voice.maxDistance = Mathf.Max(stats.audibleDist, stats.detectionRadius + 1f);
        voice.Stop();

        visibleRenderers = GetComponentsInChildren<Renderer>(true);
        if(hitColliders == null || hitColliders.Length == 0)
        {
            hitColliders = GetComponentsInChildren<Collider>(true);
        }

        hiddenState = new BansheeHiddenState(this);
        appearState = new BansheeAppearState(this);
        screamState = new BansheeScreamState(this);
        approachState = new BansheeApproachState(this);
        fleeState = new BansheeFleeState(this);

        defeatedState = new BansheeDefeatedState(this);
        initialized = true;
        stateMachine.Initialize(hiddenState);
        BuildScreamChoices();
        if (screamChoices.Count == 0)
        {
            Debug.LogWarning("Assign at least one clip in the Banshee SO under screamClips", this);
        }

    }
    public override void Start()
    {
        if (initialized)
        {
            NotifyHealthChanged();
        }
    }

    public override void Update()
    {
        if (initialized == false) return;
        stateMachine.Tick();
    }

    public void StopMoving()
    {
        if(agent == null)
        {
            return;
        }
        if (agent.isActiveAndEnabled == false || agent.isOnNavMesh == false) return;
        agent.isStopped = true;
        agent.ResetPath();
        agent.velocity = Vector3.zero;
    }

    public void RunAway()
    {
        if(playerTarget == null || agent ==  null) return;
        if (agent.enabled == false || agent.isOnNavMesh == false) return;
        
        Vector3 away = transform.position - playerTarget.position;
        away.y = 0;
        away.Normalize();

        for (int i = 1; i <= 3; i++)
        {
            Vector3 target = transform.position + away * (stats.fleedistance / i);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(target, out hit, 2f, agent.areaMask))
            {
                float currentDist = Vector3.Distance(transform.position, playerTarget.position);
                float newDist = Vector3.Distance(hit.position,playerTarget.position);

                if (newDist > currentDist)
                {
                    NavMeshPath path = new NavMeshPath();
                    if(agent.CalculatePath(hit.position,path)&& path.status == NavMeshPathStatus.PathComplete)
                    {
                        agent.speed = stats.chaseSpeed;
                        agent.isStopped = false;
                        agent.SetPath(path);
                        return;
                    }
                }
            }
        }
        StopMoving();
    }
    public void SetVisible(bool _visible)
    {
        visible = _visible;

        for(int i =0; i < visibleRenderers.Length; i++)
        {
            if(visibleRenderers[i] != null)
            {
                visibleRenderers[i].enabled = _visible;
            }
        }
        for(int i = 0;i < hitColliders.Length; i++)
        {
            if(hitColliders[i] != null)
            {
                hitColliders[i].enabled = _visible;
            }
        }
    }
    public void GoHidden(float _delay)
    {
        hiddenState.WaitFor(_delay);
        stateMachine.ChangeState(hiddenState);

    }
    private void BuildScreamChoices()
    {
        screamChoices.Clear();
        if(stats.screamClips == null)
        {
            Debug.LogWarning("Be sure the screamClips are filled in in the BansheeSO");
            return;
        }

        foreach(AudioClip clip in stats.screamClips)
        {
            if(clip != null && screamChoices.Contains(clip) == false)
            {
                screamChoices.Add(clip);
            }
        }
    }
    public float PlayScream()
    {
        PlayVFXandSFX(stats.screamFeedback, firePoint.position);

        BuildScreamChoices();
        if(screamChoices.Count == 0)
        {
            return 1f;
        }
        if (screamChoices.Count > 1)
        {
            screamChoices.Remove(lastVoiceClip);
        }
        AudioClip chosen = screamChoices[Random.Range(0, screamChoices.Count)];
        lastVoiceClip = chosen;
        voice.clip = chosen;
        voice.volume = stats.screamVolume;
        voice.maxDistance = Mathf.Max(stats.audibleDist, stats.detectionRadius + 1);
        voice.Play();
        return chosen.length;
    }
    public void StopScream()
    {
        voice.Stop();
    }

    public void Revive()
    {
        InitializeEnemyHealth();
        GoHidden(0f);
        NotifyHealthChanged();
    }

    public override void OnDamage(float _amount)
    {
        if(initialized == false || isActiveAndEnabled == false) { return; }
        if(IsDead || visible == false) { return; }
        if (_amount <= 0f) return;

        base.OnDamage(_amount);
    }

    public override void Die()
    {
        if(initialized == false || IsDead) {  return; }
        if(stats.canReturn == false)
        {
            if(stateMachine.currentState !=null)
            {
                stateMachine.currentState.Exit();
                stateMachine.currentState = null;   
            }
            StopScream();
            SetVisible(false);

            base.Die();
            return;
        }
        IsDead = true;
        CurrentHealth = 0f;
        stateMachine.ChangeState(defeatedState);
        NotifyHealthChanged();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if(initialized == false)
        {
            return;
        }

        voice.Stop();
        SetVisible(false);
        if(IsDead == false)
        {
            GoHidden(stats.scareCooldown);
        }
    }
}
