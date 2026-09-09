using System;
using UnityEngine;

public class TutorialManager : MonoBehaviour {
    
    [Header("Tutorial Barriers")]
    [SerializeField] private GameObject introBarrier;
    [SerializeField] private GameObject entrywayBarrier;

    private void OnEnable() {
        GameManager.OnStageChanged += HandleStageChanged;
        NarrationManager.OnDialogueFinished += HandleDialogueFinished;
    }
    private void OnDisable() {
        GameManager.OnStageChanged -= HandleStageChanged;
        NarrationManager.OnDialogueFinished -= HandleDialogueFinished;
    }
    private void HandleStageChanged(GameStage newStage) {
        switch (newStage) {
            case GameStage.HomeBase_Tut_Spawn:
                HandleIntro();
                break;

            case GameStage.HomeBase_Tut_Entryway:
                HandleEntryway();
                break;

            case GameStage.HomeBase_Tut_Equipment:
                HandleEquipment();
                break;

            case GameStage.HomeBase_Tut_WeaponsMagic:
                HandleWeaponsAndMagic();
                break;

            case GameStage.HomeBase_Tut_Abilities:
                HandleAbilities();
                break;

            case GameStage.HomeBase_Tut_Combat:
                HandleCombat();
                break;

            case GameStage.HomeBase_Tut_Complete:
                HandleTutorialComplete();
                break;
        }
    }
    private void HandleIntro() {
        Debug.Log("Tutorial: Intro started.");

        ObjectiveManager.Instance.SetObjective("Listen to the Archmage");
    }
    private void HandleEntryway() {
        Debug.Log("Tutorial: Entryway started.");

        ObjectiveManager.Instance.SetObjective("Head into the Entryway");
    }
    private void HandleEquipment() {
        Debug.Log("Tutorial: Equipment started.");

        ObjectiveManager.Instance.SetObjective("Pickup Equipment");
    }
    private void HandleWeaponsAndMagic() {
        Debug.Log("Tutorial: Weapons & Magic started.");

        ObjectiveManager.Instance.SetObjective("Cycle between Weapons and Magic elements");
    }
    private void HandleAbilities() {
        Debug.Log("Tutorial: Abilities started.");

        ObjectiveManager.Instance.SetObjective("Test your abilities!");
    }

    private void HandleCombat() {
        Debug.Log("Tutorial: Combat started.");

        ObjectiveManager.Instance.SetObjective("Defeat the targets");
    }

    private void HandleTutorialComplete() {
        Debug.Log("Tutorial: Complete!");

        ObjectiveManager.Instance.SetObjective("Congrats, now please head to your office!");
    }
    private void HandleDialogueFinished(DialogueData data) {
        
        if (GameManager.Instance.GameStage == GameStage.HomeBase_Tut_Spawn) {
            HandleIntroDialogueFinished();
        }
        if (GameManager.Instance.GameStage == GameStage.HomeBase_Tut_Entryway) {
            HandleEntrywayDialogueFinished();
        }
    }
    private void HandleIntroDialogueFinished() {
        
        if (introBarrier != null) {
            introBarrier.SetActive(false);
        }

        Debug.Log("Tutorial: Intro dialogue finished. Barrier removed.");
    }
    private void HandleEntrywayDialogueFinished() {

        if (entrywayBarrier != null) {
            entrywayBarrier.SetActive(false);
        }

        Debug.Log("Tutorial: Entryway dialogue finished. Barrier removed.");
    }
}