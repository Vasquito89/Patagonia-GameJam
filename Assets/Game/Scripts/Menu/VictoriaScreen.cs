using UnityEngine;
using UnityEngine.UIElements;

public class VictoriaScreen : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    private VisualElement rootVisualElement;
    private VisualElement Victoria;

    private void OnEnable()
    {
        rootVisualElement = uiDocument.rootVisualElement;
        Victoria = rootVisualElement.Q<VisualElement>("VictoriaVE");
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Victoria.style.display = DisplayStyle.Flex;
        }
    }
}
