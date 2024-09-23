using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    Rigidbody myRB;
    Camera playerCam;

    Vector2 camRotation;

    public Transform weaponSlotBlaster;
    public Transform weaponSlotSword;
    public EnemyController enemyScript;

    // Player and camera values

    public bool sprintMode = false;         // DO NOT FORGET TO GET THE PARRY BUTTON WORKING. Needs to have a cooldown, short parry window, and have it do something that dodge doesnt (maybe. last part could be handled in the future)
    public bool isGrounded = true;

    [Header("Movement Settings")]
    public float speed = 10f;
    public float sprintMultiplier = 1.5f;
    public float jumpHeight = 5f;
    public float groundDetectDistance = 2f;
    public bool canDodge = true;
    public bool isDodging = false;
    public float dodgeCooldown = 3f;
    
      

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
    public float dodgeWindow = 0.5f;
    public float parryWindow = 1f;
    public bool canHit = true;
    public bool canParry = true;
    public bool playerArmor = true;
    public bool isParrying = false;

    [Header("Blaster Weapon Stats")]
    public GameObject shot;
    public int blasterID = -1;
    public float shotVel = 0f;
    public float fireRate = 0f;
    public float maxAmmo = 0f;
    public float currentAmmo = 0f;
    public float pickupAmmo = 0f;
    public float shotLifespan = 0f;
    public bool canFire = true;
    public bool isAimed = false;

    [Header("Sword Weapon Stats")]
    public int swordID = -1;

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

        if (Input.GetMouseButtonDown(0) && swordID >= 0 && !isAimed)
        {
            Debug.Log("normal swing");

            if (isDodging)
            {
                Debug.Log("dodge swing");
            }
        }

        // firing code
        if(Input.GetMouseButtonDown(0) && canFire && isAimed && blasterID >= 0 && currentAmmo > 0)
        {
            GameObject s = Instantiate(shot, weaponSlotBlaster.position, weaponSlotBlaster.rotation);
            s.GetComponent<Rigidbody>().AddForce(playerCam.transform.forward * shotVel);
            Destroy(s, shotLifespan);
            
            canFire = false;
            currentAmmo--;
            StartCoroutine("cooldownFire");
            Debug.Log("fired weapon");
        }

        // aiming code
        if(Input.GetMouseButtonDown(1) && !isDodging)
        {
            isAimed = true;
        }
        if(Input.GetMouseButtonUp(1))
        {
            isAimed = false;
        }
        if(isAimed)
        {
            playerCam.fieldOfView = 40;
        }
        if(!isAimed)
        {
            playerCam.fieldOfView = 60;
        }

        // movement stuff. will probably change due to jank
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

        // janky dodge code. figure out if you're going to keep this or change it to use a lerp or transform.position on monday
        if (Input.GetKeyDown(KeyCode.LeftAlt) && canDodge && !sprintMode && !isParrying) 
        {
            speed += 20f;
            canDodge = false;
            isDodging = true;
            canHit = false;
            isAimed = false;
            playerCam.fieldOfView = 90;
            StartCoroutine("cooldownDodge");
            StartCoroutine("dodgingReset");
            StartCoroutine("dodgingWindow");
            Debug.Log("Is dodging");
            // make sure to have dodge animation, probably with some sort of glitchy effect
        }

        // janky parrying code
        if (Input.GetKeyDown(KeyCode.C) && !sprintMode && canParry)
        {
            isParrying = true;
            isAimed = false;
            canHit = false;
            canParry = false;
            StartCoroutine("parryingWindow");
            StartCoroutine("cooldownParry");
            // make sure to have parry animation, preferably with a deflecting animation if you land the parry
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

        // following stuff is just to recognize input only during dashing state
        if (Input.GetKeyDown(KeyCode.X) && isDodging)
        {
            Debug.Log("dodgeattack");
        }
        // delete this later
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "enemy")
        {
            if (playerArmor && canHit)
            {
                playerArmor = false;
                canHit = false;
                StartCoroutine("cooldownHit");
                Debug.Log("armor is hit");
            }

            if (!playerArmor && canHit)
            {
                playerHealth -= 25f;
                canHit = false;
                StartCoroutine("cooldownHit");
                Debug.Log("health is hit and armor is reset i hope");
            }

            if (isDodging)
            {
                Debug.Log("dodge worked");
                playerArmor = true; // remember that this is going to be the main way to get armor back in the future
                StartCoroutine("cooldownHit");
            }

            if (isParrying)
            {
                Debug.Log("attack parried"); // deflection animation will go here
                playerArmor = true;
                playerHealth += 10;
                StartCoroutine("parryingWindow");
            }
        }
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
                    blasterID = 0;
                    shotVel = 10000;
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

    IEnumerator cooldownDodge()
    {
        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
        Debug.Log("dodge cooldown finished");
    }

    IEnumerator cooldownHit()
    {
        yield return new WaitForSeconds(hitCooldown);
        canHit = true;
    }

    IEnumerator dodgingReset()
    {
        yield return new WaitForSeconds(0.2f);
        speed = 10f;
        playerCam.fieldOfView = 60;
        Debug.Log("dodge speed over");
    }

    IEnumerator dodgingWindow()
    {
        yield return new WaitForSeconds(dodgeWindow);
        isDodging = false;
        canHit = true;
        Debug.Log("dodge window is over");
    }

    IEnumerator parryingWindow()
    {
        yield return new WaitForSeconds(parryWindow);
        isParrying = false;
        canHit = true;
    }

    IEnumerator cooldownParry()
    {
        yield return new WaitForSeconds(3f);
        canParry = true;
    }

}
