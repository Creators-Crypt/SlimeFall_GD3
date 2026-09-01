using UnityEngine;

public class CastleGate : MonoBehaviour, IInteractable {

    [SerializeField] private bool hasKey = false;

    public string InteractionPrompt => (hasKey) ? "Press Z to Enter" : "Go find the key!";

    private void OnEnable() {
        MiniBossCheck.OnKeyGiven += MiniBoss_OnKeyGiven;
    }
    private void OnDisable() {
        MiniBossCheck.OnKeyGiven -= MiniBoss_OnKeyGiven;
    }
    private void MiniBoss_OnKeyGiven(bool state) {
        hasKey = state;
        ObjectiveManager.Instance.SetObjective("Congrats with the Key! Go unlock the Castle.");
    }

    public void Interact() {

        if (!hasKey) {

            GameManager.Instance.SetStage(GameStage.Castle_ForgotKey);
            ActivateKeyQuestEnemy();
        } else {

            GameManager.Instance.SetStage(GameStage.Castle_Explore);
            OpenCastleGate();
        }
    }
    private void ActivateKeyQuestEnemy() {

        ObjectiveManager.Instance.SetObjective("Go find the key!");
    }
    private void OpenCastleGate() {
        
        gameObject.SetActive(false);
        ObjectiveManager.Instance.SetObjective("Go and Explore my lovely Castle!");
    }
}