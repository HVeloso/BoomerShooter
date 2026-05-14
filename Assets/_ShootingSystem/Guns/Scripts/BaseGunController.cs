using System.Collections;
using UnityEngine;

public abstract class BaseGunController : MonoBehaviour, IWeaponHandler
{
    //   -   -   -   -   -   -   PARAMETERS   -   -   -   -   -   -
    [Header("Gun Parameters")]
    [SerializeField] protected GunParameters _parameters;
    [SerializeField] private LayerMask _layersToHit;

    [Header("Gun Structure")]
    [SerializeField] private GameObject _gunHolder;
    [SerializeField] protected Transform _bulletSpawnPoint;
    [Tooltip("Ponto oposto ao spawn do projétil usado para raycast de colisão."), 
        SerializeField] protected Transform _collisionRayOrigin;

    // Shooting Control Parameters
    protected Transform _cameraTransform;
    private float _shootingCooldown = 0f;
    private Coroutine _shootingCoroutine;

    // Properties
    public Vector3 GunDirection => (_bulletSpawnPoint.position - _collisionRayOrigin.position).normalized;
    string IWeaponHandler.WeaponName => _parameters.GunName;

    //   -   -   -   -   -   -   FUNCTIONS   -   -   -   -   -   -
    // MonoBehaviour
    protected virtual void Start()
    {
        _cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        ShootingTimer();
    }

    // Weapon Interface
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

    // Shooting Functions
    protected virtual void Shoot()
    {
        throw new System.NotImplementedException();
    }

    protected bool TryHit(Transform objectHitted, Vector3 hitPoint, Vector3 bulletDirection)
    {
        if (!objectHitted.TryGetComponent(out IHittable hittable)) return false;

        ProjectileParameters projectileParameters = new(_cameraTransform.position, bulletDirection, hitPoint, _parameters);
        hittable.Hit(projectileParameters);

        return true;
    }

    private void ShootingTimer()
    {
        if (_shootingCooldown > 0f)
            _shootingCooldown -= Time.deltaTime;
    }

    private IEnumerator Shooting()
    {
        while (true)
        {
            yield return null;

            if (_shootingCooldown > 0f) continue;
            _shootingCooldown = _parameters.FireRate;

            for (int i = 0; i < _parameters.BulletParameters.ProjectilesPerShot; i++)
            {
                Shoot();
            }
        }
    }

    // Shoot Checks and Settings
    protected bool IsGunInsideSomething(out RaycastHit hit)
    {
        float distance = Vector3.Distance(_bulletSpawnPoint.position, _collisionRayOrigin.position);

        return Physics.Raycast(_collisionRayOrigin.position, GunDirection, out hit, distance, _layersToHit);
    }

    protected Vector3 GetSpreadHitPoint(Vector3 origin, Vector3 defaultFoward)
    {
        Vector3 spreadDirection = GetSpreadDirection(defaultFoward);

        if (Physics.Raycast(origin, spreadDirection, out RaycastHit hit, _parameters.TotalRange, _layersToHit))
            return hit.point;

        return origin + (spreadDirection * _parameters.TotalRange);
    }
    
    private Vector3 GetSpreadDirection(Vector3 bulletFoward)
    {
        float spread = _parameters.SpreadAngle;

        Quaternion randomAngle = Quaternion.Euler(
            Random.Range(-spread, spread),
            Random.Range(-spread, spread),
            Random.Range(-spread, spread));

        return (randomAngle * bulletFoward).normalized;
    }
}
