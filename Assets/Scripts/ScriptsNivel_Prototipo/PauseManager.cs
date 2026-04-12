using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject pauseButton;
    public Animator pauseAnimator;

    public Button[] abilityButtons; 

    private bool isPaused = false;

    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;

        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);

        if (pauseAnimator != null)
        {
            pauseAnimator.Play("PauseMenu", 0, 0f);
        }

        Time.timeScale = 0f;

        
        foreach (Button btn in abilityButtons)
        {
            btn.interactable = false;
        }
    }

    public void ResumeGame()
    {
        isPaused = false;

        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);

        Time.timeScale = 1f;

       
        foreach (Button btn in abilityButtons)
        {
            btn.interactable = true;
        }
    }
}