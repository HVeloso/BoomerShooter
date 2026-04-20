using System.Collections.Generic;
using UnityEngine;

public class ScenarioObstaclesHandler : MonoBehaviour
{
    [SerializeField] private float _maxHorizontalRange;
    [SerializeField] private float _minHorizontalRange;
    [Space]
    [SerializeField] private float _maxVerticalRange;
    [SerializeField] private float _minVerticalRange;
    [Space]
    [SerializeField] private List<Transform> _scenarioObstacles;

    private void Start()
    {
        RandomizeObstacles();
    }

    private void RandomizeObstacles()
    {
        foreach (Transform obstacle in _scenarioObstacles)
        {
        float verticalPosition = Random.Range(_minVerticalRange, _maxVerticalRange);
        float horizontalPosition = Random.Range(_minHorizontalRange, _maxHorizontalRange);

            Vector3 originalPosition = obstacle.position;
            originalPosition.x = horizontalPosition;
            originalPosition.z = verticalPosition;

            obstacle.position = originalPosition;
        }
    }
}
