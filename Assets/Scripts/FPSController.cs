/*
 * 2022 WraithWinterly
 * Help from https://github.com/beaucarnes/unity_fps/blob/master/complete_project/Assets/Scripts/Player%20Scripts/PlayerMovement.cs
 * WraithWinterly
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FPSController : MonoBehaviour
{
    public bool IsSprinting { get; private set; }

    public bool IsGrounded => _controller.isGrounded;
    public bool IsMoving { get; private set; }
    public bool IsRising => _yVel > 0;
    public bool IsFalling => _yVel < 0;
    public bool IsCrouching { get; private set; } = false;

    // Set these!!!
    [HideInInspector] public float xAxis;
    [HideInInspector] public float zAxis;
    [HideInInspector] public bool inputJumpTap;
    [HideInInspector] public bool inputJumpHold;
    [HideInInspector] public bool inputJumpLetGo;
    [HideInInspector] public bool inputCrouch;
    [HideInInspector] public bool inputSprint;
    [HideInInspector] public bool inputSprintTap;
    [HideInInspector] public bool shooting;
    [HideInInspector] public bool reloading;
    [HideInInspector] public bool switchingWeapons;

    [HideInInspector] public bool aiming;
    // Set those ^^^

    private CharacterController _controller;

    private Vector3 _movement;
    private Vector3 _velocity;

    private float _yVel;
    private float _currentSpeed;
    private float _jumpTimer;
    private float _lerpSpeed;
    private float _jumpBufferTimer;

    private bool _jumpBuffer;
    private bool _jumping;
    private bool _canRise;
    private bool _isRising;
    private bool _yVelReset;
    private bool _falling;
    private bool _onCeiling;
    private bool _landEffect;


    // Having events in inspector cause them to not work (because you would have to manually add them there)
    [HideInInspector] public UnityEvent landed = new();

    [Header("General")] [SerializeField] private LayerMask whatIsCeiling;
    [SerializeField] private Transform ceilingCheck;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float walkSpeed = 5.5f;
    [SerializeField] private float crouchSpeed = 3.5f;
    [SerializeField] private float landEffectSpeed = -8.5f;

    [Tooltip("Height, Offset, Ceiling Collider Position")] [SerializeField]
    private Vector3 collisionHeightOffsetNormal = new(2.5f, 0, -0.4f);

    [Tooltip("Height, Offset, Ceiling Collider Position")] [SerializeField]
    private Vector3 collisionHeightOffsetCrouch = new(1.75f, -0.4f, 0.5f);

    [Header("Jumping / Falling")] [SerializeField]
    private float maxRiseTime = 0.125f;

    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float riseForce = 45;
    [SerializeField] private float jumpBufferTimerMax = 0.2f;
    [SerializeField] private float gravity = 29.4f;

    [Header("Lerp Speeds")] [SerializeField]
    private float walkLerpSpeed = 15f;

    [SerializeField] private float sprintLerpSpeed = 10f;
    [SerializeField] private float airLerpSpeed = 7f;

    [Header("Sounds")] [SerializeField] private AudioSource jumpAudioSource;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Line order matters!!!
        HandleCrouching();
        HandleSprinting();
        SetSpeeds();

        if (IsGrounded)
        {
            IsMoving = Mathf.Abs(xAxis) + Mathf.Abs(zAxis) > 0;
        }

        if (!_controller.isGrounded)
            _yVel -= gravity * Time.deltaTime;

        _movement = new Vector3(xAxis, 0f, zAxis);
        _movement = transform.TransformDirection(_movement);
        _movement = _movement.normalized;
        _movement *= _currentSpeed;

        HandleJumping();
        HandleLerpSpeed();

        _velocity.x = Mathf.Lerp(_velocity.x, _movement.x, _lerpSpeed * Time.deltaTime);
        _velocity.z = Mathf.Lerp(_velocity.z, _movement.z, _lerpSpeed * Time.deltaTime);
        _velocity.y = _movement.y;

        _controller.Move(new Vector3(_velocity.x * Time.deltaTime, _velocity.y, _velocity.z * Time.deltaTime));

        _onCeiling = Physics.OverlapSphere(ceilingCheck.transform.position, 0.5f, whatIsCeiling).Length > 0;

        if (_onCeiling && _yVel > 0)
        {
            _yVel = 0;
            _jumping = false;
        }
    }

    public void InputJumpTap()
    {
        inputJumpTap = true;
    }

    public void InputJumpHold()
    {
        inputJumpHold = true;
    }

    public void InputJumpLetGo()
    {
        inputJumpLetGo = true;
    }

    public void InputMove(float h, float z)
    {
        xAxis = h;
        zAxis = z;
    }

    public void InputCrouch()
    {
        inputCrouch = true;
    }

    public void InputSprint()
    {
        inputSprint = true;
    }

    public void InputSprintTap()
    {
        inputSprintTap = true;
    }

    private void SetSpeeds()
    {
        if (IsCrouching)
        {
            _currentSpeed = crouchSpeed;
        }
        else if (IsSprinting)
        {
            _currentSpeed = sprintSpeed;
        }
        else
        {
            _currentSpeed = walkSpeed;
        }
    }

    private void HandleJumping()
    {
        if (_controller.isGrounded)
        {
            if (_landEffect)
            {
                _landEffect = false;
                landed.Invoke();
            }

            //Prevent gravity from increasing when on floor
            if (!_yVelReset && !inputJumpTap && !_jumpBuffer && _yVel < -0.4f)
            {
                _yVel = -0.45f;
                _yVelReset = true;
            }

            if (_jumping && !_jumpBuffer)
            {
                // Reset Jump
                _jumping = false;
                _canRise = false;
                _jumpTimer = maxRiseTime;
            }

            if (inputJumpTap)
            {
                Jump();
            }

            if (_jumpBuffer)
            {
                _jumpBuffer = false;
                _jumpBufferTimer = 0;

                Jump();
            }
        }
        else // Not Grounded
        {
            if (_yVel <= landEffectSpeed)
            {
                _landEffect = true;
            }

            if (inputJumpHold && _canRise)
            {
                // Jump Rise
                _jumpTimer -= Time.deltaTime;
                _yVel += riseForce * Time.deltaTime;
            }

            if (_falling && inputJumpTap)
            {
                _jumpBuffer = true;
                _jumpBufferTimer = jumpBufferTimerMax;
            }

            if (_jumpBuffer)
            {
                _jumpBufferTimer -= Time.deltaTime;
                if (_jumpBufferTimer <= 0)
                {
                    _jumpBuffer = false;
                }
            }

            if (inputJumpLetGo || _jumpTimer <= 0)
            {
                _canRise = false;
            }
        }

        _movement.y = _yVel * Time.deltaTime;
        _falling = _yVel < 0;
    }

    private void Jump()
    {
        IsCrouching = false;
        jumpAudioSource.Play();
        _yVel = jumpForce;
        _jumpTimer = maxRiseTime;
        _yVelReset = false;
        _jumping = true;
        _canRise = true;
    }

    private void HandleCrouching()
    {
        if (inputCrouch)
        {
            if (!IsCrouching) IsSprinting = false;
            IsCrouching = !IsCrouching;
        }

        if (IsCrouching)
        {
            _controller.height = collisionHeightOffsetCrouch.x;
            _controller.center = new Vector3(0, collisionHeightOffsetCrouch.y, 0);
            var localPosition = ceilingCheck.localPosition;
            localPosition = new Vector3(localPosition.x, collisionHeightOffsetCrouch.z, localPosition.z);
            ceilingCheck.localPosition = localPosition;
        }
        else
        {
            _controller.height = collisionHeightOffsetNormal.x;
            _controller.center = new Vector3(0, collisionHeightOffsetNormal.y, 0);
            var localPosition = ceilingCheck.localPosition;
            localPosition = new Vector3(localPosition.x, collisionHeightOffsetNormal.z, localPosition.z);
            ceilingCheck.localPosition = localPosition;
        }
    }

    private void HandleSprinting()
    {
        if (shooting || switchingWeapons || aiming || reloading)
        {
            IsSprinting = false;
            return;
        }

        if (!_controller.isGrounded)
        {
            return;
        }

        if (IsCrouching && inputSprint)
        {
            IsCrouching = false;
            IsSprinting = true;
        }

        IsSprinting = inputSprint && zAxis > 0;
    }

    private void HandleLerpSpeed()
    {
        if (_controller.isGrounded)
        {
            _lerpSpeed = IsSprinting ? sprintLerpSpeed : walkLerpSpeed;
        }
        else
        {
            _lerpSpeed = IsSprinting ? airLerpSpeed / 2 : airLerpSpeed;
        }
    }
}