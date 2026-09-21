using System;
using UnityEngine;

public class ObjectiveManager : Singleton<ObjectiveManager> {

    public static event Action<string> OnObjectiveChanged;

    private string currentObjective;

    public void SetObjective(string newObjective)
    {
        currentObjective = newObjective;
        OnObjectiveChanged?.Invoke(currentObjective);
    }    
}
