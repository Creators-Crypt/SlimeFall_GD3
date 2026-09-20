using System.Collections.Generic;
using UnityEngine;

public class SequentialPuzzleManager : MonoBehaviour {
    [Header("Puzzle Order Setup")]
    [Tooltip("Drag the runes into this list in the exact sequence they must be activated.")]
    [SerializeField] private List<EmotionalRune> correctSequence = new();

    [Header("Rewards & Gate")]
    [Tooltip("The Tier 2 Buoyancy Gear pickup that appears or unlocks upon success.")]
    [SerializeField] private GameObject tier2GearPickup;
    [Tooltip("Optional door or gate that slides open when completed.")]
    [SerializeField] private Transform vaultDoor;
    [SerializeField] private Vector3 doorOpenOffset = new(0, -5f, 0);

    private int currentStep = 0;
    private bool isPuzzleSolved = false;

    private void Start() {
        // Ensure the reward gear is hidden or locked away until solved
        if (tier2GearPickup != null && vaultDoor == null)
            tier2GearPickup.SetActive(false);

        InitializeRunes();
    }

    private void InitializeRunes() {
        // Give each rune a reference back to this manager
        foreach (var rune in correctSequence) {
            if (rune != null) rune.SetupManager(this);
        }
    }

    public void OnRuneActivated(EmotionalRune activatedRune) {
        if (isPuzzleSolved) return;

        // Check if the pressed rune matches the next step in our sequence
        if (correctSequence[currentStep] == activatedRune) {
            currentStep++;
            Debug.Log($"Correct! Step {currentStep}/{correctSequence.Count} completed.");

            // Play a success tone/visual effect on the rune itself
            activatedRune.SetState(RuneState.Correct);

            // Check if the entire sequence is complete
            if (currentStep >= correctSequence.Count) {
                CompletePuzzle();
            }
        } else {
            // Wrong choice! Reset the sequence
            Debug.Log("Wrong rune order! Resetting puzzle...");
            ResetPuzzle();
        }
    }

    private void CompletePuzzle() {
        isPuzzleSolved = true;
        Debug.Log("Puzzle Solved! The Deep Vault opens.");

        // Reveal the Tier 2 Gear
        if (tier2GearPickup != null) tier2GearPickup.SetActive(true);

        // Animate or slide the door open
        if (vaultDoor != null) {
            vaultDoor.position += doorOpenOffset;
            // For Unity 6, you could easily swap this for a smooth coroutine or simple tweening
        }

        // Trigger your narrator's dialogue hook here (e.g., Narrator.Speak("Acceptance... at last."))
    }

    private void ResetPuzzle() {
        currentStep = 0;

        // Tell all runes to turn back off
        foreach (var rune in correctSequence) {
            if (rune != null) rune.SetState(RuneState.Off);
        }

        // Hook for the Narrator to sigh or gently mock the player's failure
    }
}