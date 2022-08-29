/*
 * 2022 WraithWinterly
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shootable : MonoBehaviour
{
    [HideInInspector] public UnityEvent<int> shotEvent = new();

    public bool deathShot = false;
    public bool friendlyFire = false;

    public void Shot(int damage)
    {
        shotEvent.Invoke(damage);

        if (deathShot)
        {
            Refs.Inst.EventManager.playerKilledEnemy.Invoke();
            deathShot = false;
        }
        else if (friendlyFire)
        {
            Refs.Inst.EventManager.playerShotFriendly.Invoke();
            friendlyFire = false;
        }
        else
        {
            Refs.Inst.EventManager.playerShotEnemy.Invoke();
        }
    }
}
