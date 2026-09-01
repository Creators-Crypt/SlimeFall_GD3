using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float bulletLifetime = 5.0f;

    public Vector3 direction;
    public float speed;
    public float damage;
 
    

    // Update is called once per frame
   public virtual void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public virtual void Fire(Vector3 _direction, float _speed, float _Damage)
    {
        direction = _direction;
        speed = _speed;
        damage = _Damage;
        Destroy(this.gameObject, bulletLifetime);
    }

    public virtual void OnTriggerEnter(Collider other)
    {

        if (other.TryGetComponent<HealthSystem>(out var health))
        {
            health.OnDamage(damage);
            Debug.Log($"I've collided with {other.gameObject.name} and dealt {damage} damage.");

            Destroy(this.gameObject);
            return;
        }

       
        if (other.TryGetComponent<IDamageable>(out var damagable))
        {
            damagable.OnDamage(damage);
            Debug.Log($"I've collided with {other.gameObject.name} and dealt {damage} damage.");
            Destroy(this.gameObject);

        }
    }
}
