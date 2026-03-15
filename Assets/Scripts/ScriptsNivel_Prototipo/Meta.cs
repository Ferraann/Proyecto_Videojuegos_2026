using UnityEngine;
using System.Collections;

public class Meta : MonoBehaviour
{
    public GameObject panelVictoria;
    public MonoBehaviour movimientoJugador;
    public AudioSource audioVictoria;

    private bool activado = false;

    private void Start()
    {
        if (panelVictoria != null)
            panelVictoria.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activado) return;

        if (other.CompareTag("Player"))
        {
            activado = true;

            if (panelVictoria != null)
                panelVictoria.SetActive(true);

            if (movimientoJugador != null)
                movimientoJugador.enabled = false;

            StartCoroutine(ReproducirVictoria());
        }
    }

    private IEnumerator ReproducirVictoria()
    {
        if (audioVictoria != null)
        {
            audioVictoria.ignoreListenerPause = true;
            audioVictoria.Play();
            yield return new WaitForSecondsRealtime(0.1f);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }
}