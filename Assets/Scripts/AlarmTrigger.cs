using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!GameManager.AlarmActive)
            {
                GameManager.AlarmActive = true;
                Destroy(gameObject);
            }
        }
    }
}
