using UnityEngine;

public class PuzzleRoomCompletion : MonoBehaviour
{
    [SerializeField] private GameObject[] equipmentRewards;
    [SerializeField] private GameObject bossPortal;

    [SerializeField] private int slimeKillsNeeded = 3; // Change back to 15

    private int slimeKills;
    private bool roomComplete;

    private void OnEnable()
    {
        GameManager.OnPlayerAction += HandlePlayerAction;
    }
    private void OnDisable()
    {
        GameManager.OnPlayerAction -= HandlePlayerAction;
    }
    void Start()
    {
        HideRewards();
        
        if(bossPortal != null)
        {
            bossPortal.SetActive(false);
        }
    }
    
    private void HandlePlayerAction(string actionKey)
    {
        if (roomComplete)
            return;

        if(actionKey == "SlimeKilled")
        {
            slimeKills++;

            if(slimeKills >= slimeKillsNeeded)
            {
                CompleteRoom();
            }
        }
    }
    public void CompleteRoom()
    {
        roomComplete = true;
        foreach(GameObject reward in equipmentRewards)
        {
            if (reward != null)
                reward.SetActive(true);
        }

        if (bossPortal != null)
            bossPortal.SetActive(true);

        ObjectiveManager.Instance.SetObjective("Proceed through the portal");
    }

   private void HideRewards()
    {
        foreach (GameObject reward in equipmentRewards)
        {
            if(reward != null)
            {
                reward.SetActive(false);
            }
        }
    }
}
