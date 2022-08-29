/*
 * 2022 WraithWinterly
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    public GunMag GunMag { get; private set; }
    public float AimSpeed { get; private set; }
    public float FireRate { get; private set; }
    public float SwapSpeed { get; private set; } = 0.1f;
    public float ShootingTime { get; private set; }
    public bool Aiming { get; private set; }
    public bool Reloading { get; private set; }
    public bool Shooting { get; private set; }
    public float TimeNextFireAllowed { get; private set; }

    public Vector2 RecoilRate => _recoilRate;
    public WeaponSObj SObj => sObj;

    public WeaponSystem WeaponSystem { get; set; }

    private readonly WaitForSeconds _laserLineDuration = new(0.07f);
    private readonly float[] _fireRates = { 0.065f, 0.1f, 0.125f, 0.15f, 0 };
    private readonly float[] _aimSpeeds = { 0.00001f, 0.005f, 0.1f, 0.15f, 0 };
    private readonly float[] _swapSpeeds = { 0.001f, 0.01f, 0.2f, 0.325f, 0 };
    private readonly float[] _recoilRateXs = { 2.5f, 1f, 0.3f, 0.1f, 0 };
    private readonly float[] _recoilRateYs = { 4f, 2.5f, 2f, 1f, 0 };

    private LineRenderer _laserLine;
    private Vector2 _recoilRate;

    private float _reloadTimer = 0.0f;
    private float _basePitch;

    // Dependency Injection by the weapon manager
    public WeaponBloom bloom;
    public Camera cam;
    public WeaponSObj sObj;

    [Header("Effects")] [SerializeField] private AudioSource gunShotSound;
    [SerializeField] private AudioSource reloadSoundStart;
    [SerializeField] private AudioSource reloadSoundFinished;
    [SerializeField] private Transform gunBarrel;

    private void Awake()
    {
        GunMag = GetComponent<GunMag>();

        _laserLine = GetComponent<LineRenderer>();
        _laserLine.enabled = false;

        _basePitch = gunShotSound.pitch;
    }

    private void Start()
    {
        ApplyGunSettings();
    }

    private void Update()
    {
        ApplyGunSettings();
        HandleInput();
        HandleReloading();
        HandleShooting();
        HandleWeaponNoAmo();
        _laserLine.SetPosition(0, gunBarrel.position);

        if (SObj.semiAuto)
        {
            if (WeaponSystem.InputShootJustPressed && !CanShoot())
            {
                // shoot buffer
            }
        }


    }

    private void OnEnable()
    {
        _laserLine.enabled = false;
    }

    private void OnDisable()
    {
        _laserLine.enabled = false;
    }

    private void HandleInput()
    {
        Aiming = WeaponSystem.InputAim && CanAim();

        if (WeaponSystem.InputShoot && CanShoot())
        {
            Shoot();
        }


        if (WeaponSystem.InputShoot && CanShoot(includeFireRateCondition: false))
        {
            if (!sObj.semiAuto)
            {
                Shooting = true;
                ShootingTime += Time.deltaTime;
            }
            else
            {
                Shooting = true;
                ShootingTime += Time.deltaTime * 1000f;
            }
        }
        else
        {
            Shooting = false;
            ShootingTime = 0;
        }




        if (WeaponSystem.InputReload)
        {
            TryReload();
        }

        if (!Reloading) return;

        bool reloadCancelAttempt = WeaponSystem.InputShootJustPressed || WeaponSystem.InputSprintJustPressed;
        bool cancelReload = Reloading && WeaponSystem.IsPlayer && reloadCancelAttempt;

        if (!cancelReload) return;

        if (WeaponSystem.InputShootJustPressed) WeaponSystem.PlayerMouseUpRequired = true;
        TryCancelReload();
        Shooting = false;
        ShootingTime = 0;
    }

    private void HandleReloading()
    {
        if (Reloading)
        {
            _reloadTimer += Time.deltaTime;
            // % 60 allows getting the time in IRL seconds
            bool reloadFinished = _reloadTimer % 60 >= SObj.reloadTimeSeconds;
            if (!reloadFinished) return;
            ReloadFinished();
        }
        else
        {
            _reloadTimer = 0;
        }
    }

    private void ReloadFinished()
    {
        WeaponSystem.PlaySound(WeaponSystem.AudioSourceID.ReloadSoundFinish, reloadSoundFinished.clip);
        GunMag.FinishReload();
        Reloading = false;
        WeaponSystem.PlayerMouseUpRequired = false;
        _reloadTimer = 0;
    }

    private void TryCancelReload()
    {
        if (GunMag.CurrentMagAmo <= 0) return;
        CancelReload();
        Reloading = false;
    }

    public void CancelReload()
    {
        _reloadTimer = 0;
        WeaponSystem.StopSound(WeaponSystem.AudioSourceID.ReloadSoundStart);
        Reloading = false;
    }

    private void StartReloadEffect()
    {
        WeaponSystem.PlaySound(WeaponSystem.AudioSourceID.ReloadSoundStart, reloadSoundStart.clip);
    }

    private bool CanAim()
    {
        return !WeaponSystem.SwitchingWeapons && !WeaponSystem.IsReloading();
    }

    private bool CanShoot(bool includeFireRateCondition = true)
    {
        bool condition;
        if (includeFireRateCondition)
        {
            condition = GunMag.CurrentMagAmo > 0 && Time.time > TimeNextFireAllowed && !Reloading && !WeaponSystem.SwitchingWeapons;
        }
        else
        {
            condition = GunMag.CurrentMagAmo > 0 && !Reloading && !WeaponSystem.SwitchingWeapons;
        }

        if (!WeaponSystem.IsPlayer) return condition;

        if (WeaponSystem.PlayerMouseUpRequired) return false;
        if (sObj.semiAuto && !WeaponSystem.InputShootJustPressed) return false;

        return condition;
    }


    private void ShotHit(RaycastHit hit)
    {
        _laserLine.SetPosition(1, hit.point);
    }

    private void OnShotNoTarget()
    {
        Transform camTransform = Refs.Inst.Player.Cam.transform;
        _laserLine.SetPosition(1,
            camTransform.position + (camTransform.forward * 99));
    }

    private void Shoot()
    {
        TimeNextFireAllowed = Time.time + FireRate;
        WeaponSystem.AddRecoil(_recoilRate);
        GunMag.Shoot();
        StartCoroutine(ShootEffect());

        // Send hit scan in direction
        if (Physics.Raycast(bloom.CreateRayWithBloom(), out RaycastHit hit, Mathf.Infinity,
                Const.layerMaskExceptPlayer))
        {
            ShotHit(hit);

            GameObject decalInGame = Instantiate(Refs.Inst.DecalPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            decalInGame.transform.parent = hit.collider.transform;

            Shootable shootable = hit.collider.gameObject.GetComponent<Shootable>();

            if (!shootable) return;

            Transform forward = cam.transform;
            float distance = Vector3.Distance(forward.position, hit.point);
            int damageDropOff = (int)((Mathf.Ceil(distance) - sObj.damage) * (SObj.damage * 0.025f));
            damageDropOff = Mathf.Clamp(damageDropOff, 0, (int)Mathf.Ceil(SObj.damage * 0.5f));
            shootable.Shot(sObj.damage - damageDropOff);
        }
        else
        {
            // Nothing hit, send a straight line
            OnShotNoTarget();
        }
    }

    public void TryReload()
    {
        if (Reloading || !GunMag.CanReload())
        {
            return;
        }

        Reloading = true;
        StartReloadEffect();
    }

    private void HandleShooting()
    {
        // Stop shooting when switching weapons / view
        if (WeaponSystem.SwitchingWeapons || (WeaponSystem.IsPlayer && RTSManager.RTSMode()))
        {
            Shooting = false;
            ShootingTime = 0;
        }
    }

    private void HandleWeaponNoAmo()
    {
        if (WeaponSystem.SwitchingWeapons) return;

        // Problem: Get Component Every Frame
        int primaryAmo =
            WeaponSystem.Primary.GetComponent<GunScript>().GunMag.TotalAmo;
        int secondaryAmo =
            WeaponSystem.Secondary.GetComponent<GunScript>().GunMag.TotalAmo;

        // Auto reload when running out of mag amo
        if (GunMag.CurrentMagAmo <= 0 && !(GunMag.TotalAmo <= 0))
        {
            TryReload();
        }

        if (!WeaponSystem.InputShoot) return;

        //Auto switch weapons when out of total amo
        if (GunMag.CurrentMagAmo <= 0 && GunMag.TotalAmo <= 0)
        {
            bool holdingPrimary = WeaponSystem.ID == 0;

            if (holdingPrimary)
            {
                if (secondaryAmo > 0)
                {
                    WeaponSystem.SwitchWeapon(1);
                }
            }
            else
            {
                if (primaryAmo > 0)
                {
                    WeaponSystem.SwitchWeapon(0);
                }
            }
        }

        bool noAmo = primaryAmo <= 0 && secondaryAmo <= 0;
        bool tryingToShoot = WeaponSystem.IsPlayer && WeaponSystem.InputShootJustPressed;

        if (noAmo && tryingToShoot)
        {
            WeaponSystem.PlaySound(WeaponSystem.AudioSourceID.NoAmmoSoundClick);
        }
    }

    private void ApplyGunSettings()
    {
        FireRate = _fireRates[(int)SObj.fireRate];
        AimSpeed = _aimSpeeds[(int)SObj.aimSpeed];
        SwapSpeed = _swapSpeeds[(int)SObj.swapSpeed];
        _recoilRate.x = _recoilRateXs[(int)SObj.recoilRateX];
        _recoilRate.y = _recoilRateYs[(int)SObj.recoilRateY];
    }

    private IEnumerator ShootEffect()
    {
        WeaponSystem.PlaySound(WeaponSystem.AudioSourceID.ShootSound, gunShotSound.clip, _basePitch + Random.Range(-0.01f, 0.01f));

        // Setting the camera FOV back will make an effect,
        // if will automatically be damped back
        Refs.Inst.Player.Cam.fieldOfView += 0.75f;

        Vector3 armLocalPos = WeaponSystem.transform.localPosition;
        WeaponSystem.transform.localPosition = Aiming
            ? new Vector3(armLocalPos.x + Random.Range(-sObj.xWeaponKick, sObj.xWeaponKick), armLocalPos.y,
                armLocalPos.z + sObj.zWeaponKickAim)
            : new Vector3(armLocalPos.x + Random.Range(-sObj.xWeaponKick, sObj.xWeaponKick), armLocalPos.y,
                armLocalPos.z + sObj.zWeaponKick);

        _laserLine.enabled = true;
        yield return _laserLineDuration;
        // ReSharper disable once Unity.InefficientPropertyAccess
        _laserLine.enabled = false;
    }
}