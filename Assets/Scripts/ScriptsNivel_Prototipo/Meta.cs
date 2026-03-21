using System.Collections; // Necesario para los tiempos de espera
using UnityEngine;

public class Meta : MonoBehaviour
{
    [Header("Textos e Interfaz")]
    public GameObject mensajeUI;
    public GameObject textoSobrePuerta;

    [Header("Animación de la Puerta")]
    public Animator animatorPuerta; // Arrastra aquí el objeto que tiene el Animator
    public string nombreAnimacion = "AbrirPuerta"; // El nombre exacto del estado en el Animator
    public float tiempoDeEspera = 2f; // Segundos que dura tu animación antes de mostrar la victoria

    private bool jugadorCerca = false;
    private bool metaAlcanzada = false; // Para evitar que se pulse la E varias veces

    void Start()
    {
        if (mensajeUI != null) mensajeUI.SetActive(false);
        if (textoSobrePuerta != null) textoSobrePuerta.SetActive(false);
    }

    void Update()
    {
        // Solo funciona si el jugador está cerca, pulsa E, y AÚN NO ha llegado a la meta
        if (jugadorCerca && !metaAlcanzada && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(RutinaMeta());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !metaAlcanzada)
        {
            jugadorCerca = true;
            if (mensajeUI != null) mensajeUI.SetActive(true);
            if (textoSobrePuerta != null) textoSobrePuerta.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !metaAlcanzada)
        {
            jugadorCerca = false;
            if (mensajeUI != null) mensajeUI.SetActive(false);
            if (textoSobrePuerta != null) textoSobrePuerta.SetActive(false);
        }
    }

    // Esta es la Corrutina que controla los tiempos
    private IEnumerator RutinaMeta()
    {
        metaAlcanzada = true; // Bloqueamos la puerta para que no se active de nuevo

        // 1. Ocultamos los textos de "Pulsa E"
        if (mensajeUI != null) mensajeUI.SetActive(false);
        if (textoSobrePuerta != null) textoSobrePuerta.SetActive(false);

        // 2. Reproducimos la animación
        if (animatorPuerta != null)
        {
            animatorPuerta.Play(nombreAnimacion);
        }
        else
        {
            Debug.LogWarning("Falta asignar el Animator de la puerta en el inspector.");
        }

        // 3. Esperamos a que la animación termine
        yield return new WaitForSeconds(tiempoDeEspera);

        // 4. Mostramos el mensaje de meta final
        Debug.Log("¡Has llegado a la meta!");

        // =====================================================================
        // AQUÍ PUEDES AÑADIR EL CÓDIGO PARA MOSTRAR LA PANTALLA DE VICTORIA
        // =====================================================================

        // Ejemplo de cómo sería (descomenta borrando las "//" cuando tengas tu pantalla):

        // if (pantallaVictoriaUI != null)
        // {
        //     pantallaVictoriaUI.SetActive(true); // Activa el Canvas de victoria
        // }
        // 
        // Cursor.lockState = CursorLockMode.None; // Muestra el ratón si lo tenías oculto
        // Cursor.visible = true;
        // Time.timeScale = 0f; // Pausa el juego (opcional)

        // =====================================================================
    }
}