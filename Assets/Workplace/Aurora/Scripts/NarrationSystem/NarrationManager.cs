using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NarrationManager : MonoBehaviour {
    
    [System.Serializable]
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
        
        InitializeDynamicComments();
        narrationLookup = new Dictionary<GameStage, DialogueData>();
        foreach (var mapping in stageNarrations) {
            narrationLookup[mapping.stage] = mapping.dialogue;
        }
    }

    private void Start() {

        queueProcessingCoroutine = StartCoroutine(ProcessDialogueQueue());
    }

    private void OnEnable() {

        GameManager.OnStageChanged += HandleStageChanged;
        GameManager.OnPlayerAction += HandleDynamicAction;
    }

    private void OnDisable() {

        GameManager.OnStageChanged -= HandleStageChanged;
        GameManager.OnPlayerAction -= HandleDynamicAction;
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
    private void HandleStageChanged(GameStage newStage) {
        if (narrationLookup.TryGetValue(newStage, out DialogueData data)) {
            RequestNarration(data);
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

        if (newLine.priority == NarrationPriority.High_CriticalStory) {
            InterruptCurrentLine(newLine);
            return;
        }
        if (activeLine != null &&
            activeLine.priority == NarrationPriority.High_CriticalStory &&
            newLine.priority == NarrationPriority.Low_CasualCommentary) {
            Debug.Log($"Dropped low priority line: '{newLine.subtitleText}' because critical story is playing.");
            return;
        }
        dialogueQueue.Add(newLine);

        dialogueQueue.Sort((line1, line2) => line2.priority.CompareTo(line1.priority));
    }
    private IEnumerator ProcessDialogueQueue() {
        
        while (true) {

            if (activeLine == null && dialogueQueue.Count > 0) {

                activeLine = dialogueQueue[0];
                dialogueQueue.RemoveAt(0);

                PlayAudioAndUI(activeLine);

                float waitTime = activeLine.voiceAudio != null ? activeLine.voiceAudio.length : activeLine.displayDuration;
                yield return new WaitForSeconds(waitTime);

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
        
        if (data.voiceAudio != null) {
            audioSource.PlayOneShot(data.voiceAudio);
        }

        // Send text data to your console / UI box
        Debug.Log($"[{data.speakerName}]: {data.subtitleText} (Priority: {data.priority})");

        // Hook up to UI system here when ready:
        // UIManager.Instance.ShowSubtitle(data.speakerName, data.subtitleText, data.displayDuration);
        if (data.isSpecialIntroLine) {
            // UIManager.Instance.TriggerSplash(data.characterSplashImage, data.titleCardText);
        }
    }
}