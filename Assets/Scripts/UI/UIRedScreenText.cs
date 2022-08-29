/*
 * 2022 WraithWinterly
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRedScreenText : MonoBehaviour
{
    private WeaponSystem WeaponSystem => Refs.Inst.Player.WeaponSystem;

    private Text _text;

    private void Awake()
    {
        _text = GetComponent<Text>();
    }

    private void Update()
    {
        if (WeaponSystem.HoldingGun)
        {
            HandleAmoWarning();
        }
        else
        {
            _text.color = Utils.DampColorAlpha(_text.color, 0, Const.smoothingFast, Time.deltaTime);
        }
    }

    private void HandleAmoWarning()
    {
        bool outOfAmo = WeaponSystem.GetCurrentMagAmmo() <= 0 && WeaponSystem.GetTotalAmmo() <= 0;
        bool canShow = !Refs.Inst.Player.WeaponSystem.IsAiming() && !RTSManager.RTSMode();

        if (outOfAmo && canShow)
        {
            _text.color = Utils.DampColorAlpha(_text.color, 1, Const.smoothingFast, Time.deltaTime);
            _text.text = "No Amo";
            _text.color = new Color(Color.red.r, Color.red.g, Color.red.g, _text.color.a);
        }
        else
        {
            _text.color = Utils.DampColorAlpha(_text.color, 0, Const.smoothingFast, Time.deltaTime);
        }
    }
}
