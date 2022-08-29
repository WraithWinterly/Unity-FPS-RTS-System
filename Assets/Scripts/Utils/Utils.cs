/*
 * 2022 WraithWinterly
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static float Damp(float source, float target, float smoothing, float dt)
    {
        return Mathf.Lerp(source, target, 1 - Mathf.Pow((smoothing * 0.01f), dt));
    }

    public static Vector2 Damp(Vector2 source, Vector2 target, float smoothing, float dt)
    {
        source.x = Mathf.Lerp(source.x, target.x, 1 - Mathf.Pow((smoothing * 0.01f), dt));
        source.y = Mathf.Lerp(source.y, target.y, 1 - Mathf.Pow((smoothing * 0.01f), dt));
        return source;
    }

    public static Vector3 Damp(Vector3 source, Vector3 target, float smoothing, float dt)
    {
        source.x = Mathf.Lerp(source.x, target.x, 1 - Mathf.Pow((smoothing * 0.01f), dt));
        source.y = Mathf.Lerp(source.y, target.y, 1 - Mathf.Pow((smoothing * 0.01f), dt));
        source.z = Mathf.Lerp(source.z, target.z, 1 - Mathf.Pow((smoothing * 0.01f), dt));
        return source;
    }

    public static Quaternion Damp(Quaternion source, Quaternion target, float smoothing, float dt)
    {
        Vector3 sourceRot = source.eulerAngles;
        Vector3 targetRot = target.eulerAngles;
        sourceRot.x = Mathf.LerpAngle(sourceRot.x, targetRot.x, 1 - Mathf.Pow((smoothing * 0.01f), dt));
        sourceRot.y = Mathf.LerpAngle(sourceRot.y, targetRot.y, 1 - Mathf.Pow((smoothing * 0.01f), dt));
        sourceRot.z = Mathf.LerpAngle(sourceRot.z, targetRot.z, 1 - Mathf.Pow((smoothing * 0.01f), dt));
        Quaternion newQuat = Quaternion.Euler(sourceRot);
        return newQuat;
    }

    public static Color DampColorAlpha(Color color, float target, float smoothing, float dt)
    {
        float colorA = color.a;
        colorA = Damp(colorA, target, smoothing, dt);
        color = new Color(color.r, color.g, color.b, colorA);
        return color;
    }

    public static int ToLayer(LayerMask layer)
    {
        return (1 >> layer);
    }

    public static void DestroyAllChildren(GameObject gObj)
    {
        foreach (Transform transform in gObj.transform)
        {
            Debug.Log(transform);
            Object.Destroy(transform.gameObject);
        }
    }
}
