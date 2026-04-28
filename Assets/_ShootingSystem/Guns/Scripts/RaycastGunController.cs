using UnityEngine;

public class RaycastGunController : BaseGunController
{
    private const float _lineTimeDuration = 0.15f;

    protected override void Shoot()
    {
        // Checar se a arma está dentro de alvo.
        if (CheckIfGunIsInsideSomething(out RaycastHit hit))
        {
            Vector3 direction = (_bulletSpawnPoint.position - _gunBasePoint.position).normalized;

            TryHit(hit.collider.transform, hit.point, direction);
            DrawShootTrail(hit.point);
            return;
        }

        // Checar se o objeto atingido está muito perto
        Vector3 spreadDirection = GetSpreadDirection(_cameraTranform.forward);
        Vector3 shootEndPoint = GetCameraRayHitPoint(spreadDirection, out hit);

        float distanceToHitPoint = Vector3.Distance(_cameraTranform.position, shootEndPoint);
        float distanceToBulletSpawn = Vector3.Distance(_cameraTranform.position, _bulletSpawnPoint.position);

        if (distanceToHitPoint < distanceToBulletSpawn)
        {
            TryHit(hit.collider.transform, shootEndPoint, _cameraTranform.forward);
            DrawShootTrail(shootEndPoint);
            return;
        }

        // Disparar raio a partir da arma
        Vector3 gunDirection = (shootEndPoint - _bulletSpawnPoint.position).normalized;
        Vector3 gunHitPoint = GetGunRayHitPoint(gunDirection, out hit);

        if (hit.collider != null)
        {
            TryHit(hit.collider.transform, hit.point, gunHitPoint);
            DrawShootTrail(hit.point);
            return;
        }

        DrawShootTrail(gunHitPoint);
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

    // Visual FX (Temp)
    private void DrawShootTrail(Vector3 hitPosition)
    {
        TrailRenderer line = new GameObject("Line").AddComponent<TrailRenderer>();

        line.widthMultiplier = 0.08f;

        line.gameObject.transform.position = _bulletSpawnPoint.position;
        line.AddPosition(hitPosition);

        Destroy(line.gameObject, _lineTimeDuration);
    }
}
