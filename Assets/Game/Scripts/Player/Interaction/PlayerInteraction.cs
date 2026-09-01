using Goru.Core;
using Goru.Inputs;
using Goru.Controller;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuración Raycast")]
    [SerializeField] private float reachDistance = 10.0f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Transform cameraTransform;

    private PlayerController _playerController;
    private IInteractable _currentInteractable;

    // UI Toolkit
    [SerializeField] private UIDocument uiDocument;
    private Label _promptLabel;

    private InputProvider _input;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
        _input = GetComponent<InputProvider>();
    }

    private void OnEnable()
    {
        if (uiDocument == null) uiDocument = FindAnyObjectByType<UIDocument>();
        if (uiDocument != null && uiDocument.rootVisualElement != null)
        {
            _promptLabel = uiDocument.rootVisualElement.Q<Label>("PromptLabel");
            HidePrompt();
        }
    }

    private void Update()
    { 
        businessRaycast();
        if (_input.SecarRequested)
        {
            OnInteractInput();
        }
        if(_input.BaldeRequested && _playerController.ICanCarryWater())
        {
            Debug.Log("acceso a cargar agua");
            _playerController.StartWaterFill();
        }
    }

    private void businessRaycast()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, reachDistance, interactableLayer))
        {
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                _currentInteractable = interactable;
                ShowPrompt(interactable.GetInteractPrompt());
                Debug.Log("Chocaste con un elemento interactuable!!!");
                return;
            }
        }

        _currentInteractable = null;
        HidePrompt();
    }

    // Llamar a este método desde el evento del Input System (al presionar la tecla asignada)
    public void OnInteractInput()
    {
        if (_currentInteractable != null)
        {
            Debug.Log("Pulsaste la tecla para interactuar");
            _currentInteractable.Interact(_playerController);
            _input.ConsumeSecar();
        }
    }

    private void ShowPrompt(string texto)
    {
        if (_promptLabel == null) return;
        _promptLabel.text = texto;
        _promptLabel.style.display = DisplayStyle.Flex;
    }

    private void HidePrompt()
    {
        if (_promptLabel != null)
            _promptLabel.style.display = DisplayStyle.None;
    }
}