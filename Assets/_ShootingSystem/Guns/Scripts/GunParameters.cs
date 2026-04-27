using UnityEngine;

[CreateAssetMenu(fileName = "Gun Parameters", menuName = "Boomer Shooter/Gun Parameters")]
public class GunParameters : ScriptableObject
{
    [Header("Gun Parameters")]
    [SerializeField] private string _gunName;
    [Tooltip("Delay Between shoots"), SerializeField, Min(0)] private float _fireRate;
    [SerializeField, Min(0)] private float _spreadAngle;

    [Header("Shoot Parameters")]
    [SerializeField] private float _gunDamage;
    [SerializeField, Min(0)] private float _damagePerProjectile;
    [SerializeField, Min(0)] private float _totalDamage;
    [Space]
    [SerializeField] private float _gunSpeed;
    [SerializeField, Min(0)] private float _totalSpeed;
    [Space]
    [SerializeField] private float _gunRange;
    [SerializeField, Min(0)] private float _totalRange;

    [Header("Bullet")]
    [SerializeField] private BulletParameters _bulletParameters;

    // Only Gun Properties
    public string GunName => _gunName;
    public float FireRate => _fireRate;
    public float SpreadAngle => _spreadAngle;

    // Gun and Bullet Parameters
    public float GunDamage => _gunDamage;
    public float GunSpeed => _gunSpeed;
    public float GunRange => _gunRange;

    public float TotalDamagePerProjectile => _damagePerProjectile;
    public float TotalDamage => _totalDamage;
    public float TotalSpeed => _totalSpeed;
    public float TotalRange => _totalRange;

    // Bullet
    public BulletParameters BulletParameters => _bulletParameters;

#if UNITY_EDITOR
    private bool _hasBulletParameters = true;

    private float _lastGunDamage = 0f;
    private float _lastTotalDamage = 0f;
    private float _lastDamagePerProjectile = 0f;

    private float _lastGunSpeed = 0f;
    private float _lastTotalSpeed = 0f;

    private float _lastGunRange = 0f;
    private float _lastTotalRange = 0f;

    private void OnValidate()
    {
        UpdateDefaultParameters();
        if (_bulletParameters == null) return;
        UpdateDamages();
        UpdateSpeeds();
        UpdateRanges();
    }

    private void UpdateDefaultParameters()
    {
        if (_bulletParameters != null)
        {
            if (_hasBulletParameters) return;

            // Primeira vez colocando os parâmetros de bala
            _hasBulletParameters = true;
            _totalDamage = Mathf.Max(_bulletParameters.TotalDamage + _gunDamage, 0f);
            _damagePerProjectile = Mathf.Max(_totalDamage / _bulletParameters.ProjectilesPerShot, 0f);
            _totalSpeed = Mathf.Max(_bulletParameters.Speed + _gunSpeed, 0f);
            _totalRange = Mathf.Max(_bulletParameters.Range + _gunRange, 0f);
        }
        else if(_hasBulletParameters)
        {
            _hasBulletParameters = false;
        }
    }

    private void UpdateDamages()
    {
        if (_lastGunDamage != _gunDamage)
        {
            _gunDamage = Mathf.Max(_gunDamage, -_bulletParameters.TotalDamage);

            UpdateGunDamage(_gunDamage);
            UpdateTotalDamage(_gunDamage + _bulletParameters.TotalDamage);
            UpdateDamagePerProjectile(_totalDamage / _bulletParameters.ProjectilesPerShot);
        }
        else if (_lastTotalDamage != _totalDamage)
        {
            UpdateTotalDamage(_totalDamage);
            UpdateGunDamage(_totalDamage - _bulletParameters.TotalDamage);
            UpdateDamagePerProjectile(_totalDamage / _bulletParameters.ProjectilesPerShot);
        }
        else if (_lastDamagePerProjectile != _damagePerProjectile)
        {
            UpdateDamagePerProjectile(_damagePerProjectile);
            UpdateTotalDamage(_bulletParameters.ProjectilesPerShot * _damagePerProjectile);
            UpdateGunDamage(_totalDamage - _bulletParameters.TotalDamage);
        }
    }

    private void UpdateGunDamage(float newDamage)
    {
        _gunDamage = newDamage;
        _lastGunDamage = _gunDamage;
    }

    private void UpdateTotalDamage(float newDamage)
    {
        _totalDamage = newDamage;
        _lastTotalDamage = _totalDamage;
    }

    private void UpdateDamagePerProjectile(float newDamage)
    {
        _damagePerProjectile = newDamage;
        _lastDamagePerProjectile = _damagePerProjectile;
    }

    private void UpdateSpeeds()
    {
        if (_lastGunSpeed != _gunSpeed)
        {
            _gunSpeed = Mathf.Max(_gunSpeed, -_bulletParameters.Speed);

            _lastGunSpeed = _gunSpeed;
            _totalSpeed = _gunSpeed + _bulletParameters.Speed;
            _lastTotalSpeed = _totalSpeed;
        }
        else if (_lastTotalSpeed != _totalSpeed)
        {
            _lastTotalSpeed = _totalSpeed;
            _gunSpeed = _totalSpeed - _bulletParameters.Speed;
            _lastGunSpeed = _gunSpeed;
        }
    }

    private void UpdateRanges()
    {
        if (_lastGunRange != _gunRange)
        {
            _gunRange = Mathf.Max(_gunRange, -_bulletParameters.Range);

            _lastGunRange = _gunRange;
            _totalRange = _gunRange + _bulletParameters.Range;
            _lastTotalRange = _totalRange;
        }
        else if (_lastTotalRange != _totalRange)
        {
            _lastTotalRange = _totalRange;
            _gunRange = _totalRange - _bulletParameters.Range;
            _lastGunRange = _gunRange;
        }
    }
#endif
}
