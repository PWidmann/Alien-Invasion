using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeRegenStation : MonoBehaviour
{
    public GameObject healLight;

    float lightTimer = 0;

    float healtimer = 0;

    bool startSound = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            healLight.SetActive(true);
            lightTimer = 0;

            if (!startSound)
            {
                SoundManager.instance.PlayHealing();
                startSound = true;
            }

            healtimer += Time.deltaTime;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SoundManager.instance.StopHealing();
        startSound = false;
        healtimer = 0f;
    }

    private void Update()
    {
        lightTimer += Time.deltaTime;
        


        if (lightTimer >= 0.3f)
        {
            healLight.SetActive(false);
        }

        if (healtimer >= 1f)
        {
            if (PlayerCharacterController.Instance.health < 10)
            {
                PlayerCharacterController.Instance.health += 1;
                healtimer = 0f;
            }
        }
    }
}
