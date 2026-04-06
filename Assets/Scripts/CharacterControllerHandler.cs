using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerHandler : MonoBehaviour, IBodyHandler
{
    // Components
    private CharacterController _characterController;

    // Parameters
    [Header("Movement Parameters")]
    [SerializeField, Min(0)] private float _moveSpeed = 15.0f;
    [SerializeField, Min(0)] private float _airControlMultiplier = 0.5f;

    [Header("Jump Parameters")]
    [SerializeField, Min(0)] private float _jumpHeight = 5.0f;
    [SerializeField, Min(0)] private float _jumpToleranceTime = 0.25f;
    [SerializeField, Range(0, 1)] private float _jumpCutMultiplier = 0.4f;

    [Header("Gravity Parameters")]
    [SerializeField, Min(0)] private float _gravity = 35.0f;
    [SerializeField, Min(1)] private float _fallGravityMultiplier = 1.5f;

    // Horizontal Movement
    private Vector3 _movementDirection;

    // Vertical Movement
    private float _verticalVelocity;

    // States
    private bool _isGrounded;
    private bool _jumpRequested;
    private bool _hasJumped;

    // Timer
    private float _jumpBufferTimer;

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
            _jumpRequested = true;
            _jumpBufferTimer = _jumpToleranceTime;
            return;
        }

        if (_verticalVelocity > 0f)
            _verticalVelocity = _jumpCutMultiplier;
    }

    // Control Functions
    private void GroundCheck()
    {
        _isGrounded = _characterController.isGrounded;

        if (!_isGrounded) return;

        _hasJumped = false;

        // Mantém o jogador no chão, impedindo "tropeços".
        if (_verticalVelocity < 0f)
            _verticalVelocity = -1f;
    }

    private void HandleJump()
    {
        bool hasBufferedJump = _jumpRequested || _jumpBufferTimer > 0f;
        if (!_isGrounded || !hasBufferedJump) return;

        _verticalVelocity = GetJumpForce();

        _jumpBufferTimer = 0f;
        _jumpRequested = false;
        _hasJumped = true;
    }

    private void ApplyGravity()
    {
        if (_isGrounded) return;

        _verticalVelocity -= GetTotalGravity() * Time.deltaTime;
    }

    // Movement Functions
    private void HandleMovement()
    {
        Vector3 horizontalVelocity = GetHorizontalVelocity();
        Vector3 verticalVelocity = Vector3.up * _verticalVelocity;

        Vector3 totalVelocity = horizontalVelocity + verticalVelocity;
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
        return Mathf.Sqrt(_jumpHeight * 2f * _gravity);
    }

    // Jump Buffer Functions
    private void UpdateTimer()
    {
        if (_jumpBufferTimer > 0f)
            _jumpBufferTimer -= Time.deltaTime;
    }
}
