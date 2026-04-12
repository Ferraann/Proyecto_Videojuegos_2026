using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    public Button button;
    public Image cooldownOverlay;

    private Coroutine currentCooldown;

    private void Start()
    {
        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 0f;
            cooldownOverlay.gameObject.SetActive(false);
        }
    }

    public void StartCooldown(float duration)
    {
        if (currentCooldown != null)
        {
            StopCoroutine(currentCooldown);
        }

        currentCooldown = StartCoroutine(CooldownRoutine(duration));
    }

    private IEnumerator CooldownRoutine(float duration)
    {
        button.interactable = false;

        cooldownOverlay.gameObject.SetActive(true);
        cooldownOverlay.fillAmount = 1f;

        float timeLeft = duration;

        while (timeLeft > 0f)
        {
            timeLeft -= Time.unscaledDeltaTime;
            cooldownOverlay.fillAmount = timeLeft / duration;
            yield return null;
        }

        cooldownOverlay.fillAmount = 0f;
        cooldownOverlay.gameObject.SetActive(false);
        button.interactable = true;

        currentCooldown = null;
    }
}
