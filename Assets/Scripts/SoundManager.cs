using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Range(0, 1)]
    public float volume = 0.6f;
    public AudioClip[] audioClips;
    [Header("Interface Elements")]
    public Slider soundSlider;
    public Text soundValueText;
    public AudioSource audioSourceFX;
    public AudioSource alarmAudioSourceFX;
    private float soundPlayTimer = 0.15f;
    private bool canPlaySound = true;

    
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        audioSourceFX.GetComponent<AudioSource>();
        if (PlayerPrefs.HasKey("SoundVolume"))
        {
            soundSlider.value = PlayerPrefs.GetFloat("SoundVolume");
        }
    }

    private void Update()
    {

        if (soundPlayTimer >= 0)
        {
            soundPlayTimer -= Time.deltaTime;
        }
        else
        {
            soundPlayTimer = 0.15f;
            canPlaySound = true;
        }

        audioSourceFX.volume = volume;
    }


    public void PlaySound(int index)
    {
        if (index == 6 || index == 7)
        {
            alarmAudioSourceFX.PlayOneShot(audioClips[index]);
        }
        else
        {
            if (canPlaySound)
            {
                audioSourceFX.PlayOneShot(audioClips[index]);
                canPlaySound = false;
            }
        }
        
    }

    public void PlayHealing()
    {
        audioSourceFX.Play();
    }

    public void StopHealing()
    {
        audioSourceFX.Stop();
    }


}
