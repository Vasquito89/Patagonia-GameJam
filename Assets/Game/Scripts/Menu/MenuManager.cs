using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    //[Header("Buttons")]
    private Button matchButton;
    private Button optionButton;
    private Button exitButton;
    

    private void OnEnable()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        VisualElement visualElement = uiDocument.rootVisualElement;
        ScrollView scrollView = visualElement.Q<ScrollView>();

        matchButton = scrollView.Q<Button>("MatchButton");
        optionButton = scrollView.Q<Button>("OptionButton");
        exitButton = scrollView.Q<Button>("QuitButton");

        matchButton.clicked += StartMatchScene;
        optionButton.clicked += StartOptionScene;
        exitButton.clicked += OnExit;

    }

    public void OnExit() => Application.Quit();


    public void StartMatchScene()
    {
        SceneManager.LoadScene("Nivel1");
    }

    public void StartOptionScene()
    {
        SceneManager.LoadScene("Option");
    }
}
