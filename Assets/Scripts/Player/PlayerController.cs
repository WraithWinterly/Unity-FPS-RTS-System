/*
 * 2022 WraithWinterly
 * This class has help from
 * https://github.com/beaucarnes/unity_fps/blob/master/complete_project/Assets/Scripts/Player%20Scripts/PlayerMovement.cs
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IController
{
    private FPSController _controller;
    private Player _player;

    [SerializeField] private WeaponSystem weaponSystem;
    [SerializeField] private AudioSource landAudioSource;

    private void Awake()
    {
        _controller = GetComponent<FPSController>();

        _player = GetComponent<Player>();
        OnEnable();
    }

    private void OnEnable()
    {
        _controller?.landed.AddListener(OnLand);
    }

    private void Update()
    {
        if (RTSManager.RTSMode())
        {
            ResetInput();
            return;
        }

        _controller.xAxis = Input.GetAxisRaw(Const.horizontal);
        _controller.zAxis = Input.GetAxisRaw(Const.vertical);
        _controller.inputJumpTap = Input.GetButtonDown(Const.jump);
        _controller.inputJumpLetGo = Input.GetButtonUp(Const.jump);
        _controller.inputJumpHold = Input.GetButton(Const.jump);
        _controller.inputSprint = Input.GetButton(Const.sprint);
        _controller.inputSprintTap = Input.GetButtonDown(Const.sprint);
        _controller.inputCrouch = Input.GetButtonDown(Const.crouch);

        _controller.shooting = weaponSystem.IsShooting();
        _controller.reloading = weaponSystem.IsReloading();
        _controller.aiming = weaponSystem.IsAiming();
        _controller.switchingWeapons = weaponSystem.SwitchingWeapons;
    }

    private void ResetInput()
    {
        _controller.xAxis = 0;
        _controller.zAxis = 0;
        _controller.inputJumpTap = false;
        _controller.inputJumpLetGo = false;
        _controller.inputJumpHold = false;
        _controller.inputSprint = false;
        _controller.inputSprintTap = false;
        _controller.inputCrouch = false;
    }

    private void OnLand()
    {
        Transform camTransform = _player.Cam.transform;
        Vector3 localPosition = camTransform.localPosition;

        localPosition = new Vector3(localPosition.x, localPosition.y - 0.15f, localPosition.z);
        camTransform.localPosition = localPosition;
        landAudioSource.Play();
    }

    // IController
    public bool IsGrounded()
    {
        return _controller.IsGrounded;
    }

    public bool IsSprinting()
    {
        return _controller.IsSprinting;
    }

    public bool IsCrouching()
    {
        return _controller.IsCrouching;
    }

    public bool IsMoving()
    {
        return _controller.IsMoving;
    }

    public bool IsRising()
    {
        return _controller.IsRising;
    }
    public bool IsFalling()
    {
        return _controller.IsFalling;
    }
    // End IController

}