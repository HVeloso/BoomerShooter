using System.Collections.Generic;
using UnityEngine;

public class ProjectilePoolManager : MonoBehaviour
{
    public static ProjectilePoolManager Instance { get; private set; }
    
    [SerializeField] private GameObject _projectilePrefab;
    private readonly Queue<IProjectileHandler> _projectilePool = new();

    private void Awake()
    {
        SetSingleTon();
    }

    private void OnDisable()
    {
        UnregisterEvents();

        if (Instance == this)
            Instance = null;
    }

    private void SetSingleTon()
    {
        if (Instance != null)
            Destroy(gameObject);

        Instance = this;
    }

    public void InstantiateProjectile(Vector3 originPoint, Vector3 direction, GunParameters gunParams)
    {
        ProjectileParameters parameters = new(originPoint, direction, gunParams);

        if (!_projectilePool.TryDequeue(out IProjectileHandler projectileHandler))
        {
            GameObject newProjectile = Instantiate(_projectilePrefab, transform);
            newProjectile.SetActive(false);

            if (!newProjectile.TryGetComponent(out projectileHandler))
                throw new System.Exception($"The object {newProjectile.name} is not recognized as a Projectile. IProjectileHandler interface is missing.");

            projectileHandler.ObjectDisabled += OnObjectDisabled;
        }

        projectileHandler.ActiveBullet(parameters);
    }

    private void UnregisterEvents()
    {
        while (_projectilePool.TryDequeue(out IProjectileHandler projectile))
        {
            projectile.ObjectDisabled -= OnObjectDisabled;
        }
    }

    private void OnObjectDisabled(IProjectileHandler projectile)
    {
        _projectilePool.Enqueue(projectile);
    }
}
