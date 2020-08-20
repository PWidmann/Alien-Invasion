using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static bool alarmActive;
    private static bool selfDestrucionActive = false;

    public static bool AlarmActive { get => alarmActive; set => alarmActive = value; }
    public static bool SelfDestrucionActive { get => selfDestrucionActive; set => selfDestrucionActive = value; }
}
