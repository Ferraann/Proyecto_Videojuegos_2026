using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryMenu : MonoBehaviour
{
    public string siguienteNivel;   // next level scene name
    public string menuPrincipal;    // main menu scene name

    public void SiguienteNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(siguienteNivel);
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuPrincipal);
    }
}