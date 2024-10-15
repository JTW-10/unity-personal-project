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
    public GameObject baseMainMenu;
    public GameObject optionsMenu;
    public PlayerController playerData;

    public Image healthBar;
    public Image armorIcon;
    public TextMeshProUGUI comboCounter;
    public TextMeshProUGUI ammoCounter;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex > 0)
            playerData = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex > 0)
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

            if (playerData.meleeID < 0)
            {
                comboCounter.gameObject.SetActive(false);
            }
            else
            {
                comboCounter.gameObject.SetActive(true);
                comboCounter.text = "Combo: " + playerData.comboCounter;
            }

            if (!playerData.playerArmor)
            {
                armorIcon.gameObject.SetActive(false);
            }
            else
            {
                armorIcon.gameObject.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
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

        if(SceneManager.GetActiveScene().buildIndex < 0)
        {
            // placeholder, might need this for later, got no clue
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

    public void NewGame()
    {
        SceneManager.LoadScene(1);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        Time.timeScale = 1;

        isPaused = false;
    }
    
    public void MainMenuButton()
    {
        SceneManager.LoadScene(0);
    }

    public void OptionsButtonMainMenu()
    {
        baseMainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void LoadMenuFromOptions()
    {
        optionsMenu.SetActive(false);
        baseMainMenu.SetActive(true);
    }
}
