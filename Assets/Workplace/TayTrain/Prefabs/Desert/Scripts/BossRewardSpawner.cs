using UnityEngine;

public class BossRewardSpawner : MonoBehaviour
{
    [Header("Reward")]
    [SerializeField] private GameObject rewardChest;

    private bool rewardSpawned = false;

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
