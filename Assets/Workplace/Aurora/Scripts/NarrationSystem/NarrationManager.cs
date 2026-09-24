using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrationManager : MonoBehaviour {

    public static Action<DialogueData> OnDialogueFinished;

    [Serializable]
    public struct StageDialogueMapping {
        public GameStage stage;
        public DialogueData dialogue;
    }
    private Dictionary<GameStage, DialogueData> narrationLookup;
    private List<DialogueData> dialogueQueue = new();

    [Header("Setup Stages")]
    [SerializeField] private List<StageDialogueMapping> stageNarrations;
    [SerializeField] private AudioSource audioSource;

    [Header("Setup Comments")]
    [SerializeField] private List<DynamicCommentGroup> dynamicComments;
    private Dictionary<string, DynamicCommentGroup> commentLookup; 

    // The current line actively playing right now
    private DialogueData activeLine;

    private Coroutine queueProcessingCoroutine;

    private void Awake() {

        Debug.Log("<color=lime>NARRATION MANAGER AWAKE</color>");

        InitializeDynamicComments();
        BuildNarrationLookupTable();
        queueProcessingCoroutine = StartCoroutine(ProcessDialogueQueue());
    }
    private void OnEnable() {

        Debug.Log("<color=yellow>NARRATION MANAGER ENABLED</color>");

        GameManager.Instance.OnStageChanged += HandleStageChanged;
        GameManager.Instance.OnPlayerAction += HandleDynamicAction;

        GameInitializer.OnSceneSetupComplete += CatchUpInitialStage;
    }

    private void OnDisable() {

        Debug.Log("<color=yellow>NARRATION MANAGER DISABLED</color>");

        GameManager.Instance.OnStageChanged -= HandleStageChanged;
        GameManager.Instance.OnPlayerAction -= HandleDynamicAction;
        GameInitializer.OnSceneSetupComplete -= CatchUpInitialStage;
    }
    private void BuildNarrationLookupTable() {
        if (narrationLookup != null && narrationLookup.Count > 0) return;

        InitializeDynamicComments();
        narrationLookup = new Dictionary<GameStage, DialogueData>();

        if (stageNarrations == null) {
            Debug.LogError("[NarrationManager] Critical error! 'Stage Narrations' list is unassigned in the inspector.");
            return;
        }

        foreach (var mapping in stageNarrations) {
            if (mapping.dialogue != null) {
                narrationLookup[mapping.stage] = mapping.dialogue;
            }
        }

        Debug.Log($"[NarrationManager] Lookup table successfully compiled with {narrationLookup.Count} stage tracks.");
    }
    private void CatchUpInitialStage() {

        BuildNarrationLookupTable();

        if (GameManager.Instance != null) {
            Debug.Log($"[NarrationManager] CatchUpInitialStage triggered. Evaluating current stage: {GameManager.Instance.GameStage}");
            HandleStageChanged(GameManager.Instance.GameStage);
        }
    }
    private void InitializeDynamicComments() {

        commentLookup = new Dictionary<string, DynamicCommentGroup>();

        foreach (var group in dynamicComments) {
            
            if (!string.IsNullOrEmpty(group.actionKey)) {

                commentLookup[group.actionKey] = group;
            }
        }
    }
    // This handles primary story milestones from the GameManager
    public void HandleStageChanged(GameStage newStage) {

        Debug.Log($"[NarrationManager] HandleStageChanged intercepting stage: {newStage}. Lookup table size: {(narrationLookup != null ? narrationLookup.Count : 0)}");

        if (narrationLookup == null) {
            BuildNarrationLookupTable();
        }

        if (narrationLookup.TryGetValue(newStage, out DialogueData data)) {
            Debug.Log($"<color=lime>[NarrationManager] MATCH FOUND FOR:</color> {newStage}. Playing clip: {data.name}");
            RequestNarration(data);
        } else {
            Debug.LogWarning($"[NarrationManager] WARNING: No dialogue clip asset mapped to enum state: {newStage}. Check your inspector mapping elements array list.");
        }
    }
    // This handles real-time actions (combat, falling, smashing objects)
    private void HandleDynamicAction(string actionKey) {
        
        // Check if we have a registered DM comment group for this specific action
        if (commentLookup.TryGetValue(actionKey, out DynamicCommentGroup group)) {
            DialogueData chosenLine = group.GetRandomLine();
            if (chosenLine != null) {
                // Send it right into our priority queue system!
                RequestNarration(chosenLine);
            }
        }
    }
    // Call this function whenever you want the DM to say something!
    public void RequestNarration(DialogueData newLine) {
        
        if (newLine == null) return;

        Debug.Log($"[NarrationManager] RequestNarration received line: '{newLine.name}' with Priority: {newLine.priority}");

        if (newLine.priority == NarrationPriority.High_CriticalStory) {
            if (activeLine != null) {
                InterruptCurrentLine(newLine);
            } else {
                dialogueQueue.Insert(0, newLine);
            }
            return;
        }

        if (activeLine != null &&
            activeLine.priority == NarrationPriority.High_CriticalStory &&
            newLine.priority == NarrationPriority.Low_CasualCommentary) {
            Debug.Log(
                $"Dropped low priority line: '{newLine.subtitleText}' because critical story is playing."
            );
            return;
        }

        dialogueQueue.Add(newLine);

        dialogueQueue.Sort(
            (line1, line2) => line2.priority.CompareTo(line1.priority)
        );
    }
    private IEnumerator ProcessDialogueQueue() {
        
        while (true) {

            if (activeLine == null && dialogueQueue.Count > 0) {

                activeLine = dialogueQueue[0];
                dialogueQueue.RemoveAt(0);

                PlayAudioAndUI(activeLine);

                float waitTime = activeLine.voiceAudio != null ? activeLine.voiceAudio.length : activeLine.displayDuration;
                yield return new WaitForSeconds(waitTime);

                OnDialogueFinished?.Invoke(activeLine);

                activeLine = null;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void InterruptCurrentLine(DialogueData criticalLine) {
        
        Debug.Log("<color=red>CRITICAL INTERRUPT!</color> Stopping current dialogue for main story.");

        if (queueProcessingCoroutine != null) StopCoroutine(queueProcessingCoroutine);
        audioSource.Stop();

        dialogueQueue.Insert(0, criticalLine);
        activeLine = null;

        queueProcessingCoroutine = StartCoroutine(ProcessDialogueQueue());
    }
    private void PlayAudioAndUI(DialogueData data) {

        Debug.Log($"PLAYING NARRATION: {data.name}");

        if (audioSource == null) {
            Debug.LogError("NarrationManager: No AudioSource assigned!");
            return;
        }

        if (data.voiceAudio != null) {
            Debug.Log(
                $"<color=cyan>NARRATION PLAY:</color> " +
                $"'{data.subtitleText}' | Clip: {data.voiceAudio.name}"
            );

            audioSource.clip = data.voiceAudio;
            audioSource.Play();
        } else {
            Debug.LogWarning(
                $"NarrationManager: Dialogue '{data.name}' has no AudioClip."
            );
        }

        Debug.Log(
            $"[{data.speakerName}]: {data.subtitleText} " +
            $"(Priority: {data.priority})"
        );

        if (data.isSpecialIntroLine) {
            // UIManager.Instance.TriggerSplash(data.characterSplashImage, data.titleCardText);
        }
        
    }
}