using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// We will change this script to have objects damage the player and entities
/// Change script to WorldDamage or ObjectDamage
/// 
/// </summary>

public class Damage : MonoBehaviour
{
    enum damageType { bullet, stationary, DOT }
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int bulletSpeed;
    // Takes care of bullets that fly into space so they wont take up a bunch of memory
    [SerializeField] int bulletDestroyTime;
    [SerializeField] ParticleSystem hitEffect;

    //This controls the timer
    bool isDamaging;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (type == damageType.bullet)
        {
            rb.linearVelocity = transform.forward * bulletSpeed;
            Destroy(gameObject, bulletDestroyTime);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;
        // bullet and stationary will use on trigger enter
        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg != null && type != damageType.DOT)
        {
            dmg.OnDamage(damageAmount);

        }

        if (type == damageType.bullet)
        {
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
            return;
        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg != null && type == damageType.DOT && !isDamaging)
        {
            StartCoroutine(damageOther(dmg));
        }
    }

    IEnumerator damageOther(IDamageable d)
    {
        isDamaging = true;
        d.OnDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
}
