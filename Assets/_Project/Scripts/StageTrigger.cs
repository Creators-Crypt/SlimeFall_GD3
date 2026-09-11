using System;
using UnityEngine;

public class StageTrigger : MonoBehaviour {

    public static Action<int> OnPhaseUpdate;
    
    [Header("Stage Settings")]
    [SerializeField] private GameStage stageToTrigger;
    [SerializeField] private int phaseValue;

    [Header("Filter Settings")]
    [Tooltip("If true, this trigger will only activate if the game is currently on this specific stage.")]
    [SerializeField] private bool requireCurrentStage;
    [SerializeField] private GameStage expectedCurrentStage;

    private void OnTriggerEnter(Collider other) {

        if (other.CompareTag("Player")) {

            if (requireCurrentStage && GameManager.Instance.GameStage != expectedCurrentStage)  return;

            Debug.Log($"<color=orange>STAGE TRIGGER ACTIVATED:</color> Shifting game to {stageToTrigger}");

            GameManager.Instance.SetStage(stageToTrigger);
            OnPhaseUpdate?.Invoke(phaseValue);

            Destroy(gameObject);
        }
    }
}