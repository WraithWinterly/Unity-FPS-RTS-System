/*
 * 2022 WraithWinterly
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICrosshair : MonoBehaviour
{
    private WeaponSystem WeaponSystem => Refs.Inst.Player.WeaponSystem;

    private const float RealOffset = 1.2f; // Offset to make crosshair accurate to bloom in world
    private readonly Vector2 _noGunSpread = new Vector2(25, 25);

    private Image[] _images;
    private RectTransform _rectTransform;
    private RectTransform _left;
    private RectTransform _right;
    private RectTransform _up;
    private RectTransform _down;
    private Image _debugBox;

    private float _currentPenalty;

    [Header("Crosshair")] [SerializeField] private float length = 15;
    [SerializeField] private float width = 1;
    [SerializeField] private float alpha = 1;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _left = GameObject.Find("Left").GetComponent<RectTransform>();
        _right = GameObject.Find("Right").GetComponent<RectTransform>();
        _up = GameObject.Find("Up").GetComponent<RectTransform>();
        _down = GameObject.Find("Down").GetComponent<RectTransform>();
        _debugBox = GetComponent<Image>();
    }

    private void Start()
    {
        _images = new Image[4];
        _images[0] = _left.GetComponent<Image>();
        _images[1] = _right.GetComponent<Image>();
        _images[2] = _up.GetComponent<Image>();
        _images[3] = _down.GetComponent<Image>();

#if !UNITY_EDITOR
        Destroy(_debugBox);
#endif
    }

    private void Update()
    {
#if UNITY_EDITOR
        _debugBox.enabled = UIDebugMenu.AlwaysShowCrosshair;
#endif
        if (WeaponSystem.HoldingGun)
        {
            HandleGunCrosshair();
        }
        CalculateWidthLength();
    }

    private void HandleGunCrosshair()
    {
        if (RTSManager.FPSMode())
        {
            if (Refs.Inst.Player.WeaponSystem.IsAiming())
            {
                FadeOut();
            }
            else
            {
                FadeIn();
            }
        }
        else
        {
            FadeOut();
        }
    }

    private void FadeIn()
    {
        foreach (Image image in _images)
        {
            image.color = Utils.DampColorAlpha(image.color, alpha, Const.smoothingNormal, Time.deltaTime);
        }
    }

    private void FadeOut()
    {
#if UNITY_EDITOR
        if (UIDebugMenu.AlwaysShowCrosshair) return;
#endif

        foreach (Image image in _images)
        {
            image.color = Utils.DampColorAlpha(image.color, 0, Const.smoothingFast, Time.deltaTime);
        }
    }

    private void CalculateWidthLength()
    {
        _left.sizeDelta = new Vector2(length, width);
        _right.sizeDelta = new Vector2(length, width);
        _up.sizeDelta = new Vector2(width, length);
        _down.sizeDelta = new Vector2(width, length);

        float bloom = Refs.Inst.Player.WeaponSystem.GetCurrentBloom();
        _rectTransform.sizeDelta = new Vector2(
            (bloom * RealOffset) + 20,
            (bloom * RealOffset) + 20);
    }
}