using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keycard : MonoBehaviour
{
    private void Update()
    {
        RotateKey();

        if (Vector3.Distance(PlayerCharacterController.Instance.transform.position, transform.position) < 2f)
        {
            SoundManager.instance.PlaySound(5);
            GameInterface.Instance.ShowMessage("You have picked up a keycard.");
            PlayerCharacterController.Instance.HasKey = true;
            Destroy(gameObject);
        }
    }

    void RotateKey()
    {
        transform.Rotate(0, 0, Time.deltaTime * 35, Space.Self);
    }
}
