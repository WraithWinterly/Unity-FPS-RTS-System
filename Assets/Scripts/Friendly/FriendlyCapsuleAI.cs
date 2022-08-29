/*
 * 2022 WraithWinterly
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FriendlyCapsuleAI : MonoBehaviour
{
    private Shootable _shootable;

    private int _health = 100;

    [SerializeField] private NavMeshAgent agent;

    private void Start()
    {
        _shootable = GetComponent<Shootable>();
        agent = GetComponent<NavMeshAgent>();
        OnEnable();
    }

    private void OnEnable()
    {
        _shootable?.shotEvent.AddListener(Shot);
    }

    private void Shot(int damage)
    {
        _shootable.friendlyFire = true;
        _health -= damage;

        if (_health <= 0)
        {
            _shootable.deathShot = true;
            Destroy(gameObject);
        }
    }
}
