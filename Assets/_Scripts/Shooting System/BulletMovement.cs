using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletMovement : MonoBehaviour
{
    public float Speed;
    private float _range;
    private Vector3 _startPosition;

    private Rigidbody _bulletRb;

    private void Awake()
    {
        _bulletRb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, _startPosition) >= _range)
            Destroy(gameObject);
    }

    public void ShootBullet(Vector3 angle, float range)
    {
        _range = range;
        _startPosition = _bulletRb.position;
        _bulletRb.linearVelocity = angle * Speed;
    }
}
