using UnityEngine;

public class BossEncounterTrigger : MonoBehaviour
{
    public MonoBehaviour boss;
    public BossHealthBarUI bossHealthBar;

    private bool fired;

    public void Awake()
    {
        BoxCollider encounterArea = GetComponent<BoxCollider>();

        Debug.Log($"[Trigger] Awake. isTrigger = {encounterArea.isTrigger} layer = {gameObject.layer}");

        if (boss == null || bossHealthBar == null)
        {
            Debug.LogError("Be sure the boss and the Canvas health bar is added to this trigger plz.");
        }
    }
 
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[Trigger] ENTER {other.name}  tag = {other.tag}");

        CheckForPlayer(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CheckForPlayer(other);
    }

    private void CheckForPlayer(Collider _other)
    {
        if (fired) return;

        if(isActiveAndEnabled == false || boss == null|| bossHealthBar == null)
        {
            Debug.Log($"[Trigger] fail. enabled= {isActiveAndEnabled} boss={(boss==null?"NULL":boss.name)} bar= {(bossHealthBar ==null?"NULL":bossHealthBar.name)}");
            return;
        }

        if (_other.CompareTag("Player"))
        {
            fired = true;
            Debug.Log($"[Trigger] ShowBar ->{bossHealthBar.name}, boss = {boss.GetType().Name}");
            bossHealthBar.ShowBar(boss);

            BossAI  bossAI = boss as BossAI; 
            if (bossAI != null)
            {
                bossAI.ActivateEncounter();
            }
            else
            {
                Debug.LogWarning("[Trigger] the boss field is not a BossAI so the encounter did not start");
            }
            enabled = false;
                return;
        }
    }
}
