using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    // Singleton
    public static SceneTransitionManager Instance { get; private set; }

    // Variables
    private bool isChanging = false;

    [Header("References")]
    [SerializeField] RectTransform blackRectangleTransform;

    [Header("Config")]
    [SerializeField] float transitionTime = 1.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        blackRectangleTransform.gameObject.SetActive(false);
    }

    public void ChangeScene(string sceneName)
    {
        if (isChanging) return;
        StartCoroutine(ChangeSceneRoutine(sceneName));
    }

    private IEnumerator ChangeSceneRoutine(string sceneName)
    {
        isChanging = true;
        blackRectangleTransform.gameObject.SetActive(true);

        // Black Rectangle comes in

        Vector3 initialPos = new Vector3(blackRectangleTransform.position.x - blackRectangleTransform.rect.width, blackRectangleTransform.position.y, blackRectangleTransform.position.z);
        Vector3 finalPos = blackRectangleTransform.position;

        float t = 0.0f;

        while (t < transitionTime)
        {
            t += Time.deltaTime;
            blackRectangleTransform.position = Vector3.Lerp(initialPos, finalPos, t / transitionTime);
            yield return null;
        }

        blackRectangleTransform.position = finalPos;
        
        //Scene Loads
        yield return SceneManager.LoadSceneAsync(sceneName);

        // Black Rectangle comes out

        initialPos = blackRectangleTransform.position;
        finalPos = new Vector3(blackRectangleTransform.position.x + blackRectangleTransform.rect.width, blackRectangleTransform.position.y, blackRectangleTransform.position.z);

        t = 0.0f;

        while (t < transitionTime)
        {
            t += Time.deltaTime;
            blackRectangleTransform.position = Vector3.Lerp(initialPos, finalPos, t / transitionTime);
            yield return null;
        }

        blackRectangleTransform.position = initialPos;
        blackRectangleTransform.gameObject.SetActive(false);

        isChanging = false;
    }

}
