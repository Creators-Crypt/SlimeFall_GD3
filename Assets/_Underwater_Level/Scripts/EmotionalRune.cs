using UnityEngine;

public enum RuneState { Off, Correct, Failed }

public class EmotionalRune : MonoBehaviour, IInteractable {
    [Header("Rune Identity")]
    [SerializeField] private string emotionName = "Sadness";
    [SerializeField] private Material litMaterial;
    [SerializeField] private Material unlitMaterial;

    private SequentialPuzzleManager manager;
    private Renderer runeRenderer;
    private bool isAlreadyActivated = false;

    public string InteractionPrompt => $"Invoke {emotionName}";

    private void Awake() {
        runeRenderer = GetComponent<Renderer>();
        if (runeRenderer != null && unlitMaterial != null) {
            runeRenderer.material = unlitMaterial;
        }
    }
    public void SetupManager(SequentialPuzzleManager puzzleManager) { manager = puzzleManager; }
    public void Interact() {

        if (isAlreadyActivated) return;

        if (manager != null) {
            manager.OnRuneActivated(this);
        }
    }
    public void SetState(RuneState state) {
        switch (state) {
            case RuneState.Off:
                isAlreadyActivated = false;
                if (runeRenderer != null && unlitMaterial != null) runeRenderer.material = unlitMaterial;
                break;

            case RuneState.Correct:
                isAlreadyActivated = true;
                if (runeRenderer != null && litMaterial != null) runeRenderer.material = litMaterial;
                // Pro Tip: Instantiate an underwater bubble particle effect here!
                break;
        }
    }
}