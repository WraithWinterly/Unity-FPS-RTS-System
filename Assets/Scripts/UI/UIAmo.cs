/*
 * 2022 WraithWinterly
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAmo : MonoBehaviour
{
    [SerializeField] private Text amoUIText;

    private WeaponSystem WeaponSystem => Refs.Inst.Player.WeaponSystem;

    private void Update()
    {
        if (WeaponSystem.HoldingGun)
        {
            ShowWeaponAmo();
        }
        else
        {
            amoUIText.text = "<color=white>-- / --</color>";
        }
    }

    private void ShowWeaponAmo()
    {
        string amoText;

        int currentAmo = WeaponSystem.GetCurrentMagAmmo();
        int magSize = WeaponSystem.GetMagSize();
        int totalAmo = WeaponSystem.GetTotalAmmo();

        if (currentAmo <= 0)
        {
            amoText = $"<color=red>{currentAmo}</color>";
        }
        else if (currentAmo == magSize)
        {
            amoText = $"<color=lime>{currentAmo}</color>";
        }
        else
        {
            amoText = $"<color=white>{currentAmo}</color>";
        }

        var totalAmoText = totalAmo <= 0 ? $"<color=red>{totalAmo}</color>" : $"<color=white>{totalAmo}</color>";

        amoUIText.text = $"{amoText} / {totalAmoText}";
    }
}
