using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestDummy : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private PlayerMovement player;

    void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerMovement>();
    }

    void FixedUpdate()
    {
        navAgent.SetDestination(player.transform.position);
    }
}
