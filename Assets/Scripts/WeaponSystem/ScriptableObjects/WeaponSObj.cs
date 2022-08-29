/*
 * 2022 WraithWinterly
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "", menuName = "ScriptableObjects/Weapon ScriptableObject")]
public class WeaponSObj : ScriptableObject
{
    public enum WeaponType
    {
        Gun,
        Knife,
        Throwable
    }

    public enum FireRates
    {
        Extreme,
        Fast,
        Normal,
        Slow,
        None
    }

    public enum AimSpeeds
    {
        Extreme,
        Fast,
        Normal,
        Slow,
        None
    }

    public enum SwapSpeeds
    {
        Extreme,
        Fast,
        Normal,
        Slow,
        None
    }

    public enum RecoilRatesX
    {
        Extreme,
        High,
        Normal,
        Low,
        None
    }

    public enum RecoilRatesY
    {
        Extreme,
        High,
        Normal,
        Low,
        None
    }

    public enum ZoomFactors
    {
        High = 30,
        Normal = 20,
        Low = 10,
        None = 0
    }

    public enum BloomRates
    {
        High = 30,
        Normal = 10,
        Low = 5,
        None = 0
    }

    public enum MagSizes
    {
        _26 = 26,
        _31 = 31,
        _46 = 46,
        _61 = 61,
        _100000 = 100000,
        None = 0
    }

    [Header("All")] public WeaponType weaponType = WeaponType.Gun;
    public SwapSpeeds swapSpeed = SwapSpeeds.Normal;
    public Vector3 defaultPos = new(0.3f, -0.275f, 0.3f);
    public int damage = 30;

    [Header("Guns")] public FireRates fireRate = FireRates.Normal;
    public AimSpeeds aimSpeed = AimSpeeds.Normal;
    public RecoilRatesX recoilRateX = RecoilRatesX.Normal;
    public RecoilRatesY recoilRateY = RecoilRatesY.Normal;
    public ZoomFactors zoomFactor = ZoomFactors.Normal;
    public BloomRates bloomRate = BloomRates.Normal;
    public MagSizes magSize = MagSizes._31;
    public Vector3 aimPos = new(0, -0.254f, 0.1f);
    public Vector3 reloadPos = new(0.3f, -0.475f, 0.3f);
    public float xWeaponKick = 0.001f;
    public float zWeaponKick = 0.05f;
    public float zWeaponKickAim = -0.06f;
    public float reloadTimeSeconds = 1;
    public int magCount = 5;
    public bool semiAuto;

    [Header("Knife")] public float knifeSpeedSeconds = 0.5f;

    [Header("Throwables")] public float cookTimeSeconds = -1;
    public float throwDistance = 5;

    public void Awake()
    {
        if (weaponType != WeaponType.Gun)
        {
            fireRate = FireRates.None;
            aimSpeed = AimSpeeds.None;
            recoilRateX = RecoilRatesX.None;
            recoilRateY = RecoilRatesY.None;
            zoomFactor = ZoomFactors.None;
            bloomRate = BloomRates.None;
            magSize = MagSizes.None;
            aimPos = Vector3.zero;
            reloadPos = Vector3.zero;
            xWeaponKick = 0;
            zWeaponKick = 0;
            zWeaponKickAim = 0;
            reloadTimeSeconds = 0;
            magCount = 0;
            semiAuto = false;
        }

        if (weaponType != WeaponType.Knife)
        {
            knifeSpeedSeconds = 0;
        }

        // ReSharper disable once InvertIf
        if (weaponType != WeaponType.Throwable)
        {
            cookTimeSeconds = 0;
            throwDistance = 0;
        }
    }
}