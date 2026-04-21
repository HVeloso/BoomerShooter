using UnityEngine;

public class RaycastGunController : BaseGunController
{
    protected override void Shoot()
    {
        Ray shootingRay = new(_cameraTranform.position, _cameraTranform.forward);
        if (!Physics.Raycast(shootingRay, out RaycastHit hit, _parameters.Range)) return;

        if (hit.collider.TryGetComponent(out IHittable hittable))
        {
            ProjectileParameters projectileParameters = new(_cameraTranform.position, _cameraTranform.forward, _parameters);
            hittable.Hit(projectileParameters);
        }
    }
}
