using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovementInput : MonoBehaviour, IMovementInputHandler
{
    public Vector2 MovementDirection { get; private set; }

    public event Action<bool> JumpPressed;

    public void OnMove(InputValue value)
    {
        MovementDirection = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        JumpPressed?.Invoke(value.isPressed);
    }
}
