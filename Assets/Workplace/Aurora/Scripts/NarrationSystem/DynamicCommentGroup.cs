using System.Collections.Generic;
using UnityEngine;

public class DynamicCommentGroup : MonoBehaviour {

    [Header("Trigger Settings")]
    [Tooltip("The unique text key that triggers this comment pool (e.g., 'Player_Fell')")]
    public string actionKey;

    [Header("Dialogue Options")]
    [Tooltip("The DM will pick one random line from this list when the action happens.")]
    public List<DialogueData> commentOptions;

    public DialogueData GetRandomLine() {

        if (commentOptions == null || commentOptions.Count == 0) return null;

        int randomIndex = Random.Range(0, commentOptions.Count);
        return commentOptions[randomIndex];
    }
}