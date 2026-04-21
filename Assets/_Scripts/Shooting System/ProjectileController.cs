using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileController : MonoBehaviour, IProjectileHandler
{
    private Queue<IProjectileHandler> _returnQueue;
    private ProjectileParameters _parameters;

    private Rigidbody _projectileRb;

    private void Awake()
    {
        _projectileRb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float distanceTraveled = Vector3.Distance(transform.position, _parameters.OriginPoint);
        
        if (distanceTraveled < _parameters.Range)
            return;

        DisableProjectile();
    }

    void IProjectileHandler.ActiveBullet(ProjectileParameters parameters, Queue<IProjectileHandler> returnQueue)
    {
        _returnQueue = returnQueue;
        
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
        _returnQueue.Enqueue(this);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IHittable hit))
            hit.Hit(_parameters);

        DisableProjectile();
    }
}
