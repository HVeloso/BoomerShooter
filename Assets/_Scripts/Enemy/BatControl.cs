using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BatControl : MonoBehaviour, IHittable
{
    [SerializeField, Min(0)] private float _health;
    [SerializeField, Min(0)] private float _safeDistance;
    [SerializeField, Min(0)] private float _nonMoveDistance;
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
        _agent.isStopped = false;
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

        Vector3 targetPosition = _player.position;

        if (distance < _safeDistance)
        {
            _agent.isStopped = false;

            Vector3 direction = (transform.position - _player.position).normalized;
            targetPosition += direction * _nonMoveDistance;
        }
        else if (distance < _nonMoveDistance)
        {
            _agent.isStopped = true;
        }
        else
            _agent.isStopped = false;

        _agent.destination = targetPosition;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _nonMoveDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _safeDistance);
    }
}
