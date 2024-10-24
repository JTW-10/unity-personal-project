using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    GameManager gm;
    PlayerController playerData;

    public Transform player;
    public Transform playerObject;
    public Transform lookDirection;
    public Rigidbody myRB;

    public float playerHeight;
    public float groundDrag;
    public float doubleJump;
    public LayerMask isGround;

    [Header("Movement Settings")]
    public float speed = 10;
    public float jumpHeight = 5;
    public float airMultiplier;
    public bool canJump = true;
    public bool isGrounded = true;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerData = GameObject.Find("Player").GetComponent<PlayerController>();
        myRB = GetComponent<Rigidbody>();
        myRB.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!gm.isPaused && playerData.isAlive) // figure out why this is not working despite it literally being copied from playercontroller
        {
            float verticalMove = Input.GetAxisRaw("Vertical");
            float horizontalMove = Input.GetAxisRaw("Horizontal");

            // movement stuff
            Vector3 moveDirection;
            moveDirection = lookDirection.forward * verticalMove + lookDirection.right * horizontalMove;

            if (Physics.Raycast(transform.position, -transform.up, playerHeight * 0.5f + 0.2f, isGround))
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
            
            if (isGrounded)
            {
                myRB.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Force);
                myRB.drag = groundDrag;
                doubleJump = 0;
            }
            else
            {
                    myRB.AddForce(moveDirection.normalized * speed * 5f * airMultiplier, ForceMode.Force);
                    myRB.drag = 0f;
            }

            SpeedCap();

            if (Input.GetKeyDown(KeyCode.Space) && canJump)
            {
                if (isGrounded || doubleJump < 1)
                {
                    myRB.velocity = new Vector3(myRB.velocity.x, 0f, myRB.velocity.z);
                    myRB.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
                    doubleJump++;
                }
            }
        }
    }

    private void SpeedCap ()
    {
        Vector3 flatVelocity = new Vector3(myRB.velocity.x, 0f, myRB.velocity.z);

        if(flatVelocity.magnitude > speed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * speed;
            myRB.velocity = new Vector3(limitedVelocity.x, myRB.velocity.y, limitedVelocity.z);
        }
    }
}
