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

    private void OnTriggerEnter(Collider other) {

        if (other.CompareTag("Player")) {

            if (requireCurrentStage && GameManager.Instance.GameStage != expectedCurrentStage)  return;

            switch (executionMode) {
                case TriggerExecutionMode.None:
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
            Destroy(gameObject);
        }
    }
    private void ExecuteStageChange() {
        GameManager.Instance.SetStage(stageToTrigger);
    }
    private void ExecutePlayerAction() {
        GameManager.Instance.PlayerPerformAction(playerAction);
    }
}