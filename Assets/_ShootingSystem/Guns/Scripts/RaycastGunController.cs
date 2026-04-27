using UnityEngine;

public class RaycastGunController : BaseGunController
{
    private const float _lineTimeDuration = 0.15f;

    protected override void Shoot()
    {
        Vector3 bulletDirection = GerSpreadDirection(_cameraTranform.forward);

        // Get crosshair target point
        Vector3 shootEndPoint = GetCameraRayHitPoint(bulletDirection, out RaycastHit hit);

        // Checks if gun is inside an object
        float distanceToEndPoint = Vector3.Distance(shootEndPoint, _cameraTranform.position);
        float distanceToBulletSpawn = Vector3.Distance(_bulletSpawnPoint.position, _cameraTranform.position);

        if (distanceToEndPoint > distanceToBulletSpawn) // Gun is not inside an object
        {
            // Shoot ray from the gun
            bulletDirection = (shootEndPoint - _bulletSpawnPoint.position).normalized;
            shootEndPoint = GetGunRayHitPoint(bulletDirection, out hit);

            // Visual FX
            DrawShootTrail(shootEndPoint, _lineTimeDuration);
        }

        // Checks if an hittable object was hitted
        if (hit.collider == null) return;

        if (hit.collider.TryGetComponent(out IHittable hittable))
        {
            ProjectileParameters projectileParameters = new(_cameraTranform.position, bulletDirection, _parameters);
            projectileParameters.SetHitPoint(shootEndPoint);
            hittable.Hit(projectileParameters);
        }
    }

    private Vector3 GetCameraRayHitPoint(Vector3 rayDirection, out RaycastHit cameraHit)
    {
        if (Physics.Raycast(_cameraTranform.position, rayDirection, out cameraHit, _parameters.TotalRange))
            return cameraHit.point;

        return _cameraTranform.position + (_cameraTranform.forward * _parameters.TotalRange);
    }

    private Vector3 GetGunRayHitPoint(Vector3 rayDirection, out RaycastHit gunHit)
    {
        if (Physics.Raycast(_bulletSpawnPoint.position, rayDirection, out gunHit, _parameters.TotalRange))
            return gunHit.point;

        return _bulletSpawnPoint.position + (rayDirection * _parameters.TotalRange);
    }

    // Visual FX (Temp)
    private void DrawShootTrail(Vector3 hitPosition, float time)
    {
        TrailRenderer line = new GameObject("Line").AddComponent<TrailRenderer>();

        line.widthMultiplier = 0.08f;

        line.gameObject.transform.position = _bulletSpawnPoint.position;
        line.AddPosition(hitPosition);

        Destroy(line.gameObject, time);
    }
}
