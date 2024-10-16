using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public EnemyDetection enemyDetection;
    public Transform player;
    public NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()  // for some reason using this code in this script rather than the enemy script makes them stalk you?
    {

        if(enemyDetection.isAggro)
        {
            agent.destination = player.transform.position;
        }
        else
        {
            agent.destination = transform.position;
        }

        if (enemyDetection.swarmingMode)
        {
            enemyDetection.detectRadius = 9999;
            enemyDetection.isAggro = true;
        }
    }
}
