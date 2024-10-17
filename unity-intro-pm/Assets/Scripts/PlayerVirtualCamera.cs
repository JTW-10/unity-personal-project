using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVirtualCamera : MonoBehaviour
{
    Camera playerCam;
    Transform cameraHolder;
    GameManager gm;
    Vector2 camRotation;
    public float mouseSensitivity = 2f;
    public float camRotationLimit = 90f;
    public float Xsensitivity = 2f;
    public float Ysensitivity = 2f;

    // Start is called before the first frame update
    void Start()
    {
        playerCam = Camera.main;
        camRotation = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if(!gm.isPaused)
        {
            camRotation.x += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
            camRotation.y += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

            camRotation.y = Mathf.Clamp(camRotation.y, -camRotationLimit, camRotationLimit);

            cameraHolder.transform.rotation = Quaternion.Euler(-camRotation.y, camRotation.x, 0);
            transform.localRotation = Quaternion.AngleAxis(camRotation.x, Vector3.up);
        }
    }
}
