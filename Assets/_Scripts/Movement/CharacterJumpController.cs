using UnityEngine;

public class CharacterJumpController : MonoBehaviour
{
    // Pegar por um service locator
    [SerializeField] private GameObject _bodyHandlerObj;
    private IBodyHandler _bodyHandler;

    // MonoBehaviour
    private void OnEnable()
    {
        PlayerInputsHandler.JumpInputed += OnJumpInputed;
    }

    private void OnDisable()
    {
        PlayerInputsHandler.JumpInputed -= OnJumpInputed;
    }

    private void Awake()
    {
        _bodyHandler = _bodyHandlerObj.GetComponent<IBodyHandler>();
    }

    // Event Functions
    private void OnJumpInputed(bool isJumpPressed)
    {
        _bodyHandler.UpdateJumpInput(isJumpPressed);
    }
}
