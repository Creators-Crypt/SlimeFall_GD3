using UnityEngine;

public class BossRewardSpawner : MonoBehaviour
{
    [Header("Boss")]
    [SerializeField] private EnemyAI boss;

    [Header("Reward")]
    [SerializeField] private GameObject rewardChest;
    [SerializeField] private GameObject levelKeyPrefab;
    [SerializeField] private Transform keySpawnPoint;

    private bool rewardSpawned = false;

    private void OnEnable()
    {
        if (boss != null)
        {
            boss.OnDeath += SpawnReward;
        }
    }

    private void OnDisable()
    {
        if(boss != null)
        {
            boss.OnDeath -= SpawnReward;
        }
    }

    public void SpawnReward()
    {
        if (rewardSpawned)
            return;

        rewardSpawned = true;

        if (rewardChest != null)
        {
            rewardChest.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No reward chest assigned.");
        }
        
        if(levelKeyPrefab != null)
        {
            levelKeyPrefab.SetActive(true);
        }
      

        if(DesNarManager.Instance != null)
        {
            DesNarManager.Instance.PlayLine(DesNarLine.BossDefeatedChestSpawned);
        }
        Debug.Log("Boss reward chest spawned.");
    }

    
}
