using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Tiempo")]
    public float tiempoTranscurrido = 0f;
    public bool nivelTerminado = false;

    [Header("Llaves")]
    public int llavesRecogidas = 0;
    public int llavesTotales = 7;

    [Header("Puntuacion")]
    public int puntuacionFinal = 0;
    private bool fueVictoria = false;

    [Header("UI Victoria")]
    public TMP_Text textoTiempo;
    public TMP_Text textoPuntuacion;
    public TMP_Text textoLlaves;

    [Header("UI Derrota")]
    public TMP_Text textoTiempoDerrota;
    public TMP_Text textoPuntuacionDerrota;
    public TMP_Text textoLlavesDerrota;

    [Header("Estrellas")]
    public GameObject estrella1;
    public GameObject estrella2;
    public GameObject estrella3;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (estrella1 != null) estrella1.SetActive(false);
        if (estrella2 != null) estrella2.SetActive(false);
        if (estrella3 != null) estrella3.SetActive(false);
    }

    private void Update()
    {
        if (!nivelTerminado)
            tiempoTranscurrido += Time.deltaTime;
    }

    public void RecogerLlave()
    {
        llavesRecogidas++;
    }

    public void FinalizarNivel()
    {
        nivelTerminado = true;
        fueVictoria = true;
        ActualizarUI();

        int estrellas = CalcularEstrellas();
        MostrarEstrellas(estrellas);
    }

    public void FinalizarDerrota()
    {
        nivelTerminado = true;
        fueVictoria = false;
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        int minutos = Mathf.FloorToInt(tiempoTranscurrido / 60f);
        int segundos = Mathf.FloorToInt(tiempoTranscurrido % 60f);

        puntuacionFinal = CalcularPuntuacion(fueVictoria);

        string tiempoTxt = "Tiempo: " + minutos.ToString("00") + ":" + segundos.ToString("00");
        string puntuacionTxt = "Puntuación: " + puntuacionFinal;
        string llavesTxt = "Llaves recogidas: " + llavesRecogidas + "/" + llavesTotales;

        if (textoTiempo != null) textoTiempo.text = tiempoTxt;
        if (textoPuntuacion != null) textoPuntuacion.text = puntuacionTxt;
        if (textoLlaves != null) textoLlaves.text = llavesTxt;

        if (textoTiempoDerrota != null) textoTiempoDerrota.text = tiempoTxt;
        if (textoPuntuacionDerrota != null) textoPuntuacionDerrota.text = puntuacionTxt;
        if (textoLlavesDerrota != null) textoLlavesDerrota.text = llavesTxt;
    }

    private int CalcularPuntuacion(bool victoria)
    {
        int puntosPorLlaves = llavesRecogidas * 200;

        if (!victoria)
        {
            // Hiçbir şey yapmadan kaybettiyse 0 puan
            if (llavesRecogidas == 0)
                return 0;

            // Kaybettiyse sadece topladığı anahtarlar sayılır
            return puntosPorLlaves;
        }

        int bonusTiempo = 0;

        if (tiempoTranscurrido <= 30f)
            bonusTiempo = 1000;
        else if (tiempoTranscurrido <= 60f)
            bonusTiempo = 700;
        else if (tiempoTranscurrido <= 90f)
            bonusTiempo = 400;
        else if (tiempoTranscurrido <= 120f)
            bonusTiempo = 200;
        else
            bonusTiempo = 0;

        int bonusVictoria = 500;

        return puntosPorLlaves + bonusTiempo + bonusVictoria;
    }

    private int CalcularEstrellas()
    {
        if (tiempoTranscurrido <= 60f) return 3;
        else if (tiempoTranscurrido <= 90f) return 2;
        else return 1;
    }

    private void MostrarEstrellas(int cantidad)
    {
        if (estrella1 != null) estrella1.SetActive(cantidad >= 1);
        if (estrella2 != null) estrella2.SetActive(cantidad >= 2);
        if (estrella3 != null) estrella3.SetActive(cantidad >= 3);
    }
}