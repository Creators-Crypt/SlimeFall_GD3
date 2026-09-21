using UnityEngine;
using TMPro;

public class MainObjectiveUI : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI objectiveText;

    private void OnEnable() {
        ObjectiveManager.OnObjectiveChanged += SetObjective;
    }
    private void OnDisable() {
        ObjectiveManager.OnObjectiveChanged -= SetObjective;
    }
    public void SetObjective(string newObjective) {
        objectiveText.text = newObjective;
    }
}