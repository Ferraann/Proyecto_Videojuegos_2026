using UnityEngine;
using UnityEngine.SceneManagement;

public class DetectorCamara : MonoBehaviour
{
    [Header("Conexiones")]
    public GeneradorCono cono;
    public Transform jugador;

    [Header("Configuración")]
    public LayerMask capaObstaculos; // Aquí selecciona solo "Mapa"
    public string tagJugador = "Player";

    [Header("Ajustes de Derrota")]
    public bool reiniciarAlPerder = true;
    public float tiempoEspera = 1.0f;

    private bool yaDetectado = false;

    void Update()
    {
        if (jugador == null || cono == null || yaDetectado) return;

        Vector3 haciaJugador = jugador.position - transform.position;
        float distancia = haciaJugador.magnitude;

        // 1. ¿Está cerca?
        if (distancia <= cono.distanciaVision)
        {
            // 2. ¿Está en el ángulo del cono?
            float anguloApertura = Mathf.Atan2(cono.radioBase, cono.distanciaVision) * Mathf.Rad2Deg;
            float anguloAlJugador = Vector3.Angle(transform.forward, haciaJugador.normalized);

            if (anguloAlJugador <= anguloApertura)
            {
                // 3. ¿Hay visión directa? 
                // Lanzamos un rayo. Si no choca con nada de la capa "Mapa", vemos al jugador.
                if (!Physics.Linecast(transform.position, jugador.position, capaObstaculos))
                {
                    Perder();
                }
            }
        }
    }

    void Perder()
    {
        yaDetectado = true;
        Debug.Log("<color=red><b>¡JUGADOR DETECTADO!</b></color>");

        // Feedback visual: cono rojo
        if (GetComponent<MeshRenderer>() != null)
        {
            GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, 0.4f);
        }

        if (reiniciarAlPerder)
        {
            Invoke("Reiniciar", tiempoEspera);
        }
    }

    void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}