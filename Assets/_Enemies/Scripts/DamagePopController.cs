using System;
using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class DamagePopController : MonoBehaviour
{
    [SerializeField, Min(0)] private float _heightLimit;
    [SerializeField, Min(0)] private float _animSpeed;

    public event Action<DamagePopController> ObjectDisabled;
    private Coroutine _animCoroutine;
    private TextMeshPro _popText;

    private void OnDisable()
    {
        if (_animCoroutine != null)
            StopCoroutine(_animCoroutine);
    }

    private void Awake()
    {
        _popText = GetComponent<TextMeshPro>();
    }

    public void StartPopAnimation(Vector3 spawnPoint, Transform lookTarget, string text)
    {
        if (_animCoroutine != null)
            StopCoroutine(_animCoroutine);

        gameObject.SetActive(true);
        _popText.text = text;
        _animCoroutine = StartCoroutine(PopAnimation(spawnPoint, lookTarget));
    }

    private IEnumerator PopAnimation(Vector3 spawnPoint, Transform lookTarget)
    {
        transform.position = spawnPoint;

        while (transform.position.y < spawnPoint.y + _heightLimit)
        {
            // Position 
            Vector3 newPos = transform.position;
            newPos.y += _animSpeed * Time.deltaTime;
            transform.position = newPos;

            // Rotation
            transform.LookAt(lookTarget);

            Vector3 angles = transform.eulerAngles;
            angles.x = 0f;
            angles.y += 180f;

            transform.eulerAngles = angles;

            yield return null;
        }

        gameObject.SetActive(false);
        ObjectDisabled?.Invoke(this);
    }
}
