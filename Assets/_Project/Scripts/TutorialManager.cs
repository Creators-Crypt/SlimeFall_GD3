using System;
using UnityEngine;

public class TutorialManager : MonoBehaviour {
    
    [Header("Tutorial Barriers")]
    [SerializeField] private GameObject introBarrier;
    [SerializeField] private GameObject entrywayBarrier;

    [Header("Tutorial State")]
    private bool tutorialActive = true;
    [SerializeField] private int tutorialPhase = 0;

    [Header("Phase 1 Checklist")]
    private int equipmentPickedUp = 0;
    [SerializeField] private int neededEquipmentPickedUp = 3;

    [Header("Phase 2 Checklist")]
    private int weaponPickedUp = 0;
    [SerializeField] private int neededWeaponPickedUp = 1;
    private bool weaponCycled, weaponFired, magicCycled;

    [Header("Phase 3 Checklist")]
    private bool jumped, teleported;

    [Header("Phase 4 Checklist")]
    private int targetsDefeated = 0;
    [SerializeField] private int neededTargetsDefeated = 3;
    private bool dodged, concentrated;

    [Header("Phase 5 Checklist")]
    private bool flashlightUsed;

    private void OnEnable() {
        GameManager.OnStageChanged += HandleStageChanged;
        GameManager.OnPlayerAction += HandlePlayerAction;
        NarrationManager.OnDialogueFinished += HandleDialogueFinished;
        StageTrigger.OnPhaseUpdate += HandleTutorialPhaseUpdate;
    }
    private void OnDisable() {
        GameManager.OnStageChanged -= HandleStageChanged;
        GameManager.OnPlayerAction -= HandlePlayerAction;
        NarrationManager.OnDialogueFinished -= HandleDialogueFinished;
        StageTrigger.OnPhaseUpdate -= HandleTutorialPhaseUpdate;
    }
    private void HandleStageChanged(GameStage newStage) {
        switch (newStage) {
            case GameStage.HomeBase_Tut_Spawn:          HandleIntro(); break;
            case GameStage.HomeBase_Tut_Entryway:       HandleEntryway(); break;
            case GameStage.HomeBase_Tut_Equipment:      HandleEquipment(); break;
            case GameStage.HomeBase_Tut_WeaponsMagic:   HandleWeaponsAndMagic(); break;
            case GameStage.HomeBase_Tut_Abilities:      HandleAbilities(); break;
            case GameStage.HomeBase_Tut_Combat:         HandleCombat(); break;
            case GameStage.HomeBase_Tut_Complete:       HandleTutorialComplete(); break;
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
        tutorialPhase = 1;
        UpdateTutorialUI();
    }
    private void HandleWeaponsAndMagic() {
        Debug.Log("Tutorial: Weapons & Magic started.");
        tutorialPhase = 2;
        UpdateTutorialUI();
    }
    private void HandleAbilities() {
        Debug.Log("Tutorial: Abilities started.");
        tutorialPhase = 3;
        UpdateTutorialUI();
    }
    private void HandleCombat() {
        Debug.Log("Tutorial: Combat started.");
        tutorialPhase = 4;
        UpdateTutorialUI();
    }
    private void HandleTutorialComplete() {
        Debug.Log("Tutorial: Complete!");
        tutorialPhase = 5;
        UpdateTutorialUI();
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
    private void HandlePlayerAction(string actionKey) {
        if (!tutorialActive) return;

        switch (actionKey) {

            case "EquipmentPickedUp":
                if (equipmentPickedUp < neededEquipmentPickedUp) { equipmentPickedUp++; }
                break;
            case "WeaponPickup":
                if (weaponPickedUp < neededWeaponPickedUp) { weaponPickedUp++; }
                break;
            case "TargetDefeated":
                if (targetsDefeated < neededTargetsDefeated) { targetsDefeated++; }
                break;
            case "WeaponCycle":     weaponCycled = true; break;
            case "WeaponFire":      weaponFired = true; break;
            case "MagicCycle":      magicCycled = true; break;
            case "Jump":            jumped = true; break;
            case "Teleport":        teleported = true; break;
            case "Dodge":           dodged = true; break;
            case "Concentrate":     concentrated = true; break;
            case "Flashlight":      flashlightUsed = true; break;   
        }
        UpdateTutorialUI();
        CheckTutorialProgress();
    }
    private void UpdateTutorialUI() {
        string tutorialText = "TUTORIAL\n";

        if (tutorialPhase == 1) {
            tutorialText += $"Gather Armor & Gear ({equipmentPickedUp}/{neededEquipmentPickedUp})";
        } else if (tutorialPhase == 2) {
            tutorialText += weaponPickedUp >= neededWeaponPickedUp ? " [X] Claim a Weapon\n" : " [ ] Claim a Weapon\n";
            tutorialText += weaponCycled ? " [X] Swap Weapons [Scrollwheel]\n" : " [ ] Swap Weapons [Scrollwheel]\n";
            tutorialText += magicCycled ? " [X] Cycle Elements [R]\n" : " [ ] Cycle Elements [R]\n";
            tutorialText += weaponFired ? " [X] Fire Weapon [RMB]" : " [ ] Fire Weapon [RMB]";
        } else if (tutorialPhase == 3) {
            tutorialText += jumped ? " [X] Jump [SPACE]\n" : " [ ] Jump [SPACE]\n";
            tutorialText += teleported ? " [X] Teleport [E]" : " [ ] Teleport [E]";
        } else if (tutorialPhase == 4) {
            tutorialText += dodged ? " [X] Dodge [L ALT]\n" : " [ ] Dodge [L ALT]\n";
            tutorialText += concentrated ? " [X] Concentrate [C]\n" : " [ ] Concentrate [C]\n";
            tutorialText += $"Defeat Training Targets ({targetsDefeated}/{neededTargetsDefeated})";
        } else if (tutorialPhase == 5) {
            tutorialText += flashlightUsed ? " [X] Use Flashlight [F]\n" : " [ ] Use Flashlight [F]\n";
            tutorialText += "Congrats, now please head to your office!";
        }
        ObjectiveManager.Instance.SetObjective(tutorialText);
    }

    private void CheckTutorialProgress() {

        if (tutorialPhase == 1 && equipmentPickedUp >= neededEquipmentPickedUp) {
            GameManager.Instance.SetStage(GameStage.HomeBase_Tut_WeaponsMagic);
        }
        else if (tutorialPhase == 2 && weaponPickedUp >= neededWeaponPickedUp && weaponCycled && weaponFired && magicCycled) {
            GameManager.Instance.SetStage(GameStage.HomeBase_Tut_Abilities);
        }
        else if (tutorialPhase == 3 && jumped && teleported) {
            GameManager.Instance.SetStage(GameStage.HomeBase_Tut_Combat);
        }
        else if (tutorialPhase == 4 && dodged && concentrated && targetsDefeated >= neededTargetsDefeated) {
            GameManager.Instance.SetStage(GameStage.HomeBase_Tut_Complete);
        }
        else if (tutorialPhase == 5 && flashlightUsed) {
            tutorialActive = false;
            ObjectiveManager.Instance.SetObjective("Proceed to your office.");
            Debug.Log("Tutorial completely cleared!");
        }
    }
    private void HandleTutorialPhaseUpdate(int value) {
        tutorialPhase = value;
    }
}