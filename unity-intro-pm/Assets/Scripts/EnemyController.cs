using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    Rigidbody myRB;
    public PlayerController playerScript;
    public Transform Player;

    [Header ("Basic Enemy Settings")]
    public float health = 100;
    public float maxHealth = 100;
    public float stunWindow = 1;
    public float knockback = 10;
    public bool isStunned = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Shot")
        {
            Vector3 awayFromPlayer = Player.transform.position - transform.position;

            Destroy(other.gameObject);
            health -= 10;
            myRB.AddForce(awayFromPlayer * knockback, ForceMode.Impulse); // Isn't working, find some way to fix it
            Debug.Log("enemy has taken damage");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && playerScript.isParrying == true)
        {
            Vector3 awayFromPlayer = Player.transform.position - transform.position;

            isStunned = true;
            StartCoroutine("stunEnd");
            myRB.AddForce(awayFromPlayer * knockback, ForceMode.Impulse);
        }
    }
    IEnumerator stunEnd()
    {
        yield return new WaitForSeconds(stunWindow);
        isStunned = false;
    }
}