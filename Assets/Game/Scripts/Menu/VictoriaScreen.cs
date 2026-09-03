using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class VictoriaScreen : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    private VisualElement rootVisualElement;
    private VisualElement Victoria;
    private VideoPlayer victoryVideo;

    private void OnEnable()
    {
        rootVisualElement = uiDocument.rootVisualElement;
        Victoria = rootVisualElement.Q<VisualElement>("VictoriaVE");

        victoryVideo = FindAnyObjectByType<VideoPlayer>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Victoria.style.display = DisplayStyle.Flex;
            victoryVideo.Play();
            Time.timeScale = 0f;
        }
    }
}
