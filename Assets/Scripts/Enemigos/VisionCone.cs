using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VisionCone : MonoBehaviour
{
    public Material visionConeMaterial;

    public float visionRange = 10f;
    [Range(0, 360)]
    public float visionAngle = 90f;
    public LayerMask visionObstructingLayer;
    public int resolution = 100;

    public Color normalColor = Color.green;
    public Color alertColor = Color.red;

    public bool playerDetected = false;

    Mesh mesh;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = visionConeMaterial;

        visionConeMaterial.color = normalColor;
    }

    void Update()
    {
        playerDetected = false;

        DrawVisionCone();

        if (playerDetected)
        {
            visionConeMaterial.color = alertColor;
            Debug.Log("Jugador detectado");
        }
        else
        {
            visionConeMaterial.color = normalColor;
        }
    }

    void DrawVisionCone()
    {
        int vertexCount = resolution + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(resolution - 1) * 3];

        vertices[0] = Vector3.zero;

        float angleStep = visionAngle / (resolution - 1);
        float currentAngle = -visionAngle / 2;

        for (int i = 0; i < resolution; i++)
        {
            float rad = currentAngle * Mathf.Deg2Rad;

            Vector3 dir = new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad));
            dir = transform.rotation * dir;

            Ray ray = new Ray(transform.position, dir);
            RaycastHit hit;

            float distance = visionRange;

            if (Physics.Raycast(ray, out hit, visionRange))
            {
                distance = hit.distance;

                if (hit.collider.CompareTag("Player"))
                {
                    playerDetected = true;
                }
            }

            Vector3 point = dir * distance;
            vertices[i + 1] = transform.InverseTransformPoint(transform.position + point);

            Debug.DrawRay(transform.position, dir * distance, Color.red);

            currentAngle += angleStep;
        }

        for (int i = 0; i < resolution - 1; i++)
        {
            int index = i * 3;
            triangles[index] = 0;
            triangles[index + 1] = i + 1;
            triangles[index + 2] = i + 2;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}