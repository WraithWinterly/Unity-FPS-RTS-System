/*
 * 2022 WraithWinterly
 * Help from
 * https://github.com/beaucarnes/unity_fps/blob/master/complete_project/Assets/Scripts/Player%20Scripts/MouseLook.cs
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCamera : MonoBehaviour
{
    public Camera Cam { get; private set; }
    public Vector2 Rotation => _rotation;
    public float Sensitivity { get; private set; } = 2f;

    private const float FOVSmoothing = Const.smoothingNormal;
    private const float PosYNormal = 1f;
    private const float PosYCrouch = 0.25f;
    private const int FOVSprintAddition = 10;

    private Vector2 _rotation;
    private Vector2 _mouseMovement;

    [SerializeField] private Player player;
    [SerializeField] private FPSController controller;
    [SerializeField] private WeaponSystem weaponSystem;

    [Header("Camera")]
    [SerializeField] private Camera weaponCamera;

    [Header("FOV (Do NOT Use The Camera Component's FOV)")]
    [SerializeField] private float camFOV = 70f;

    private void Awake()
    {
        Cam = GetComponent<Camera>();
    }

    private void Update()
    {
        HandleRotation();
        HandleTransform();
        HandleSens();
        HandleFOV();
    }

    private void HandleRotation()
    {
        if (CanMoveFPSCamera())
        {
            _mouseMovement = new Vector2(Input.GetAxis(Const.mouseX), Input.GetAxis(Const.mouseY));

            _rotation.x += _mouseMovement.x * Sensitivity;
            _rotation.y += _mouseMovement.y * Sensitivity * (SettingsManager.InvertY ? 1f : -1f);
        }

        _rotation.x += weaponSystem.GetCurrentRecoil().x;
        _rotation.y -= weaponSystem.GetCurrentRecoil().y;

        _rotation.y = Mathf.Clamp(_rotation.y, Const.CameraLimit.x, Const.CameraLimit.y);

        Cam.transform.localRotation = Quaternion.Euler(_rotation.y, 0f, 0f);
        //Cam.transform.localRotation = Quaternion.Euler(_rotation.y, 0f, 0f);
        player.transform.localRotation = Quaternion.Euler(0f, _rotation.x, 0f);
    }

    private bool CanMoveFPSCamera()
    {
        return RTSManager.FPSMode() && Cursor.lockState == CursorLockMode.Locked && !Cursor.visible;
    }

    private void HandleTransform()
    {
        if (controller.IsCrouching)
        {
            Cam.transform.localPosition = new Vector3(
                x: 0,
                y: Utils.Damp(Cam.transform.localPosition.y, PosYCrouch, Const.smoothingNormal, Time.deltaTime),
                z: 0);
        }
        else
        {
            Cam.transform.localPosition = new Vector3(
                x: 0,
                y: Utils.Damp(Cam.transform.localPosition.y, PosYNormal, Const.smoothingNormal, Time.deltaTime),
                z: 0);
        }
    }

    private void HandleSens()
    {
        Sensitivity = Utils.Damp(
            Sensitivity, weaponSystem.IsAiming() ? SettingsManager.SensitivityADS
                : SettingsManager.Sensitivity, Const.smoothingFast, Time.deltaTime);
    }

    private void HandleFOV()
    {
        // FOV Changes
        if (controller.IsSprinting)
        {
            Cam.fieldOfView = Utils.Damp(Cam.fieldOfView, camFOV + FOVSprintAddition, FOVSmoothing, Time.deltaTime);
        }
        else if (weaponSystem.IsAiming())
        {
            Cam.fieldOfView = Utils.Damp(Cam.fieldOfView, camFOV - (int)weaponSystem.SObj.zoomFactor, FOVSmoothing, Time.deltaTime);
        }
        else
        {
            Cam.fieldOfView = Utils.Damp(Cam.fieldOfView, camFOV, FOVSmoothing, Time.deltaTime);
        }
        weaponCamera.fieldOfView = player.Cam.fieldOfView;
    }


}

