using UnityEngine;
using System;

public class GemCollectionManager : Singleton<GemCollectionManager>
{
    public static event Action<int, int> OnGemCountChanged;
    public static event Action OnAllGemsCollected;

    [Header("Gem Progress")]
    [SerializeField] private int gemsRequired = 3;
    [SerializeField] private int gemsCollected = 0;

    public int GemsCollected => gemsCollected;
    public int GemsRequired => gemsRequired;

    public void CollectGem()
    {
        if (gemsCollected >= gemsRequired)
            return;

        gemsCollected++;

        if(DesNarManager.Instance != null)
        {
            switch(gemsCollected)
            {
                case 1:
                    DesNarManager.Instance.PlayLine(DesNarLine.FirstGem);
                    break;

                case 2:
                    DesNarManager.Instance.PlayLine(DesNarLine.SecondGem);
                    break;

                case 3:
                    DesNarManager.Instance.PlayLine(DesNarLine.ThirdGem);
                    break;
            }
        }

        Debug.Log($"Gem Collected: {gemsCollected}/{gemsRequired}");

        OnGemCountChanged?.Invoke(gemsCollected, gemsRequired) ;

        if(ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.SetObjective($"Collect the Gems: {gemsCollected}/{gemsRequired}");
        }

        if(gemsCollected >= gemsRequired)
        {
            Debug.Log("All gems collected!");

            OnAllGemsCollected?.Invoke();
        }
    }
}
