using UnityEngine;

public class DummyEnemy : MonoBehaviour, IHittable
{
    private Transform _playerTransform;
    
    private void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void IHittable.Hit(ProjectileParameters parameters)
    {
        DamageUIPopPool.Instance.InstantiateUIPop(parameters.HitPoint, _playerTransform, parameters.Damage);
    }
}
