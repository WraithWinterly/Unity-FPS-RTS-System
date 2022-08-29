/*
 * 2022 WraithWinterly
 * Follow raywenderlich's Style Guide!
 * https://github.com/raywenderlich/c-sharp-style-guide
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTransform : MonoBehaviour
{
    private const float WeaponBobHeightCrouch = 0.01f;
    private const float WeaponBobHeightNormal = 0.015f;
    private const float WeaponBobHeightSprint = 0.02f;
    private const float BobSpeedIdle = 0.75f;
    private const float BobSpeedNormal = 4f;
    private const float BobSpeedSprint = 5f;
    private const float SwayStrength = 0.001f;
    private const float SwayStrengthAim = -0.0005f;

    // Bobbing
    private float _weaponBobTime;
    private float _weaponBobHeight;
    private float _bobSpeed;

    private WeaponSystem _weaponSystem;

    private IController Controller => _weaponSystem.Controller;

    [SerializeField] private bool isPlayer;

    private void Start()
    {
        _weaponSystem = GetComponent<WeaponSystem>();

    }

    private void Update()
    {
        HandleWeaponSway();
        HandleWeaponBobbing();
        HandleWeaponPosition();
    }

    private void HandleWeaponSway()
    {
        if (!isPlayer) return;

        var swayStrength = SwayStrength;

        if (_weaponSystem.IsAiming())
        {
            swayStrength = SwayStrengthAim;
        }

        if (Cursor.visible) return;

        Vector3 localPos = transform.localPosition;
        localPos.x += Input.GetAxis(Const.mouseX) * Refs.Inst.Player.CamScript.Sensitivity * swayStrength;
        localPos.y += Input.GetAxis(Const.mouseY) * Refs.Inst.Player.CamScript.Sensitivity * swayStrength;
        transform.localPosition = localPos;
    }

    private Vector3 HandleWeaponBobbing()
    {
        // Only bob on ground
        if (Controller.IsGrounded())
        {
            // Set bob speeds
            if (Controller.IsMoving())
            {
                if (Controller.IsCrouching())
                {
                    _weaponBobHeight = WeaponBobHeightCrouch;
                    _bobSpeed = BobSpeedNormal;
                }
                else
                {
                    if (Controller.IsSprinting())
                    {
                        _weaponBobHeight = WeaponBobHeightSprint;
                        _bobSpeed = BobSpeedSprint;
                    }
                    else
                    {
                        _weaponBobHeight = WeaponBobHeightNormal;
                        _bobSpeed = BobSpeedNormal;
                    }
                }
            }
            else
            {
                _weaponBobHeight = WeaponBobHeightCrouch;
                _bobSpeed = BobSpeedIdle;

            }
        }
        // Weapon rising effect when falling
        else
        {
            if (!Controller.IsRising())
            {
                _weaponBobHeight = 0.03f;
                _weaponBobTime = 1;
            }
            // Reset all, no bob
            else
            {
                _weaponBobHeight = 0;
                _bobSpeed = 0;
                _weaponBobTime = 0;
            }
        }

        // Trig to calculate wave
        float wave = Mathf.Sin(_weaponBobTime) * _weaponBobHeight;

        _weaponBobTime += Time.deltaTime * _bobSpeed;
        return new Vector3(0, wave, 0);
    }

    private void HandleWeaponPosition()
    {
        // Sin wave is used for the weapon bobbing
        Vector3 sineWave = HandleWeaponBobbing();

        if (_weaponSystem.SwitchingWeapons)
        {
            if (_weaponSystem.SwitchingWeaponsPart1)
            {
                transform.localPosition =
                    Utils.Damp(transform.localPosition, _weaponSystem.SwapPos, _weaponSystem.Gun.SwapSpeed, Time.deltaTime);
            }
            else if (_weaponSystem.SwitchingWeaponsPart2)
            {
                transform.localPosition =
                    Utils.Damp(transform.localPosition, _weaponSystem.SObj.defaultPos, Const.smoothingNormal, Time.deltaTime);
            }
            return;
        }

        if (_weaponSystem.IsReloading())
        {
            transform.localPosition = Utils.Damp(transform.localPosition, _weaponSystem.ReloadPos,
                Const.smoothingNormal, Time.deltaTime);
        }
        else if (_weaponSystem.IsAiming())
        {
            transform.localPosition = Utils.Damp(transform.localPosition,
                _weaponSystem.SObj.aimPos + sineWave * 0.1f, _weaponSystem.GetAimSpeed(),
                Time.deltaTime);
        }
        else
        {
            transform.localPosition = Utils.Damp(transform.localPosition, _weaponSystem.SObj.defaultPos + sineWave,
                _weaponSystem.GetAimSpeed(), Time.deltaTime);
        }
    }

}
