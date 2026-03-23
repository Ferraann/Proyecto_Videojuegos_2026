using UnityEngine;
using TMPro;

public class DetectionHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;

    public void Show(float timeRemaining)
    {
        countdownText.gameObject.SetActive(true);
        countdownText.text = timeRemaining.ToString("F1");
    }

    public void Hide()
    {
        countdownText.gameObject.SetActive(false);
    }
}