using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterGunController : MonoBehaviour
{
    [Header("INPUT")]
    [SerializeField] private GameObject _weaponInputObj;
    private IWeaponInputHandler _weaponInput;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _gunNameTextMesh;

    [Header("Guns Parameters")]
    [SerializeField] private List<GameObject> _gunPrefabs;

    private List<IGunHandler> _guns;
    private IGunHandler _currentGun;
    private int _gunIndex = 0;

    private void OnDisable()
    {
        _weaponInput.AttackInputed += OnShootInputed;
        _weaponInput.ScrollInputed += OnScrollInputed;
    }

    private void Awake()
    {
        LoadInterfaces();
        InitializeGunList();
    }

    private void Start()
    {
        UpdateCurrentGun();
    }

    private void LoadInterfaces()
    {
        if (InterfaceTreatment.TryExtractInterface(_weaponInputObj, out _weaponInput))
        {
            _weaponInput.AttackInputed += OnShootInputed;
            _weaponInput.ScrollInputed += OnScrollInputed;
        }
    }

    private void OnScrollInputed(float value)
    {
        if (value == 0f) return;
        value = Mathf.Sign(value);

        int count = _guns.Count;
        _gunIndex = (_gunIndex + (int)value % count + count) % count;

        UpdateCurrentGun();
    }

    private void OnShootInputed(bool value)
    {
        _currentGun.ShootInputed(value);
    }

    private void InitializeGunList()
    {
        _guns = new();

        foreach (GameObject gunObj in _gunPrefabs)
        {
            GameObject gun = Instantiate(gunObj, transform);

            if (!gun.TryGetComponent(out IGunHandler gunHandler))
                throw new System.Exception($"The object {gun.name} is not recognized as a Gun. IGunHandler interface is missing.");

            gun.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            gunHandler.SetActive(false);
            _guns.Add(gunHandler);
        }
    }

    private void UpdateCurrentGun()
    {
        _currentGun?.SetActive(false);

        _currentGun = _guns[_gunIndex];
        _currentGun.SetActive(true);

        _gunNameTextMesh.text = _currentGun.GunName;
    }
}
