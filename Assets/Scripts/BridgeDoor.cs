using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BridgeDoor : MonoBehaviour
{
    public Animator leftDoorAnimator;
    public Animator rightDoorAnimator;

    public float doorTimer = 1f;

    private bool open = false;
    private bool isInDoor = false;
    private float saveDoorTimer;

    private bool messageDoorOpenedplayed = false;

    private void Start()
    {
        saveDoorTimer = doorTimer;
    }

    private void Update()
    {
        if (open)
        {
            leftDoorAnimator.SetBool("open", true);
            rightDoorAnimator.SetBool("open", true);
            doorTimer -= Time.deltaTime;

            if (doorTimer <= 0 && !isInDoor)
            {
                if (open)
                    SoundManager.instance.PlaySound(3);

                open = false;
                doorTimer = saveDoorTimer;
            }
        }
        else
        {
            leftDoorAnimator.SetBool("open", false);
            rightDoorAnimator.SetBool("open", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "AlienBody")
        {
            if (!open)
                SoundManager.instance.PlaySound(2);

            open = true;
        }

        if (other.gameObject.tag == "Player")
        {
            if (PlayerCharacterController.Instance.HasKey)
            {
                if (!open)
                    SoundManager.instance.PlaySound(2);

                if (!messageDoorOpenedplayed)
                {
                    GameInterface.Instance.ShowMessage("You opened the door with your key card.");
                    messageDoorOpenedplayed = true;
                }
                
                open = true;
            }
            else
            {
                GameInterface.Instance.ShowMessage("You need a key card to open the door. Maybe a enemy has one.");
            }
            
        }
    }

    private void OnTriggerStay(Collider other)
    {
        isInDoor = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isInDoor = false;
    }
}

