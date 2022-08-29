/*
 * 2022 WraithWinterly
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCaspuleAI : MonoBehaviour
{
    private Vector3 _walkPoint;
    private Shootable _shootable;

    private int _health = 100;

    private bool _isWalkPointSet;
    private bool _alreadyAttacked;
    private bool _playerInSightRange;
    private bool _playerInAttackRange;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsPlayer;

    [SerializeField] private float sightRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float walkPointRange;
    [SerializeField] private float timeBetweenAttacks;

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


    private void Update()
    {
        // Check for sight / attack range
        Vector3 position = transform.position;
        _playerInSightRange = Physics.CheckSphere(position, sightRange, whatIsPlayer);
        _playerInAttackRange = Physics.CheckSphere(position, attackRange, whatIsPlayer);

        switch (_playerInSightRange)
        {
            case false when !_playerInAttackRange:
                Patrolling();
                break;
            case true when !_playerInAttackRange:
                ChasePlayer();
                break;
            case true when _playerInAttackRange:
                AttackPlayer();
                break;
        }
    }

    private void Patrolling()
    {
        if (!_isWalkPointSet)
        {
            SearchWalkPoint();
        }
        else
        {
            agent.SetDestination(_walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - _walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            _isWalkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        Vector3 position = transform.position;

        _walkPoint = new Vector3(
            position.x + randomX,
            position.y,
            position.z + randomZ);

        if (Physics.Raycast(_walkPoint, -transform.up, 2f, whatIsGround))
        {
            _isWalkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(Refs.Inst.Player.transform.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(Refs.Inst.Player.transform);

        if (_alreadyAttacked) return;

        Invoke(nameof(ResetAttack), timeBetweenAttacks);
        _alreadyAttacked = true;
    }

    private void ResetAttack()
    {
        _alreadyAttacked = false;
    }

    private void Shot(int damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            _shootable.deathShot = true;
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 position = transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(position, sightRange);

    }
}
