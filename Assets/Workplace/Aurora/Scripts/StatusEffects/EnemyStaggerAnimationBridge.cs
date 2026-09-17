using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PoiseTracker))]
public class EnemyStaggerAnimationBridge : MonoBehaviour {

    [Header("Animation Control")]
    [Tooltip("The Animator component on this enemy. Will look locally and in children automatically.")]
    [SerializeField] private Animator animator;

    [SerializeField] private NavMeshAgent navMeshAgent;

    [Tooltip("Optional: Trigger parameter name in your Animator controller for hit animations.")]
    [SerializeField] private string staggerTriggerName = "Stagger";

    private PoiseTracker poiseTracker;
    private float originalAnimationSpeed = 1f;

    private void Awake() {
        poiseTracker = GetComponent<PoiseTracker>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        // Auto-locate the Animator component if unassigned
        if (animator == null) {
            animator = GetComponent<Animator>();
            if (animator == null) animator = GetComponentInChildren<Animator>();
        }
    }
    private void OnEnable() {
        if (poiseTracker != null) {
            poiseTracker.OnStaggered += HandleStaggerTriggered;
            poiseTracker.OnPoiseRecovered += HandlePoiseRecovered;
        }
    }
    private void OnDisable() {
        if (poiseTracker != null) {
            poiseTracker.OnStaggered -= HandleStaggerTriggered;
            poiseTracker.OnPoiseRecovered -= HandlePoiseRecovered;
        }
    }
    /// <summary> Called the exact frame the enemy stance breaks. Freezes animation states. </summary>
    private void HandleStaggerTriggered() {
        if (animator == null) return;
        if (navMeshAgent != null && navMeshAgent.isOnNavMesh) { navMeshAgent.isStopped = true; }
        // 1. Cache the current playback speed so we can restore it accurately later
        originalAnimationSpeed = animator.speed;

        // 2. Fire the hit reaction trigger if configured in the Animator state machine
        if (!string.IsNullOrEmpty(staggerTriggerName)) {
            animator.SetTrigger(staggerTriggerName);
        }

        // 3. FREEZE EXECUTION: Stop animation progression entirely to simulate a heavy physics stun
        animator.speed = 0f;

        Debug.Log($"[Anim Bridge] Frozen animator component on '{gameObject.name}' due to stance break.");
    }
    /// <summary> Called when the poise tracker resets balance. Unfreezes animation states. </summary>
    private void HandlePoiseRecovered() { UnfreezeAnimator(); }
    private void UnfreezeAnimator() {
        if (animator == null) return;
        if (navMeshAgent != null && navMeshAgent.isOnNavMesh) { navMeshAgent.isStopped = false; }
        // Restore animation speed to turn its logic loop back on safely
        animator.speed = originalAnimationSpeed > 0f ? originalAnimationSpeed : 1f;

        Debug.Log($"[Anim Bridge] Restored animator component speed on '{gameObject.name}' to default.");
    }

    // Safety clean loop step if the script is forcefully disabled mid-stagger
    private void OnDestroy() { UnfreezeAnimator(); }
}