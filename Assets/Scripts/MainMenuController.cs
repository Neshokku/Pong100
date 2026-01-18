using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string matchSceneName;

    public void StartGame()
    {
        SceneManager.LoadScene(matchSceneName);
    }
}
