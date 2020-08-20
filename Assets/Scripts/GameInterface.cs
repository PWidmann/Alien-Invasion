using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInterface : MonoBehaviour
{
    public GameObject pistolImage;

    private void Update()
    {
        pistolImage.SetActive(WeaponHandler.Instance.pistolActive);
    }
}
