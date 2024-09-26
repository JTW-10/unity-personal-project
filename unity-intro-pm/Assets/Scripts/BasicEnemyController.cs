using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyController : MonoBehaviour
{
    public PlayerController player;
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
        //agent.destination = player.transform.position; // find way to check area for player.
                                                       // if found, shoot raycast at direction of player to check for obstacle blocking vision
                                                       // if raycast hits player, make the agent destination the player for a limited amount of time
                                                       // once time is up, repeat
                                                       // possibly make enemy roam and look around for player although I have no clue how to do this

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Shot")
        {
            Destroy(other.gameObject);
            health -= player.weaponDamage;
            Debug.Log("enemy has taken damage");
        }
    }
    private void detectionRadius(Vector3 center, float radius) // try to figure out how this actually works
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        foreach (var hitcollider in hitColliders)
        {
            hitcollider.SendMessage("AddDamage");
        }
    }
}

