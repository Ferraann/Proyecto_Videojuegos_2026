using UnityEngine;

public class Meta : MonoBehaviour
{
    [Header("Interfaz")]
    [Tooltip("Arrastra aquí el objeto de texto del Canvas")]
    public GameObject mensajeUI; // Referencia al texto en pantalla

    private bool jugadorCerca = false;

    void Start()
    {
        // Nos aseguramos de que el mensaje esté invisible al empezar el nivel
        if (mensajeUI != null)
        {
            mensajeUI.SetActive(false);
        }
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            LlegarAMeta();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;

            // Hacemos visible el mensaje en pantalla
            if (mensajeUI != null)
            {
                mensajeUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;

            // Ocultamos el mensaje porque el jugador se alejó
            if (mensajeUI != null)
            {
                mensajeUI.SetActive(false);
            }
        }
    }

    private void LlegarAMeta()
    {
        Debug.Log("¡Has llegado a la meta!");

        // Ocultamos el mensaje al interactuar para que no se quede en pantalla
        if (mensajeUI != null)
        {
            mensajeUI.SetActive(false);
        }

        // Aquí puedes poner tu fundido a negro, cargar siguiente escena, etc.
        // Ejemplo: SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}