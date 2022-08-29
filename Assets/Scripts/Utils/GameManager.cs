/*
 * 2022 WraithWinterly
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{

    [ClearOnReload] public static bool InMenu = false;

    [SerializeField] private int frameRateLimit = -1;

    private void Update()
    {
        if (frameRateLimit > 0)
        {
            Application.targetFrameRate = frameRateLimit;
        }
        else
        {
            Application.targetFrameRate = -1;
        }

        if (FullScreen())
        {
            Screen.fullScreen = !Screen.fullScreen;
        }

        if (ClickToCaptureCursor())
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private bool FullScreen() => Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Return);
    private bool ClickToCaptureCursor() => Input.GetMouseButtonDown(Const.leftClick)
                && Cursor.visible && RTSManager.FPSMode() && !GameManager.InMenu;


    public static void TryLockCursor()
    {
        if (RTSManager.FPSMode() && !InMenu)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public static void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
