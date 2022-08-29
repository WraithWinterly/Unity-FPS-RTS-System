/*
 * 2022 WraithWinterly
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMag : MonoBehaviour
{
    public int CurrentMagAmo { get; private set; }
    public int MagSize { get; private set; }
    public int TotalAmo { get; private set; }

    private GunScript _gunScript;

    public bool disabled = false;

    public void Start()
    {
        _gunScript = GetComponent<GunScript>();
        MagSize = (int)_gunScript.SObj.magSize;
        CurrentMagAmo = MagSize;
        TotalAmo = _gunScript.SObj.magCount * MagSize;
    }

    public void FinishReload()
    {
        if (TotalAmo <= 0) return;
        if (CurrentMagAmo >= MagSize) return;

        // Don't reload extra bullet if whole mag was drained
        int newAmoAmount;

        if (CurrentMagAmo == 0)
        {
            newAmoAmount = (MagSize - 1) - CurrentMagAmo;

        }
        else
        {
            newAmoAmount = (MagSize) - CurrentMagAmo;
        }

        TotalAmo -= newAmoAmount;

        // If total amo is less than zero, e.g. you are adding less than the mag size
        if (TotalAmo < 0)
        {
            newAmoAmount += TotalAmo;
            TotalAmo = 0;
        }
        CurrentMagAmo += newAmoAmount;
    }

    public bool CanReload()
    {
        return TotalAmo > 0 && CurrentMagAmo < MagSize;
    }

    public void Shoot()
    {
        CurrentMagAmo -= 1;
        CurrentMagAmo = Mathf.Clamp(CurrentMagAmo, 0, MagSize);
    }

}
