using UnityEngine;

[CreateAssetMenu(fileName = "Gun Parameters", menuName = "Boomer Shooter/Gun Parameters")]
public class GunParameters : ScriptableObject
{
    [Header("Gun Parameters")]
    [SerializeField] private string _gunName;

    [Header("Shoot Parameters")]
    [SerializeField, Min(0)] private float _damage;
    [SerializeField, Min(0)] private float _speed = 50f;
    [SerializeField, Min(0)] private float _fireRate;
    [SerializeField, Min(0)] private float _range;

    [Header("Spread Parameters")]
    [SerializeField, Min(0)] private int _bulletsPerShoot;
    [SerializeField, Min(0)] private float _spreadAngle;

    public string GunName => _gunName;
    public float Damage => _damage;
    public float Speed => _speed;
    public float FireRate => _fireRate;
    public float Range => _range;
    public int BulletsPerShoot => _bulletsPerShoot;
    public float SpreadAngle => _spreadAngle;
}
