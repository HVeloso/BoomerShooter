using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerHandler : MonoBehaviour
{
    // Components
    private CharacterController _characterController;

    // Parameters
    [Header("INPUT")]
    [SerializeField] private GameObject _movementInputObj;
    private IMovementInputHandler _movementInput;

    [Header("Movement Parameters")]
    [SerializeField] private Transform _characterRotationReference;
    [SerializeField, Min(0)] private float smoothValue = 0.05f;
    [SerializeField, Min(0)] private float _moveSpeed = 15.0f;
    [SerializeField, Min(0)] private float _airControlMultiplier = 1.2f;

    [Header("Jump Parameters")]
    [SerializeField, Min(0)] private float _jumpHeight = 5.0f;
    [SerializeField, Min(0)] private int _jumpsInAir = 0;
    [Space]
    [SerializeField] private bool _allowJumpCut;
    [SerializeField, Range(0, 1)] private float _jumpCutMultiplier = 0.4f;
    [Space]
    [SerializeField, Min(0)] private float _jumpBufferToleranceTime = 0.25f;
    [SerializeField, Min(0)] private float _coyoteToleranceTime = 0.15f;

    [Header("Gravity Parameters")]
    [SerializeField, Min(0)] private float _gravity = 35.0f;
    [SerializeField, Min(0)] private float _fallGravityMultiplier = 1.5f;
    [SerializeField] private float _groundStickForce = -1;

    // Horizontal Movement
    private Vector3 _currentHorizontal;
    private Vector3 _horizontalVelocitySmooth; // Internal factor that SmoothDamp uses to calculate smoothing over time.

    // Vertical Movement
    private float _verticalVelocity;
    private int _currentJumpsInAir;

    // States
    private bool _jumpRequested;
    private bool _isGrounded;
    private bool _hasJumped; // Has Jumped exists cuz if the player was launched up from an enemy or a scenario prop the gravity multiplier will not be active.

    // Timer
    private float _jumpBufferTimer;
    private float _coyoteTimer;

    // MonoBehaviours
    private void OnDisable()
    {
        _movementInput.JumpPressed -= OnJumpPressed;
    }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        LoadInterfaces();
    }

    private void Update()
    {
        UpdateTimer();

        ApplyGravity();
        HandleJump();
        HandleMovement();
        GroundCheck();
        CollisionCheck();
    }

    private void LoadInterfaces()
    {
        if (InterfaceTreatment.TryExtractInterface(_movementInputObj, out _movementInput))
        {
            _movementInput.JumpPressed += OnJumpPressed;
        }
    }

    // Input Functions
    private void OnJumpPressed(bool isJumpPressed)
    {
        if (isJumpPressed)
        {
            if (_isGrounded) _jumpRequested = true;

            _jumpBufferTimer = _jumpBufferToleranceTime;
            return;
        }

        if (_allowJumpCut)
        {
            _jumpBufferTimer = 0f;

            if (_verticalVelocity > 0f)
                _verticalVelocity *= _jumpCutMultiplier;
        }

        _jumpRequested = false;
    }

    // Control Functions
    private void CollisionCheck()
    {
        if (_characterController.collisionFlags != CollisionFlags.CollidedAbove) return;
        if (_verticalVelocity <= 0f) return;

        _verticalVelocity = 0f;
    }

    private void GroundCheck()
    {
        _isGrounded = _characterController.isGrounded;

        if (!_isGrounded) return;

        _coyoteTimer = _coyoteToleranceTime;
        _hasJumped = false;
        _currentJumpsInAir = _jumpsInAir;

        // Keep the player on the ground
        if (_verticalVelocity < 0f)
            _verticalVelocity = _groundStickForce;
    }

    private void HandleJump()
    {
        bool canJump = _isGrounded || _coyoteTimer > 0f || _currentJumpsInAir > 0;
        bool hasBufferedJump = _jumpRequested || _jumpBufferTimer > 0f;

        if (!canJump || !hasBufferedJump) return;

        if (!_isGrounded && _coyoteTimer <= 0f)
            _currentJumpsInAir--;

        _verticalVelocity = GetJumpForce();

        _jumpBufferTimer = 0f;
        _coyoteTimer = 0f;
        _jumpRequested = false;
        _hasJumped = true;
    }

    private void ApplyGravity()
    {
        if (_isGrounded && _verticalVelocity <= 0f) return;

        _verticalVelocity -= GetTotalGravity() * Time.deltaTime;
    }

    // Movement Functions
    private void HandleMovement()
    {
        Vector3 horizontalVelocity = GetHorizontalVelocity();
        Vector3 verticalVelocity = Vector3.up * _verticalVelocity;

        _currentHorizontal = Vector3.SmoothDamp(
            _currentHorizontal,
            horizontalVelocity,
            ref _horizontalVelocitySmooth,
            smoothValue
        );

        Vector3 totalVelocity = _currentHorizontal + verticalVelocity;
        _characterController.Move(totalVelocity * Time.deltaTime);
    }

    private Vector3 GetHorizontalVelocity()
    {
        float currentMoveSpeed = _moveSpeed;

        if (!_isGrounded)
            currentMoveSpeed *= _airControlMultiplier;

        Vector3 characterForward = _characterRotationReference.forward;
        characterForward.y = 0f;
        characterForward.Normalize();

        Vector3 characterRight = _characterRotationReference.right;
        characterRight.y = 0f;
        characterRight.Normalize();

        Vector3 movementDirection = characterForward * _movementInput.MovementDirection.y;
        movementDirection += characterRight * _movementInput.MovementDirection.x;

        return movementDirection * currentMoveSpeed;
    }

    // Functions for calculating values
    private float GetTotalGravity()
    {
        float totalGravity = _gravity;

        if (_hasJumped && _verticalVelocity < 0f)
            totalGravity *= _fallGravityMultiplier;

        return totalGravity;
    }

    private float GetJumpForce()
    {
        /* Essa função usa a equação de Torricelli pra calcular a força do pulo.
         * Equação de Torricelli - é usada pra calcular velocidade, aceleração ou distância percorrida sem ter o valor do tempo.
         * A equação -> Vf^2 = Vi^2 + 2 * a * DeltaS.
         * (Velocidade final ao quadrado é igual a Velocidade inicial ao quadrado mais 2 vezes a aceleração vezes a Variação de posição [Delta S]).
         * 
         * No caso:
         * Velocidade inicial é igual a zero, então é desconsiderada na soma.
         * A aceleração é a da gravidade - já que é a força que temos que vencer para subir.
         * A variação de posição (DeltaS) é a altura do pulo.
         * O número 2 é uma constante da própria fórmula.
         * E a raiz quadrada vem do corte do expoente da velocidade final.
         */

        return Mathf.Sqrt(_jumpHeight * 2f * _gravity);
    }

    // Jump Buffer Functions
    private void UpdateTimer()
    {
        if (_jumpBufferTimer > 0f)
            _jumpBufferTimer -= Time.deltaTime;

        if (_coyoteTimer > 0f)
            _coyoteTimer -= Time.deltaTime;
    }
}
