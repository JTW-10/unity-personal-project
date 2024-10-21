using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class TrainingDummyController : MonoBehaviour
{
    Rigidbody myRB;
    public PlayerController player;
    

    [Header ("Basic Enemy Settings")]
    public float health = 100;
    public float maxHealth = 100;
    public float stunWindow = 1;
    public float knockback = 10;
    public bool isStunned = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
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
        Vector3 knockbackDirection = transform.position - player.transform.position.normalized;

        if (other.gameObject.tag == "Shot")
        {
            Destroy(other.gameObject);
            health -= player.weaponDamage;
            Debug.Log("enemy has taken blaster damage");
        }

        if (other.gameObject.tag == "Swing")
        {
            if (player.comboCounter > 2)
            {
                Debug.Log("final attack");
            }
            else
            { 
                Debug.Log("regular attack");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // placeholder, probably want to put something for parrying here or add parry knockback in the main playercontroller script
    }
    IEnumerator stunEnd()
    {
        yield return new WaitForSeconds(stunWindow);
        isStunned = false;
    }
}