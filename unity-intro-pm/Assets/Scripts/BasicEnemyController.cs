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
    public Transform knockbackReference;
    public PlayerMovement playerMovement;
    public Rigidbody playerRB;
    public Rigidbody rb;

    [Header("Basic Enemy Settings")]
    public float health = 375;
    public float maxHealth = 100;
    public float damageGiven = 10;
    public float stunWindow = 1;
    public float groundDrag = 2;
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
        rb = GetComponent<Rigidbody>();
        playerRB = GameObject.Find("Player").GetComponent<Rigidbody>();
        knockbackReference = player.transform.Find("KnockbackReference");
        playerMovement = GetComponent<PlayerMovement>();
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

        if (isStunned)
        {
            agent.speed = 0f;
        }
        else
        {
            agent.speed = 10f;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Vector3 knockbackDirection = transform.position - knockbackReference.transform.position;
            rb.AddForce(knockbackDirection * player.knockbackForce);
            rb.drag = groundDrag;
            isStunned = true;
            StartCoroutine("HardStunReset");
            //playerRB.AddForce(-knockbackDirection * 100);
        }
    }

    void Die()
    {
        Destroy(gameObject);
        isAlive = false;
        player.enemiesKilled++;
    }

    void hardHit()
    {
        Vector3 knockbackDirection = transform.position - knockbackReference.transform.position;
        rb.AddForce(knockbackDirection * player.knockbackForce);
        rb.AddForce(Vector3.up * player.knockbackForce);
        rb.drag = groundDrag;
        player.stunDuration += 0.3f;
        health -= player.meleeDamage;
        Debug.Log("final attack");
        enemyDetection.swarmingMode = true;
        canHit = false;
        isStunned = true;
        StartCoroutine("HardStunReset");
        StartCoroutine("iframes");
    }

    void softHit()
    {
        Vector3 knockbackDirection = transform.position - knockbackReference.transform.position;
        player.knockbackForce -= 50;
        rb.AddForce(knockbackDirection * player.knockbackForce);
        rb.drag = groundDrag;
        health -= player.meleeDamage;
        Debug.Log("regular attack");
        enemyDetection.swarmingMode = true;
        canHit = false;
        isStunned = true;
        StartCoroutine("StunReset");
        StartCoroutine("iframes");
    }

    IEnumerator iframes()
    {
        yield return new WaitForSeconds(player.swingSpeed);
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
        player.stunDuration -= 0.3f;
        isStunned = false;
    }

    IEnumerator knockbackReset()
    {
        yield return new WaitForSeconds(player.swingSpeed);
        player.knockbackForce += 50;
    }
}

