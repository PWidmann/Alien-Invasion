using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public static WeaponHandler Instance;

    public GameObject pistolObject;
    public ParticleSystem muzzleFlash;


    public Animator playerAnimator;
    public Texture2D crossHair;

    public bool pistolActive = false;

    private void Awake()
    {

        if (Instance != this || Instance == null)
            Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            pistolActive = !pistolActive;
            SoundManager.instance.PlaySound(1);
            muzzleFlash.Play();
        }

        PistolActive();

        if (PlayerCharacterController.IsAiming && pistolActive)
        {
            Cursor.SetCursor(crossHair, new Vector2(25f, 25f), CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    void PistolActive()
    {
        if (pistolActive)
        {
            playerAnimator.SetBool("pistolActive", true);
            pistolObject.SetActive(true);
        }
        else
        {
            playerAnimator.SetBool("pistolActive", false);
            pistolObject.SetActive(false);
        }
    }
}
