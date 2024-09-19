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

    public Transform weaponSlotBlaster;
    public Transform weaponSlotSword;

    // Player and camera values

    public bool sprintMode = false;
    public bool isGrounded = true;

    [Header("Movement Settings")]
    public float speed = 10f;
    public float sprintMultiplier = 1.5f;
    public float jumpHeight = 5f;
    public float groundDetectDistance = 2f;
    public bool canDash = true;
    public float dashDistance = 10000f;
    public float dashCooldown = 3f;

    [Header("User Settings")]
    public float mouseSensitivity = 3f;
    public float Xsensitivity = 3f;
    public float Ysensitivity = 3f;
    public float camRotationLimit = 90f;
    public bool sprintToggleOption = false;

    [Header("Player Stats")]
    public float playerMaxHealth = 100f;
    public float playerHealth = 100f;
    public float pickupHealth = 25f;
    public float hitCooldown = 2f;
    public float armorCooldown = 10f;
    public bool canHit = true;
    public bool playerArmor = true;

    [Header("Blaster Weapon Stats")]
    public GameObject shot;
    public int weaponID = -1;
    public float shotVel = 10000f;
    public float fireRate = 0f;
    public float maxAmmo = 0f;
    public float currentAmmo = 0f;
    public float pickupAmmo = 0f;
    public float shotLifespan = 0f;
    public bool canFire = true;
    public bool isAimed = false;

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

        if(Input.GetMouseButtonDown(0) && canFire && weaponID >= 0 && currentAmmo > 0)
        {
            GameObject s = Instantiate(shot, weaponSlotBlaster.position, weaponSlotBlaster.rotation);
            s.GetComponent<Rigidbody>().AddForce(playerCam.transform.forward * shotVel);
            Destroy(s, shotLifespan);
            
            canFire = false;
            currentAmmo--;
            StartCoroutine("cooldownFire");
        }

        if(Input.GetMouseButtonDown(1))
        {
            isAimed = true;
            playerCam.transform.localPosition += Vector3.forward * 2f;
        }
        if(Input.GetMouseButtonUp(1))
        {
            isAimed = false;
            playerCam.transform.localPosition += -Vector3.forward * 2f;
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

        if (Input.GetKey(KeyCode.LeftAlt) && canDash && verticalMove > 0)
        {
            myRB.AddForce(transform.forward * dashDistance);
            canDash = false;
            StartCoroutine("cooldownDash");
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

        if (other.gameObject.tag == "weaponBlaster")
        {
            other.gameObject.transform.SetPositionAndRotation(weaponSlotBlaster.position, weaponSlotBlaster.rotation);

            other.gameObject.transform.SetParent(weaponSlotBlaster);

            switch(other.gameObject.name)
            {
                case "weaponBlaster0":
                    weaponID = 0;
                    shotVel = 1000;
                    fireRate = 0.05f;
                    maxAmmo = 100.0f;
                    currentAmmo = 50.0f;
                    pickupAmmo = 10.0f;
                    shotLifespan = 10;
                    isAimed = false;
                    break;


                default:
                    break;
            }
        }

        if (other.gameObject.tag == "enemy")
        {
            if (playerArmor)
            {
                playerArmor = false;
                StartCoroutine("cooldownArmor");
                Debug.Log("armor is hit");
            }

            else
            {
                playerHealth -= 25f;
                canHit = false;
                StartCoroutine("cooldownHit");
                StartCoroutine("cooldownArmor");
                Debug.Log("health is hit and armor is reset i hope");
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

    IEnumerator cooldownFire()
    {
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }

    IEnumerator cooldownDash()
    {
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    IEnumerator cooldownHit()
    {
        yield return new WaitForSeconds(hitCooldown);
        canHit = true;
    }

    IEnumerator cooldownArmor() // this is likely going to need to be changed later, as I want armor to come from parries and dodges
    {
        yield return new WaitForSeconds(armorCooldown);
        playerArmor = true;
    }
}
