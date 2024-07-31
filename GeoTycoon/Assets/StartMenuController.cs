using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    public Button startButton;
    public Button tutorialButton;
    public Button creditButton;
    public Button exitButton;
    public GameObject creditsPanel;

    private string tutorialURL = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";

    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
        tutorialButton.onClick.AddListener(OnTutorialButtonClicked);
        creditButton.onClick.AddListener(OnCreditButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
        creditsPanel.SetActive(false);
    }

    void OnStartButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void OnTutorialButtonClicked()
    {
        Application.OpenURL(tutorialURL);
    }

    void OnCreditButtonClicked()
    {
        creditsPanel.SetActive(true);
    }

    void OnExitButtonClicked()
    {
        Application.Quit();
    }
}
