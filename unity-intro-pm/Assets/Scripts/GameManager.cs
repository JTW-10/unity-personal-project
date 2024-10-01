using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public bool isPaused = false;

    public GameObject pauseMenu;
    public GameObject mainHUD;
    public PlayerController playerData;

    public Image healthBar;
    public Image armorIcon;
    public TextMeshProUGUI ammoCounter;

    // Start is called before the first frame update
    void Start()
    {
        playerData = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = Mathf.Clamp(playerData.playerHealth / playerData.playerMaxHealth, 0, 1);

        if (playerData.blasterID < 0)
        {
            ammoCounter.gameObject.SetActive(false);
        }
        else
        {
            ammoCounter.gameObject.SetActive(true);
            ammoCounter.text = "Ammo: " + playerData.currentAmmo;
        }

        if(!playerData.playerArmor)
        {
            armorIcon.gameObject.SetActive(false);
        }
        else
        {
            armorIcon.gameObject.SetActive(true);
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                pauseMenu.SetActive(true);

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                Time.timeScale = 0;

                isPaused = true;
            }

            else
                Resume();
        }
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        Time.timeScale = 1;

        isPaused = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadLevel(int SceneID)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneID);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }
}
