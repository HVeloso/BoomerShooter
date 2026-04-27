using System.Collections;
using UnityEngine;

public abstract class BaseGunController : MonoBehaviour, IWeaponHandler
{
    // Consts
    protected const string HITTABLE_LAYER = "Hittable";

    [Header("Gun Parameters")]
    [SerializeField] protected GunParameters _parameters;
    [SerializeField] private GameObject _gunHolder;
    [SerializeField] protected Transform _bulletSpawnPoint;

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
}
