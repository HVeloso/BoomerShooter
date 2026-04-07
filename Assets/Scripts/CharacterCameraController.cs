using Unity.Cinemachine;
using UnityEngine;

public class CharacterCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineInputAxisController _cameraInputs;
    [SerializeField] private CinemachinePanTilt _cameraPanTilt;
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

    private Vector2 _rollDirection;

    private float _roll; // Rotação no eixo z (gira para os lados)

    private void OnEnable()
    {
        PlayerInputsHandler.MovementDirectionInputed += OnMoveInputed;
    }

    private void OnDisable()
    {
        PlayerInputsHandler.MovementDirectionInputed -= OnMoveInputed;
    }

    private void LateUpdate()
    {
        UpdateCameraRoll();
        UpdateCharacterRotation();
    }

    private void Start()
    {
        SettingUpCinemachineCamera();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _roll = transform.localEulerAngles.z;
    }

    private void OnMoveInputed(Vector2 inputDirection)
    {
        _rollDirection = inputDirection;
    }

    private void SettingUpCinemachineCamera()
    {
        _cameraInputs.Controllers[0].Input.Gain = _yawSensitivity; // horizontal
        _cameraInputs.Controllers[1].Input.Gain = -_pitchSensitivity; // vertical

        _cameraPanTilt.TiltAxis.Range = new(_minPitch, _maxPitch);
    }

    private void UpdateCameraRoll()
    {
        float targetRoll = 0f;

        if (_rollDirection.x != 0f)
            targetRoll = -Mathf.Sign(_rollDirection.x) * _maxRollAngle;

        _roll = Mathf.LerpAngle(_roll, targetRoll, _cameraRollVelocity * Time.deltaTime);

        Vector3 currentEuler = transform.localEulerAngles;
        _characterHead.localRotation = Quaternion.Euler(currentEuler.x, currentEuler.y, _roll);
    }

    private void UpdateCharacterRotation()
    {
        _characterBody.localEulerAngles = _cameraInputs.transform.localEulerAngles;
    }

    private void OnValidate()
    {
        SettingUpCinemachineCamera();
    }
}
