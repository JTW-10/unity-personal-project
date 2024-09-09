using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // This is handling the Rigidbody
    Rigidbody myRB;

    public float speed = 10.0f;
    public float jumpHeight = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 temp = myRB.velocity;

        temp.x = Input.GetAxisRaw("Vertical") * speed;
        temp.z = Input.GetAxisRaw("Horizontal") * speed;

        if (Input.GetKeyDown(KeyCode.Space))
            temp.y = jumpHeight;

        myRB.velocity = (temp.x * transform.forward) + (temp.z * transform.right) + (temp.y * transform.up);
    }
}
