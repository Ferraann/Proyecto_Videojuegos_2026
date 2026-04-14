using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextoPuntuacionDerrotaFinal : MonoBehaviour
{
    private TextMeshProUGUI textoPuntuacion;

    [Header("Ajustes")]
    public int puntosPorLlave = 200;
    public int bonusMaximoTiempo = 500;
    public float penalizacionPorSegundo = 5f;
    public float multiplicadorDerrota = 0.65f;

    private void Awake()
    {
        textoPuntuacion = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        if (GameManager.Instance == null || TemporizadorGlobal.Instance == null)
            return;

        int llaves = GameManager.Instance.llavesRecogidas;
        float tiempo = TemporizadorGlobal.Instance.tiempoTranscurrido;

        int puntuacionFinal = 0;

        if (llaves == 0)
        {
            puntuacionFinal = 0;
        }
        else
        {
            int puntosLlaves = llaves * puntosPorLlave;
            int bonusTiempo = Mathf.Max(0, bonusMaximoTiempo - Mathf.FloorToInt(tiempo * penalizacionPorSegundo));

            puntuacionFinal = puntosLlaves + bonusTiempo;
            puntuacionFinal = Mathf.FloorToInt(puntuacionFinal * multiplicadorDerrota);
        }

        textoPuntuacion.text = $"Puntuación: {puntuacionFinal}";
    }
}