using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextToUI : MonoBehaviour
{
    public Text timeOfDay;
    public Text numberofDays;
    public Text years;

    private void Update()
    {
        timeOfDay.text = "Time of Day (0-1): " + DayNightCycle.instance.timeOfDay.ToString();

        numberofDays.text = "Number of Days: " + DayNightCycle.instance.dayNumber.ToString();
    }
}
