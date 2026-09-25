using UnityEngine;

public class StageTrigger : MonoBehaviour {

    public enum TriggerExecutionMode { 
        None, //None is default and here to ensure a manual switch.
        StageChangeOnly,
        PlayerActionOnly,
        BothStageAndAction
    }

    [SerializeField] private TriggerExecutionMode executionMode = TriggerExecutionMode.None;

    [Header("Stage Settings")]
    [SerializeField] private GameStage stageToTrigger;
    [SerializeField] private string playerAction;

    [Header("Filter Settings")]
    [Tooltip("If true, this trigger will only activate if the game is currently on this specific stage.")]
    [SerializeField] private bool requireCurrentStage;
    [SerializeField] private GameStage expectedCurrentStage;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other) {

        if (!other.CompareTag("Player")) return;

        if (hasTriggered) return;


        Debug.Log($"[StageTrigger] Player tag confirmed. Current GameManager Stage: {GameManager.Instance.GameStage}");

        if (requireCurrentStage && GameManager.Instance.GameStage != expectedCurrentStage) {
            Debug.Log($"[StageTrigger] Trigger expected stage '{expectedCurrentStage}', but GameManager is currently on '{GameManager.Instance.GameStage}'.");
            return;
        }

        hasTriggered = true;

        Debug.Log($"[StageTrigger] Conditions cleared. Executing Mode: {executionMode}");

        switch (executionMode) {
            case TriggerExecutionMode.None:
                Debug.LogWarning("[StageTrigger] Warning: TriggerExecutionMode is set to None! No actions taken.");
                break;
            case TriggerExecutionMode.StageChangeOnly:
                ExecuteStageChange();
                break;
            case TriggerExecutionMode.PlayerActionOnly:
                ExecutePlayerAction();
                break;
            case TriggerExecutionMode.BothStageAndAction:
                ExecuteStageChange();
                ExecutePlayerAction();
                break;
        }

        Debug.Log($"[StageTrigger] Execution completed successfully. Destroying trigger volume object: {gameObject.name}");
        Destroy(gameObject);
        
    }
    private void ExecuteStageChange() {
        GameManager.Instance.SetStage(stageToTrigger);
    }
    private void ExecutePlayerAction() {
        GameManager.Instance.PlayerPerformAction(playerAction);
    }
}