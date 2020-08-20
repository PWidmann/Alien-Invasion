using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInterface : MonoBehaviour
{
    public static GameInterface Instance;

    public GameObject pistolImage;
    public Text healthText;
    public GameObject messagePanel;

    private void Start()
    {
        if (Instance == null || Instance != this)
            Instance = this;
    }
    private void Update()
    {
        pistolImage.SetActive(WeaponHandler.Instance.pistolActive);

        healthText.text = "HP: " + PlayerCharacterController.Instance.health.ToString();
    }

    public void ShowMessage(string message)
    {
        if (message != null)
        {
            messagePanel.SetActive(true);
            messagePanel.GetComponent<Message>().ShowMessage(message);
        }
    }
}
