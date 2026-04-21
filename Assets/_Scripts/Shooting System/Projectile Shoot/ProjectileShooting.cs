using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;

public class ProjectileShooting : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _gunNameTextMesh;
    [SerializeField] private GameObject _bulletPrefab;
    [Space]
    [SerializeField] private List<GunParameters> _gunParameters;
    private GunParameters _currentGun;
    private int _currentGunIndex;

    private float _shootingTimer;
    private Coroutine _shootingCoroutine;

    private void OnEnable()
    {
        PlayerInputsHandler.ShootInputed += OnShootInputed;
        PlayerInputsHandler.ScrollInputed += OnScrollInputed;
    }

    private void OnDisable()
    {
        PlayerInputsHandler.ShootInputed -= OnShootInputed;
        PlayerInputsHandler.ScrollInputed -= OnScrollInputed;
    }

    private void Start()
    {
        _currentGunIndex = 0;
        UpdateCurrentGun();
    }

    private void Update()
    {
        ShootingTimer();
    }

    private void OnShootInputed(bool isShooting)
    {
        if (_shootingCoroutine != null)
            StopCoroutine(_shootingCoroutine);

        if (isShooting)
            _shootingCoroutine = StartCoroutine(Shooting());
    }

    private void OnScrollInputed(float value)
    {
        if (value == 0f) return;
        value = Mathf.Sign(value);

        int count = _gunParameters.Count;
        _currentGunIndex = (_currentGunIndex + (int)value % count + count) % count;
        UpdateCurrentGun();
    }

    private IEnumerator Shooting()
    {
        while (true)
        {
            yield return null;
            if (_shootingTimer > 0f) continue;

            for (int i = 0; i < _currentGun.BulletsPerShoot; i++)
            {
                Shoot();
            }

            _shootingTimer = _currentGun.FireRate;
        }
    }

    private void Shoot()
    {
        float spread = _currentGun.SpreadAngle;
        Vector3 bulletDirection = Quaternion.Euler(
            Random.Range(-spread, spread),
            Random.Range(-spread, spread),
            0f)
            * transform.forward;

        GameObject bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<BulletMovement>().ShootBullet(bulletDirection, _currentGun.Range);
        bullet.GetComponent<BulletCollision>().SetDamage(_currentGun.Damage);
    }

    private void ShootingTimer()
    {
        if (_shootingTimer > 0f)
            _shootingTimer -= Time.deltaTime;
    }

    private void UpdateCurrentGun()
    {
        _shootingTimer = 0f;
        _currentGun = _gunParameters[_currentGunIndex];
        _gunNameTextMesh.text = _currentGun.GunName;
    }
}
