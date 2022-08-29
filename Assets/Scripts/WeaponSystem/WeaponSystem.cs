/*
 * 2022 WraithWinterly
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public enum AudioSourceID
    {
        NoAmmoSoundClick = 0,
        ShootSound = 1,
        ReloadSoundStart = 2,
        ReloadSoundFinish = 3,
    }

    public IController Controller { get; private set; }
    public WeaponSObj SObj { get; private set; }

    public GameObject Primary { get; private set; }

    public GameObject Secondary { get; private set; }

    // private Knife
    // private Tac
    // private Lethal
    public GunScript Gun { get; private set; }

    public int ID { get; private set; } = 0;

    public bool SwitchingWeapons { get; private set; } = false;
    public bool PlayerMouseUpRequired { get; set; }

    public bool InputAim { get; set; }
    public bool InputShoot { get; set; }
    public bool InputShootJustPressed { get; set; }
    public bool InputSprintJustPressed { get; set; }
    public bool InputReload { get; set; }

    public bool SwitchingWeaponsPart1 { get; private set; }
    public bool SwitchingWeaponsPart2 { get; private set; }

    public Vector3 SwapPos => _swapPos;
    public Vector3 ReloadPos => SObj.reloadPos;
    public Vector3 AimPos => SObj.aimPos;
    public Camera Cam => cam;
    public bool IsPlayer => isPlayer;
    public bool HoldingGun => ID is 0 or 1;

    private const float SwapCheckOffset = 0.05f;
    private const float ShootBufferTime = 0.05f;
    private readonly Vector3 _swapPos = new(0.35f, -0.8f, -0.25f);

    private AudioSource[] _audioSources;

    private WeaponBloom _bloom;
    private WeaponRecoil _recoil;
    private WeaponTransform _weaponTransform;

    private GameObject _knife;
    private GameObject _tactical;
    private GameObject _lethal;

    private float _shootBuffer;
    private int _idToSwitchTo;

    private bool _throwingThrowable;
    private bool _usingKnife;

    [SerializeField] private GameObject objectWithIController;
    [SerializeField] private Camera cam;

    [Header("Prefabs")] [SerializeField] private GameObject primaryWeaponPrefab;
    [SerializeField] private GameObject secondaryWeaponPrefab;
    [SerializeField] private GameObject knifePrefab;
    [SerializeField] private GameObject tacticalPrefab;
    [SerializeField] private GameObject lethalPrefab;

    [Header("Holders")] [SerializeField] private GameObject primaryHolder;
    [SerializeField] private GameObject secondaryHolder;
    [SerializeField] private GameObject knifeHolder;
    [SerializeField] private GameObject tacticalHolder;
    [SerializeField] private GameObject lethalHolder;
    [SerializeField] private bool isPlayer = false;

    private void Awake()
    {
        _audioSources = GetComponents<AudioSource>();

        Controller = objectWithIController.GetComponent<IController>();
        _bloom = GetComponent<WeaponBloom>();
        _recoil = GetComponent<WeaponRecoil>();
        _weaponTransform = GetComponent<WeaponTransform>();
    }

    private void Start()
    {
        LoadPrimary(primaryWeaponPrefab);
        LoadSecondary(secondaryWeaponPrefab);
        //LoadKnife(knifePrefab);
        //LoadTactical(tacticalPrefab);
        //LoadLethal(lethalPrefab);

        // You can NOT start a CoRoutine in Awake!
        StartCoroutine(SetCurrentWeapon(0));
        OnEnable();
    }

    private void OnEnable()
    {
        Refs.Inst.EventManager?.modeSwitchedFPS.AddListener(OnFPSMode);
        Controller = objectWithIController.GetComponent<IController>();
    }

    private void Update()
    {
        HandleWeaponSwitching();

        if (isPlayer)
        {
            if (Input.GetButtonUp(Const.shoot))
            {
                PlayerMouseUpRequired = false;
            }

        }
    }

    #region InputRecievers

    public void InputSwitchId(int id)
    {
        SwitchWeapon(id);
    }

    public void CycleWeaponUp()
    {
        int id = ID;
        id--;

        if (SettingsManager.IncludeKnifeInCycle)
        {
            if (id <= -1)
            {
                id = 2;
            }
        }
        else
        {
            id = Mathf.Abs(id);
        }

        SwitchWeapon(id);
    }

    public void CycleWeaponDown()
    {
        int id = ID;
        id++;
        if (SettingsManager.IncludeKnifeInCycle)
        {
            if (id >= 3)
            {
                id = 0;
            }
        }
        else if (id >= 2)
        {
            id = 0;
        }

        SwitchWeapon(id);
    }

    #endregion

    #region PublicGetters

    public int GetCurrentMagAmmo()
    {
        return HoldingGun ? Gun.GunMag.CurrentMagAmo : 0;
    }

    public int GetMagSize()
    {
        return HoldingGun ? Gun.GunMag.MagSize : 0;
    }

    public int GetTotalAmmo()
    {
        return HoldingGun ? Gun.GunMag.TotalAmo : 0;
    }

    public float GetFireRate()
    {
        if (HoldingGun)
        {
            return Gun.FireRate;
        }

        return 0;
    }

    public float GetShootingTime()
    {
        if (HoldingGun)
        {
            //print(Gun.ShootingTime);
            return Gun.ShootingTime;
        }

        return 0;
    }

    public float GetAimSpeed()
    {
        if (HoldingGun)
        {
            return Gun.AimSpeed;
        }

        return 0;
    }

    public bool IsAiming()
    {
        return HoldingGun && Gun.Aiming;
    }

    public bool IsShooting()
    {
        return HoldingGun && Gun.Shooting;
    }

    public bool IsFiring()
    {
        return HoldingGun && Gun.Shooting && IsNextFireAllowed();
    }

    public bool IsReloading()
    {
        return HoldingGun && Gun.Reloading;
    }

    public bool IsNextFireAllowed()
    {
        return HoldingGun && Gun.TimeNextFireAllowed > Time.time;
    }

    public Vector2 GetCurrentRecoil()
    {
        return _recoil.CurrentRecoil;
    }

    public float GetCurrentBloom()
    {
        return _bloom.CurrentBloom;
    }

    #endregion

    public void AddRecoil(Vector2 recoil)
    {
        _recoil.AddRecoil(recoil);
    }

    private void OnFPSMode()
    {
        // If holding left click while switching back to FPS, don't shoot
        if (isPlayer && Input.GetButtonDown(Const.shoot))
        {
            PlayerMouseUpRequired = true;
        }
    }

    private void HandleWeaponSwitching()
    {
        if (!SwitchingWeapons) return;

        bool switchingStarted = SwitchingWeaponsPart1 || SwitchingWeaponsPart2;

        if (!switchingStarted)
        {
            SwitchingWeaponsPart1 = true;
        }

        bool switchDirUp = SwapPos.y > SObj.defaultPos.y;

        bool changeWeaponOffScreen = switchDirUp
            ? transform.localPosition.y >= _swapPos.y - SwapCheckOffset
            : transform.localPosition.y <= _swapPos.y + SwapCheckOffset;

        if (SwitchingWeaponsPart1 && changeWeaponOffScreen)
        {
            // Actually change the weapon once off screen
            StartCoroutine(SetCurrentWeapon(_idToSwitchTo));
            SwitchingWeaponsPart1 = false;
            SwitchingWeaponsPart2 = true;
        }
        else if (SwitchingWeaponsPart2)
        {
            bool inFinishedPosition = switchDirUp
                ? transform.localPosition.y - SwapCheckOffset <= SObj.defaultPos.y
                : transform.localPosition.y + SwapCheckOffset >= SObj.defaultPos.y;

            if (inFinishedPosition)
            {
                SwitchingWeapons = false;
                SwitchingWeaponsPart2 = false;
                PlayerMouseUpRequired = false;
            }
        }

#if UNITY_EDITOR
        bool debugTesting = !_weaponTransform.enabled;

        if (!debugTesting) return;

        StartCoroutine(SetCurrentWeapon(_idToSwitchTo));
        SwitchingWeapons = false;
        SwitchingWeaponsPart1 = false;
        SwitchingWeaponsPart2 = false;
        PlayerMouseUpRequired = false;
#endif
    }

    public void SwitchWeapon(int id)
    {
        if (id == ID) return;
        _idToSwitchTo = id;
        Gun.CancelReload();
        SwitchingWeapons = true;
    }

    public void PlaySound(AudioSourceID id, AudioClip clip = null, float pitch = 1, float volume = 1)
    {
        // Weapon click sound fix
        if (id == 0 && _audioSources[0].isPlaying) return;

        AudioSource audioSource = _audioSources[(int)id];
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        if (clip != audioSource.clip && clip != null)
        {
            audioSource.clip = clip;
        }

        audioSource.Play();
    }

    public void StopSound(AudioSourceID id)
    {
        _audioSources[(int)id].Stop();
    }

    private IEnumerator SetCurrentWeapon(int id)
    {
        _recoil.ResetRecoil();
        ID = id;

        switch (id)
        {
            case 0:
                primaryHolder.SetActive(true);
                Gun = Primary.GetComponent<GunScript>();
                SObj = Gun.SObj;
                break;
            case 1:
                secondaryHolder.SetActive(true);
                Gun = Secondary.GetComponent<GunScript>();
                SObj = Gun.SObj;
                break;
            case 2:
                knifeHolder.SetActive(true);
                break;
            case 3:
                tacticalHolder.SetActive(true);
                break;
            case 4:
                lethalHolder.SetActive(true);
                break;
        }

        // Weapons must have a chance to initialize before disabling
        yield return new WaitForEndOfFrame();
        DisableAllWeaponsExcept(id);
    }

    private void DisableAllWeaponsExcept(int id)
    {
        if (id != 0) primaryHolder.SetActive(false);
        if (id != 1) secondaryHolder.SetActive(false);
        if (id != 2) knifeHolder.SetActive(false);
        if (id != 3) tacticalHolder.SetActive(false);
        if (id != 4) lethalHolder.SetActive(false);
    }

    private void SetGunScript(GunScript gunScript)
    {
        gunScript.WeaponSystem = this;
        gunScript.bloom = _bloom;
        gunScript.cam = cam;
    }

    #region LoadWeapons

    private void LoadPrimary(GameObject gunPrefab)
    {
        Utils.DestroyAllChildren(primaryHolder);
        Instantiate(gunPrefab, primaryHolder.transform);
        Primary = primaryHolder.gameObject.transform.GetChild(0).transform.GetChild(0).transform.gameObject;
        SetGunScript(Primary.GetComponent<GunScript>());
    }

    private void LoadSecondary(GameObject gunPrefab)
    {
        Utils.DestroyAllChildren(secondaryHolder);
        Instantiate(gunPrefab, secondaryHolder.transform);
        Secondary = secondaryHolder.gameObject.transform.GetChild(0).transform.GetChild(0).transform.gameObject;
        SetGunScript(Secondary.GetComponent<GunScript>());
    }

    private void LoadKnife(GameObject knife)
    {
    }

    private void LoadTactical(GameObject tactical)
    {
    }

    private void LoadLethal(GameObject lethal)
    {
    }

    #endregion
}