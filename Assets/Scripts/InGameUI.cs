using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameUI : MonoBehaviour
{
    public WeaponHandler weaponHandler;
    public PlayerHandler playerHandler;
    public GameObject MP5XImage;
    public GameObject berettaImage;
    public Slider healthSlider;
    public Image healthSliderImage;
    public TextMeshProUGUI pocketBulletsTextMesh;
    public TextMeshProUGUI magazineBulletsTextMesh;
    public TextMeshProUGUI scoreTextMesh;
    public TextMeshProUGUI gameOverScoreTextMesh;
    public TextMeshProUGUI gameOverHighScoreTextMesh;
    public GameObject ammoFillHintPanel;
    public GameObject healthPlaceholder;
    public EnemySpawner enemySpawner;


    public GameObject menuPanel;
    public GameObject gameOverPanel;
    public AssetLoader assetLoader;    

    public bool shouldPause = false;
    public bool isGameOver = false;

    private float timeElapsedSinceGameOver = 0f;
    


    private void Update()
    {

        if (playerHandler.isDead)
        {
            isGameOver = true;
            GameOver();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            shouldPause = !shouldPause;
        }

        if (!isGameOver)
        {
            if (shouldPause)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }

        if (weaponHandler.currentWeapon.weaponName == "MP5X")
        {
            MP5XImage.SetActive(true);
        }
        else
        {
            MP5XImage.SetActive(false);
        }

        if (weaponHandler.currentWeapon.weaponName == "pistol")
        {
            berettaImage.SetActive(true);
        }
        else
        {
            berettaImage.SetActive(false);
        }

        healthSlider.value = playerHandler.health / 100f;
        if (playerHandler.health < 15f)
        {
            healthSliderImage.color = new Color32(252,92,81,170);
        }
        else if (playerHandler.health < 50f)
        {
            healthSliderImage.color = new Color32(242,252,81,170);
        }

        magazineBulletsTextMesh.text = weaponHandler.currentWeapon.magazineBullets.ToString();
        pocketBulletsTextMesh.text = weaponHandler.currentWeapon.pocketBullets.ToString();
        scoreTextMesh.text = playerHandler.score.ToString();

    }

    private void GameOver()
    {
        timeElapsedSinceGameOver += Time.unscaledDeltaTime;
        playerHandler.isGamePaused = true;
        int previousHighScore = PlayerPrefs.GetInt("highScore");
        if (previousHighScore < playerHandler.score)
        {
            PlayerPrefs.SetInt("highScore", playerHandler.score);
        }

        PlayerPrefs.Save();
        Time.timeScale = 0.1f;

        if (timeElapsedSinceGameOver > 3f)
        {
            gameOverPanel.SetActive(true);    
            healthPlaceholder.SetActive(false);
            gameOverScoreTextMesh.text = playerHandler.score.ToString();

            gameOverHighScoreTextMesh.text = PlayerPrefs.GetInt("highScore").ToString();
            freeMouse();
            
        }
    }

    public void RestartGame()
    {
        enemySpawner.DeleteAllEnemies();
        Time.timeScale = 1f;
        assetLoader.LoadScene(1);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }


    public void ResumeGame()
    {
        shouldPause = false;
        Debug.Log("called res");
        Time.timeScale = 1f;
        playerHandler.isGamePaused = false;
        healthPlaceholder.SetActive(true);
        lockMouse();
        menuPanel.SetActive(false);
    }

    public void PauseGame()
    {
        menuPanel.SetActive(true);
        Time.timeScale = 0f;
        playerHandler.isGamePaused = true;
        healthPlaceholder.SetActive(false);
        freeMouse();
    }

    private static void freeMouse()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private static void lockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowAmmoFillHintPanel()
    {
        ammoFillHintPanel.SetActive(true);
    }

    public void HideAmmoFillHintPanel()
    {
        ammoFillHintPanel.SetActive(false);
    }
}
