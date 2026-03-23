using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DetectorCamara : MonoBehaviour
{
    [Header("Conexiones y Configuración")]
    public Transform jugador;
    public string tagJugador = "Player";
    public LayerMask capaObstaculos;

    [Header("Dimensiones del Cono")]
    public float rangoVision = 10f;
    [Range(0, 180)] public float aperturaHorizontal = 60f;
    [Range(0, 180)] public float aperturaVertical = 40f;
    public int resolucion = 20; // Calidad de la malla y precisión de detección

    [Header("Visualización")]
    public Color colorNormal = new Color(0, 1, 0, 0.3f); // Verde
    public Color colorAlerta = new Color(1, 0, 0, 0.5f); // Rojo

    private Mesh mesh;
    private Material materialCono;
    private bool detectado = false;

    void Start()
    {
        // Inicializamos la malla y el material
        mesh = new Mesh();
        mesh.name = "Malla_Detector_Camara";
        GetComponent<MeshFilter>().mesh = mesh;
        materialCono = GetComponent<MeshRenderer>().material;
        materialCono.color = colorNormal;
    }

    void LateUpdate()
    {
        if (detectado) return;

        bool encontrado = false;
        GenerarConoYDetectar(ref encontrado);

        if (encontrado)
        {
            LogicaPerder();
        }
    }

    void GenerarConoYDetectar(ref bool hayContacto)
    {
        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(Vector3.zero); // Origen (punta de la cámara)

        // Bucle para crear la rejilla de puntos (filas y columnas)
        for (int y = 0; y <= resolucion; y++)
        {
            float tY = (float)y / resolucion;
            float angY = Mathf.Lerp(-aperturaVertical / 2, aperturaVertical / 2, tY);

            for (int x = 0; x <= resolucion; x++)
            {
                float tX = (float)x / resolucion;
                float angX = Mathf.Lerp(-aperturaHorizontal / 2, aperturaHorizontal / 2, tX);

                // Dirección local del rayo
                Vector3 dirLocal = Quaternion.Euler(angY, angX, 0) * Vector3.forward;
                Vector3 dirGlobal = transform.TransformDirection(dirLocal);

                float distanciaFinal = rangoVision;

                // LANZAMIENTO DEL RAYO (Igual que en VisionCone)
                RaycastHit hit;
                if (Physics.Raycast(transform.position, dirGlobal, out hit, rangoVision))
                {
                    distanciaFinal = hit.distance;

                    // Si choca con el jugador, marcamos detección
                    if (hit.collider.CompareTag(tagJugador))
                    {
                        hayContacto = true;
                    }
                }

                // Añadimos el punto a la lista de vértices de la malla
                vertices.Add(dirLocal * distanciaFinal);
            }
        }

        // --- CONSTRUCCIÓN DE TRIÁNGULOS (Lados y Tapa) ---
        List<int> tri = new List<int>();
        int vFila = resolucion + 1;

        for (int y = 0; y < resolucion; y++)
        {
            for (int x = 0; x < resolucion; x++)
            {
                int i = y * vFila + x + 1;

                // Triángulos de la cara frontal (tapa del cono)
                tri.Add(i); tri.Add(i + 1); tri.Add(i + vFila);
                tri.Add(i + vFila); tri.Add(i + 1); tri.Add(i + vFila + 1);

                // Triángulos laterales (conectan los bordes con el origen 0)
                if (y == 0) { tri.Add(0); tri.Add(i + 1); tri.Add(i); } // Borde superior
                if (y == resolucion - 1) { tri.Add(0); tri.Add(i + vFila); tri.Add(i + vFila + 1); } // Borde inferior
                if (x == 0) { tri.Add(0); tri.Add(i); tri.Add(i + vFila); } // Borde izquierdo
                if (x == resolucion - 1) { tri.Add(0); tri.Add(i + vFila + 1); tri.Add(i + 1); } // Borde derecho
            }
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = tri.ToArray();
        mesh.RecalculateNormals();
    }

    void LogicaPerder()
    {
        detectado = true;
        materialCono.color = colorAlerta;
        Debug.Log("<color=red><b>[CÁMARA]:</b> ¡JUGADOR DETECTADO! Reiniciando...</color>");

        // Reiniciamos la escena tras 1 segundo
        Invoke("ReiniciarNivel", 1.2f);
    }

    void ReiniciarNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}