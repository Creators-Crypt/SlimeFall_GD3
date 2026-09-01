using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class PlayerSlimedEffect : MonoBehaviour
{
    [Range(0.05f, 1f)]public float speedMultiplier =1f;

    public float recoverySpeed = 1.5f;
    [Range(0.05f, 1f)] public float slowestAlloowed = .25f;

    [Header("VFX")]
    public VisualEffect slimeVFX;
    public AudioSource slimeSound;

    private float slimedEndTime = 0f;
    private float targetMultiplier = 1f;

    private bool vfxPlaying = false;

    private NavMeshAgent agent;
    private float agentNormalSpeed;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if(agent != null )
        {
            agentNormalSpeed = agent.speed;
        }
    }

    private void Update()
    {
        if (IsSlimed())
        {
            speedMultiplier = Mathf.MoveTowards(speedMultiplier, targetMultiplier, recoverySpeed * 2f * Time.deltaTime);
        }
        else
        {
            speedMultiplier = Mathf.MoveTowards(speedMultiplier,1f, recoverySpeed *  Time.deltaTime);

            if(speedMultiplier >= 1f)
            {
                if(slimeVFX != null && vfxPlaying)
                {
                    slimeVFX.Stop();
                    vfxPlaying = false;
                }

                if(slimeSound != null && slimeSound.isPlaying)
                {
                    slimeSound.Stop();
                }
            }
        }
        SetAgentSpeed();
    }

    public bool IsSlimed()
    {
        return Time.time < slimedEndTime;
    }

    public void ApplySlime(float _multiplier, float _duration)
    {
        targetMultiplier = Mathf.Clamp(_multiplier, slowestAlloowed, 1f);
        float newEndTime = Time.time + _duration;

        if(newEndTime > slimedEndTime)
        {
            slimedEndTime = newEndTime;
        }

        if(slimeVFX != null && vfxPlaying == false )
        {
            slimeVFX.Play();
            vfxPlaying = true;
        }
        if(slimeSound != null && slimeSound.isPlaying == false)
        {
            slimeSound.Play();
        }
    }
    public void ClearSlime()
    {
        slimedEndTime = 0f;
        targetMultiplier = 1f;
        speedMultiplier = 1f;
        SetAgentSpeed();
    }

    private void SetAgentSpeed()
    {
        if(agent != null )
        {
            agent.speed = agentNormalSpeed * speedMultiplier; 
        }
    }

}
