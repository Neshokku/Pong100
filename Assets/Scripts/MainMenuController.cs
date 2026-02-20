using UnityEngine;
using UnityEngine.SceneManagement;

public enum MenuScreens
{
    Main,
    Settings
}

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string matchSceneName;

    [Header("Screens")]
    [SerializeField] GameObject main;
    [SerializeField] GameObject settings;

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
    }

    public void SetScreen(int screen)
    {
        main.SetActive(screen == (int)MenuScreens.Main);
        settings.SetActive(screen == (int)MenuScreens.Settings);
    }
}
