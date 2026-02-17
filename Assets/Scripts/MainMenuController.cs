using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string matchSceneName;

    private bool pressedStart = false;
    
    public void StartGame()
    {
        if (pressedStart) return;
        pressedStart = true;
        SceneTransitionManager.Instance.ChangeScene(matchSceneName);
    }
}
