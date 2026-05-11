using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseGunController : MonoBehaviour, IWeaponHandler
{
    [Header("Gun Parameters")]
    [SerializeField] protected GunParameters _parameters;
    [SerializeField] private GameObject _gunHolder;
    [SerializeField] protected Transform _bulletSpawnPoint;
    [Tooltip("Deve ser posicionado no lado oposto do bullet spawn point." +
        "É usado como ponto de disparo de raycast pra verificar colisão com paredes.")]
    [SerializeField] protected Transform _gunBasePoint;

    string IWeaponHandler.WeaponName => _parameters.GunName;

    protected Transform _cameraTranform;
    private float _shootingCooldown = 0f;
    private Coroutine _shootingCoroutine;

    protected virtual void Start()
    {
        _cameraTranform = Camera.main.transform;
    }

    private void Update()
    {
        ShootingTimer();
    }

    void IWeaponHandler.SetActive(bool active)
    {
        _gunHolder.SetActive(active);
    }

    void IWeaponHandler.AttackInputed(bool inputValue)
    {
        if (_shootingCoroutine != null)
            StopCoroutine(_shootingCoroutine);

        if (inputValue)
            _shootingCoroutine = StartCoroutine(Shooting());
    }

    private IEnumerator Shooting()
    {
        while (true)
        {
            yield return null;
            if (_shootingCooldown > 0f) continue;

            for (int i = 0; i < _parameters.BulletParameters.ProjectilesPerShot; i++)
            {
                Shoot();
            }

            _shootingCooldown = _parameters.FireRate;
        }
    }

    protected virtual void Shoot()
    {
        throw new System.NotImplementedException();
    }

    private void ShootingTimer()
    {
        if (_shootingCooldown > 0f)
            _shootingCooldown -= Time.deltaTime;
    }

    protected Vector3 GetSpreadDirection(Vector3 bulletFoward)
    {
        float spread = _parameters.SpreadAngle;
        Quaternion randomAngle = Quaternion.Euler(
            Random.Range(-spread, spread),
            Random.Range(-spread, spread),
            Random.Range(-spread, spread));

        return (randomAngle * bulletFoward).normalized;
    }

    protected Vector3 GetCameraRayHitPoint(Vector3 rayDirection, out RaycastHit cameraHit)
    {
        if (Physics.Raycast(_cameraTranform.position, rayDirection, out cameraHit, _parameters.TotalRange))
            return cameraHit.point;

        return _cameraTranform.position + (_cameraTranform.forward * _parameters.TotalRange);
    }

    protected Vector3 GetSpreadHitPoint(Vector3 origin,Vector3 defaultFoward)
    {
        Vector3 spreadDirection = GetSpreadDirection(defaultFoward);

        if (Physics.Raycast(origin, spreadDirection, out RaycastHit hit, _parameters.TotalRange))
            return hit.point;

        return origin + (defaultFoward * _parameters.TotalRange);
    }

    protected bool CheckIfGunIsInsideSomething(out RaycastHit hit)
    {
        Vector3 direction = (_bulletSpawnPoint.position - _gunBasePoint.position).normalized;
        float distance = Vector3.Distance(_bulletSpawnPoint.position, _gunBasePoint.position);

        return Physics.Raycast(_gunBasePoint.position, direction, out hit, distance);
    }
}
