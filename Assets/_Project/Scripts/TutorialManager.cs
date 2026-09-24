using System;
using System.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour {
    
    [Header("Tutorial Barriers")]
    [SerializeField] private GameObject introBarrier;
    [SerializeField] private GameObject entrywayBarrier;
    [SerializeField] private GameObject kitchenBarrier;
    [SerializeField] private GameObject officeBarrier;
    [SerializeField] private GameObject bridge;
    [SerializeField] private GameObject shield;

    [Header("Scene Lighting Setup")]
    [SerializeField] private Light sunLight;
    [SerializeField] private float transitionDuration = 2.0f;

    [Header("Day Settings")]
    [SerializeField] private Color daySunColor = Color.white;
    [SerializeField] private float daySunIntensity = 1.0f;
    [SerializeField] private float dayAmbientIntensity = 1.0f;
    [SerializeField] private float daySkyboxExposure = 1.0f;

    [Header("Night Settings")]
    [SerializeField] private Color nightSunColor = new(0.2f, 0.3f, 0.5f);
    [SerializeField] private float nightSunIntensity = 0.05f;
    [SerializeField] private float nightAmbientIntensity = 0.2f;
    [SerializeField] private float nightSkyboxExposure = 0.1f;

    private Coroutine LightingTransitionCO;

    [Header("Tutorial State")]
    private bool tutorialActive = true;
    [SerializeField] private int tutorialPhase = 1;

    [Header("Phase 1 Checklist (Equipment)")]
    private int equipmentPickedUp = 0;
    [SerializeField] private int neededEquipmentPickedUp = 3;

    [Header("Phase 2 Checklist (Weapons)")]
    private int weaponPickedUp = 0;
    [SerializeField] private int neededWeaponPickedUp = 1;
    private bool weaponCycled, weaponFired, magicCycled;
    //private bool isChestOpen; TODO: Add this to the Tutorial

    [Header("Phase 3 Checklist (Abilities & Atmosphere)")]
    private bool flashlightUsed;
    private bool jumped; 
    private bool teleported;
    [SerializeField] private bool isPlayerAtPlatforms;
    [SerializeField] private bool reachedLastPlatforms;
    [SerializeField] private bool isOnLastPlatform;

    [Header("Phase 4 Checklist (Combat Arena)")]
    private int targetsDefeated = 0;
    [SerializeField] private int neededTargetsDefeated = 3;
    private bool dodged, concentrated;

    private void OnEnable() {
        if (GameManager.Instance != null) {
            GameManager.Instance.OnStageChanged += HandleStageChanged;
            GameManager.Instance.OnPlayerAction += HandlePlayerAction;
        }
        
        NarrationManager.OnDialogueFinished += HandleDialogueFinished;
        GameInitializer.OnSceneSetupComplete += EvaluateCurrentSceneStage;
    }
    private void OnDisable() {
        if (GameManager.Instance != null) {
            GameManager.Instance.OnStageChanged -= HandleStageChanged;
            GameManager.Instance.OnPlayerAction -= HandlePlayerAction;
        }

        NarrationManager.OnDialogueFinished -= HandleDialogueFinished;
        GameInitializer.OnSceneSetupComplete -= EvaluateCurrentSceneStage;
    }
    private void EvaluateCurrentSceneStage() {

        if (GameManager.Instance != null) {
            Debug.Log($"[NarrationManager] CatchUpInitialStage triggered. Evaluating current stage: {GameManager.Instance.GameStage}");
            HandleStageChanged(GameManager.Instance.GameStage);
        } else {
            Debug.LogError("[NarrationManager] CatchUp failed: GameManager.Instance is still NULL during setup complete!");
        }
    }
    public void HandleStageChanged(GameStage newStage) {
        switch (newStage) {
            case GameStage.HomeBase_Tut_Spawn:          HandleIntro(); break;
            case GameStage.HomeBase_Tut_Entryway:       HandleEntryway(); break;
            case GameStage.HomeBase_Tut_Equipment:      HandleEquipment(); break;
            case GameStage.HomeBase_Tut_WeaponsMagic:   HandleWeaponsAndMagic(); break;
            case GameStage.HomeBase_Tut_Abilities:      HandleAbilities(); break;
            case GameStage.HomeBase_Tut_Combat:         HandleCombat(); break;
            case GameStage.HomeBase_Tut_Complete:       HandleTutorialComplete(); break;
            case GameStage.HomeBase_Tut_Kitchen:        HandleKitchen(); break;
            case GameStage.HomeBase_Tut_Office:         HandleOffice(); break;
            case GameStage.HomeBase_Tut_Portal:         HandlePortal(); break;
        }
    }
    private void HandleIntro() {
        ObjectiveManager.Instance.SetObjective("Listen to the Archmage");
    }
    private void HandleEntryway() {
        ObjectiveManager.Instance.SetObjective("Head into the Entryway");
    }
    private void HandleEquipment() {
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

        DimGlobalLightsToNight(true);

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
    private void HandleKitchen() {
        Debug.Log("Kitchen started");
    }
    private void HandleOffice() {
        ObjectiveManager.Instance.SetObjective("Kitchen, is W.I.P., please use the key to enter the office!");
    }
    private void HandlePortal() {
        ObjectiveManager.Instance.SetObjective("Proceed through the Portal!");
    }
    private void DimGlobalLightsToNight(bool state) {
        
        if (LightingTransitionCO != null) {
            StopCoroutine(LightingTransitionCO);
        }
        LightingTransitionCO = StartCoroutine(TransitionLightingRoutine(state));
    }
    private IEnumerator TransitionLightingRoutine(bool toNight) {

        float elapsedTime = 0f;

        Color startSunColor = sunLight != null ? sunLight.color : daySunColor;
        float startSunIntensity = sunLight != null ? sunLight.intensity : daySunIntensity;
        float startAmbient = RenderSettings.ambientIntensity;

        float startSkybox = 1f;
        if (RenderSettings.skybox != null && RenderSettings.skybox.HasProperty("_Exposure")) {
            startSkybox = RenderSettings.skybox.GetFloat("_Exposure");
        }

        Color targetSunColor = toNight ? nightSunColor : daySunColor;
        float targetSunIntensity = toNight ? nightSunIntensity : daySunIntensity;
        float targetAmbient = toNight ? nightAmbientIntensity : dayAmbientIntensity;
        float targetSkybox = toNight ? nightSkyboxExposure : daySkyboxExposure;

        Debug.Log(toNight ? "<color=purple>LIGHTING: Transitioning to Night...</color>"
                          : "<color=yellow>LIGHTING: Restoring Day...</color>");

        while (elapsedTime < transitionDuration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;

            // Smoothly interpolate between values using Lerp
            if (sunLight != null) {
                sunLight.color = Color.Lerp(startSunColor, targetSunColor, t);
                sunLight.intensity = Mathf.Lerp(startSunIntensity, targetSunIntensity, t);
            }

            RenderSettings.ambientIntensity = Mathf.Lerp(startAmbient, targetAmbient, t);

            if (RenderSettings.skybox != null && RenderSettings.skybox.HasProperty("_Exposure")) {
                RenderSettings.skybox.SetFloat("_Exposure", Mathf.Lerp(startSkybox, targetSkybox, t));
            }

            yield return null;
        }
        if (sunLight != null) {
            sunLight.color = targetSunColor;
            sunLight.intensity = targetSunIntensity;
        }
        RenderSettings.ambientIntensity = targetAmbient;
        if (RenderSettings.skybox != null && RenderSettings.skybox.HasProperty("_Exposure")) {
            RenderSettings.skybox.SetFloat("_Exposure", targetSkybox);
        }
    }
    private void HandleDialogueFinished(DialogueData data) {
        
        if (GameManager.Instance.GameStage == GameStage.HomeBase_Tut_Spawn) {
            HandleIntroDialogueFinished();
        }
        if (GameManager.Instance.GameStage == GameStage.HomeBase_Tut_Entryway) {
            HandleEntrywayDialogueFinished();
        }
        if (GameManager.Instance.GameStage == GameStage.HomeBase_Tut_Complete) {
            HandlePlayerActionsDialogueFinished();
        }
        if (GameManager.Instance.GameStage == GameStage.HomeBase_Tut_Kitchen) {
            HandleKitchenEntranceDialogueFinished();
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
    private void HandlePlayerActionsDialogueFinished() {

        if (bridge != null) bridge.SetActive(true);
        if (shield != null) shield.SetActive(false);
    }
    private void HandleKitchenEntranceDialogueFinished() {

        if (kitchenBarrier != null) {
            kitchenBarrier.SetActive(false);
        }
        Debug.Log("Tutorial: Kitchen Entryway dialogue finished. Barrier removed.");
    }
    private void HandleOfficeEntranceDialogueFinished() {

        if (officeBarrier != null) {
            officeBarrier.SetActive(false);
        }
        Debug.Log("Tutorial: Office Entryway dialogue finished. Barrier removed.");
    }
    private void HandlePlayerAction(string actionKey) {
        if (!tutorialActive) return;

        switch (actionKey) {
            #region Phase 1
            case "EquipmentPickedUp":
                if (equipmentPickedUp < neededEquipmentPickedUp) { equipmentPickedUp++; }
                break;
            #endregion
            #region Phase 2
            case "WeaponPickup":
                if (weaponPickedUp < neededWeaponPickedUp) { weaponPickedUp++; }
                break;
            case "WeaponCycle":     weaponCycled = true; break;
            case "WeaponFire":      weaponFired = true; break;
            case "MagicCycle":      magicCycled = true; break;
            //case "ChestOpen":       isChestOpen = true; break;

            #endregion
            #region Phase 3
            case "Flashlight":      flashlightUsed = true; break;   
            case "Jump":            jumped = true; break;
            case "PlayerReachedPlatforms": isPlayerAtPlatforms = true; break;
            case "NeedToCrossLastPlatforms": reachedLastPlatforms = true; break;
            case "Teleport":        teleported = true; break;
            case "OnLastPlatform":  isOnLastPlatform = true; break;
            #endregion
            #region Phase 4
            case "Dodge":           dodged = true; break;
            case "Concentrate":     concentrated = true; break;
            case "TargetDefeated":
                if (targetsDefeated < neededTargetsDefeated) { targetsDefeated++; }
                break;
                #endregion
        }
        UpdateTutorialUI();
        CheckTutorialProgress();
    }
    private void UpdateTutorialUI() {
        string tutorialText = "TUTORIAL\n";

        if (tutorialPhase == 1) {
            tutorialText += $"Gather Armor & Gear ({equipmentPickedUp}/{neededEquipmentPickedUp})";
        } 
        else if (tutorialPhase == 2) {
            
            tutorialText += weaponPickedUp >= neededWeaponPickedUp ? " [X] Claim a Weapon\n" : " [ ] Claim a Weapon\n";
            tutorialText += weaponCycled ? " [X] Swap Weapons [Scrollwheel]\n" : " [ ] Swap Weapons [Scrollwheel]\n";
            tutorialText += magicCycled ? " [X] Cycle Elements [1,2,3,4]\n" : " [ ] Cycle Elements [1,2,3,4]\n";
            tutorialText += weaponFired ? " [X] Fire Weapon [RMB]" : " [ ] Fire Weapon [RMB]";
        } 
        else if (tutorialPhase == 3) {

            if (!flashlightUsed && tutorialPhase == 3) {
                tutorialText += flashlightUsed ? " [X] Use Flashlight [F]\n" : " [ ] Use Flashlight [F] (It's Dark!)\n";
            } else if (reachedLastPlatforms) {
                tutorialText += teleported ? " [X] Teleport [T]" : " [ ] Teleport [T]\n";
                tutorialText += "Traverse toward the last two platforms.";
            } else if (isPlayerAtPlatforms) {
                tutorialText += jumped ? " [X] Jump [SPACE]\n" : " [ ] Jump [SPACE]\n";
            } else {
                tutorialText += "Proceed through the Platform Section.";
            }  
        } 
        else if (tutorialPhase == 4) {
            
            tutorialText += dodged ? " [X] Dodge [L ALT]\n" : " [ ] Dodge [L ALT]\n";
            tutorialText += concentrated ? " [X] Concentrate [C]\n" : " [ ] Concentrate [C]\n";
            tutorialText += $"Defeat Training Targets ({targetsDefeated}/{neededTargetsDefeated})";
        } 
        else if (tutorialPhase == 5) {
            tutorialText += "Congrats, now please head to the Kitchen!";
        }
        ObjectiveManager.Instance.SetObjective(tutorialText);
    }
    private void CheckTutorialProgress() {

        if (tutorialPhase == 1 && equipmentPickedUp >= neededEquipmentPickedUp) {
            GameManager.Instance.SetStage(GameStage.HomeBase_Tut_WeaponsMagic);
        } else if (tutorialPhase == 2 && weaponPickedUp >= neededWeaponPickedUp && weaponCycled && weaponFired && magicCycled) {
            GameManager.Instance.SetStage(GameStage.HomeBase_Tut_Abilities);
        } else if (tutorialPhase == 3 && flashlightUsed) {
            DimGlobalLightsToNight(false);
        } else if (tutorialPhase == 3 && isOnLastPlatform) {
            GameManager.Instance.SetStage(GameStage.HomeBase_Tut_Combat);
        } 
        else if (tutorialPhase == 4 && dodged && concentrated && targetsDefeated >= neededTargetsDefeated) {
            GameManager.Instance.SetStage(GameStage.HomeBase_Tut_Complete);
        } else if (tutorialPhase == 5) {
            GameManager.Instance.SetStage(GameStage.HomeBase_Tut_Kitchen);
        }
    }
}