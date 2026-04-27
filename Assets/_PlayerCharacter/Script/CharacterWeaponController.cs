using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterWeaponController : MonoBehaviour
{
    [Header("INPUT")]
    [SerializeField] private GameObject _weaponInputObj;
    private IWeaponInputHandler _weaponInput;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _weaponNameTextMesh;

    [Header("Weapon Parameters")]
    [SerializeField] private List<GameObject> _weaponPrefabs;
    
    private List<IWeaponHandler> _weaponsList;
    private IWeaponHandler _currentWeapon;
    private int _weaponIndex = 0;

    private void OnDisable()
    {
        _weaponInput.AttackInputed += OnShootInputed;
        _weaponInput.ScrollInputed += OnScrollInputed;
    }

    private void Awake()
    {
        LoadInterfaces();
        InitializeWeaponList();
    }

    private void Start()
    {
        UpdateCurrentWeapon();
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
        if (_weaponsList.Count <= 0) return;

        value = Mathf.Sign(value);

        int count = _weaponsList.Count;
        _weaponIndex = (_weaponIndex + (int)value % count + count) % count;

        UpdateCurrentWeapon();
    }

    private void OnShootInputed(bool value)
    {
        _currentWeapon?.AttackInputed(value);
    }

    private void InitializeWeaponList()
    {
        _weaponsList = new();

        foreach (GameObject weaponObj in _weaponPrefabs)
        {
            GameObject weapon = Instantiate(weaponObj, transform);

            if (!weapon.TryGetComponent(out IWeaponHandler weaponHandler))
                throw new System.Exception($"The object {weapon.name} is not recognized as a Weapon. IWeaponHandler interface is missing.");

            weapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            weaponHandler.SetActive(false);
            _weaponsList.Add(weaponHandler);
        }
    }

    private void UpdateCurrentWeapon()
    {
        if (_weaponsList.Count <= 0) return;
        _currentWeapon?.SetActive(false);

        _currentWeapon = _weaponsList[_weaponIndex];
        _currentWeapon.SetActive(true);

        _weaponNameTextMesh.text = _currentWeapon.WeaponName;
    }
}
