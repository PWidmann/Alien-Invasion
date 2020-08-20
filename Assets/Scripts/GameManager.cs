using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static bool alarmActive;
    private static bool selfDestrucionActive = false;

    private static int aliensKilled = 0;

    public static bool AlarmActive { get => alarmActive; set => alarmActive = value; }
    public static bool SelfDestrucionActive { get => selfDestrucionActive; set => selfDestrucionActive = value; }
    public static int AliensKilled { get => aliensKilled; set => aliensKilled = value; }
}
