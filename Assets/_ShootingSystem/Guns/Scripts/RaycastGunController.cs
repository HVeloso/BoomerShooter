using UnityEngine;

public class RaycastGunController : BaseGunController
{
    protected override void Shoot()
    {
        Vector3 shootHitPoint;
        Vector3 shootDirection;

        if (CheckIfGunIsInsideSomething(out RaycastHit hit))
        {
            shootDirection = (_bulletSpawnPoint.position - _gunBasePoint.position).normalized;
            shootHitPoint = hit.point;
        }
        else
        {
            Vector3 spreadHitPoint = GetSpreadHitPoint(_cameraTranform.position, _cameraTranform.forward);
            shootDirection = (spreadHitPoint - _bulletSpawnPoint.position).normalized;
            shootHitPoint = GetGunRayHitPoint(shootDirection, out hit);
        }

        if (hit.collider != null)
            TryHit(hit.collider.transform, shootHitPoint, shootDirection);

        DrawShootTrail(shootHitPoint);
    }

    private bool TryHit(Transform objectHitted, Vector3 hitPoint, Vector3 bulletDirection)
    {
        if (objectHitted.TryGetComponent(out IHittable hittable))
        {
            ProjectileParameters projectileParameters = new(_cameraTranform.position, bulletDirection, _parameters);
            projectileParameters.SetHitPoint(hitPoint);
            hittable.Hit(projectileParameters);

            return true;
        }

        return false;
    }

    private Vector3 GetGunRayHitPoint(Vector3 rayDirection, out RaycastHit gunHit)
    {
        if (Physics.Raycast(_bulletSpawnPoint.position, rayDirection, out gunHit, _parameters.TotalRange))
            return gunHit.point;

        return _bulletSpawnPoint.position + (rayDirection * _parameters.TotalRange);
    }

    // Visual FX (Temp) --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- ---
    private const float _lineTimeDuration = 0.15f;

    private void DrawShootTrail(Vector3 hitPosition)
    {
        TrailRenderer line = new GameObject("Line").AddComponent<TrailRenderer>();

        line.widthMultiplier = 0.08f;

        line.gameObject.transform.position = _bulletSpawnPoint.position;
        line.AddPosition(hitPosition);

        Destroy(line.gameObject, _lineTimeDuration);
    }
}
