/*
 * 2022 WraithWinterly
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDebugMenu : MonoBehaviour
{
    [ClearOnReload] public static bool AlwaysShowCrosshair;
    [ClearOnReload] public static bool ToggleADS;
    [ClearOnReload] public static bool NoRecoil;
    [ClearOnReload] public static bool NoBloom;

    [SerializeField] private UICrosshair crosshair;
    [SerializeField] private GameObject panel;

    private void Start()
    {
        panel.SetActive(false);

        AlwaysShowCrosshair = false;
        ToggleADS = false;
        NoRecoil = false;
        NoBloom = false;

#if !UNITY_EDITOR
        Destroy(gameObject);
#endif
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Home))
        {
            if (panel.activeInHierarchy)
            {
                panel.SetActive(false);
                GameManager.InMenu = false;
                GameManager.TryLockCursor();
            }
            else
            {
                panel.SetActive(true);
                GameManager.InMenu = true;
                GameManager.UnlockCursor();
            }
        }
    }

    public void AlwaysShowCrosshairButton()
    {
        AlwaysShowCrosshair = !AlwaysShowCrosshair;
        GameObject.Find("AlwaysShowCrosshairText").GetComponent<Text>().text =
            $"Always Show Crosshair: {AlwaysShowCrosshair}";
    }

    public void ToggleADSButton()
    {
        ToggleADS = !ToggleADS;
        SettingsManager.ToggleADS = !SettingsManager.ToggleADS;
        GameObject.Find("ToggleADSText").GetComponent<Text>().text = $"Toggle ADS: {ToggleADS}";
    }

    public void NoRecoilButton()
    {
        NoRecoil = !NoRecoil;
        GameObject.Find("NoRecoilText").GetComponent<Text>().text = $"No Recoil: {NoRecoil}";
    }

    public void NoBloomButton()
    {
        NoBloom = !NoBloom;
        GameObject.Find("NoBloomText").GetComponent<Text>().text = $"No Bloom: {NoBloom}";
    }
}