using UnityEngine;

public class BossDamageZone : MonoBehaviour, IDamageable
{
    public BossAI boss;
    public float damageMultiplier = 1f;

    public bool isWeakPoint = false;

    [Header("Effects")]
    public GameObject hitVFXPrefab;
    public AudioClip hitSound;
    public AudioClip nonHitSound;


    private void Awake()
    {
        if(boss == null)
        {
            boss = GetComponentInParent <BossAI>();
        }
    }

    public void SetBoss(BossAI _boss)
    {
       if(boss == null)
        {
            boss = _boss;
        }
    }

    public void OnDamage(float _damage)
    {
        if (boss == null) return;

        bool noHit = boss.isInvulnerable;

        boss.ApplyZoneDamage(_damage, damageMultiplier, isWeakPoint);

        if(hitVFXPrefab  != null)
        {
            GameObject vfx = Instantiate(hitVFXPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, 3f);
        }

        if(noHit)
        {
            if(nonHitSound != null)
            {
                AudioSource.PlayClipAtPoint(nonHitSound, transform.position);
            }
        }
        else
        {
            if(hitSound != null)
            {
                AudioSource.PlayClipAtPoint(hitSound,transform.position);
            }
        }
    }

}
