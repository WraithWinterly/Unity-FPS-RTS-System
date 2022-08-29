/*
 * 2022 WraithWinterly
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSCamera : MonoBehaviour
{
    private const float AnimationSpeedFPS = 0.000_000_001f;
    private const float AnimationSpeedRts = 0.05f;
    private const float CameraSwapPoint = 0.175f;
    private const float RTSCameraHMoveSpeed = 7f;

    private Camera _cam;

    private Vector3 _lastPosRts = new(0, 20, 0);
    private Quaternion _lastRtsRot = Quaternion.Euler(60, 0, 0);
    private Vector3 _intendedPos = new(0, 20, 0);
    private Quaternion _intendedRot = Quaternion.Euler(60, 0, 0);

    private float _animationSpeed;

    private bool _modeSwapped = false;
    private bool _doNotDamp = false;

    [SerializeField] private GameManager game;
    [SerializeField] private RTSManager rtsManager;
    [SerializeField] private Vector2 yLimit = new(10, 40);
    [Tooltip("Min X, Max X, Min Z, Max Z")]
    [SerializeField] private Vector4 hLimit = new(-100, 100, -100, 100);

    private void Start()
    {
        _cam = GetComponent<Camera>();
        _cam.enabled = false;
        OnEnable();
    }

    private void OnEnable()
    {
        Refs.Inst.EventManager?.modeSwitchedFPS.AddListener(OnFPSMode);
        Refs.Inst.EventManager?.modeSwitchedRTS.AddListener(OnRTSMode);
    }

    private void OnFPSMode()
    {
        _doNotDamp = false;
        _lastPosRts = _intendedPos;
        _lastRtsRot = _intendedRot;
        _modeSwapped = true;
        GameManager.TryLockCursor();
    }

    private void OnRTSMode()
    {
        _doNotDamp = false;
        _intendedPos = _lastPosRts;
        _intendedRot = _lastRtsRot;
        _modeSwapped = false;
        GameManager.UnlockCursor();
        _cam.enabled = true;
        rtsManager.WeaponCam.enabled = false;
        rtsManager.FPSCam.enabled = false;
    }

    private void Update()
    {
        if (RTSManager.RTSMode())
        {
            if (_modeSwapped)
            {

            }

            RunRTSCamera();

        }
        else if (RTSManager.FPSMode())
        {
            if (!_modeSwapped)
            {

            }

            RunFPSCamera();
        }

        if (_doNotDamp)
        {
            _cam.transform.SetPositionAndRotation(_intendedPos, _intendedRot);
        }
        else
        {
            _cam.transform.SetPositionAndRotation(
                Utils.Damp(_cam.transform.position, _intendedPos, _animationSpeed, Time.deltaTime),
                Utils.Damp(_cam.transform.rotation, _intendedRot, _animationSpeed, Time.deltaTime));
        }

    }

    private void RunFPSCamera()
    {
        _cam.fieldOfView = Refs.Inst.Player.Cam.fieldOfView;
        _animationSpeed = AnimationSpeedFPS;

        Transform camTransform = Refs.Inst.Player.Cam.transform;

        _intendedPos = camTransform.position;
        _intendedRot = camTransform.rotation;

        if (ShouldSwitchToPlayerCamera)
        {
            SwitchToPlayerCamera();
        }
    }

    private bool ShouldSwitchToPlayerCamera =>
        transform.position.y <= Refs.Inst.Player.Cam.transform.position.y + CameraSwapPoint;

    private void SwitchToPlayerCamera()
    {
        rtsManager.RTSCam.enabled = false;
        rtsManager.FPSCam.enabled = true;
        rtsManager.WeaponCam.enabled = true;
    }

    private void RunRTSCamera()
    {
        _cam.fieldOfView = 70;
        _animationSpeed = AnimationSpeedRts;

        Vector3 hPosInput = new()
        {
            x = Input.GetAxisRaw(Const.horizontal),
            y = 0,
            z = Input.GetAxisRaw(Const.vertical)
        };

        hPosInput = hPosInput.normalized;
        // Slow down when zoomed in, faster when zoomed out
        hPosInput *= RTSCameraHMoveSpeed * Mathf.Abs(_intendedPos.y);

        _intendedPos.y -= Input.GetAxis(Const.mouseScroll) * 20;
        _intendedPos += hPosInput * Time.deltaTime;

        KeepInLimits();
    }

    private void KeepInLimits()
    {
        const float smoothing = 0.0001f;

        if (_intendedPos.y < yLimit.x)
        {
            _intendedPos.y = Utils.Damp(_intendedPos.y, yLimit.x, smoothing, Time.deltaTime);
        }
        else if (_intendedPos.y > yLimit.y)
        {
            _intendedPos.y = Utils.Damp(_intendedPos.y, yLimit.y, smoothing, Time.deltaTime);
        }

        _intendedPos.x = Mathf.Clamp(_intendedPos.x, hLimit.x, hLimit.y);
        _intendedPos.z = Mathf.Clamp(_intendedPos.z, hLimit.z, hLimit.w);
    }
}
