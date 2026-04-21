using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    private float _damage;
    private Vector3 _originPosition;

    private void Awake()
    {
        _originPosition = transform.position;
    }

    public void SetDamage(float damage) => _damage = damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IHittable hit))
            hit.Hit(_originPosition, _damage);
        
        Destroy(gameObject);
    }
}
