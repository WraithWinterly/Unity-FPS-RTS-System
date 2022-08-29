/*
 * 2022 WraithWinterly
 */

using UnityEngine;

public class Player : MonoBehaviour
{
    public WeaponSystem WeaponSystem { get; private set; }
    public PlayerCamera CamScript { get; private set; }
    public Camera Cam => CamScript.Cam;

    private void Awake()
    {
        WeaponSystem = GameObject.Find("WeaponSystem").GetComponent<WeaponSystem>();
        CamScript = GameObject.Find("Camera").GetComponent<PlayerCamera>();
    }
}