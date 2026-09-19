using UnityEngine;

public class BossRewardSpawner : MonoBehaviour
{
    [Header("Boss")]
    [SerializeField] private EnemyAI boss;

    [Header("Reward")]
    [SerializeField] private GameObject rewardChest;

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

        if(rewardChest == null)
        {
            Debug.LogWarning("No Reward chest assigned.");
            return;
        }

        rewardSpawned = true;
        rewardChest.SetActive(true);

        if(DesNarManager.Instance != null)
        {
            DesNarManager.Instance.PlayLine(DesNarLine.BossDefeatedChestSpawned);
        }
        Debug.Log("Boss reward chest spawned.");
    }

    //private void OnEnable()
    //{
    //    BossEvents.OnBossDefeated += SpawnReward();
    //}
    //private void OnDisable()
    //{
    //    BossEvents.OnBossDefeated -= SpawnReward;
    //}
}
