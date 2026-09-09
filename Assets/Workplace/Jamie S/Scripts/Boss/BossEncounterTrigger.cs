using UnityEngine;

public class BossEncounterTrigger : MonoBehaviour
{
    public MonoBehaviour boss;
    public BossHealthBarUI bossHealthBar;

    public void Awake()
    {
        BoxCollider encounterArea = GetComponent<BoxCollider>();

        if (boss == null || bossHealthBar == null)
        {
            Debug.LogError("Be sure the boss and the Canvas health bar is added to this trigger plz.");
        }
    }
 
    
    private void OnTriggerEnter(Collider other)
    {
        CheckForPlayer(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CheckForPlayer(other);
    }

    private void CheckForPlayer(Collider _other)
    {
        if(isActiveAndEnabled == false || boss == null|| bossHealthBar == null)
        {
            return;
        }

        if (_other.CompareTag("Player"))
        {
            bossHealthBar.ShowBar(boss);
            return;
        }
    }
}
