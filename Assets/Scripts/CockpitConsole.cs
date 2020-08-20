using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CockpitConsole : MonoBehaviour
{
    float textTimer = 0;
    bool insideTrigger = false;

    private void OnTriggerStay(Collider other)
    {
        textTimer = 0;
        GameInterface.Instance.interactText.SetActive(true);
        insideTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        insideTrigger = false;
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
            if (insideTrigger)
            {
                GameManager.SelfDestrucionActive = true;
                GameInterface.Instance.ShowMessage("Self destruction started! Go back to the entrance to escape the ship!");
            }
        }
    }
}
