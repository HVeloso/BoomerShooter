using System.Collections.Generic;
using UnityEngine;

public class DamageUIPopPool : MonoBehaviour
{
    public static DamageUIPopPool Instance { get; private set; }

    [SerializeField] private GameObject _textPrefab;
    private readonly Queue<DamagePopController> _textPool = new();

    private void OnDisable()
    {
        StopAllCoroutines();
        UnregisterEvents();
    }

    private void Awake()
    {
        SetSingleTon();
    }

    private void SetSingleTon()
    {
        if (Instance != null)
            Destroy(gameObject);

        Instance = this;
    }

    public void InstantiateUIPop(Vector3 spawnPoint, Transform player, float damage)
    {
        if (!_textPool.TryDequeue(out DamagePopController damagePop))
        {
            GameObject newTextMesh = Instantiate(_textPrefab, transform);

            if (!newTextMesh.TryGetComponent(out damagePop))
                throw new System.Exception($"The object {newTextMesh.name} is not recognized as a Projectile. IProjectileHandler interface is missing.");

            damagePop.ObjectDisabled += OnObjectDisabled;
        }

        spawnPoint += (player.position - spawnPoint).normalized * 0.15f;
        damagePop.StartPopAnimation(spawnPoint, player, damage.ToString("N0"));
    }

    private void UnregisterEvents()
    {
        while(_textPool.TryDequeue(out DamagePopController pop))
        {
            pop.ObjectDisabled -= OnObjectDisabled;
        }
    }

    private void OnObjectDisabled(DamagePopController pop)
    {
        _textPool.Enqueue(pop);
    }
}
