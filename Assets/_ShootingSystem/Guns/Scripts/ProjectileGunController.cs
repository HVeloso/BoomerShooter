using UnityEngine;

public class ProjectileGunController : BaseGunController
{
    [Header("Bullet Parameters")]
    [SerializeField] private GameObject _bulletPrefab;

    protected override void Shoot()
    {
        Vector3 shootEndPoint = GetCameraRayHitPoint(_cameraTranform.forward, out RaycastHit hit);

        float distanceToEndPoint = Vector3.Distance(shootEndPoint, _cameraTranform.position);
        float distanceToBulletSpawn = Vector3.Distance(_bulletSpawnPoint.position, _cameraTranform.position);

        if (distanceToEndPoint > distanceToBulletSpawn) // Gun is not inside an object
        {
            Vector3 bulletFoward = GetBulletFoward();
            Vector3 bulletDirection = GetSpreadDirection(bulletFoward);

            ProjectilePoolManager.Instance.InstantiateProjectile(_bulletSpawnPoint.position, bulletDirection, _parameters);
            return;
        }

        // Checks if an hittable object was hitted
        if (hit.collider == null) return;

        if (hit.collider.TryGetComponent(out IHittable hittable))
        {
            ProjectileParameters projectileParameters = new(_cameraTranform.position, _cameraTranform.forward, _parameters);
            projectileParameters.SetHitPoint(shootEndPoint);
            hittable.Hit(projectileParameters);
        }
    }

    private Vector3 GetBulletFoward()
    {
        Ray ray = new(_cameraTranform.position, _cameraTranform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
            return (hitInfo.point - _bulletSpawnPoint.position).normalized;

        return _cameraTranform.forward;
    }
}
