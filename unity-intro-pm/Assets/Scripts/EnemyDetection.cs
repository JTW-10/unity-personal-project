using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDetection : MonoBehaviour
{
    public BasicEnemyController basicEnemy;
    public PlayerController player;
    public Transform Player;
    public bool isAggro = false;
    public float searchTime = 5;
    public float detectRadius = 5;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        basicEnemy = GameObject.Find("BasicEnemy").GetComponent<BasicEnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        var distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < detectRadius)
        {
            isAggro = true;
            Debug.Log("playerfound");
        }
        else
        {
            isAggro = false;
        }

        if (isAggro)
        {
            basicEnemy.agent.isStopped = false;
            basicEnemy.agent.destination = player.transform.position;
        }
        if (!isAggro)
        {
            StartCoroutine("enemySearching");
        }
    }

    IEnumerator enemySearching()
    {
        yield return new WaitForSeconds(searchTime);
        basicEnemy.agent.isStopped = true;
    }
}
