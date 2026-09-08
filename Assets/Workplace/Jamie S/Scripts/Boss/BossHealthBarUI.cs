using UnityEngine.UI;
using UnityEngine;

public class BossHealthBarUI : MonoBehaviour
{
    public GameObject bossBarPanel;
    public Image healthFill;

    private BossAI currentBoss;

    private void Awake()
    {
        if(bossBarPanel == null || healthFill == null)
        {
            Debug.Log("Assing the boss bar panel and its fill Image.");
            return;
        }        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentBoss == null || currentBoss.currentPhase == BossPhase.Dead)
        {
            HideBar();
            return;
        }

        healthFill.fillAmount = currentBoss.GetHealthPercent();
    }

    public void HideBar()
    {
        currentBoss = null;
        if (bossBarPanel != null) { bossBarPanel.SetActive(false); }
    }
    public void ShowBar(BossAI _boss)
    {
        if(_boss == null || _boss.currentPhase == BossPhase.Dead)
        {
            return;
        }

        currentBoss = _boss;
        healthFill.fillAmount = currentBoss.GetHealthPercent();
        bossBarPanel.SetActive(true);w
    }
}
