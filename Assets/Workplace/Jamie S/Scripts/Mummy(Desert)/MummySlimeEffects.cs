using Unity.VisualScripting;
using UnityEngine;

public static class MummySlimeEffects 
{
   public static bool DealDamage(Collider _hit, float _damage)
    {
        if( _hit == null) {  return false; }
        if(_hit.TryGetComponent<HealthSystem>(out var health))
        {
            health.OnDamage(_damage); return true;
        }
        if(_hit.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.OnDamage(_damage);
            return true;
        }
        return false;
    }

    public static void ApplySlow(Transform _target, float _multiplier, float _duration)
    {
        if (_target == null )
        {
            return;
        }

        PlayerSlimedEffect slimed = _target.GetComponent<PlayerSlimedEffect>();
        if( slimed != null )
        {
            slimed.ApplySlime(_multiplier, _duration);
        }
    }

    public static void PullToward (Transform _target,Vector3 _point, float _distance)
    {
        if(_target == null ) {  return; }

        Vector3 direction = _point - _target.position;
        direction.y = 0f;

        float gap = direction.magnitude;

        float step = Mathf.Min( _distance, gap );
        Vector3 move = (direction / gap) * step;

        CharacterController  controller = _target.GetComponentInParent<CharacterController>();
        if( controller != null )
        {
           
            controller.Move( move );
            return;
        }

        Rigidbody body = _target.GetComponentInParent<Rigidbody>();
        if( body != null  && body.isKinematic == false)
        {
            body.MovePosition(body.position + move);
            return;
        }

        _target.position = _target.position + move;
    }

    public static void AttachBandageVFX(Transform _target, GameObject _prefab, float _lifetime)
    {
        if(_target == null || _prefab == null) { return; }

        GameObject spawned = Object.Instantiate(_prefab, _target.position, _target.rotation, _target); 
        Object.Destroy(spawned, _lifetime);
    }

    public static Vector3 GroundPoint(Vector3 _point, LayerMask _groundMask)
    {
        Vector3 start = _point + Vector3.up * 5f;

        RaycastHit hit;
        if (Physics.Raycast(start, Vector3.down, out hit, 35f, _groundMask))
        {
            return hit.point;
        }
        return _point;
    }
}
