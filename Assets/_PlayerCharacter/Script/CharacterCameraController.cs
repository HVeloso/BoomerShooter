using Unity.Cinemachine;
using UnityEngine;

public class CharacterCameraController : MonoBehaviour
{
    [Header("INPUT")]
    [SerializeField] private GameObject _movementInputObj;
    private IMovementInputHandler _movementInput;

    [Header("Camera References")]
    [SerializeField] private CinemachineCamera _cineCamera;
    [SerializeField] private CinemachineInputAxisController _cameraInputs;
    [SerializeField] private CinemachinePanTilt _cameraPanTilt;
    [SerializeField] private Transform _characterBody;

    [Header("Camera Sensitivity")]
    [SerializeField][Min(0)] private float _pitchSensitivity;
    [SerializeField][Min(0)] private float _yawSensitivity;
    [SerializeField][Min(0)] private float _cameraRollVelocity;

    [Header("Camera Limits")]
    [SerializeField] private float _maxPitch;
    [SerializeField] private float _minPitch;
    [SerializeField][Min(0)] private float _maxRollAngle;

    // Rool Parameters
    private float _rollValue; // Rotação no eixo z (gira para os lados)

    // MonoBehaviour
    private void Awake()
    {
        LoadInterfaces();
    }

    private void Start()
    {
        SettingUpCinemachineCamera();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _rollValue = transform.localEulerAngles.z;
    }

    private void LateUpdate()
    {
        UpdateCameraRoll();
        UpdateCharacterRotation();
    }

    // Inputs
    private void LoadInterfaces()
    {
        InterfaceTreatment.TryExtractInterface(_movementInputObj, out _movementInput);
    }

    // Initialization
    private void SettingUpCinemachineCamera()
    {
        _cameraInputs.Controllers[0].Input.Gain = _yawSensitivity; // horizontal
        _cameraInputs.Controllers[1].Input.Gain = -_pitchSensitivity; // vertical

        _cameraPanTilt.TiltAxis.Range = new(_minPitch, _maxPitch);
    }

    // Angle Functions
    private void UpdateCameraRoll()
    {
        float targetRoll = 0f;
        float rollDirection = _movementInput.MovementDirection.x;

        if (rollDirection != 0f)
            targetRoll = -Mathf.Sign(rollDirection) * _maxRollAngle;

        _rollValue = Mathf.LerpAngle(_rollValue, targetRoll, _cameraRollVelocity * Time.deltaTime);
        _cineCamera.Lens.Dutch = _rollValue;
    }

    private void UpdateCharacterRotation()
    {
        Vector3 currentRotation = _characterBody.eulerAngles;
        currentRotation.y = _cameraPanTilt.PanAxis.Value;

        _characterBody.localRotation = Quaternion.Euler(currentRotation);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        SettingUpCinemachineCamera();
    }
#endif
}
