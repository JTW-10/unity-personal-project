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
    public Rigidbody rb;

    [Header("Basic Enemy Settings")]
    public float health = 100;
    public float maxHealth = 100;
    public float damageGiven = 10;
    public float stunWindow = 1;
    public bool isStunned = false;

    [Header("Knockback Settings")]
    public bool placeholder = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            enemyDetection.swarmingMode = true;
        }


        if (health <= 0)
        {
            Destroy(gameObject);
        }

        if (enemyDetection.isAggro)
        {
            agent.destination = player.transform.position;
        }

        if (enemyDetection.swarmingMode)
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
            Vector3 knockbackDirection = other.transform.position - transform.position;
            knockbackDirection.y = 0;

            if (player.comboCounter > 2)
            {
                health -= player.meleeDamage;
                Debug.Log("final attack");
                enemyDetection.swarmingMode = true;
                player.knockbackForce = player.knockbackForce * 0.5f;
                rb.AddForce(knockbackDirection.normalized * player.knockbackForce, ForceMode.Impulse);
                StartCoroutine("knockbackReset");
            }
            else
            {
                health -= player.meleeDamage;
                Debug.Log("regular attack");
                enemyDetection.swarmingMode = true;
                rb.AddForce(knockbackDirection.normalized * player.knockbackForce, ForceMode.Impulse);
            }
        }
    }

    IEnumerator knockbackReset()
    {
        yield return new WaitForSeconds(0.25f);
        player.knockbackForce = player.knockbackForce / 0.5f;
    }
}

