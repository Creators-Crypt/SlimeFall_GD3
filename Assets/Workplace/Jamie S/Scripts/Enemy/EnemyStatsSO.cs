using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatsSO", menuName = "Scriptable Objects/EnemyStatsSO")]
public class EnemyStatsSO : ScriptableObject
{
   
    public enum EnemyType { Melee,Ranged,Bomber, Boss};

    [Header("Type")]
    public EnemyType enemyType;
    [Header("Health")]
    public float maxHealth = 10f;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float patrolRadius = 8f;
    public float patrolWaitTimeMin = 1.5f;
    public float patrolWaitTimeMax = 3.5f;

    [Header("Detection")]
    public float detectionRadius = 10f;
    [Range(1,360)]public float detectionAngle = 90f;
    public float lostSightTime = 3f;
    public LayerMask obstacleMask;

    [Header("Attack")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1.2f;
    public float attackDamage = 5f;

    [Header("Ranged Attack")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;

    [Header("Bomber Attack")]
    public float explosionRadius = 3f;
    public GameObject explosionVFX;

    [Header("Split")]
    [Range(0f, 1f)] public float splitChance = 0.5f;
    public GameObject splitPrefab;
    public int splitCount = 2;
    public float splitRadius = .75f;
    public GameObject splitVFXPrefab;
    public float splitGrowthSpeed;
   
    
}
