using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour 
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] CharacterController controller;
    [SerializeField] PlayerStats stats;
    [SerializeField] HealthSystem healthSystem;
    [SerializeField] StaminaController staminaController;
    [SerializeField] ConcentrationController concentrationController;
    [SerializeField] SpellCaster spellCaster;
    [SerializeField] DeathHandler deathHandler;
    [SerializeField] private GameObject concentrationLight;


    [Header("Player Visuals")]
    [SerializeField] private TrailRenderer teleportTrail;

    //[Header("Equipment Display Name")]
    //[SerializeField] float equipmentLookDistance = 30f;

    [Header("Jump")]
    [Range(1, 30)][SerializeField] int jumpSpeed = 5;
    [Range(1, 10)][SerializeField] int jumpMax = 1;
    [SerializeField, Range(1, 10)] int gravity = 10;


    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerState currentState;
    private PlayerAudioController playerAudioController;

    private PlayerState previousState = (PlayerState)(-1);
    
    [Header("Teleport")]
    //[Range(0.05f, 10f)][SerializeField] float teleportTrailTime = 0.2f;
    [Range(0.1f, 3f)][SerializeField] float teleportCooldown = 1.0f;
    [Range(1f, 100f)][SerializeField] float teleportDistance = 100f;
    [SerializeField] float teleportAnimationDuration = 0.2f;

    //Jumps
    int jumpCount;

    //Equipment Mods
    int bonusJumps = 0;
    float speedMult = 1f;
    float gravityMult = 1f;
    float concentrationSpeedMult = 1f;
    float healthRegenMult = 1f;
    float healthRegenTimer;
    float teleportCooldownReduction = 0f;
    float teleportDistanceBonus = 0f;
    float dodgeCooldownReduction = 0f;
    float dodgeSpeedBonus = 0f;

    //Movement
    Vector3 moveDir;
    Vector3 playerVel;

    //speed
    [SerializeField] float currentSpeed;  
    [SerializeField] private bool isPlayerSprinting = false;
    [SerializeField] float staminaTimer;
    [SerializeField] float stamina;

    //Dodge
    bool isDodging;
    float dodgeTimer;
    float dodgeCooldownTimer;
    float originalControllerHeight;
    Vector3 originalControllerCenter;
    Vector3 dodgeDirection;

    [SerializeField] float dodgeControllerHeight = 0.01f;

    //Teleport
    bool isTeleporting;
    float teleportCooldownTimer;

    //Concentration
    bool isConcentrating;
    float concentrationTimer;

    //Player size
    Vector3 originalScale;

    //LayerMask
    [SerializeField] LayerMask ignoreLayer;

    // PlayerState
    public enum PlayerState {
        Idle,
        Walk,
        Sprint,
        Jump,
        Dodge,
        Teleport,
        Concentrate,
        Cast,
        Dead
    }

    public PlayerState CurrentState => currentState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        controller = GetComponent<CharacterController>();

        healthSystem = GetComponent<HealthSystem>();

        staminaController = GetComponent<StaminaController>();

        concentrationController = GetComponent<ConcentrationController>();

        inputHandler = GetComponent<PlayerInputHandler>();

        currentSpeed = stats.walkSpeed;

        deathHandler = GetComponent<DeathHandler>();

        spellCaster = GetComponent<SpellCaster>();

        originalScale = transform.localScale;

        originalControllerHeight = controller.height;

        originalControllerCenter = controller.center;

        playerAudioController = GetComponent<PlayerAudioController>();

        if(teleportTrail != null)
        {
            teleportTrail.emitting = false;
        }

        if(animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }
    // Update is called once per frame
    void Update() {

        stamina = staminaController.Current;
        isPlayerSprinting = !isConcentrating && 
            inputHandler.SprintHeld &&
            moveDir.sqrMagnitude > 0.01f &&
            staminaController.Current > 0f ;

        staminaController.IsConsuming = isPlayerSprinting;
        currentSpeed = (isPlayerSprinting) ? stats.sprintSpeed : stats.walkSpeed;
        if (isPlayerSprinting) {
            staminaController.ContinousSpent(stats.sprintStaminaCost);
        }

        if (dodgeCooldownTimer > 0) {
            dodgeCooldownTimer -= Time.deltaTime;
        }

        if (teleportCooldownTimer > 0)
        {
            teleportCooldownTimer -= Time.deltaTime;
        }


        teleport();
        dodge();
        concentrate();
        movement();
        healthRegen();
        //lookAtEquipment();
        updateState();
        updateAnimator();
    }
    private void LateUpdate() 
    {
    }
  
    void movement() 
    {
        if (controller.isGrounded && playerVel.y < 0) 
        {
            jumpCount = 0;
            playerVel.y = -2f;
        }

        if (!isTeleporting && !isDodging && !isConcentrating) 
        {
            moveDir = new Vector3(inputHandler.MoveInput.x, 0f, inputHandler.MoveInput.y);

            moveDir = transform.TransformDirection(moveDir);

            controller.Move(moveDir.normalized * (currentSpeed * speedMult) * Time.deltaTime);
        }

        if(!isConcentrating)
        {
            jump();
        }

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= (gravity * gravityMult) * Time.deltaTime;
    }
    void dodge() {


        if (inputHandler.DodgePressed)
        {
           if(dodgeCooldownTimer <= 0 && staminaController.TrySpend(stats.dodgeStaminaCost))
            {
                isDodging = true;

                dodgeTimer = stats.dodgeDuration;
                dodgeCooldownTimer = Mathf.Max(0f, stats.dodgeCooldown - dodgeCooldownReduction);

                dodgeDirection = moveDir.normalized;

                if (dodgeDirection == Vector3.zero)
                {
                    dodgeDirection = transform.forward;
                }

                controller.height = dodgeControllerHeight;

                float heightDifference = originalControllerHeight - dodgeControllerHeight;

                controller.center = originalControllerCenter - new Vector3(0f, heightDifference / 2f, 0f);

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.PlayerPerformAction("Dodge");
                }
            }

            inputHandler.UseDodge();
        }
       

        if (isDodging) {

            transform.localScale = Vector3.Lerp(transform.localScale,
                originalScale * 0.1f, 15f * Time.deltaTime);

            controller.Move(dodgeDirection *
                stats.walkSpeed *
                (stats.dodgeSpeedMultiplier + dodgeSpeedBonus) * Time.deltaTime);

            dodgeTimer -= Time.deltaTime;

            if (dodgeTimer <= 0)
            {
                isDodging = false;

                controller.height = originalControllerHeight;
                controller.center = originalControllerCenter;
            }
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale,
                originalScale, 15f * Time.deltaTime);
        }
    }
    void teleport ()
    {
        if (inputHandler.TeleportPressed)
        {
           if(teleportCooldownTimer <= 0 && !isTeleporting)
            {

                isTeleporting = true;

                teleportCooldownTimer = Mathf.Max(0f, teleportCooldown - teleportCooldownReduction);

                RaycastHit hit;

                Vector3 teleportDirection = Camera.main.transform.forward;
                Vector3 teleportPoint;

                if (Physics.Raycast(Camera.main.transform.position,
                    teleportDirection, out hit, teleportDistance + teleportDistanceBonus, ~ignoreLayer))
                {
                    teleportPoint = hit.point - teleportDirection * 1.5f;
                }

                else
                {
                    teleportPoint = transform.position + teleportDirection * (teleportDistance + teleportDistanceBonus);
                }

                RaycastHit groundHit;

                Vector3 groundCheck = teleportPoint + Vector3.up * 10f;

                if (Physics.Raycast(groundCheck, Vector3.down, out groundHit, 20f, ~ignoreLayer))
                {
                    float controllerBottom = controller.center.y - (controller.height / 2f);

                    float groundOffset = -controllerBottom;
                    if (teleportPoint.y < groundHit.point.y + groundOffset)
                    {
                        teleportPoint.y = groundHit.point.y + groundOffset;
                    }
                }

                if (teleportTrail != null)
                {
                    teleportTrail.Clear();
                    teleportTrail.emitting = true;
                }

                controller.enabled = false;
                transform.position = teleportPoint;
                controller.enabled = true;
                StartCoroutine(finishTeleport());
               
               if(teleportTrail != null)
                {
                    StartCoroutine(stopTeleportTrail());
                }

                if(GameManager.Instance != null)
                {
                    GameManager.Instance.PlayerPerformAction("Teleport");
                }
            }
           inputHandler.UseTeleport();
        }
    }
    IEnumerator stopTeleportTrail()
    {
        yield return null;

        if(teleportTrail != null)
        {
            teleportTrail.emitting = false;
        }
    }
    IEnumerator finishTeleport()
    {
        yield return new WaitForSeconds(teleportAnimationDuration);
        isTeleporting = false;
    }
