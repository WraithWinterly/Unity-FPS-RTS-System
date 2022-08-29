/*
 * 2022 WraithWinterly
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSManager : MonoBehaviour
{
    [ClearOnReload]
    private static Modes _mode = Modes.FPS;

    public Modes Mode => _mode;

    public Camera FPSCam => fpsCam;
    public Camera WeaponCam => weaponCam;
    public Camera RTSCam => rtsCam;

    public enum Modes
    {
        FPS,
        RTS,
    }

    [SerializeField] private Camera fpsCam;
    [SerializeField] private Camera weaponCam;
    [SerializeField] private Camera rtsCam;

    private void Update()
    {
        if (Input.GetButtonDown(Const.switchView))
        {
            switch (_mode)
            {
                case Modes.FPS:
                    SwitchModeRTS();
                    break;
                case Modes.RTS:
                    SwitchModeFPS();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void SwitchModeRTS()
    {
        _mode = Modes.RTS;
        Refs.Inst.EventManager.modeSwitchedRTS.Invoke();
    }

    private void SwitchModeFPS()
    {
        _mode = Modes.FPS;
        Refs.Inst.EventManager.modeSwitchedFPS.Invoke();
    }

    public static bool FPSMode()
    {
        return _mode == Modes.FPS;
    }

    public static bool RTSMode()
    {
        return _mode == Modes.RTS;
    }
}
