using UnityEngine;

public class RaycastGunController : BaseGunController
{
    private const float _lineTimeDuration = 0.15f;

    protected override void Shoot()
    {
        // Ideia 1
        // Checar se a arma está dentro de um objeto. - SphereCast
        // Se sim: Acertar esse objeto.

        // Pegar o ponto de acerto com um Raycast
            // O alvo está antes do cano da arma?
                // Se sim: De dano no alvo - Sem efeito de tracing
                // Se não:
                    // Jogar um raio do cano da arma até o ponto de acerto do raycast anterior
                        // Se encontrar algo: Acertar esse objeto.


        // Checa se a arma está dentro de um objeto.
        RaycastHit[] sphereHits = Physics.SphereCastAll(_bulletSpawnPoint.position, 0.1f, _bulletSpawnPoint.forward);

        foreach (RaycastHit hit1 in sphereHits)
        {
            Debug.Log($"HIT! - {hit1.collider.name}");
            if (hit1.collider.TryGetComponent(out IHittable sphereHitable)) // Se sim acerta.
            {
                ProjectileParameters projectileParameters = new(_cameraTranform.position, _cameraTranform.forward, _parameters);
                projectileParameters.SetHitPoint(_bulletSpawnPoint.position);
                sphereHitable.Hit(projectileParameters);
            }
        }

        if (sphereHits.Length > 0) return;

        // Pega o ponto de impacto da crosshair
        Vector3 hitPoint = GetCameraRayHitPoint(_cameraTranform.forward, out RaycastHit hit);

        float distanceToHitPoint = Vector3.Distance(_cameraTranform.position, hitPoint);
        float distanceToBulletSpawn = Vector3.Distance(_cameraTranform.position, _bulletSpawnPoint.position);

        if (distanceToHitPoint <=  distanceToBulletSpawn) // Alvo esta muito proximo
        {
            if (hit.collider.TryGetComponent(out IHittable sphereHitable)) // Acerta
            {
                ProjectileParameters projectileParameters = new(_cameraTranform.position, _cameraTranform.forward, _parameters);
                projectileParameters.SetHitPoint(hitPoint);
                sphereHitable.Hit(projectileParameters);
            }

            DrawShootTrail(hitPoint);
            return;
        }

        // Atira um raio do cano da arma.
        Vector3 shootDirection = (hitPoint - _bulletSpawnPoint.position).normalized;
        hitPoint = GetGunRayHitPoint(shootDirection, out hit);

        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent(out IHittable sphereHitable)) // Acerta
            {
                ProjectileParameters projectileParameters = new(_cameraTranform.position, _cameraTranform.forward, _parameters);
                projectileParameters.SetHitPoint(hitPoint);
                sphereHitable.Hit(projectileParameters);
            }
        }

            DrawShootTrail(hitPoint);

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

        // Ideia 2
        // Dispara um raio em direção a crosshair.
           // O alvo está antes do cano da arma?
                // Se sim: Dê dano no alvo - Sem efeito de tracing.
                // Se não:
                    // Checar se a arma está dentro de um objeto.
                        // Se sim: Dê dano no alvo - Sem efeito de tracing.
                        // Se não:
                        // Se encontrar algo: Acertar esse objeto.
                            // Dispara um raio a partir do cano.
                            // Se encontrar algo: Acertar esse objeto.
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(_bulletSpawnPoint.position, 0.1f);
    }
}