void jump() {
        if (inputHandler.JumpPressed) 
        {
            if(jumpCount < jumpMax + bonusJumps)
            {
                jumpCount++;
                playerVel.y = jumpSpeed;

                playerAudioController?.PlayJump();

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.PlayerPerformAction("Jump");
                }
            }

            inputHandler.UseJump();
        }
    }

    //Armor stats
    public void setTeleportCooldownReduction(float amount)
    {
        teleportCooldownReduction = amount;
    }
    public void setTeleportDistanceBonus(float amount)
    {
        teleportDistanceBonus = amount;
    }
    public void setDodgeSpeedBonus(float amount)
    {
        dodgeSpeedBonus = amount;
    }
    public void setDodgeCooldownReduction(float amount)
    {
        dodgeCooldownReduction = amount;
    }
    //Amulet stats
    public void setConcentrationSpeedMult(float amount)
    {
        concentrationSpeedMult = amount;
    }
    public void setHealthRegenMult(float amount)
    {
        healthRegenMult = amount;
    }
    void healthRegen()
    {
        if (healthRegenMult <= 1f)
            return;

        healthRegenTimer += Time.deltaTime;

        if(healthRegenTimer >= 1f)
        {
            healthSystem.OnHeal(healthRegenMult);
            healthRegenTimer = 0f;
        }
    }
    //Boots stats
    public void setBonusJumps(int amount)
    {
        bonusJumps = amount;
    }
    public void setSpeedMult(float amount)
    {
        speedMult = amount;
    }
    public void setGravityMult(float amount)
    {
        gravityMult = amount;
    }
    void concentrate()
    {
        if (inputHandler.ConcentratePressed)
        {
            if(!isConcentrating)
            {
                isConcentrating = true;
                concentrationTimer = stats.refillConcentrationTime / concentrationSpeedMult;

                if(concentrationLight != null)
                {
                    concentrationLight.SetActive(true);
                }
            }

            inputHandler.UseConcentrate();
        }

        if(isConcentrating)
        {
            concentrationTimer-= Time.deltaTime;

            if(concentrationTimer <= 0f)
            {
                concentrationController.refill();

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.PlayerPerformAction("Concentrate");
                }

                isConcentrating = false;
                concentrationTimer = 0f;

                if(concentrationLight != null)
                {
                    concentrationLight.SetActive(false);
                }
            }
        }
    }

    void updateState()
    {
        if (healthSystem.IsDead)
        {
            currentState = PlayerState.Dead;
        }
        else if (isDodging)
        {
            currentState = PlayerState.Dodge;
        }
        else if (isTeleporting)
        {
            currentState = PlayerState.Teleport;
        }
        else if (isConcentrating)
        {
            currentState = PlayerState.Concentrate;
        }
        else if (spellCaster != null && spellCaster.IsCasting)
        {
            currentState = PlayerState.Cast;
        }
        else if (!controller.isGrounded)
        {
            currentState = PlayerState.Jump;
        }
        else if (isPlayerSprinting && moveDir.sqrMagnitude > 0.0f)
        {
            currentState = PlayerState.Sprint;
        }
        else if (moveDir.sqrMagnitude > 0.01f)
        {
            currentState = PlayerState.Walk;
        }
        else
        {
            currentState = PlayerState.Idle;
        }
    }

    //void lookAtEquipment()
    //{
    //    RaycastHit hit;

    //    Debug.DrawRay(Camera.main.transform.position,
    //        armPivot.forward * equipmentLookDistance, Color.green);

    //    if (Physics.Raycast(Camera.main.transform.position,
    //        armPivot.forward, out hit, equipmentLookDistance))
    //    {
    //        EquipmentPickup pickup = hit.collider.GetComponentInParent<EquipmentPickup>();

    //        if(pickup != null)
    //        {
    //            Debug.Log(pickup.GetEquipmentName());
    //        }
    //    }
    //}

    void updateAnimator()
    {
        if(animator == null || currentState == previousState)
        {
            return;
        }

        string animationName = currentState switch
        {
            PlayerState.Idle => "Idle",
            PlayerState.Walk => "Walk",
            PlayerState.Sprint => "Running",
            PlayerState.Jump => "Jump",
            PlayerState.Dodge => "Dodge",
            PlayerState.Teleport => "Teleport",
            PlayerState.Concentrate => "Concentrate",
            PlayerState.Cast => "Cast",
            _ => "Idle"
        };

        float fadeTime = currentState == PlayerState.Cast ? 0.05f : 0.15f;

        animator.CrossFade(animationName, fadeTime);

        previousState = currentState;
    }
}