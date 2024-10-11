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
    GameManager gm;

    Vector2 camRotation;

    public Transform weaponSlotBlaster;
    public Transform weaponSlotSword;
    public GameObject swingHitbox;
    public TrainingDummyController dummy;
    public BasicEnemyController basicEnemy;
    public PlayerMovement playerMove;
    public EnemyDetection enemyAI;

    // Player and camera values

    [Header("Movement Settings")]
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
    public float weaponDamage = 0f;
    public bool canFire = true;
    public bool isAimed = false;

    [Header("Melee Weapon Stats")]
    public int meleeID = -1;
    public float swingSpeed = 0f;
    public float meleeDamage = 0f;
    public float parryWindow = 1f;
    public float dodgeWindow = 0.5f;
    public float stunDuration = 1f;
    //stuff outside of base stats I think, separating regardless
    public bool canSwing = true;
    public float comboWindow = 1f;
    public float comboCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        myRB = GetComponent<Rigidbody>();
        playerCam = Camera.main;

        camRotation = Vector2.zero;
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        dummy = GameObject.Find("TrainingDummy").GetComponent<TrainingDummyController>();
        basicEnemy = GameObject.Find("BasicEnemy").GetComponent<BasicEnemyController>();
        enemyAI = GameObject.Find("BasicEnemy").GetComponent<EnemyDetection>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!gm.isPaused)
        {
            if (Input.GetMouseButtonDown(0) && meleeID >= 0 && !isAimed && canSwing && comboCounter == 0)
            {
                Debug.Log("First swing");
                swingHitbox.SetActive(true);
                canSwing = false;
                StartCoroutine("cooldownSwing"); //need to tie combo counter to the weapon in order to track which swing you are on properly
                comboCounter++;
                StartCoroutine("comboEnd");
            }

            // firing code
            if (Input.GetMouseButtonDown(0) && canFire && isAimed && blasterID >= 0 && currentAmmo > 0)
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
            if (Input.GetMouseButtonDown(1) && !isDodging)
            {
                isAimed = true;
            }
            if (Input.GetMouseButtonUp(1))
            {
                isAimed = false;
            }

            // Keeping this as a placeholder just in case I want the movement script to be done in PlayerController rather than PlayerMovement

            // janky dodge code. figure out if you're going to keep this or change it to use a lerp or transform.position on monday
            if (Input.GetKeyDown(KeyCode.LeftAlt) && canDodge && !isParrying)
            {
                playerMove.speed += 14f;
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
            if (Input.GetKeyDown(KeyCode.C) && canParry)
            {
                isParrying = true;
                isAimed = false;
                canHit = false;
                canParry = false;
                StartCoroutine("parryingWindow");
                StartCoroutine("cooldownParry");
                // make sure to have parry animation, preferably with a deflecting animation if you land the parry
            }
        }
    }

    private void OnCollisionEnter(Collision collision) // 
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
                playerHealth -= basicEnemy.damageGiven;
                canHit = false;
                StartCoroutine("cooldownHit");
                Debug.Log("health is hit and armor is reset i hope");
            }

            if (isDodging)
            {
                Debug.Log("dodge worked");
                playerArmor = true; // remember that this is going to be the main way to get armor back in the future
                Time.timeScale = 0.3f;
                StartCoroutine("hitStop");
                StartCoroutine("cooldownHit");
            }

            if (isParrying)
            {
                Debug.Log("attack parried"); // deflection animation will go here
                playerArmor = true;
                playerHealth += 10;
                basicEnemy.isStunned = true;
                Time.timeScale = 0.3f;
                StartCoroutine("hitStop");
                StartCoroutine("parryingWindow");
                StartCoroutine("stunEnd");
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
                    weaponDamage = 15;
                    isAimed = false;
                    break;


                default:
                    break;
            }
        }

        if (other.gameObject.tag == "weaponSword")
        {
            other.gameObject.transform.SetPositionAndRotation(weaponSlotSword.position, weaponSlotSword.rotation);

            other.gameObject.transform.SetParent(weaponSlotSword);

            switch(other.gameObject.name)
            {
                case "weaponSword0":
                    meleeID = 0;
                    swingSpeed = 0.2f;
                    meleeDamage = 25f;
                    parryWindow = 1f;
                    dodgeWindow = 0.5f;
                    stunDuration = 1f;
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
        playerMove.speed = 7f;
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

    IEnumerator stunEnd()
    {
        yield return new WaitForSeconds(stunDuration);
        basicEnemy.isStunned = false;
        basicEnemy.agent.isStopped = false;
    }

    IEnumerator hitStop()
    {
        yield return new WaitForSeconds(0.1f);
        Time.timeScale = 1;
    }

    IEnumerator cooldownSwing()
    {
        yield return new WaitForSeconds(swingSpeed);
        swingHitbox.SetActive(false);
        canSwing = true;
    }
    
    IEnumerator comboEnd()
    {
        yield return new WaitForSeconds(comboWindow);
        comboCounter = 0;
    }
}
