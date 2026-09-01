using Goru.Core;
using Goru.Controller;
using Goru.Inputs;
using UnityEngine;
using UnityEngine.UIElements;

public class Lake : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    private Label _promptLabel;

    private string mensajePrompt = "Presiona [Q] para cargar agua";

    private void OnEnable()
    {
        if (uiDocument == null) uiDocument = FindAnyObjectByType<UIDocument>();
        if (uiDocument != null && uiDocument.rootVisualElement != null)
        {
            _promptLabel = uiDocument.rootVisualElement.Q<Label>("LakeLabel");
            _promptLabel.style.display = DisplayStyle.None;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Toca el agua");
            // Muestra la UI desde el gestor o interfaz
            _promptLabel.text = mensajePrompt;
            _promptLabel.style.display = DisplayStyle.Flex;

            /*if (other.TryGetComponent<PlayerController>(out var player) &&
                other.TryGetComponent<InputProvider>(out var input))
            {
                Debug.Log("Entro para ver si cargo agua");
                // Usamos la referencia local 'input' que garantizamos que existe
                if (input.BaldeRequested && player.ICanCarryWater())
                {
                    Debug.Log("acceso a cargar agua");
                    player.StartWaterFill();
                }
            }*/
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Muestra la UI desde el gestor o interfaz
            _promptLabel.style.display = DisplayStyle.None;
        }
    }
}