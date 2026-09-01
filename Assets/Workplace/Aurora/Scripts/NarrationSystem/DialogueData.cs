using UnityEngine;

public enum NarrationPriority {

    Low_CasualCommentary,
    Medium_QuestUpdate,
    High_CriticalStory
}

[CreateAssetMenu(fileName = "New Dialogue Line", menuName = "Narration/Dialogue Line")]
public class DialogueData : ScriptableObject {

    public string speakerName = "The DM";
    [TextArea(3, 5)] public string subtitleText;
    public AudioClip voiceAudio;
    public float displayDuration = 3f;
    public NarrationPriority priority;

    [Header("Extra Settings")]
    public bool isSpecialIntroLine;
    public Sprite characterSplashImage;
    public string titleCardText = "The Slime Lover";
}