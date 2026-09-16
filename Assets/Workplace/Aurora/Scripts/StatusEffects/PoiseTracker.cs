using System;
using UnityEngine;

public class PoiseTracker : MonoBehaviour {

    // Public events so enemy AI or animation scripts can listen for stagger states!
    public event Action OnStaggered;
    public event Action OnPoiseRecovered;

    [Header("Poise Configuration")]
    [Tooltip("Total poise health. Heavy enemies should have higher numbers.")]
    [SerializeField] private float maxPoise = 50f;
    [Tooltip("How fast poise recovers per second after avoiding damage.")]
    [SerializeField] private float recoverySpeed = 10f;
    [Tooltip("Seconds of safety needed after being hit before recovery starts.")]
    [SerializeField] private float recoveryDelay = 3f;

    private float currentPoise;
    private float delayTimer;
    private bool isStaggered;

    public bool IsStaggered => isStaggered;

    private void Start() { currentPoise = maxPoise; }
    private void Update() {
        if (isStaggered) return; // Stagger states are usually handled/reset by animations or timers

        // Tick recovery safety buffers
        if (delayTimer > 0f) {
            delayTimer -= Time.deltaTime;
            return;
        }

        // Recover poise over time cleanly
        if (currentPoise < maxPoise) {
            currentPoise = Mathf.Min(maxPoise, currentPoise + recoverySpeed * Time.deltaTime);
            if (currentPoise >= maxPoise) { OnPoiseRecovered?.Invoke(); }
        }
    }
    /// <summary> Processes incoming poise force. Triggers staggers if pool drops to 0. </summary>
    public void ReceivePoiseDamage(float amount) {
        if (isStaggered) return;

        delayTimer = recoveryDelay; // Reset recovery timer on every hit
        currentPoise = Mathf.Max(0f, currentPoise - amount);

        Debug.Log($"[POISE] {gameObject.name} took {amount} Poise Damage. Remaining: {currentPoise}/{maxPoise}");

        if (currentPoise <= 0f) { TriggerStagger(); }
    }
    private void TriggerStagger() {
        isStaggered = true;
        Debug.Log($"<color=orange>[STAGGER]</color> {gameObject.name}'s stance broke!");

        OnStaggered?.Invoke();

        // Safe Fallback: Auto-recover from stagger after a short window if no AI script resets it
        Invoke(nameof(ResetStagger), 1.2f);
    }
    public void ResetStagger() {
        if (!isStaggered) return;
        isStaggered = false;
        currentPoise = maxPoise;
        Debug.Log($"[POISE] {gameObject.name} recovered balance.");
    }
}