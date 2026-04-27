using UnityEngine;

public class ProjectileGunController : BaseGunController
{
    [Header("Bullet Parameters")]
    [SerializeField] private GameObject _bulletPrefab;

    protected override void Shoot()
    {
        Vector3 bulletFoward = GetBulletFoward();
        Vector3 bulletDirection = GerSpreadDirection(bulletFoward);

        ProjectilePoolManager.Instance.InstantiateProjectile(_bulletSpawnPoint.position, bulletDirection, _parameters);
    }

    private Vector3 GetBulletFoward()
    {
        Ray ray = new(_cameraTranform.position, _cameraTranform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            return (hitInfo.point - _bulletSpawnPoint.position).normalized;
        }

        return _cameraTranform.forward;
    }
}
