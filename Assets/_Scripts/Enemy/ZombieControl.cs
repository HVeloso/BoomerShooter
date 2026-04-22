using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieControl : MonoBehaviour, IHittable
{
    [SerializeField, Min(0)] private float _health;
    [SerializeField, Min(0)] private float _chaseDistance;
    [SerializeField, Min(0)] private float _speed;
    [SerializeField] private TextMeshPro _healthTextMesh;
    [Space]
    [SerializeField] private Transform _player;

    private float _currentHealth;
    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        _currentHealth = _health;
        _agent.speed = _speed;
        UpdateHealth();
    }

    private void Update()
    {
        UpdateHealthTextRotation();
        UpdateNavMeshTarget();
    }

    void IHittable.Hit(ProjectileParameters parameters)
    {
        if (_currentHealth <= 0f) return;

        _currentHealth = Mathf.Max(0f, _currentHealth - parameters.Damage);
        if (_currentHealth <= 0f)
        {
            _agent.isStopped = false;
            _healthTextMesh.gameObject.SetActive(false);
            Destroy(gameObject);
        }

        UpdateHealth();
    }

    private void UpdateNavMeshTarget()
    {
        if (_currentHealth <= 0f) return;

        float distance = Vector3.Distance(transform.position, _player.position);

        _agent.isStopped = distance <= _chaseDistance;
        _agent.destination = _player.position;
    }

    private void UpdateHealthTextRotation()
    {
        _healthTextMesh.transform.LookAt(_player);

        Vector3 angles = _healthTextMesh.transform.eulerAngles;
        angles.x *= -1f;
        angles.y += 180f;

        _healthTextMesh.transform.eulerAngles = angles;
    }

    private void UpdateHealth()
    {
        _healthTextMesh.text = _currentHealth.ToString("N2");
    }
}
