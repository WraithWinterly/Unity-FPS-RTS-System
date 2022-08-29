/*
 * 2022 WraithWinterly
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    public UICrosshair Crosshair { get; private set; }

    private UIRedScreenText _redScreenText;
    private UIHitMarker _hitMarker;
    private UIAmo _amo;


    private void Awake()
    {
        Crosshair = GameObject.Find("Crosshair").GetComponent<UICrosshair>();
        _redScreenText = GameObject.Find("RedScreenText").GetComponent<UIRedScreenText>();
        _hitMarker = GameObject.Find("HitMarker").GetComponent<UIHitMarker>();
        _amo = GameObject.Find("HitMarker").GetComponent<UIAmo>();
    }


    private void Update()
    {

    }
}
