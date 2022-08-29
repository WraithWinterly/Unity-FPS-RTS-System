/*
 * 2022 WraithWinterly
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// WraithWinterly
public class WeaponRecoil : MonoBehaviour
{
    public Vector2 QueuedRecoilAdd => _queuedRecoilAdd;
    public Vector2 QueuedRecoilSubtract => _queuedRecoilSubtract;
    public Vector2 CurrentRecoil => _currentRecoil;

    private const float LerpSpeed = 20f;

    private WeaponSystem _weaponSystem;

    private Vector2 _queuedRecoilAdd;
    private Vector2 _queuedRecoilSubtract;
    private Vector2 _currentRecoil;

    private void Start()
    {
        _weaponSystem = GetComponent<WeaponSystem>();
    }


    private void Update()
    {
#if UNITY_EDITOR
        if (UIDebugMenu.NoRecoil)
        {
            _queuedRecoilAdd = Vector2.zero;
            _queuedRecoilSubtract = Vector2.zero;
            _currentRecoil = Vector2.zero;
            return;
        }
#endif

        bool autoWeaponRecoilUp = !_weaponSystem.SObj.semiAuto && _queuedRecoilAdd.y >= 0.5f;
        bool semiWeaponRecoilUp = _weaponSystem.SObj.semiAuto && _queuedRecoilAdd.y >= 0.3125f;

        if (autoWeaponRecoilUp || semiWeaponRecoilUp)
        {
            RecoilUp();
        }
        else
        {
            RecoilRecovery();
        }
    }

    private void RecoilUp()
    {
        _currentRecoil.x = Mathf.Lerp(_currentRecoil.x, QueuedRecoilAdd.x, LerpSpeed) * Time.deltaTime * 10;
        _currentRecoil.y = Mathf.Lerp(_currentRecoil.y, QueuedRecoilAdd.y, LerpSpeed) * Time.deltaTime * 10;

        float xRotation = Mathf.Ceil(WrapEulerAngle(_weaponSystem.Cam.transform.localEulerAngles.x));

        if (xRotation > Const.CameraLimit.x)
        {
            _queuedRecoilSubtract.y += _currentRecoil.y;
        }

        _queuedRecoilSubtract.x += _currentRecoil.x;

        _queuedRecoilAdd.x -= _currentRecoil.x;
        _queuedRecoilAdd.y -= _currentRecoil.y;

        RecoilMouseCancellation();

        if (_queuedRecoilAdd.y < 0)
        {
            _queuedRecoilAdd.y = 0;
        }
        if (QueuedRecoilSubtract.y < 0)
        {
            _queuedRecoilSubtract.y = 0;
        }
    }

    private void RecoilRecovery()
    {
        _queuedRecoilAdd = Vector2.zero;
        _currentRecoil = Vector2.zero;
        _currentRecoil.x = -Mathf.Lerp(_currentRecoil.x, _queuedRecoilSubtract.x, LerpSpeed * 50) * Time.deltaTime *
                           5;
        _currentRecoil.y = -Mathf.Lerp(_currentRecoil.y, _queuedRecoilSubtract.y, LerpSpeed * 50) * Time.deltaTime *
                           5;

        _queuedRecoilSubtract.x -= -_currentRecoil.x;
        _queuedRecoilSubtract.y -= -_currentRecoil.y;

        if (QueuedRecoilSubtract.y < 0)
        {
            _queuedRecoilSubtract.y = 0;
        }
    }

    // Wraps a Euler Angle from 0, 360 to -90, 90
    private float WrapEulerAngle(float rot)
    {
        rot %= 360;

        if (rot > 180)
        {
            return rot - 360;
        }

        return rot;
    }

    private void RecoilMouseCancellation()
    {
        if (!_weaponSystem.IsPlayer) return;

        float sens = Refs.Inst.Player.CamScript.Sensitivity;

        if (Input.GetAxis(Const.mouseY) < 0)
        {
            _queuedRecoilSubtract.y -= Mathf.Abs(Input.GetAxis(Const.mouseY) * sens);
        }

        if (Input.GetAxis(Const.mouseX) < 0 && _currentRecoil.x > 0)
        {
            _queuedRecoilSubtract.x += Input.GetAxis(Const.mouseX) * sens;

            if (_queuedRecoilSubtract.x < 0)
            {
                _queuedRecoilSubtract.x = 0;
            }
        }
        else if (Input.GetAxis(Const.mouseX) > 0 && _currentRecoil.x < 0)
        {
            _queuedRecoilSubtract.x -= Input.GetAxis(Const.mouseX) * sens;

            if (_queuedRecoilSubtract.x < 0)
            {
                _queuedRecoilSubtract.x = 0;
            }
        }
    }

    public void ResetRecoil()
    {
        _queuedRecoilAdd = Vector2.zero;
    }


    public void AddRecoil(Vector2 recoil)
    {
        _queuedRecoilAdd.y += recoil.y;

        float bounds = recoil.x;

        _queuedRecoilAdd.x += recoil.x * Random.Range(bounds, -bounds);
        _queuedRecoilAdd.x = Mathf.Clamp(_queuedRecoilAdd.x, -bounds, bounds);
    }
}