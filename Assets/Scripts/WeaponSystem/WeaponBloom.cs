/*
 * 2022 WraithWinterly
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponBloom : MonoBehaviour
{
    public float CurrentBloom { get; private set; }

    private const float DampSpeed = 0.000001f;
    private const float NormalFactor = 10f; // Always active
    private const float CrouchFactor = -10f;
    private const float MoveFactor = 35f;
    private const float SprintFactor = 10f;
    private const float AirFactor = 20f;
    private const float RisingFactor = 5f;
    private const float ShootingFactorMult = 500f;
    private const float ShootingFactorMax = 175f;

    private WeaponSystem _weaponSystem;
    private IController Controller => _weaponSystem.Controller;

    private void Awake()
    {
        _weaponSystem = GetComponent<WeaponSystem>();

        //_controller = GetComponent<FPSController>();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (UIDebugMenu.NoBloom)
        {
            CurrentBloom = 0;
            return;
        }
#endif
        CalculateBloom();
    }

    private void CalculateBloom()
    {
        if (!_weaponSystem.HoldingGun)
        {
            ResetBloom();
            return;
        }
        var desiredFactor = 0f;

        desiredFactor += NormalFactor;

        if (Controller.IsMoving())
        {
            desiredFactor += MoveFactor;
        }

        if (Controller.IsCrouching())
        {
            desiredFactor += CrouchFactor;
        }

        if (Controller.IsSprinting())
        {
            desiredFactor += SprintFactor;
        }

        if (!Controller.IsGrounded())
        {
            desiredFactor += AirFactor;
        }

        if (Controller.IsRising())
        {
            desiredFactor += RisingFactor;
        }

        desiredFactor += (int)_weaponSystem.SObj.bloomRate;

        float shootingFactor = ShootingFactorMult * _weaponSystem.GetShootingTime();

        shootingFactor = Mathf.Clamp(shootingFactor, 0, ShootingFactorMax);
        desiredFactor += shootingFactor;

        CurrentBloom = _weaponSystem.IsAiming()
            ? Utils.Damp(CurrentBloom, 0, DampSpeed, Time.deltaTime)
            : Utils.Damp(CurrentBloom, desiredFactor, DampSpeed, Time.deltaTime);
    }

    public Ray CreateRayWithBloom()
    {
        Ray ray = _weaponSystem.Cam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
        ray.direction = (ray.direction * 100 + Random.insideUnitSphere * (CurrentBloom / 11)).normalized;
        return ray;
    }

    private void ResetBloom()
    {
        CurrentBloom = 0;
    }
}
