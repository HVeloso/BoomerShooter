using System.Collections;
using TMPro;
using UnityEngine;

public class DummyEnemy : MonoBehaviour, IHittable
{
    [SerializeField] private TextMeshPro _hitTextMesh;
    [SerializeField] private Transform _player;

    private void Start()
    {
        _hitTextMesh.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_hitTextMesh.gameObject.activeSelf)
        {
            _hitTextMesh.transform.LookAt(_player);
            
            Vector3 angles = _hitTextMesh.transform.eulerAngles;
            angles.x *= -1f;
            angles.y += 180f;

            _hitTextMesh.transform.eulerAngles = angles;
        }
    }

    public void Hit()
    {
        StopAllCoroutines();
        StartCoroutine(ShowHit());
    }

    private IEnumerator ShowHit()
    {
        _hitTextMesh.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        _hitTextMesh.gameObject.SetActive(false);
    }
}
