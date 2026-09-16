using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatsSO", menuName = "Scriptable Objects/EnemyStatsSO")]
public class EnemyStatsSO : ScriptableObject
{
   
    public enum EnemyType { Melee,Ranged,Bomber,Mortar,Wave,Jump,JumpNdWave};

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

    [Header("Mortar Attack")]
    public GameObject mortarPrefab;
    public GameObject mortarHitPosDisplayPrefab;
    public float mortarSpeed = 15f;
    public int mortarShellsPreSalvo = 1;
    public float mortarTimeBetweenShells = .35f;
    public float mortarScatter = 3f;
    public float mortarArcHeight = 10f;
    public float mortarSplashRadius = 5f;
    public float mortarAimAheadOfPlayer = 1f;
    public LayerMask mortarGroundMask; //Walls flors or anything else that can trigger a hit
    public LayerMask mortarDamageMask; //Things that take mortar damage
    
    [Header("Jump Attack")]
    public float jumpdamage = 5f;
    public float jumpWindupTime = .4f;
    public float jumpDuration = .65f;
    public float jumpHeight = 1.5f;
    public float jumpMaxDistance = 5f;
    public float jumpHitRadius = 0.5f;
    public float jumpRecovery = 0.4f;
    public LayerMask jumpObstacleMask;
    
    [Header("Slime Wave Attack")]
    public GameObject aoeWavePrefab;
    public float aoeWaveWaringTime = 1f;
    public float aoeWaveRadius = 9f;
    public float aoeWaveSpeed = 10f;

    [Header("JumpNdWave")]
    public float jumpToWaveDelay = .25f;
    public float jumpWaveDamage = 4f;

    [Header("Attack VFX and SFX")]
    public AttackFeedback jumpWindup = new AttackFeedback();
    public AttackFeedback jumpTakeOff = new AttackFeedback();
    public AttackFeedback jumpHit= new AttackFeedback();
    public AttackFeedback jumpLanding = new AttackFeedback();
    public AttackFeedback waveWindup = new AttackFeedback();
    public AttackFeedback waveRelease = new AttackFeedback();
    public AttackFeedback waveHit = new AttackFeedback();   
    public AttackFeedback projectileLaunch = new AttackFeedback();
    public AttackFeedback projectileHit = new AttackFeedback();
    public AttackFeedback mortarLaunch = new AttackFeedback();
    public AttackFeedback bomberBlast = new AttackFeedback();
    public AttackFeedback bossLeapWindup = new AttackFeedback();
    public AttackFeedback bossMeleeWindup = new AttackFeedback();
    public AttackFeedback bossMeleeSwing = new AttackFeedback();
    public AttackFeedback bossMeleeHit = new AttackFeedback();
    public AttackFeedback bossLeapTakeoff = new AttackFeedback();
    public AttackFeedback bossLanding = new AttackFeedback();
    public AttackFeedback bossDetonationWindup = new AttackFeedback();
    public AttackFeedback bossDetonationBlast = new AttackFeedback();


    [Header("Split")]
    [Range(0f, 1f)] public float splitChance = 0.5f;
    public GameObject splitPrefab;
    public int splitCount = 2;
    public float splitRadius = .75f;
    public GameObject splitVFXPrefab;
    public float splitGrowthSpeed;

    [Header("Banshee - Return by Death")]
    [Tooltip("At zero health, return after death on a delay, Disable for perma death.")]
    public bool canReturn = true;
    [Min(.1f)] public float returnDelay = 15f;

    [Header("BAnshee - Scare timeing")]
    public float appearDelay = .5f;
    public float lingerAfterScream = 1f;
    public float scareCooldown = 8f;
    public float bansheeTurnSpeed;

    [Header("Banshee - Scream")]
    [Tooltip("Random scream. ")]
    public AudioClip[] screamClips = new AudioClip[0];
    [Range(0f, 1f)] public float screamVolume = .8f;
    public float audibleDist = 25f;

    [Header("Banshee - flee")]
    [Min(.1f)] public float fleeDuration = 4f;
    [Min(.1f)] public float fleedistance = 12f;

    [Header("Banshee - Scream VFX")]
    [Tooltip("Assign the VFX here and it's lifetime. Leave the sound empty as the banshee handles her own screams")]
    public AttackFeedback screamFeedback = new AttackFeedback();
    
}
