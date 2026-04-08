using UnityEngine;
using System.Collections;

public class MovimientoRutaPatrullero : MonoBehaviour
{
    private enum Estado { Patrullando, Persiguiendo, EsperandoYGirando }
    [SerializeField] private Estado estadoActual = Estado.Patrullando;

    [Header("Referencias")]
    public OjosPatrullero ojos;
    public Transform jugador;

    [Header("Ruta de Patrulla")]
    public Transform puntoA;
    public Transform puntoB;
    public float velocidadPatrulla = 3f;
    public float tiempoDeEspera = 1.5f;
    public float velocidadGiro = 5f;

    [Header("Persecución")]
    public float velocidadPersecucion = 5f;

    private Transform destinoActual;
    private bool estaCambiandoDePunto = false;

    void Start()
    {
        destinoActual = puntoB;
        if (ojos == null) ojos = GetComponent<OjosPatrullero>();
    }

    void Update()
    {
        if (ojos.viendoAlJugador)
        {
            estadoActual = Estado.Persiguiendo;
            estaCambiandoDePunto = false;
            StopAllCoroutines();
        }
        else if (estadoActual == Estado.Persiguiendo)
        {
            estadoActual = Estado.Patrullando;
        }

        switch (estadoActual)
        {
            case Estado.Patrullando:
                MoverHaciaDestino();
                break;
            case Estado.Persiguiendo:
                PerseguirJugador();
                break;
            case Estado.EsperandoYGirando:
                break;
        }
    }

    void MoverHaciaDestino()
    {
        Vector3 posPlana = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 destinoPlano = new Vector3(destinoActual.position.x, 0, destinoActual.position.z);

        // ARREGLO 1: Reducimos la distancia a casi nada (0.05f)
        if (Vector3.Distance(posPlana, destinoPlano) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(destinoPlano.x, transform.position.y, destinoPlano.z),
                velocidadPatrulla * Time.deltaTime);

            GirarHacia(destinoPlano);
        }
        else if (!estaCambiandoDePunto)
        {
            // ARREGLO 2: Forzamos la posición para que pise el punto con exactitud matemática antes de parar
            transform.position = new Vector3(destinoPlano.x, transform.position.y, destinoPlano.z);
            StartCoroutine(SecuenciaCambioDePunto());
        }
    }

    IEnumerator SecuenciaCambioDePunto()
    {
        estaCambiandoDePunto = true;
        estadoActual = Estado.EsperandoYGirando;

        yield return new WaitForSeconds(tiempoDeEspera);

        destinoActual = (destinoActual == puntoA) ? puntoB : puntoA;
        Vector3 nuevoDestinoPlano = new Vector3(destinoActual.position.x, 0, destinoActual.position.z);

        float anguloDiferencia = 180f;
        while (anguloDiferencia > 1f)
        {
            Vector3 direccion = nuevoDestinoPlano - transform.position;
            direccion.y = 0;
            Quaternion rotacionDeseada = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, velocidadGiro * Time.deltaTime);

            anguloDiferencia = Quaternion.Angle(transform.rotation, rotacionDeseada);
            yield return null;
        }

        estadoActual = Estado.Patrullando;
        estaCambiandoDePunto = false;
    }

    void PerseguirJugador()
    {
        Vector3 destinoJugador = new Vector3(jugador.position.x, transform.position.y, jugador.position.z);
        transform.position = Vector3.MoveTowards(transform.position, destinoJugador, velocidadPersecucion * Time.deltaTime);
        GirarHacia(destinoJugador);
    }

    void GirarHacia(Vector3 objetivo)
    {
        Vector3 direccion = objetivo - transform.position;
        direccion.y = 0;
        if (direccion.magnitude > 0.01f) // Evita temblores
        {
            Quaternion rotacion = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacion, velocidadGiro * Time.deltaTime);
        }
    }
}