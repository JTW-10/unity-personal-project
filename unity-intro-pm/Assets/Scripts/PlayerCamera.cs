using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    GameManager gm;

    public Transform player;
    public Transform playerObject;
    public Transform lookDirection;
    public Transform aimingLookAt;
    public Rigidbody myRB;
                                    // main issue at the moment is that the player object is not looking in same direction as aiming camera and functions more like the normal cam
    public float rotationSpeed;


    public CameraStyle currentStyle;
    public GameObject normalCam;
    public GameObject aimingCam;

    public enum CameraStyle
    {
        Normal,
        Aiming
    }

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gm.isPaused)
        {
            Vector3 viewDirection = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
            lookDirection.forward = viewDirection.normalized;

            if(currentStyle == CameraStyle.Normal)
            {
                float verticalMove = Input.GetAxisRaw("Vertical");
                float horizontalMove = Input.GetAxisRaw("Horizontal");
                Vector3 inputDirection = lookDirection.forward * verticalMove + lookDirection.right * horizontalMove;

                if (inputDirection != Vector3.zero)
                {
                    playerObject.forward = Vector3.Slerp(playerObject.forward, inputDirection.normalized, Time.deltaTime * rotationSpeed);
                    player.forward = Vector3.Slerp(player.forward, inputDirection.normalized, Time.deltaTime * rotationSpeed);
                }
            }

            else if(currentStyle == CameraStyle.Aiming)
            {
                Vector3 aimingDirection = aimingLookAt.position - new Vector3(transform.position.x, aimingLookAt.position.y, transform.position.z);
                lookDirection.forward = aimingDirection.normalized;

                playerObject.forward = aimingDirection.normalized;
            }

            if(Input.GetMouseButtonDown(1))
            {
                normalCam.SetActive(false);
                aimingCam.SetActive(true);
            }
            if(Input.GetMouseButtonUp(1))
            {
                normalCam.SetActive(true);
                aimingCam.SetActive(false);
            }
        }
    }
}
