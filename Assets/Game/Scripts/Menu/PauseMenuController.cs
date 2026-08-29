using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.InputSystem; // Importante para el nuevo Input System

public class PauseMenuController : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement rootVisualElement;
    private Button resumeButton;
    private Button quitButton;
    private bool isPaused = false;

    // Referencia a tu Input Action Asset (puedes arrastrarlo desde el inspector)
    [SerializeField] private InputActionAsset inputActions;
    private InputAction pauseAction;

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

        // Ocultar el menú al iniciar
        rootVisualElement.style.display = DisplayStyle.None;

        // Buscar botones en el UXML por su nombre
        resumeButton = rootVisualElement.Q<Button>("ResumeButton");
        quitButton = rootVisualElement.Q<Button>("QuitButton");

        // Registrar eventos de la UI
        resumeButton.clicked += ResumeGame;
        quitButton.clicked += QuitToMainMenu;

        // Habilitar y registrar el evento del Input System
        if (pauseAction != null)
        {
            pauseAction.started += OnPauseTriggered;
            pauseAction.Enable();
        }
    }

    /*void OnDisable()
    {
        resumeButton.clicked -= ResumeGame;
        quitButton.clicked -= QuitToMainMenu;

        if (pauseAction != null)
        {
            pauseAction.started -= OnPauseTriggered;
            pauseAction.Disable();
        }
    }*/

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
        rootVisualElement.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f; // Detiene la simulación física y del juego
    }

    public void ResumeGame()
    {
        isPaused = false;
        rootVisualElement.style.display = DisplayStyle.None;
        Time.timeScale = 1f; // Reanuda el juego
    }

    private void QuitToMainMenu()
    {
        Time.timeScale = 1f; // Restablecer siempre el tiempo antes de cambiar de escena
        SceneManager.LoadScene("MainMenu");
    }
}
