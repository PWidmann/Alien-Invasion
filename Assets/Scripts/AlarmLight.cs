using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmLight : MonoBehaviour
{
    public GameObject light1;
    public GameObject light2;

    float alarmTimer = 3f;
    float alarmCooldown = 3f;
    void Update()
    {
        if (GameManager.AlarmActive)
        {
            light1.SetActive(true);
            light2.SetActive(true);
            Rotate();

            alarmTimer += Time.deltaTime;
            if (alarmTimer >= alarmCooldown)
            {
                SoundManager.instance.PlaySound(6);
                alarmTimer = 0;
            }
        }
    }

    void Rotate()
    {
        transform.Rotate(0, Time.deltaTime * 40, 0, Space.Self);
    }
}
