using System.Collections;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private Transform _characterRotationBody;
    [SerializeField] private GameObject _bodyHandlerObj;

    private IBodyHandler _bodyHandler;
    private IEnumerator _movementEnumerator;

    private void OnEnable()
    {
        PlayerInputsHandler.MovementDirectionInputed += OnMoveInputed;
    }

    private void OnDisable()
    {
        PlayerInputsHandler.MovementDirectionInputed -= OnMoveInputed;
    }

    private void Awake()
    {
        _bodyHandler = _bodyHandlerObj.GetComponent<IBodyHandler>();
    }

    private void OnMoveInputed(Vector2 inputDirection)
    {
        if (_movementEnumerator != null)
            StopCoroutine(_movementEnumerator);

        if (inputDirection == Vector2.zero)
        {
            _movementEnumerator = null;
            _bodyHandler.SetMovementDirection(Vector3.zero);
            return;
        }

        Vector3 moveDirection = new(
            inputDirection.x,
            0f,
            inputDirection.y);

        _movementEnumerator = SetCharVelocity(moveDirection);
        StartCoroutine(_movementEnumerator);
    }

    private IEnumerator SetCharVelocity(Vector3 velocity)
    {
        while (true)
        {
            Vector3 camFoward = _characterRotationBody.forward;
            Vector3 camRight = _characterRotationBody.right;

            camFoward.y = 0f;
            camRight.y = 0f;

            camFoward.Normalize();
            camRight.Normalize();

            Vector3 moveInput = (camFoward * velocity.z + camRight * velocity.x);
            _bodyHandler.SetMovementDirection(moveInput);
            yield return null;
        }
    }
}
