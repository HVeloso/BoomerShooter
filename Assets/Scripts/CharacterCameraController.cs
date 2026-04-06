using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CharacterCameraController : MonoBehaviour
{
    [SerializeField] private Transform _characterHead;
    [SerializeField] private Transform _characterBody;
    [Space]
    [SerializeField][Min(0)] private float _pitchSensitivity;
    [SerializeField][Min(0)] private float _yawSensitivity;
    [SerializeField][Min(0)] private float _cameraRollVelocity;
    [Space]
    [SerializeField] private float _maxPitch;
    [SerializeField] private float _minPitch;
    [Space]
    [SerializeField][Min(0)] private float _maxRollAngle;

    private Vector2 _lookDirection;
    private Vector2 _rollDirection;

    private float _yaw; // Rotação no eixo y (gira par aos lados)
    private float _pitch; // Rotação no eixo x (gira pra cima e para baixo)
    private float _roll; // Rotação no eixo z (gira para os lados)

    private void OnEnable()
    {
        PlayerInputsHandler.LookDirectionInputed += OnLookOutputed;
        PlayerInputsHandler.MovementDirectionInputed += OnMoveInputed;
    }

    private void OnDisable()
    {
        PlayerInputsHandler.LookDirectionInputed -= OnLookOutputed;
        PlayerInputsHandler.MovementDirectionInputed -= OnMoveInputed;
    }

    private void LateUpdate()
    {
        transform.position = _characterHead.position;

        UpdateCameraLookDirection();
        UpdateCameraRoll();

        UpdateCameraAngles();
        UpdateCharacterRotation();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _yaw = transform.localEulerAngles.y;
        _pitch = transform.localEulerAngles.x;
        _roll = transform.localEulerAngles.z;
    }

    private void OnLookOutputed(Vector2 inputDirection)
    {
        if (inputDirection.x != 0f)
            inputDirection.x = Mathf.Sign(inputDirection.x);

        if (inputDirection.y != 0f)
            inputDirection.y = Mathf.Sign(inputDirection.y);

        _lookDirection = inputDirection.normalized;
    }

    private void OnMoveInputed(Vector2 inputDirection)
    {
        _rollDirection = inputDirection;
    }

    private void UpdateCameraLookDirection()
    {
        if (_lookDirection == Vector2.zero) return;
        
        _yaw += _lookDirection.x * _yawSensitivity;
        _pitch -= _lookDirection.y * _pitchSensitivity;

        _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
    }

    private void UpdateCameraRoll()
    {
        float targetRoll = 0f;

        if (_rollDirection.x != 0f)
            targetRoll = Mathf.Sign(_rollDirection.x) * _maxRollAngle;

        _roll = Mathf.Lerp(_roll, -targetRoll, _cameraRollVelocity * Time.deltaTime);
    }

    private void UpdateCameraAngles()
    {
        transform.localRotation = Quaternion.Euler(_pitch, _yaw, _roll);
    }

    private void UpdateCharacterRotation()
    {
        Vector3 charRot = _characterBody.localEulerAngles;
        charRot.y = _yaw;
        _characterBody.localEulerAngles = charRot;
    }
}
