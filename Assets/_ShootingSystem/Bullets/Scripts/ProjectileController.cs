using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileController : MonoBehaviour, IProjectileHandler
{
    private ProjectileParameters _parameters;
    private Rigidbody _projectileRb;

    public event Action<IProjectileHandler> ObjectDisabled;

    private void Awake()
    {
        _projectileRb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        DisableByDistance();
    }

    void IProjectileHandler.ActiveBullet(ProjectileParameters parameters)
    {
        UpdateParameters(parameters);
        gameObject.SetActive(true);
    }

    private void UpdateParameters(ProjectileParameters newParameters)
    {
        _parameters = newParameters;

        Quaternion rotation = Quaternion.LookRotation(_parameters.Direction);
        transform.SetPositionAndRotation(_parameters.OriginPoint, rotation);

        _projectileRb.linearVelocity = transform.forward * _parameters.Speed;
    }

    private void DisableProjectile()
    {
        gameObject.SetActive(false);
        ObjectDisabled?.Invoke(this);
    }

    private void DisableByDistance()
    {
        float distanceTraveled = Vector3.Distance(transform.position, _parameters.OriginPoint);

        if (distanceTraveled < _parameters.Range)
            return;

        DisableProjectile();
    }

    private void OnCollisionEnter(Collision collision)
    {
        _parameters.SetHitPoint(collision.contacts[0].point);

        if (collision.collider.TryGetComponent(out IHittable hit))
            hit.Hit(_parameters);

        DisableProjectile();
    }
}
