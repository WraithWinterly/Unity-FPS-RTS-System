/*
 * 2022 WraithWinterly
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSSelectedComponent : MonoBehaviour
{
    private void Start()
    {
        Outline outline = gameObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = Color.cyan;
        outline.OutlineWidth = 3;
    }

    private void OnDestroy()
    {
        Destroy(gameObject.GetComponent<Outline>());
    }
}
