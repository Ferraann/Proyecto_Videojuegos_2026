using UnityEngine;
using UnityEngine.SceneManagement;

public class DerrotaMenu : MonoBehaviour
{
    public string menuPrincipal;

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
