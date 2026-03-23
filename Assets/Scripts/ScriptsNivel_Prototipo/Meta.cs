using System.Collections;
using UnityEngine;

public class Meta : MonoBehaviour
{
    [Header("Textos e Interfaz")]
    [Tooltip("El texto del Canvas que dice 'Pulsa E'")]
    public GameObject mensajeUI;

    [Tooltip("El texto 3D flotante sobre la puerta")]
    public GameObject textoSobrePuerta;

    [Tooltip("Arrastra aquí tu Canvas o panel de Pantalla de Carga/Victoria")]
    public GameObject pantallaCargaUI;

    [Header("Animación de la Puerta")]
    [Tooltip("El objeto que tiene el componente Animator")]
    public Animator animatorPuerta;

    [Tooltip("El nombre exacto del estado en el Animator")]
    public string nombreAnimacion = "AbrirPuerta";

    [Header("Cinemática del Jugador")]
    [Tooltip("Arrastra a tu jugador aquí")]
    public Transform jugador;

    [Tooltip("Objeto vacío detrás de la puerta donde caminará el jugador")]
    public Transform puntoDestino;

    [Tooltip("Velocidad a la que el jugador camina hacia la puerta")]
    public float velocidadJugador = 2.5f;

    private bool jugadorCerca = false;
    private bool metaAlcanzada = false; // Evita que se active varias veces

    void Start()
    {
        // Nos aseguramos de que toda la interfaz esté oculta al empezar
        if (mensajeUI != null) mensajeUI.SetActive(false);
        if (textoSobrePuerta != null) textoSobrePuerta.SetActive(false);
        if (pantallaCargaUI != null) pantallaCargaUI.SetActive(false);
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

    // Corrutina que controla la secuencia final
    private IEnumerator RutinaMeta()
    {
        metaAlcanzada = true;

        if (mensajeUI != null) mensajeUI.SetActive(false);
        if (textoSobrePuerta != null) textoSobrePuerta.SetActive(false);

        if (animatorPuerta != null)
        {
            animatorPuerta.Play(nombreAnimacion);
        }

        yield return new WaitForSeconds(0.5f);

        // Guardamos las referencias del jugador antes de entrar al bucle
        CharacterController cc = null;
        Rigidbody rb = null;
        PlayerMovimiento scriptMovimiento = null;

        if (jugador != null && puntoDestino != null)
        {
            // 1. Apagamos el script de movimiento para que no pelee con la cinemática
            scriptMovimiento = jugador.GetComponent<PlayerMovimiento>();
            if (scriptMovimiento != null) scriptMovimiento.enabled = false;

            // 2. Apagamos físicas
            cc = jugador.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            rb = jugador.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;

            Vector3 destinoPlano = new Vector3(puntoDestino.position.x, jugador.position.y, puntoDestino.position.z);
            float tiempoMaximo = 4f;
            float tiempoActual = 0f;

            // Caminamos...
            while (Vector3.Distance(jugador.position, destinoPlano) > 0.1f && tiempoActual < tiempoMaximo)
            {
                jugador.position = Vector3.MoveTowards(jugador.position, destinoPlano, velocidadJugador * Time.deltaTime);
                tiempoActual += Time.deltaTime;
                yield return null;
            }
        }

        Debug.Log("Cinemática de caminar terminada.");

        // =========================================================
        // VOLVEMOS A ENCENDER LAS FÍSICAS Y EL SCRIPT DE MOVIMIENTO
        // =========================================================
        if (cc != null) cc.enabled = true;
        if (rb != null) rb.isKinematic = false;
        if (scriptMovimiento != null) scriptMovimiento.enabled = true; // <--- LO VOLVEMOS A ENCENDER

        yield return new WaitForSeconds(1f);

        // Encendemos el Canvas
        if (pantallaCargaUI != null)
        {
            pantallaCargaUI.SetActive(true);
            Debug.Log("Canvas activado con éxito.");
        }
    }
}