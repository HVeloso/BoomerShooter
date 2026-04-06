using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputsHandler : MonoBehaviour
{
    public static event Action<Vector2> MovementDirectionInputed;
    public static event Action<Vector2> LookDirectionInputed;
    public static event Action<bool> JumpInputed;

    private void OnMove(InputValue value)
    {
        Vector2 direction = value.Get<Vector2>();
        direction.Normalize();

        MovementDirectionInputed?.Invoke(direction);
    }

    private void OnLook(InputValue value)
    {
        Vector2 direction = value.Get<Vector2>();
        direction.Normalize();

        LookDirectionInputed?.Invoke(direction);
    }

    private void OnJump(InputValue value)
    {
        if (value.isPressed)
            JumpInputed?.Invoke(true);
        else
            JumpInputed?.Invoke(false);
    }
}
