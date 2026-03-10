using UnityEngine;
using UnityEngine.UI;

public class PlayerMovimiento : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform camara;
    private CharacterController controlador;

    // ---- correr --------------------------

    [Header("Movimiento")]
    // [SerializeField] private bool UsaGetAxisRaw = true;
    [SerializeField] private float velocidadMovimiento = 5f;
    [SerializeField] private float velocidadCorrer = 8f;

    [SerializeField] private float estaminaMax = 100f;
    [SerializeField] private float consumoPorSegundo = 20f;
    [SerializeField] private float recuperacionPorSegundo = 15f;
    [SerializeField] private float delayRecuperacion = 0.5f;
    [SerializeField] private Image barraEstaminaFill;
    private float estaminaActual;
    private float tiempoSinCorrer = 0f;


    [Header("Gravedad")]
    [SerializeField] private float Gravedad = -9f;
    private Vector3 velocidadVertical;

    [Header("Salto")]
    [SerializeField] private float alturaSalto = 1.5f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Necesitamos inicializar el CharacterController al inicializar el objeto
    private void Awake()
    {
        // Obtener el componente CharacterController del GameObject
        controlador = GetComponent<CharacterController>();

        // Si no se ha asignado una camara, usar la camara principal
        if (camara == null && Camera.main != null)
        {
            camara = Camera.main.transform;
        }

        estaminaActual = estaminaMax;
    }

    // Update is called once per frame
    void Update()
    {
        MoverJugadorEnPlano();
        AplicarGravedad();
        Saltar();
    }

    // Método para mover al jugador en el plano horizontal basado en la orientación de la cámara
    private void MoverJugadorEnPlano()
    {

        // ---------------------------------------
        // Movimiento del personje
        // ---------------------------------------

        // Con GetAxisRaw el valor es instantaneo, sin suavizado, con GetAxis el valor va de 0 a 1 suavemente
        float Horizontal = Input.GetAxisRaw("Horizontal");
        float Vertical = Input.GetAxisRaw("Vertical");

        // F1 para ver el valor con un decimal. Comprobar el movimiento con WASD o las flechas
        // Debug.Log($"Horizontal={Horizontal:F1}, Vertical={Vertical:F1}");

        // Obtener las direcciones adelante y derecha de la camara para el movimiento
        Vector3 adelanteCamara = camara.forward;
        Vector3 derechaCamara = camara.right;

        // Eliminar la componente Y para que no afecte al movimiento horizontal. Queremos que mire pero no que pueda caminar hacia arriba o abajo
        adelanteCamara.y = 0f;
        derechaCamara.y = 0f;

        // Hacemos que no haya ningun lado que se mueva mas que el otro. Se normalizan los vectores.
        adelanteCamara.Normalize();
        derechaCamara.Normalize();

        // Esto es para movernos en diagonal. Teniamos que si pulsabamos W ibamos adelante y si pulsabamos D ibamos a la derecha. Ahora si
        // pulsamos W+D vamos en diagonal.
        Vector3 direccionPlano = (derechaCamara * Horizontal + adelanteCamara * Vertical);

        // Normalizamos el vector para que no se mueva mas rapido en diagonal. Así nos movemos igual de rapido en todas las direcciones.
        if (direccionPlano.sqrMagnitude > 0.0001f)
        {
            direccionPlano.Normalize();
        }

        // ---------------------------------------
        // Correr con el personaje
        // ---------------------------------------

        // Esta yendo a algun sitio?
        bool seEstaMoviendo = direccionPlano.sqrMagnitude > 0.0001f;
        // Está pulsando el boton de correr?
        bool botonCorrer = Input.GetKey(KeyCode.LeftShift);
        // Tiene estamina para correr?
        bool puedeCorrer = estaminaActual > 0.01f;
        // Cumple todo?
        bool corriendo = botonCorrer && seEstaMoviendo && puedeCorrer;

        if (corriendo)
        {
            // SI cumple todo (corriendo) se reduce a la estamina actual el consumo por segundo (time.deltaTime para que no importe los fps)
            estaminaActual -= consumoPorSegundo * Time.deltaTime;
            // Y como estamos corriendo el tiempo sin correr vuelve a 0
            tiempoSinCorrer = 0f;
        }
        else
        {
            // Como no estamos corrieno aumentamos el tiempo sin correr
            tiempoSinCorrer += Time.deltaTime;
            // SI el tiempo sin correr es mayor o igual al delay de recuperacion, recuperamos estamina
            if (tiempoSinCorrer >= delayRecuperacion)
            {
                // Recuperamos estamina
                estaminaActual += recuperacionPorSegundo * Time.deltaTime;
            }
        }

        // Clamp para que la estamina no se pase de los limites
        estaminaActual = Mathf.Clamp(estaminaActual, 0f, estaminaMax);

        // Ajustar la barra de estamina dependiendo del nivel de estamina
        if (barraEstaminaFill != null)
        {
            barraEstaminaFill.fillAmount = estaminaActual / estaminaMax;
        }

        // Si esta corriendo usamos la velocidad de correr, si no la de movimiento normal
        float velocidadActual = corriendo ? this.velocidadCorrer : this.velocidadMovimiento;

        // Movimiento del personaje usando CharacterController
        Vector3 desplazamientoXZ = direccionPlano * (velocidadActual * Time.deltaTime);

        // Mover el CharacterController
        controlador.Move(desplazamientoXZ);
    }

    private void AplicarGravedad()
    {
        velocidadVertical.y += Gravedad * Time.deltaTime;
        controlador.Move(velocidadVertical * Time.deltaTime);

        if (controlador.isGrounded && velocidadVertical.y < 0)
        {
            velocidadVertical.y = -2f;
        }
    }

    private void Saltar()
    {
        bool saltar = Input.GetButtonDown("Jump");

        if (saltar && controlador.isGrounded)
        {
            velocidadVertical.y = Mathf.Sqrt(2f * -Gravedad * alturaSalto);
        }
    }
}
