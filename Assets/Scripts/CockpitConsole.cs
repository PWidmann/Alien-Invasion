using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockpitConsole : MonoBehaviour
{
    float textTimer = 0;
    bool countDownStarted = false;

    

    private void OnTriggerStay(Collider other)
    {
        textTimer = 0;
        GameInterface.Instance.interactText.SetActive(true);
    }

    private void Update()
    {
        textTimer += Time.deltaTime;

        if (textTimer >= 3f)
        {
            GameInterface.Instance.interactText.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!countDownStarted)
            {
                GameManager.SelfDestrucionActive = true;
                countDownStarted = true;
                GameInterface.Instance.ShowMessage("Escape the alien ship back at the entrance!");
            }
        }
    }
}
