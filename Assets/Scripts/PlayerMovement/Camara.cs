using UnityEngine;

public class Camara : MonoBehaviour
{
    public Transform Player;
    public float Suavizado = 5f;
    private Vector3 offset;

    void Start()
    {
        offset = transform.position - Player.position;
    }

    void LateUpdate()
    {
        if (Player == null) return;
        Vector3 posicionObjetivo = Player.position + offset;
        transform.position = Vector3.Lerp(transform.position, posicionObjetivo, Suavizado * Time.deltaTime);
    }
}