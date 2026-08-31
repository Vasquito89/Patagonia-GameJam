using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class Fuego : MonoBehaviour
{
    [Header("Referencias Visuales")]
    [SerializeField] private GameObject fire;
    [SerializeField] private GameObject smoke;
    [SerializeField] private ParticleSystem fireParticleSystem;
    [SerializeField] private UIDocument uiDocument;

    [Header("Configuración de Salud y Estado")]
    [SerializeField] private int vidaMax = 20;
    public int vida;
    public bool extinguido = false;

    [Header("Temporizadores")]
    [SerializeField] private float tiempoParaApagar = 5f; // Tiempo antes de expandirse
    private float temporizadorConteo = 0f;
    private bool fuegoExpandido = false;

    [Header("Configuración de Tamaño")]
    [SerializeField] private float initialFireSize = 1f;
    [SerializeField] private float fireGrowthSpeed = 0.5f;
    [SerializeField] private float maxFireSize = 6f;

    private float currentFireSizeMultiplier;
    private VisualElement rootVisualElement;
    private Label tiempoFuego;
    private Label advertenciaLabel;
    private Label vidaFuegoLabel;
    private Collider colFuego;

    private void Awake()
    {
        colFuego = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        if (uiDocument != null)
        {
            rootVisualElement = uiDocument.rootVisualElement;
            if (rootVisualElement != null)
            {
                tiempoFuego = rootVisualElement.Q<Label>("TiempoLabel");
                advertenciaLabel = rootVisualElement.Q<Label>("AdvertenciaLabel");
                vidaFuegoLabel = rootVisualElement.Q<Label>("VidaFuegoLabel");
            }
        }
    }

    void Start()
    {
        currentFireSizeMultiplier = initialFireSize;
        vida = vidaMax;
        extinguido = false;
        fuegoExpandido = false;
        temporizadorConteo = tiempoParaApagar;

        if (fireParticleSystem != null)
        {
            var mainModule = fireParticleSystem.main;
            mainModule.startSizeMultiplier = currentFireSizeMultiplier;
        }

        if (fire != null) fire.SetActive(true);
        if (smoke != null) smoke.SetActive(false);
        StartCoroutine(ApagarLabel());
        Debug.Log("Se va a apagar el label de advertencia");
    }

    void Update()
    {
        // Si el fuego se apagó, cortamos todo el Update inmediatamente
        if (extinguido) return;

        if (!fuegoExpandido)
        {
            // FASE 1: Cuenta regresiva para apagar antes de que crezca
            temporizadorConteo -= Time.deltaTime;

            if (tiempoFuego != null)
            {
                tiempoFuego.style.display = DisplayStyle.Flex;
                tiempoFuego.text = "Tiempo para apagar fuego: " + Mathf.Max(0, temporizadorConteo).ToString("F1") + "s";
                vidaFuegoLabel.text = "Vida del fuego" + vida.ToString();
            }

            if (temporizadorConteo <= 0f)
            {
                fuegoExpandido = true;
                temporizadorConteo = 0f; // Reinicia para contar tiempo expandido
            }
        }
        else
        {
            // FASE 2: Fuego expandido, cuenta cuánto lleva encendido
            temporizadorConteo += Time.deltaTime;
            CrecerFuego();

            if (tiempoFuego != null)
            {
                tiempoFuego.style.display = DisplayStyle.Flex;
                tiempoFuego.text = "¡FUEGO EXPANDIDO! Tiempo encendido: " + temporizadorConteo.ToString("F1") + "s";
            }
        }
    }

    public void TirarAgua()
    {
        if (extinguido) return;

        // Le quitamos vida al fuego
        vida -= 20;

        if (smoke != null) smoke.SetActive(true);

        // Si se quedó sin vida, lo apagamos completamente
        if (vida <= 0)
        {
            Extinguir();
        }
    }

    private void CrecerFuego()
    {
        if (currentFireSizeMultiplier < maxFireSize)
        {
            currentFireSizeMultiplier += fireGrowthSpeed * Time.deltaTime;

            if (fireParticleSystem != null)
            {
                var mainModule = fireParticleSystem.main;
                mainModule.startSizeMultiplier = currentFireSizeMultiplier;
            }
        }
    }

    private void Extinguir()
    {
        extinguido = true;

        // 1. Ocultar interfaz inmediatamente
        if (tiempoFuego != null)
        {
            tiempoFuego.style.display = DisplayStyle.None;
            vidaFuegoLabel.style.display = DisplayStyle.None;
        }

        // 2. Apagar componentes visuales e interactivos
        if (fire != null) fire.SetActive(false);
        if (smoke != null) smoke.SetActive(false);
        if (colFuego != null) colFuego.enabled = false;

        // 3. Destruir objeto de escena
        Destroy(gameObject, 0.5f);
    }

    IEnumerator ApagarLabel()
    {
        Debug.Log("Apagándose label");

        yield return new WaitForSeconds(0.0f);

        advertenciaLabel.style.display = DisplayStyle.None;
        Debug.Log("Estado del label display: " + advertenciaLabel.style.display.value);
        
    }
}