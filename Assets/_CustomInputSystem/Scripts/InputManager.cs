using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[DisallowMultipleComponent]
public class InputManager : MonoBehaviour
{
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        _playerInput.actions.Disable();
        _playerInput.currentActionMap?.Enable();
    }
}
