using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterWeaponInput : MonoBehaviour, IWeaponInputHandler
{
    public event Action<bool> AttackInputed;
    public event Action<float> ScrollInputed;

    public void OnAttack(InputValue value)
    {
        AttackInputed?.Invoke(value.isPressed);
    }

    public void OnScroll(InputValue value)
    {
        ScrollInputed?.Invoke(value.Get<float>());
    }
}
