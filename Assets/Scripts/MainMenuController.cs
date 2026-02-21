using UnityEngine;
using UnityEngine.SceneManagement;

public enum MenuScreens
{
    Main,
    Settings,
    ControlSelection,
    Audio
}

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string matchSceneName;

    [Header("Screens")]
    [SerializeField] GameObject main;
    [SerializeField] GameObject settings;
    [SerializeField] GameObject controlSelection;
    [SerializeField] GameObject audioScreen;

    private void Awake()
    {
        SetScreen(MenuScreens.Main);
    }

    public void StartGame()
    {
        SceneTransitionManager.Instance.ChangeScene(matchSceneName);
    }

    public void SetScreen(MenuScreens screen)
    {
        main.SetActive(screen == MenuScreens.Main);
        settings.SetActive(screen == MenuScreens.Settings);
        controlSelection.SetActive(screen == MenuScreens.ControlSelection);
        audioScreen.SetActive(screen == MenuScreens.Audio);
    }

    public void SetScreen(int screen)
    {
        SetScreen((MenuScreens)(screen));
    }
}
