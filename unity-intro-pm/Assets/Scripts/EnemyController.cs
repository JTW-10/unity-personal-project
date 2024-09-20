using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float health = 100;
    public float maxHealth = 100;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Shot")
        {
            Destroy(other.gameObject);
            health -= 10;
            Debug.Log("enemy has taken damage");
        }
    }

}