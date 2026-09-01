using UnityEngine;
using TMPro;

public class MainObjectiveUI : Singleton<MainObjectiveUI> {

    [SerializeField] private TextMeshProUGUI objectiveText;

    public void SetObjective(string newObjective) {
        objectiveText.text = newObjective;
    }
}