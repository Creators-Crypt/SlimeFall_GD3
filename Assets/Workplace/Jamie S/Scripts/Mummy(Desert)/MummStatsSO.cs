using UnityEngine;

[CreateAssetMenu(fileName = "MummStatsSO", menuName = "Scriptable Objects/MummStatsSO")]
public class MummStatsSO : EnemyStatsSO
{
    [Header("Mummy - Ground")]
    [Tooltip("If we want to make a sand layer separate from ground so his abilities can only be used on sand")]
    public LayerMask sandGroundMask = ~0;

    [Header("Mummy - Bandage Pull")]
    [Tooltip("Prefab for MummyBandage")]
    public GameObject bandagePrefab;
    public float bandageWindupTime = .35f;
    public float bandageSpeed = 15f;
    public float bandageDamage = 6f;
    [Tooltip("How Far the bandage will pull the Player towards itself. 0 = no hit")]
    public float bandagePullDist = 3f;
    [Range(0f, 1f)] public float bandageSlowMultiplier = .5f;
    public float bandageSlowDuration = 3;
    public float bandageLifetime = 3f;
    public float bandageRecovery = .5f;

    [Header("Mummy - Bandage VFX on the Player")]
    public GameObject bandageOnPlayerVFX;
    public float bandageVfxLifetime = 3f;

    [Header("Mummy - Bandage feedback")]
    public AttackFeedback bandageWindup = new AttackFeedback();
    public AttackFeedback bandageLaunch = new AttackFeedback();
    public AttackFeedback bandageHit = new AttackFeedback();

    [Header("Mummy - Quicksand Pool")]
    public GameObject quicksandPoolPrefab;
    public GameObject quicksandTelegraphPrefab;
    public float quicksandCooldow = 9f;
    [Tooltip("Warning time before the pool apears.")]
    public float quicksandWarningTime = 1.2f;
    public float quicksandRadius = 3.5f;
    public float quicksandDuration = 6f;
    public float quicksandTickInterval = .5f;
    public float quicksandDamagePerTick = 2f;
    [Tooltip("Meters per second the pool drags the player to the center")]
    public float quicksandPullPerSecond = 1.4f;
    [Range(0f, 1f)] public float quicksandSlowMultiplier = .45f;
    public float quicksandAimAhead = .5f;
    public float quicksandRecovery = .6f;

    [Header("Mummy - Quicksand Feedback")]
    public AttackFeedback quicksandWindup = new AttackFeedback();
    public AttackFeedback quicksandSpawn = new AttackFeedback();
    public AttackFeedback quicksandPool = new AttackFeedback();

    [Header("Mummy - Burrow")]
    public float burrowCooldown = 12f;
    public float burrowMinDistance = 5f;
    public float burrowDigTime = .6f;
    public float burrowTravelTime = 2f;
    public float burrowRiseTime = .3f;
    public float burrowRecovery = .4f;
    [Tooltip("How far away from the player it surfaces")]
    public float burrowEmergeDistance = 2.2f;
    public float burrowDepth = 3f;
    public bool burrowInvulnerable = true;
    public float burrowEmergeDamage = 5f;
    public float burrowEmergeRadius = 2f;

    [Header("Mummy - Burrow feedback")]
    public AttackFeedback burrowDown = new AttackFeedback();
    public AttackFeedback burrowtrail = new AttackFeedback();
    public GameObject burrowMountPrefab;
    public float burrowMoundHeightOffset = 2.3f;
    public AttackFeedback burrowUp = new AttackFeedback();
}
