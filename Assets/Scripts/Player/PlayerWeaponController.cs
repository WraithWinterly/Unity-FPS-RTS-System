/*
 * 2022 WraithWinterly
 * Follow raywenderlich's Style Guide!
 * https://github.com/raywenderlich/c-sharp-style-guide
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player _player;

    private bool _toggleADS = false;

    private void Start()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        if (RTSManager.RTSMode())
        {
            ResetInput();
            return;
        }

        UpdateToggleADS();

        _player.WeaponSystem.InputShoot = Input.GetButton(Const.shoot);
        _player.WeaponSystem.InputShootJustPressed = Input.GetButtonDown(Const.shoot);
        _player.WeaponSystem.InputSprintJustPressed = Input.GetButtonDown(Const.sprint);
        _player.WeaponSystem.InputAim = SettingsManager.ToggleADS ? _toggleADS : Input.GetButton(Const.aim);

        _player.WeaponSystem.InputReload = Input.GetButtonDown(Const.reload);

        if (Input.GetAxisRaw(Const.mouseScroll) > 0)
        {
            _player.WeaponSystem.CycleWeaponUp();
        }
        else if (Input.GetAxisRaw(Const.mouseScroll) < 0)
        {
            _player.WeaponSystem.CycleWeaponDown();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _player.WeaponSystem.InputSwitchId(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _player.WeaponSystem.InputSwitchId(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _player.WeaponSystem.InputSwitchId(2);
        }
    }

    private void ResetInput()
    {
        _player.WeaponSystem.InputShoot = false;
        _player.WeaponSystem.InputAim = false;
        _player.WeaponSystem.InputReload = false;
        _toggleADS = false;
    }

    private void UpdateToggleADS()
    {
        if (Input.GetButtonDown(Const.aim))
        {
            _toggleADS = !_toggleADS;
        }
    }
}
