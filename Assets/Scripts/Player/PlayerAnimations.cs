/*
 * 2022 WraithWinterly
 * Follow raywenderlich's Style Guide!
 * https://github.com/raywenderlich/c-sharp-style-guide
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{

    private MeshRenderer _meshRenderer;

    [SerializeField] private FPSController fpsController;

    public void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.enabled = false;
        OnEnable();
    }

    public void OnEnable()
    {
        Refs.Inst.EventManager?.modeSwitchedFPS.AddListener(OnFPSMode);
        Refs.Inst.EventManager?.modeSwitchedRTS.AddListener(OnRTSMode);
    }

    private void OnFPSMode()
    {
        _meshRenderer.enabled = false;
    }

    private void OnRTSMode()
    {
        _meshRenderer.enabled = true;
    }

    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
    private void Update()
    {
        Vector3 localScale = transform.localScale;
        Vector3 localTransform = transform.localPosition;

        if (fpsController.IsCrouching)
        {
            localScale = new Vector3(localScale.x, 0.75f, localScale.z);
            localTransform = new Vector3(localTransform.x, -0.6f, localTransform.z);
        }
        else
        {
            localScale = new Vector3(localScale.x, 1.3f, localScale.z);
            localTransform = new Vector3(localTransform.x, 0, localTransform.z);
        }
        transform.localScale = localScale;
        transform.localPosition = localTransform;
    }
}
