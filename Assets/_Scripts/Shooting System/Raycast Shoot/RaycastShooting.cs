using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class RaycastShooting : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _gunNameTextMesh;
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
            if (_shootingTimer <= 0f)
            {
                Shoot();
                _shootingTimer = _currentGun.FireRate;
            }
            
            yield return null;
        }
    }

    private void Shoot()
    {
        Ray shootingRay = new(transform.position, transform.forward);
        if (!Physics.Raycast(shootingRay, out RaycastHit hit, _currentGun.Range)) return;
        
        if (hit.collider.TryGetComponent(out IHittable hittable))
        {
            hittable.Hit(transform, _currentGun.Damage);
        }
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
