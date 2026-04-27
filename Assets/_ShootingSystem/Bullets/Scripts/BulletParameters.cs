using UnityEngine;

[CreateAssetMenu(fileName = "Bullet Parameters", menuName = "Boomer Shooter/Bullet Parameters")]
public class BulletParameters : ScriptableObject
{
    [Header("Bullet Parameters")]
    [SerializeField] private string _bulletName;
    [Space]
    [SerializeField, Min(1)] private int _projectilesPerShot = 1;
    [SerializeField, Min(0)] private float _totalDamage;
    [SerializeField, Min(0)] private float _damagePerProjectile;

    [SerializeField, Min(0)] private float _speed;
    [SerializeField, Min(0)] private float _range;

    public string Name => _bulletName;
    public float ProjectilesPerShot => _projectilesPerShot;
    public float TotalDamage => _totalDamage;
    public float DamagePerProjectile => _damagePerProjectile;
    public float Speed => _speed;
    public float Range => _range;

#if UNITY_EDITOR
    private float _lastTotalDamage = 0;
    private float _lastDamagePerProjectile = 0;
    private int _lastProjectilesPerShot = 1;

    private void OnValidate()
    {
        UpdateValues();
    }

    private void UpdateValues()
    {
        if (_lastProjectilesPerShot != _projectilesPerShot)
        {
            _lastProjectilesPerShot = _projectilesPerShot;
            _totalDamage = _projectilesPerShot * _damagePerProjectile;
            _lastTotalDamage = _totalDamage;
        }
        else if (_lastTotalDamage != _totalDamage)
        {
            _lastTotalDamage = _totalDamage;
            _damagePerProjectile = _totalDamage / _projectilesPerShot;
        }
        else if(_lastDamagePerProjectile != _damagePerProjectile)
        {
            _lastDamagePerProjectile = _damagePerProjectile;
            _totalDamage = _damagePerProjectile * _projectilesPerShot;
        }
    }
#endif
}
