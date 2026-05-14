using UnityEngine;

public class ProjectileGunController : BaseGunController
{
    //   -   -   -   -   -   -   PARAMETERS   -   -   -   -   -   -

    [Header("Bullet Parameters")]
    [SerializeField] private GameObject _bulletPrefab;

    //   -   -   -   -   -   -   FUNCTIONS   -   -   -   -   -   -
    protected override void Shoot()
    {
        // Colocar VFX & SFX - - - - -

        if (IsGunInsideSomething(out RaycastHit hit))
        {
            TryHit(hit.collider.transform, hit.point, GunDirection);
            return;
        }

        Vector3 spreadHitPoint = GetSpreadHitPoint(_cameraTransform.position, _cameraTransform.forward);
        Vector3 bulletDirection = (spreadHitPoint - _collisionRayOrigin.position).normalized;

        // Substituir por um service locator depois - - - - -
        ProjectilePoolManager.Instance.InstantiateProjectile(_bulletSpawnPoint.position, bulletDirection, _parameters);
    }
}
