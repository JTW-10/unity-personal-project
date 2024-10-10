using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyController : MonoBehaviour
{
    public PlayerController player;
    public EnemyDetection enemyDetection;
    public NavMeshAgent agent;

    [Header("Basic Enemy Settings")]
    public float health = 100;
    public float maxHealth = 100;
    public float damageGiven = 10;
    public float stunWindow = 1;
    public float knockback = 10;
    public bool isStunned = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    { 
        if (health <= 0)
        {
            Destroy(gameObject);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            enemyDetection.swarmingMode = true;
        }

        if (enemyDetection.isAggro)
        {
            agent.destination = player.transform.position;
        }

        if (enemyDetection.swarmingMode)
        {
            enemyDetection.detectRadius = 9999;
            enemyDetection.isAggro = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Shot")
        {
            Destroy(other.gameObject);
            health -= player.weaponDamage;
            Debug.Log("enemy has taken damage");
            enemyDetection.swarmingMode = true;
        }
    }
}

