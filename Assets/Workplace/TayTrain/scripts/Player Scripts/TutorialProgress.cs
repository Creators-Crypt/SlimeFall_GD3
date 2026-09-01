using UnityEngine;

public class TutorialProgress : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject tutorialCage;
    [SerializeField] private GameObject tutorialSpawner;

    [Header("Tutorial Progress")]
    private bool tutorialActive;
    private int tutorialPhase = 1;

    private bool jumped;
    private bool dodged;
    private bool teleported;
    private bool concentrated;
    private bool flashlightUsed;

    //Uncomment once weapon/spell switching reports this action
    //private bool attackChanged;

    //Uncomment when Equipment UI reports this action
    //private bool equipmentShown;

    private int slimeKills;
    private int slimeKillsNeeded = 9;

    private void OnEnable()
    {
        GameManager.OnPlayerAction += HandlePlayerAction;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerAction -= HandlePlayerAction;
    }

    public void StartTutorial()
    {
        tutorialActive = true;
        UpdateTutorialUI();
    }
    
    private void HandlePlayerAction(string actionKey)
    {
        if (!tutorialActive)
            return;

        switch(actionKey)
        {
            case "Jump":
                jumped = true;
                break;

            case "Dodge":
                dodged = true;
                break;

            case "Teleport":
                teleported = true;
                break;

            case "Concentrate":
                concentrated = true;
                break;

            case "Flashlight":
                flashlightUsed = true;
                break;

            //Uncomment once weapon/spell switching is connected
            //case "weaponSpellSwitch":
            //    attackChanged = true;
            //    break;

            //Uncomment when equipmentUi pull up is connected
            //case "EquipmentUI":
            //    equipmentShown = true;
            //    break;

            case "SlimeKilled":
                if(slimeKills < slimeKillsNeeded)
                {
                    slimeKills++;
                }
                break;

        }

        UpdateTutorialUI();
        CheckTutorialComplete();
    }
    private void UpdateTutorialUI()
    {
        string tutorialText = "TUTORIAL\n";

        if (tutorialPhase == 1)
        {
            tutorialText += jumped ? " X Jump [SPACE] " : " Jump [SPACE] ";
            tutorialText += dodged ? " X Dodge [L ALT] " : " Dodge [L ALT] ";
            tutorialText += teleported ? " X Teleport [E]\n" : " Teleport [E]\n";
            tutorialText += concentrated ? " X Concentrate [C] " : " Concentrate [C] ";
        }
        else if (tutorialPhase == 2)
        { 
            tutorialText += flashlightUsed ? " X Flashlight [F] " : " Flashlight [F] ";

        //Uncomment when weapon/spell is connected
        //tutorialText += attackSwitched ? " X Switch Attack [SCROLL]" : " Switch Attack [SCROLL}";

        //Uncomment when Equipment UI is connected
        //tutorailText += equipmentShown : " X Equipment [TAB] " : " Equipment [TAB] ";
        tutorialText += "Kill Slimes (" + slimeKills + "/" + slimeKillsNeeded + ")";
        }   
        ObjectiveManager.Instance.SetObjective(tutorialText);

    }
    private void CheckTutorialComplete()
    {
        //Phase 1 
        if (tutorialPhase == 1)
        {
            if (jumped && dodged && teleported && concentrated )
            {
                tutorialPhase = 2;
                UpdateTutorialUI();
            }
            return;
        }

        //Phase 2
        if(tutorialPhase ==2)
        {
            //When EquipmentUI and Attack Switch are done replace the if () with
            //flashlightUsed && equipmentShown && attackSwitched && slimeKills >= slimeKillsNeeded
            if (flashlightUsed && slimeKills >= slimeKillsNeeded)
            {
                CompleteTutorial();
            }
        }
    }
    private void CompleteTutorial()
    {
        tutorialActive = false;

        tutorialCage.SetActive(false);
        tutorialSpawner.SetActive(false);

        GameManager.Instance.SetStage(GameStage.Intro_SlimeShowcase);

        ObjectiveManager.Instance.SetObjective("Continue through the forest");
    }
}
