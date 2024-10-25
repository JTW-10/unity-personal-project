using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject pointyCube;
    public GameObject BasicEnemy;
    public bool PUNISHMENT;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("spawnCooldown");
        //StartCoroutine("deleteCooldown");
        //StartCoroutine("enemyCooldown");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void pointySpawner()
    {
        int spawnx = Random.Range(9, -77);
        int spawny = Random.Range(60, 100);
        int spawnz = Random.Range(-49, 52);

        Vector3 spawnPosition = new Vector3(spawnx, spawny, spawnz);

        Instantiate(pointyCube, spawnPosition, Quaternion.identity);
    }

    void enemySpawner()
    {
        int spawnx = Random.Range(9, -77);
        int spawny = Random.Range(60, 100);
        int spawnz = Random.Range(-49, 52);

        Vector3 spawnPosition = new Vector3(spawnx, spawny, spawnz);

        Instantiate(BasicEnemy, spawnPosition, Quaternion.identity);
    }

    IEnumerator spawnCooldown()
    {
        yield return new WaitForSeconds(Random.Range(1, 2));
        pointySpawner();
        StartCoroutine("spawnCooldown");
    }
    IEnumerator deleteCooldown()
    {
        yield return new WaitForSeconds(20);
        Destroy(pointyCube);
    }
    IEnumerator enemyCooldown()
    {
        yield return new WaitForSeconds(Random.Range(5, 10));
        enemySpawner();
    }
}
