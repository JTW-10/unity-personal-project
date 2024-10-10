using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDetection : MonoBehaviour
{
    public PlayerController player;
    public Transform Player;
    public bool isAggro = false;
    public bool playerNear = false;
    public bool swarmingMode = false;
    public float detectRadius = 5;
    public float turnSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = player.transform.position - transform.position;
        Vector3 playerDirection = Vector3.RotateTowards(transform.forward, playerPosition, turnSpeed, 0);
        Debug.DrawRay(transform.position, playerDirection, Color.yellow);
        var distance = Vector3.Distance(transform.position, player.transform.position);

        // if raycast hits the player, turn aggro on
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            if (hit.transform == Player)
            {
                isAggro = true;
                Debug.Log("detected in line of sight");
                Debug.DrawRay(transform.position, transform.forward, Color.yellow);
            }
            else
            {
                isAggro = false;
            }
        }
                                                                              
        // checks if player is near using distance check
        if (distance < detectRadius)
        {
            playerNear = true;
            Debug.Log("PlayerNear");
        }
        else
        {
            playerNear = false;
        }

        // if player is near, rotate enemy towards player
        if (playerNear)
        {
            transform.rotation = Quaternion.LookRotation(playerDirection);
        }
    }
}
