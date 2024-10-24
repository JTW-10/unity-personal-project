using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CheatCodes : MonoBehaviour
{
    public EnemyDetection enemyDetection;
    public GameManager gm;
    public Transform player;
    public PlayerController playerData;
    public GameObject thebeast;
    public GameObject beastText;
    public GameObject beastWall;
    public bool thebeasthungers = false;
    // Start is called before the first frame update
    void Start()
    {
        playerData = GameObject.Find("Player").GetComponent<PlayerController>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!gm.isPaused)
        {
            if (gm.debugKeys)
            {
                if (Input.GetKeyDown(KeyCode.L))
                {
                    if (!thebeasthungers)
                    {
                        thebeasthungers = true;
                        thebeast.gameObject.SetActive(true);
                        beastText.gameObject.SetActive(true);
                    }
                    else
                    {
                        thebeasthungers = false;
                        thebeast.gameObject.SetActive(false);
                        beastText.gameObject.SetActive(false);
                    }
                }

                if (Input.GetKeyDown(KeyCode.O))
                {
                    Time.timeScale = 0.5f;
                }
                if (Input.GetKeyDown(KeyCode.P))
                {
                    Time.timeScale = 1f;
                }

                if(Input.GetKeyDown(KeyCode.U))
                {
                    playerData.playerHealth = 0;
                }
            }
        }
    }
}
