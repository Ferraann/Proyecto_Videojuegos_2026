using UnityEngine;
using UnityEngine.UI;

public class UI_Slider : MonoBehaviour
{
    public Slider timeSlider;
    public float decreaseRate = 0.25f;

    void Start()
    {
        if (timeSlider != null)
        {
            timeSlider.value = timeSlider.maxValue;
        }
    }

    void Update()
    {
        if (timeSlider != null && timeSlider.value > timeSlider.minValue)
        {
            timeSlider.value -= decreaseRate * Time.deltaTime;

            if (timeSlider.value <= timeSlider.minValue)
            {
                timeSlider.value = timeSlider.minValue;
            }
        }
    }
}
