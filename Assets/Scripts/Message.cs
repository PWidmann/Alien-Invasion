using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class Message : MonoBehaviour
{
    public static Message Instance;

    public GameObject messagePanel;
    public GameObject messageText;

    public int panelWidth = 600;
    private float currentWidth;
    private bool panelActive = false;
    private bool fadeOut = false;

    private float showTimer = 3f;

    private float animationSpeed = 10f;

    void Start()
    {
        if (Instance == null)
            Instance = this;

        // Set panel width to 10f
        messagePanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100f);
        currentWidth = 100;
        messageText.SetActive(false);
    }


    void Update()
    {
        if (panelActive)
        {
            // Fade in
            if (currentWidth < panelWidth && !fadeOut)
            {
                currentWidth += animationSpeed;

                if (currentWidth > panelWidth - 100f)
                {
                    messageText.SetActive(true);
                }
            }

            messagePanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentWidth);

            // When max width
            if (currentWidth == panelWidth)
            {
                showTimer -= Time.deltaTime;

                if (showTimer <= 0f)
                {
                    fadeOut = true;
                }
            }

            // Fade out
            if (fadeOut)
            {
                if (currentWidth > 100)
                {
                    currentWidth -= animationSpeed;

                    if (currentWidth < panelWidth - 100f)
                    {
                        messageText.SetActive(false);
                    }
                }
                else
                {
                    showTimer = 3f;
                    fadeOut = false;
                    panelActive = false;
                }
            }

            if (!panelActive)
                messagePanel.SetActive(false);
        }
    }

    public void ShowMessage(string message)
    {
        if (message != null)
        {
            messageText.GetComponent<Text>().text = message;

            panelActive = true;
        }
    }
}
