using UnityEngine;
using UnityEngine.UIElements;

public class FireControl : MonoBehaviour
{
    [SerializeField] private GameObject firePoint;
    [SerializeField] private GameObject fire;
    [SerializeField] private UIDocument uiDocument;
    private VisualElement rootVisualElement;
    private Label advertenciaLabel;
    private Label vidaFuegoLabel;

    private void OnEnable()
    {
        rootVisualElement = uiDocument.rootVisualElement;
        advertenciaLabel = rootVisualElement.Q<Label>("AdvertenciaLabel");
        vidaFuegoLabel = rootVisualElement.Q<Label>("VidaFuegoLabel");
    }
    private void Start()
    {
        fire.SetActive (false);
    }

    private void OnTriggerEnter(Collider colission)
    {
        if(colission.CompareTag("Player"))
        {
            advertenciaLabel.style.display = DisplayStyle.Flex;
            fire.SetActive(true);
            vidaFuegoLabel.style.display = DisplayStyle.Flex;
            Destroy(gameObject);

           
        }
    }
}
