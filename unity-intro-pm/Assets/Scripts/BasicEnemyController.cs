using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemyController : MonoBehaviour
{
    public PlayerController player;
    public EnemyDetection enemyDetection;
    public NavMeshAgent agent;
    public GameManager gm;

    [Header("Basic Enemy Settings")]
    public float health = 100;
    public float maxHealth = 100;
    public float damageGiven = 10;
    public float stunWindow = 1;
    public bool isStunned = false;
    public bool isAlive = true;
    public bool canHit = true;

    [Header("Knockback Settings")]
    public bool placeholder = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gm.isPaused)
        {
            if (gm.debugKeys)
            {
                if (Input.GetKeyDown(KeyCode.K))
                {
                    enemyDetection.swarmingMode = true;
                }
            }
        }

        if (health <= 0)
        {
            Die();
        }

        if (enemyDetection.isAggro && isAlive && !isStunned)
        {
            agent.destination = player.transform.position;
        }

        if (enemyDetection.swarmingMode && isAlive && !isStunned)
        {
            enemyDetection.detectRadius = 9999;
            enemyDetection.isAggro = true;
            agent.destination = player.transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Shot")
        {
            Destroy(other.gameObject);
            health -= player.weaponDamage;
            Debug.Log("enemy has taken blaster damage");
            enemyDetection.isAggro = true;

            if(enemyDetection.isAggro)
            {
                enemyDetection.swarmingMode = true;
            }
        }

        if (other.gameObject.tag == "Swing")
        {
            if (player.comboCounter > 2)
            {
                hardHit();
            }
            else
            {
                softHit();
            }
        }
    }

    void Die()
    {
        Destroy(gameObject);
        isAlive = false;
    }

    void hardHit()
    {
        Vector3 knockbackDirection = player.transform.position - transform.position;
        health -= player.meleeDamage;
        Debug.Log("final attack");
        enemyDetection.swarmingMode = true;
        canHit = false;
        isStunned = true;
        transform.position += Vector3.forward * 0.1f;
        StartCoroutine("HardStunReset");
        StartCoroutine("iframes");
    }

    void softHit()
    {
        Vector3 knockbackDirection = player.transform.position - transform.position;
        knockbackDirection.y = 0;
        health -= player.meleeDamage;
        Debug.Log("regular attack");
        enemyDetection.swarmingMode = true;
        canHit = false;
        isStunned = true;
        transform.position += player.transform.position * 0.1f;
        StartCoroutine("StunReset");
        StartCoroutine("iframes");
    }

    IEnumerator knockbackReset()
    {
        yield return new WaitForSeconds(0.25f);
        player.knockbackForce = player.knockbackForce / 0.5f;
    }

    IEnumerator iframes()
    {
        yield return new WaitForSeconds(0.1f);
        canHit = true;
    }

    IEnumerator StunReset()
    {
        yield return new WaitForSeconds(player.stunDuration);
        isStunned = false;
    }    

    IEnumerator HardStunReset()
    {
        yield return new WaitForSeconds(player.stunDuration);
        isStunned = false;
    }
}

