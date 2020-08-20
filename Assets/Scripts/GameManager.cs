using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static bool alarmActive;

    public static bool AlarmActive { get => alarmActive; set => alarmActive = value; }
}
