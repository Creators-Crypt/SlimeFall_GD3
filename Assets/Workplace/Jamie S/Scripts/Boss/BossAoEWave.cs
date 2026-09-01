using UnityEngine;
using System.Collections.Generic;

public class BossAoEWave : MonoBehaviour
{
    public Transform ringGrow;
    public float radiusScale = 2f;
    public float fadeOutTime = .35f;

    [SerializeField] private float maxRadius;
    [SerializeField] private float growSpeed;
    [SerializeField] private float damage;
    [SerializeField] private LayerMask targetMask;

    private float currentRadius;
    private bool playing;
    private Transform bossTransfor;
    private List<IDamageable> alreadyHit = new List<IDamageable>();

    public void Play(BossAI _boss, float _radius, float _speed, float _damageAmount, LayerMask _mask)
    {
        if (_boss != null)
        {
            bossTransfor = _boss.transform;
        }

        maxRadius = _radius;
        growSpeed = _speed;
        damage = _damageAmount;
        targetMask = _mask;

        currentRadius = .1f;
        alreadyHit.Clear();
        playing = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (playing == false) return;

        float lastRadius = currentRadius;
        currentRadius += growSpeed * Time.deltaTime;

        DamageRing(lastRadius, currentRadius);
        UpdateVisual();

        if (currentRadius >= maxRadius)
        {
            playing = false;
            Destroy(gameObject, fadeOutTime);
        }
    }

    private void DamageRing(float _innerEdge, float _outerEdge)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _outerEdge, targetMask);

        foreach (Collider hit in hits)
        {
            if (hit == null) continue;

            if (bossTransfor != null && hit.transform.IsChildOf(bossTransfor)) continue;

            float distance = Vector3.Distance(hit.ClosestPoint(transform.position), transform.position);

            if (distance < _innerEdge) continue;

            IDamageable target = hit.GetComponent<IDamageable>();
            if (target == null) continue;
            if (alreadyHit.Contains(target)) continue;

            alreadyHit.Add(target);
            target.OnDamage(damage);
        }
    }

    private void UpdateVisual()
    {
        if (ringGrow == null) return;

        float size = currentRadius * radiusScale;
        ringGrow.localScale = new Vector3(size, ringGrow.localScale.y, size);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxRadius);
    }
}
