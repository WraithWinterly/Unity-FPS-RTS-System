/*
 * 2022 WraithWinterly
 *
 * THIS FILE SHOULD ONLY HOLD REFERENCES
 * TO OBJECTS THAT WILL NOT CHANGE HIERARCHY LOCATIONS
 * Do not overuse this class, even if it is convenient
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Refs : MonoBehaviour
{
    private static Refs _inst;
    public static Refs Inst {
        get
        {
            if (_inst == null)
            {
                _inst = FindObjectOfType<Refs>();
            }
            return _inst;
        }
    }

    public Player Player { get; private set; }

    public RTSManager RTSManager { get; private set; }

    public EventManager EventManager { get; private set; }

    public UICanvas UICanvas { get; private set; }
    public GameObject DecalPrefab => decalPrefab;

    [Header("Prefabs")]
    [SerializeField] private GameObject decalPrefab;

    private void Awake()
    {
        // Assign References
        Player = GameObject.Find("Player").GetComponent<Player>();
        RTSManager = GameObject.Find("RTSManager").GetComponent<RTSManager>();
        UICanvas = GameObject.Find("UICanvas").GetComponent<UICanvas>();
        EventManager = GetComponent<EventManager>();
    }
}
