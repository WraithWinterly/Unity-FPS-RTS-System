/*
 * 2022 WraithWinterly
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    [HideInInspector] public UnityEvent playerShotEnemy = new();
    [HideInInspector] public UnityEvent playerShotFriendly = new();
    [HideInInspector] public UnityEvent playerKilledEnemy = new();

    [HideInInspector] public UnityEvent modeSwitchedFPS = new();
    [HideInInspector] public UnityEvent modeSwitchedRTS = new();
}
