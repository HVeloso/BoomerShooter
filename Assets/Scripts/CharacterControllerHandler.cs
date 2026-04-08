using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerHandler : MonoBehaviour, IBodyHandler
{
    // Components
    private CharacterController _characterController;

    // Immutable Parameters
    private readonly float _groundStickForce = -1;

    // Parameters
    [Header("Movement Parameters")]
    [SerializeField, Min(0)] private float smoothValue = 0.05f;
    [SerializeField, Min(0)] private float _moveSpeed = 15.0f;
    [SerializeField, Min(0)] private float _airControlMultiplier = 1.2f;

    [Header("Jump Parameters")]
    [SerializeField, Min(0)] private float _jumpHeight = 5.0f;
    [SerializeField] private bool _allowJumpCut;
    [SerializeField, Range(0, 1)] private float _jumpCutMultiplier = 0.4f;
    [SerializeField, Min(0)] private float _jumpBufferToleranceTime = 0.25f;
    [SerializeField, Min(0)] private float _coyoteToleranceTime = 0.15f;

    [Header("Gravity Parameters")]
    [SerializeField, Min(0)] private float _gravity = 35.0f;
    [SerializeField, Min(0)] private float _fallGravityMultiplier = 1.5f;

    // Horizontal Movement
    private Vector3 _movementDirection;
    private Vector3 _currentHorizontal;
    private Vector3 _horizontalVelocitySmooth; // Internal factor that SmoothDamp uses to calculate smoothing over time.

    // Vertical Movement
    private float _verticalVelocity;

    // States
    private bool _isGrounded;
    private bool _jumpRequested;
    private bool _hasJumped; // Has Jumped exists cuz if the player was launched up from an enemy or a scenario prop the gravity multiplier will not be active.

    // Timer
    private float _jumpBufferTimer;
    private float _coyoteTimer;

    // MonoBehaviours
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        UpdateTimer();

        GroundCheck();
        HandleJump();
        ApplyGravity();
        HandleMovement();
    }

    // Input Functions
    public void SetMovementDirection(Vector3 movementDirection)
    {
        movementDirection.y = 0f;
        _movementDirection = movementDirection.normalized;
    }

    public void UpdateJumpInput(bool isJumpPressed)
    {
        if (isJumpPressed)
        {
            if (_isGrounded) _jumpRequested = true;
            _jumpBufferTimer = _jumpBufferToleranceTime;
            return;
        }

        _jumpRequested = false;
        _jumpBufferTimer = 0f;

        if (_verticalVelocity > 0f && _allowJumpCut)
            _verticalVelocity *= _jumpCutMultiplier;
    }

    // Control Functions
    private void GroundCheck()
    {
        _isGrounded = _characterController.isGrounded;

        if (!_isGrounded) return;

        _coyoteTimer = _coyoteToleranceTime;
        _hasJumped = false;

        // Keep the player on the ground
        if (_verticalVelocity < 0f)
            _verticalVelocity = _groundStickForce;
    }

    private void HandleJump()
    {
        bool canJump = _isGrounded || _coyoteTimer > 0f;
        bool hasBufferedJump = _jumpRequested || _jumpBufferTimer > 0f;

        if (!canJump || !hasBufferedJump) return;

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

        return _movementDirection * currentMoveSpeed;
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
