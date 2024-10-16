using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatCodes : MonoBehaviour
{
    public EnemyDetection enemyDetection;
    public GameObject thebeast;
    public bool thebeasthungers = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (!thebeasthungers)
            {
                thebeasthungers = true;
                thebeast.gameObject.SetActive(true);
            }
            else
            {
                thebeasthungers = false;
                thebeast.gameObject.SetActive(false);
            }
        }
    }
}
