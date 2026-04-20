using System.Collections;
using TMPro;
using UnityEngine;

public class DummyEnemy : MonoBehaviour, IHittable
{
    [SerializeField] private TextMeshPro _hitTextMesh;
    private Rigidbody _dummyRigidbody;

    private void Awake()
    {
        _dummyRigidbody = GetComponent<Rigidbody>();
        _hitTextMesh.gameObject.SetActive(false);
    }

    public void Hit(Transform player, float damage)
    {
        StopAllCoroutines();
        StartCoroutine(ShowHit(player));

        Vector3 forceDirection = (transform.position - player.position).normalized;
        _dummyRigidbody.AddForce(forceDirection * damage, ForceMode.Impulse);
    }

    private IEnumerator ShowHit(Transform player)
    {
        _hitTextMesh.transform.LookAt(player);

        Vector3 angles = _hitTextMesh.transform.eulerAngles;
        angles.x *= -1f;
        angles.y += 180f;

        _hitTextMesh.transform.eulerAngles = angles;

        _hitTextMesh.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        _hitTextMesh.gameObject.SetActive(false);
    }
}
