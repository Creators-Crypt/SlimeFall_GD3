using UnityEngine;

public class LabyrinthObjective : MonoBehaviour {

    public enum ObjectiveType { SolvePuzzle, FindItem, FightSlime }
    public ObjectiveType roomType;

    public void OnObjectiveCompleted() {

        GameManager.Instance.PlayerPerformAction($"Completed_{roomType}");
        GameManager.Instance.SetStage(GameStage.Challenge_ObjectiveRoom);
    }
}