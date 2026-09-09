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

        HideBar();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentBoss == null)
        {
            HideBar();
            return;
        }
        if (bossHealth.IsDead)
        {
            HideBar();
            return;
        }
        UpdateHealthBar(bossHealth.CurrentHealth, bossHealth.MaxHealth);
        
    }

    public void HideBar()
    {
        if(bossHealth != null)
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
            return;
        }

        HideBar();

        currentBoss = _boss;
        bossHealth = newHealth;

        bossHealth.OnHealthChanged += UpdateHealthBar;
        bossHealth.OnDeath += HideBar;
        UpdateHealthBar(bossHealth.CurrentHealth, bossHealth.MaxHealth);
        bossBarPanel.SetActive(true);
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
}
