using UnityEngine;
using UnityEngine.UI;

public class UI_Slider : MonoBehaviour
{
    public Slider      timeSlider;
    public VisionCone  visionCone;

    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (timeSlider != null)
        {
            timeSlider.maxValue = visionCone != null ? visionCone.tiempoDeteccion : 1f;
            timeSlider.value    = timeSlider.maxValue;
        }

        canvasGroup.alpha = 0f;
    }

    void Update()
    {
        if (visionCone == null || timeSlider == null) return;

        if (visionCone.playerDetected)
        {
            canvasGroup.alpha    = 1f;
            timeSlider.value     = visionCone.tiempoDeteccion - visionCone.DetectionTimer;
        }
        else
        {
            canvasGroup.alpha    = 0f;
            timeSlider.value     = timeSlider.maxValue;
        }
    }
}