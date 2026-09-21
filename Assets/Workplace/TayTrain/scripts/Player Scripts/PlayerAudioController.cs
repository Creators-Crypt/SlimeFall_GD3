using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource audioSource;

    [Header("Action Sounds")]
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip dodgeClip;
    [SerializeField] private AudioClip teleportClip;
    [SerializeField] private AudioClip concentrateClip;

    [Header("Footsteps")]
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float sprintStepInterval = 0.3f;
    [SerializeField] private float footstepVolume = 0.7f;

    private PlayerController playerController;
    private PlayerController.PlayerState previousState;
    private float footstepTimer;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();

        if(audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        previousState = playerController.CurrentState;
    }

    private void Update()
    {
        PlayerController.PlayerState currentState = playerController.CurrentState;

        if(currentState != previousState)
        {
            PlayActionSound(currentState);
            previousState = currentState;
        }

        UpdateFootsteps(currentState);
    }

    private void PlayActionSound(PlayerController.PlayerState state)
    {
        audioSource.pitch = 1f;

        switch(state)
        {
          
            case PlayerController.PlayerState.Dodge:
                PlayClip(dodgeClip);
                break;

            case PlayerController.PlayerState.Teleport:
                PlayClip(teleportClip);
                break;

            case PlayerController.PlayerState.Concentrate:
                PlayClip(concentrateClip);
                break;
        }
    }

    private void UpdateFootsteps(PlayerController.PlayerState state)
    {
        bool walking = state == PlayerController.PlayerState.Walk;
        bool sprinting = state == PlayerController.PlayerState.Sprint;

        if(!walking && !sprinting)
        {
            footstepTimer = 0f;
            return;
        }

        footstepTimer -= Time.deltaTime;

        if(footstepTimer > 0f)
        {
            return;
        }

        audioSource.pitch = sprinting ? Random.Range(1.0f, 1.15f) : Random.Range(0.95f, 1.05f);

        audioSource.PlayOneShot(footstepClip, footstepVolume);

        footstepTimer = sprinting ? sprintStepInterval: walkStepInterval;
     }

    public void PlayJump()

    {
        PlayClip(jumpClip);
    }
    private void PlayClip (AudioClip clip)
    {
        if(clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
 }
