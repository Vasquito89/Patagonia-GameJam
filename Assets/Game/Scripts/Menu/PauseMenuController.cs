using Goru.Controller;
using Goru.Movement; // Importante para el nuevo Input System
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenuController : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement rootVisualElement;
    private VisualElement pauseVE;
    private VisualElement HUDVE;
    private VisualElement Derrota;
    private VisualElement Victoria;
    private Button resumeButton;
    private Button quitButton;
    private Button menuButton;
    private Button AceptarButton;
    private Label vidaPlayer;
    private Label tiempoFuego;
    private Label advertenciaLabel;
    private Label vidaFuegoLabel;
    private Label tiempoVidaLabel;
    private Label energyLabel;
    private bool isPaused = false;
    [SerializeField] private PlayerController player;
    private PlayerMovement playerMove;

    // Referencia a tu Input Action Asset (puedes arrastrarlo desde el inspector)
    [SerializeField] private InputActionAsset inputActions;
    private InputAction pauseAction;

    private float end = 180f;
    private bool isPlaying = false;

    void Awake()
    {
        uiDocument = GetComponent<UIDocument>();

        // Configuramos la acción de pausa (puedes cambiar "Player/Pause" por tu mapa/acción)
        // O si prefieres crearlo por código sin depender de un Asset, descomenta la línea de abajo:
        // pauseAction = new InputAction("Pause", binding: "<Keyboard>/escape");

        if (inputActions != null)
        {
            var uiMap = inputActions.FindActionMap("Menu");
            pauseAction = uiMap.FindAction("Pause");
        }
    }

    void OnEnable()
    {
        rootVisualElement = uiDocument.rootVisualElement;
        pauseVE = rootVisualElement.Q<VisualElement>("PauseVE");
        HUDVE = rootVisualElement.Q<VisualElement>("HUDVE");
        Derrota = rootVisualElement.Q<VisualElement>("DerrotaVE");
        Victoria = rootVisualElement.Q<VisualElement>("VictoriaVE");

        // Ocultar el menú al iniciar
        pauseVE.style.display = DisplayStyle.None;
        HUDVE.style.display = DisplayStyle.Flex;
        Derrota.style.display = DisplayStyle.None;
        Victoria.style.display = DisplayStyle.None;

        // Buscar botones en el UXML por su nombre
        resumeButton = rootVisualElement.Q<Button>("ResumeButton");
        quitButton = rootVisualElement.Q<Button>("QuitButton");

        vidaPlayer = rootVisualElement.Q<Label>("VidaLabel");
        tiempoFuego = rootVisualElement.Q<Label>("TiempoLabel");
        advertenciaLabel = rootVisualElement.Q<Label>("AdvertenciaLabel");
        vidaFuegoLabel = rootVisualElement.Q<Label>("VidaFuegoLabel");
        tiempoVidaLabel = rootVisualElement.Q<Label>("TiempoVidaLabel");
        energyLabel = rootVisualElement.Q<Label>("EnergyLabel");

        tiempoFuego.style.display = DisplayStyle.None;
        advertenciaLabel.style.display = DisplayStyle.None;
        vidaFuegoLabel.style.display = DisplayStyle.None;
        tiempoVidaLabel.style.display = DisplayStyle.Flex;

        menuButton = rootVisualElement.Q<Button>("MenuButton");
        AceptarButton = rootVisualElement.Q<Button>("AceptarButton");

        // Registrar eventos de la UI
        resumeButton.clicked += ResumeGame;
        quitButton.clicked += QuitToMainMenu;

        menuButton.clicked += QuitToMainMenu;
        AceptarButton.clicked += QuitToMainMenu;

        // Habilitar y registrar el evento del Input System
        if (pauseAction != null)
        {
            pauseAction.started += OnPauseTriggered;
            pauseAction.Enable();
        }
        if(tiempoVidaLabel != null)
        {
            isPlaying = true;
        }

        
    }
    private void Update()
    {
        player = FindAnyObjectByType<PlayerController>();
        string vidaRestante = player.vida.ToString();
        string vida = "Vida restante :";
        vidaPlayer.text = vida + vidaRestante;

        energyLabel.text = "Energia :" + player.energy;

        playerMove = FindAnyObjectByType<PlayerMovement>();

        if (isPlaying)
        {
            if (end > 0)
            {
                end -= Time.deltaTime;
                ActualizarTexto(end);
            }
            else
            {
                end = 0f;
                isPlaying = false;
                ActualizarTexto(end);
                LogicaTiempoTerminado();
                playerMove.Morir();
            }
        }
    }

    // Método que se ejecuta cuando el nuevo Input System detecta la pulsación
    private void OnPauseTriggered(InputAction.CallbackContext context)
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        

        isPaused = true;
        //rootVisualElement.style.display = DisplayStyle.Flex;
        HUDVE.style.display = DisplayStyle.None;
        pauseVE.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f; // Detiene la simulación física y del juego
        
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseVE.style.display = DisplayStyle.None;
        HUDVE.style.display = DisplayStyle.Flex;
        Time.timeScale = 1f; // Reanuda el juego
    }

    private void QuitToMainMenu()
    {
        Time.timeScale = 1f; // Restablecer siempre el tiempo antes de cambiar de escena
        SceneManager.LoadScene("MainMenu");
    }
    void ActualizarTexto(float tiempoEnSegundos)
    {
        if (tiempoEnSegundos < 0f) tiempoEnSegundos = 0f;

        // Operaciones matemáticas estándar para la variable flotante
        int minutos = Mathf.FloorToInt(tiempoEnSegundos / 60f);
        int segundos = Mathf.FloorToInt(tiempoEnSegundos % 60f);

        // Asignamos el formato de minutos al texto de UI Toolkit
        tiempoVidaLabel.text = "El juego termina en " + string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    void LogicaTiempoTerminado()
    {
        Debug.Log("¡Tiempo agotado!");
    }
}
