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

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        basicEnemy = GameObject.Find("BasicEnemy").GetComponent<BasicEnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.forward, out hit, Mathf.Infinity))
        {
            if (hit.transform == Player)
            {
                basicEnemy.agent.destination = player.transform.position;
                Debug.Log("detected in line of sight");
            }
            else
            {
                Debug.Log("not detected in line of sight");
            }
        }
        if (basicEnemy.isStunned)
        {
            basicEnemy.agent.isStopped = true;
            Debug.Log("enemy is stunned and should not be moving");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            transform.LookAt(Player);
            Debug.Log("player in radius");
        }
    }
}
