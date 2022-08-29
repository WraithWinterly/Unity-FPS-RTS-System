/*
 * 2022 WraithWinterly
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Const
{
    public const string mouseX = "Mouse X";
    public const string mouseY = "Mouse Y";
    public const string mouseScroll = "Mouse ScrollWheel";
    public const string jump = "Jump";
    public const string aim = "Aim";
    public const string shoot = "Shoot";
    public const string sprint = "Sprint";
    public const string crouch = "Crouch";
    public const string reload = "Reload";
    public const string switchView = "SwitchView";
    public const string horizontal = "Horizontal";
    public const string vertical = "Vertical";

    public const int leftClick = 0;
    public const int rightClick = 1;
    public const int middleClick = 2;

    public const float smoothingNormal = 0.01f;
    public const float smoothingFast = 0.0001f;

    public const int layerMaskPlayer = 1 << 6;
    public const int layerMaskExceptPlayer = ~(1 << 6);
    public const int layerMaskGround = 1 << 7;
    public const int layerMaskEnemies = 1 << 9;
    public const int layerMaskFriendly = 1 << 10;

    public const int layerPlayer = 6;
    public const int layerGround = 7;
    public const int layerEnemies = 9;
    public const int layerFriendly = 10;

    public static Vector2 CameraLimit = new(-85, 85);
}
