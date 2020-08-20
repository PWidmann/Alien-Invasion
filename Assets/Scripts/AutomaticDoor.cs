using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AutomaticDoor : MonoBehaviour
{
    public Animator leftDoorAnimator;
    public Animator rightDoorAnimator;

    public float doorTimer = 1f;

    private bool open = false;
    private bool isInDoor = false;
    private float saveDoorTimer;


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
                if(open)
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
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "AlienBody")
        {


            if (!open)
                SoundManager.instance.PlaySound(2);

            open = true;
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
