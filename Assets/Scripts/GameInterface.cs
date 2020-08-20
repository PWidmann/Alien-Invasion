using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameInterface : MonoBehaviour
{
    public static GameInterface Instance;

    public GameObject pistolImage;
    public Text healthText;
    public GameObject messagePanel;
    public GameObject welcomePanel;
    public GameObject deathPanel;
    public GameObject interactText;
    public GameObject sun;

    Color defaultSunColor;

    private float destructionTimer = 0;

    private void Start()
    {
        if (Instance == null || Instance != this)
            Instance = this;

        welcomePanel.SetActive(true);
        interactText.SetActive(false);
        defaultSunColor = sun.GetComponent<Light>().color;
    }
    private void Update()
    {
        pistolImage.SetActive(WeaponHandler.Instance.pistolActive);

        healthText.text = "HP: " + PlayerCharacterController.Instance.health.ToString();

        if (GameManager.SelfDestrucionActive)
        {
            destructionTimer += Time.deltaTime;

            if (destructionTimer >= 2)
            {
                if (sun.GetComponent<Light>().intensity == 0.3f)
                {
                    sun.GetComponent<Light>().color = new Color(255, 0, 0);
                    sun.GetComponent<Light>().intensity = 0.7f;
                }
                else
                {
                    sun.GetComponent<Light>().color = defaultSunColor;
                    sun.GetComponent<Light>().intensity = 0.3f;
                }
                destructionTimer = 0;
            }
        }
    }

    public void ShowMessage(string message)
    {
        if (message != null)
        {
            messagePanel.SetActive(true);
            messagePanel.GetComponent<Message>().ShowMessage(message);
        }
    }

    public void CloseWelcomePanel()
    {
        welcomePanel.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
