using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDetection : MonoBehaviour
{
    public BasicEnemyController basicEnemy;
    public PlayerController player;
    public Transform Player;
    public bool isAggro = false;
    public bool playerNear = false;
    public bool swarmingMode = false;
    public float searchTime = 5;
    public float detectRadius = 20;
    public float turnSpeed = 5;
    public float reactionTime = 0.75f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        basicEnemy = GameObject.Find("BasicEnemy").GetComponent<BasicEnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = player.transform.position - transform.position;
        Vector3 playerDirection = Vector3.RotateTowards(transform.forward, playerPosition, turnSpeed, 0);
        Debug.DrawRay(transform.position, playerDirection, Color.yellow);
        var distance = Vector3.Distance(transform.position, player.transform.position);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDirection, out hit, Mathf.Infinity))
        {
            if (hit.transform == Player)
            {
                basicEnemy.agent.destination = player.transform.position;
                isAggro = true;
                Debug.Log("detected in line of sight");
                Debug.DrawRay(transform.position, playerDirection, Color.yellow);
            }
            else
            {
                isAggro = false;
            }
        }

        if (distance < detectRadius)
        {
            playerNear = true;
            Debug.Log("PlayerNear");
        }
        else
        {
            playerNear = false;
        }

        if (playerNear)
        {
            transform.rotation = Quaternion.LookRotation(playerDirection);
        }

        if(swarmingMode)
        {
            detectRadius = 99999;
        }
    }
    IEnumerator searchingWindow()
    {
        yield return new WaitForSeconds(searchTime);
        basicEnemy.agent.ResetPath();
    }
}
