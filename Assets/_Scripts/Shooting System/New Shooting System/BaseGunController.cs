using UnityEngine;
using System.Collections;
using UnityEditor;

public abstract class BaseGunController : MonoBehaviour, IGunHandler
{
    [Header("Gun Parameters")]
    [SerializeField] protected GunParameters _parameters;
    [SerializeField] private GameObject _gunHolder;
    
    string IGunHandler.GunName => _parameters.GunName;

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

    void IGunHandler.SetActive(bool active)
    {
        _gunHolder.SetActive(active);
    }

    void IGunHandler.ShootInputed(bool inputValue)
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

            for (int i = 0; i < _parameters.BulletsPerShoot; i++)
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
}
