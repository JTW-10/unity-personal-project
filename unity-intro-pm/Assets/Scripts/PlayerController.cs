using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    Rigidbody myRB;
    Camera playerCam;

    Vector2 camRotation;

    public Transform weaponSlotGun;
    public Transform weaponSlotSword;

    // Player and camera values

    public bool sprintMode = false;
    public bool isGrounded = true;

    [Header("Movement Settings")]
    public float speed = 10.0f;
    public float sprintMultiplier = 1.5f;
    public float jumpHeight = 5.0f;
    public float groundDetectDistance = 2.0f;

    [Header("User Settings")]
    public float mouseSensitivity = 2.0f;
    public float Xsensitivity = 2.0f;
    public float Ysensitivity = 2.0f;
    public float camRotationLimit = 90f;
    public bool sprintToggleOption = false;

    [Header("Player Stats")]
    public float playerMaxHealth = 100.0f;
    public float playerHealth = 100.0f;
    public float pickupHealth = 25.0f; 
    public bool playerArmor = true;

    [Header("Gun Weapon Stats")]
    public int weaponID = -1;
    public float fireRate = 0.0f;
    public float maxAmmo = 0.0f;
    public float currentAmmo = 0.0f;
    public float pickupAmmo = 0.0f;
    public bool canFire = true;

    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody>();
        playerCam = transform.GetChild(0).GetComponent<Camera>();

        camRotation = Vector2.zero;
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        camRotation.x += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        camRotation.y += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        camRotation.y = Mathf.Clamp(camRotation.y, -camRotationLimit, camRotationLimit);

        playerCam.transform.localRotation = Quaternion.AngleAxis(camRotation.y, Vector3.left);
        transform.localRotation = Quaternion.AngleAxis(camRotation.x, Vector3.up);

        if(Input.GetMouseButtonDown(0) && canFire && currentAmmo > 0)
        {
            canFire = false;
            currentAmmo--;
            StartCoroutine("cooldownFire");
        }

        Vector3 temp = myRB.velocity;

        float verticalMove = Input.GetAxisRaw("Vertical");
        float horizontalMove = Input.GetAxisRaw("Horizontal");

        if (!sprintToggleOption)
        {
            if (Input.GetKey(KeyCode.LeftShift))
                sprintMode = true;

            if (Input.GetKeyUp(KeyCode.LeftShift))
                sprintMode = false;
        }

        if (sprintToggleOption)
        {
            if (Input.GetKey(KeyCode.LeftShift) && verticalMove > 0)
                sprintMode = true;

            if (verticalMove <= 0)
                sprintMode = false;
        }

        if (!sprintMode)
            temp.x = verticalMove * speed;

        if (sprintMode)
            temp.x = verticalMove * speed * sprintMultiplier;

        temp.z = horizontalMove * speed;

        if (Input.GetKeyDown(KeyCode.Space) && Physics.Raycast(transform.position, -transform.up, groundDetectDistance))
        {
            isGrounded = true;
            temp.y = jumpHeight;
        }
        myRB.velocity = (temp.x * transform.forward) + (temp.z * transform.right) + (temp.y * transform.up);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((playerHealth < playerMaxHealth) && other.gameObject.tag == "healthPickup")
        {
            playerHealth += pickupHealth;

            if (playerHealth > playerMaxHealth)
                playerHealth = playerMaxHealth;

            Destroy(other.gameObject);
        }

        if ((currentAmmo < maxAmmo) && other.gameObject.tag == "ammoPickup")
        {
            currentAmmo += pickupAmmo;

            if (currentAmmo > maxAmmo)
                currentAmmo = maxAmmo;

            Destroy(other.gameObject);
        }

        if (other.gameObject.tag == "weaponGun")
        {
            other.gameObject.transform.position = weaponSlotGun.position;

            other.gameObject.transform.SetParent(weaponSlotGun);

            switch(other.gameObject.name)
            {
                case "weaponGun0":
                    weaponID = 0;
                    fireRate = 1.0f;
                    maxAmmo = 100.0f;
                    currentAmmo = 50.0f;
                    pickupAmmo = 10.0f;
                    break;


                default:
                    break;
            }
        }
    }

    private void cooldown(bool condition, float timeLimit)
    {
        float timer = 0.0f;

        if (timer < timeLimit)
            timer += Time.deltaTime;

        else
            condition = true;
    }

    IEnumerator cooldownFire(float time)
    {
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }
}
