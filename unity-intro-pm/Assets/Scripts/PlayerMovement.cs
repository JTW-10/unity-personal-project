using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform player;
    public Transform playerObject;
    public Transform lookDirection;
    public Rigidbody myRB;
    

    public float rotationSpeed;

    [Header("Movement Settings")]
    public float speed = 10;
    public float jumpHeight = 5;
    public bool isGrounded = true;

    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody>();
        myRB.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        // orientation of player and inputs
        Vector3 viewDirection = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        lookDirection.forward = viewDirection.normalized;

        float verticalMove = Input.GetAxisRaw("Vertical");
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        Vector3 inputDirection = lookDirection.forward * verticalMove + lookDirection.right * horizontalMove;

        if (inputDirection != Vector3.zero)
        {
            playerObject.forward = Vector3.Slerp(playerObject.forward, inputDirection.normalized, Time.deltaTime * rotationSpeed);
        }

        // movement stuff
        //Vector3 moveDirection;
        //moveDirection = lookDirection.forward * verticalMove + lookDirection.right * horizontalMove;
        //myRB.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Force);
    }
}
