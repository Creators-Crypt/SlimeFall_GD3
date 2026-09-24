using UnityEngine.UI;
using UnityEngine;

public class BossHealthBarUI : MonoBehaviour
{
    public GameObject bossBarPanel;
    public Image healthFill;

    private MonoBehaviour currentBoss;
    private IHealth bossHealth;

    private void Awake()
    {
        if(bossBarPanel == null || healthFill == null)
        {
            Debug.Log("Assing the boss bar panel and its fill Image.");
            return;
        }

        //HideBar();
    }
    private void OnEnable() {
        BossEncounterTrigger.OnBossActivate += ShowBar;
    }
    private void OnDisable() {
        BossEncounterTrigger.OnBossActivate -= ShowBar;
    }
    // Update is called once per frame
    void Update()
    {
        if(currentBoss == null)
        {
            //Debug.Log($"[Bar] Hiding because null");
            HideBar();
            return;
        }
        if (bossHealth.IsDead)
        {
            //Debug.Log($"[Bar] Hiding because dead");
            HideBar();
            return;
        }
        UpdateHealthBar(bossHealth.CurrentHealth, bossHealth.MaxHealth);
        
    }

    public void HideBar()
    {
        //Debug.Log($"[Bar]hidebar. currentBoss = {(currentBoss == null ? "NULL" : currentBoss.name)} ");
        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged -= UpdateHealthBar;
            bossHealth.OnDeath -= HideBar;
            bossHealth = null;
        }
        currentBoss = null;
        if (bossBarPanel != null) { bossBarPanel.SetActive(false); }
    }
    public void ShowBar(MonoBehaviour _boss)
    {
        gameObject.SetActive(true);
        if(_boss == null)
        {
            return;
        }
        IHealth newHealth = _boss as IHealth;
        if(newHealth == null)
        {
            return;
        }
        if(currentBoss == _boss)
        {
            //Debug.Log($"[Bar] some boss showing");
            return;
        }

        HideBar();

        currentBoss = _boss;
        bossHealth = newHealth;

        bossHealth.OnHealthChanged += UpdateHealthBar;
        bossHealth.OnDeath += HideBar;
        UpdateHealthBar(bossHealth.CurrentHealth, bossHealth.MaxHealth);
        bossBarPanel.SetActive(true);
        //Debug.Log($"[Bar] Shown. activeInHierarchy={bossBarPanel.activeInHierarchy}, fill={healthFill.fillAmount}cur = {bossHealth.CurrentHealth} max {bossHealth.MaxHealth}");
    }

    private void UpdateHealthBar(float _currentHealth, float _maxHealth)
    {
        if(_maxHealth <= 0f)
        {
            healthFill.fillAmount = 0f;
            return;
        }

        healthFill.fillAmount = Mathf.Clamp01(_currentHealth / _maxHealth);
    }

    private void OnDestroy()
    {
        Debug.Log("[Bar] DESTROYED",this);
    }
}
