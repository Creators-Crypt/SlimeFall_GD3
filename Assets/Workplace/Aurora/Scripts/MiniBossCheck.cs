using System;
using UnityEngine;

public class MiniBossCheck : MonoBehaviour {

    public static event Action<bool> OnKeyGiven;

    [SerializeField] private EnemyAI enemy;

    private void Start() {
        enemy = GetComponent<EnemyAI>();
    }
    private void Update() {
        
        if (enemy.currentHealth <= 1) {
            BossDeath();
        }
    }
    private void BossDeath() {
        OnKeyGiven?.Invoke(true);
    }
}