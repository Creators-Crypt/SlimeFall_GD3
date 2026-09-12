using UnityEngine;

public class TargetDummy : MonoBehaviour, IDamageable {

    private float currentHealth, maxHealth = 100f;

    private void Start() {
        currentHealth = maxHealth;
    }
    public void OnDamage(float damage) {
        currentHealth -= damage;

        if (currentHealth <= 0) {
            currentHealth = maxHealth;
        }
        GameManager.Instance.PlayerPerformAction("TargetDefeated");
    }
}