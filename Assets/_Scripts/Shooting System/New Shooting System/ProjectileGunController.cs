using UnityEngine;

public class ProjectileGunController : BaseGunController
{
    [Header("Bullet Parameters")]
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private GameObject _bulletPrefab;

    protected override void Shoot()
    {
        Vector3 bulletFoward = GetBulletFoward();
        Vector3 bulletDirection = GetBulletDirection(bulletFoward);

        ProjectilePoolManager.Instance.InstantiateProjectile(_bulletSpawnPoint.position, bulletDirection, _parameters);
    }

    private Vector3 GetBulletFoward()
    {
        Ray ray = new(_cameraTranform.position, _cameraTranform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
            return (hitInfo.point - _bulletSpawnPoint.position).normalized;

        return _cameraTranform.forward;
    }

    private Vector3 GetBulletDirection(Vector3 bulletFoward)
    {
        float spread = _parameters.SpreadAngle;
        Quaternion randomAngle = Quaternion.Euler(
            Random.Range(-spread, spread),
            Random.Range(-spread, spread),
            Random.Range(-spread, spread));

        return (randomAngle * bulletFoward).normalized;
    }
}
