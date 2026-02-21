using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public enum MatchType
{
    PlayerVSPlayer,
    PlayerVSCPU
}



public class GameManagerController : MonoBehaviour
{
    [Header("Actors")]
    PlayerController player1Controller;
    PlayerController player2Controller;
    [SerializeField] GameObject player1GameObject;
    [SerializeField] GameObject player2GameObject;
    [SerializeField] Transform player1Spawn;
    [SerializeField] Transform player2Spawn;
    [SerializeField] GameObject ballPrefab;
    private BallController ballController;



    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerPointsUI player1ScoreUI;
    [SerializeField] private PlayerPointsUI player2ScoreUI;
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private TextMeshProUGUI multiplierTextEffect;
    [SerializeField] private TextMeshProUGUI multiplierText2;
    [SerializeField] private TextMeshProUGUI multiplierTextEffect2;
    [SerializeField] private GameObject mobileControls;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject allUI;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private ControlSet p1ControlSet;
    [SerializeField] private ControlSet p2ControlSet;

    [Header("Config")]
    [SerializeField] private float timeBetweenMultiplierIncreases = 20.0f;
    [SerializeField] private int pointMultiplier = 1;

    [Header("Sounds")]
    [SerializeField] private AudioClip multiplierUpSound;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip winSound;

    private List<float> doublePointStacks = new List<float>();

    private bool paused = false;

    public bool gameUp { get; private set; } = false;

    // Components
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        player1Controller = Instantiate(player1GameObject, new Vector3(player1Spawn.position.x, player1Spawn.position.y), Quaternion.identity).GetComponent<PlayerController>();
        player2Controller = Instantiate(player2GameObject, new Vector3(player2Spawn.position.x, player2Spawn.position.y), Quaternion.identity).GetComponent<PlayerController>();
    }

    void Start()
    {
        playerInput.actions["Pause"].performed += TogglePause;
        StartCoroutine(StartGameRoutine());
        StartCoroutine(MultiplierIncreaseRoutine());
    }

    private void Update()
    {
        if (gameUp) WinCheck();
        HandleDoublePointTimers();
    }

    public void AddScoreToPlayer1(int scoreToAdd)
    {
        player1ScoreUI.AddPoints(scoreToAdd * GetFinalMultiplier());
    }
    public void AddScoreToPlayer2(int scoreToAdd)
    {
        player2ScoreUI.AddPoints(scoreToAdd * GetFinalMultiplier());
    }

    private void WinCheck()
    {
        if (player2ScoreUI.score >= 100)
        {
            if (player2Controller.gameObject.TryGetComponent<PaddleInput>(out PaddleInput p2Input))
            {
                p2Input.enabled = false;
            }
            player1ScoreUI.gameObject.SetActive(false);
            player1Controller.Lose();
            gameUp = false;
        } else if (player1ScoreUI.score >= 100)
        {
            if (player1Controller.gameObject.TryGetComponent<PaddleInput>(out PaddleInput p1Input))
            {
                p1Input.enabled = false;
            }
            player2ScoreUI.gameObject.SetActive(false);
            player2Controller.Lose();
            gameUp = false;
        }

        if (!gameUp) StopGame();
    }

    private void TogglePause(InputAction.CallbackContext context)
    {
        if (!gameUp) return;

        if (!paused)
        {
            SetPause(true);
        } else
        {
            SetPause(false);
        }
    }

    public void SetPause(bool pause)
    {
        paused = pause;
        mobileControls.SetActive(!pause);
        allUI.SetActive(!pause);
        pauseMenu.SetActive(pause);
        Time.timeScale = pause ? 0.0f : 1.0f;
    }

    IEnumerator StartGameRoutine()
    {
        int counter = 3;

        while (counter > 0)
        {
            counterText.text = counter.ToString();
            counter--;
            yield return new WaitForSeconds(1.0f);
        }

        counterText.text = "";

        GameObject ball = Instantiate(ballPrefab, Vector2.zero, Quaternion.identity);
        ballController = ball.GetComponent<BallController>();
        WriteMultiplierText();
        player1ScoreUI.WriteScore();
        player2ScoreUI.WriteScore();
        p1ControlSet.SetActiveControls(GameSettings.instance.p1ControlType);
        p2ControlSet.SetActiveControls(GameSettings.instance.p2ControlType);
        pauseButton.SetActive(true);
        gameUp = true;
    }

    private void WriteMultiplierText()
    {
        int finalMultiplier = GetFinalMultiplier();
        multiplierText.text = "x" + finalMultiplier.ToString();
        multiplierText2.text = "x" + finalMultiplier.ToString();
    }

    private int GetFinalMultiplier()
    {
        return pointMultiplier * (doublePointStacks.Count > 0 ? (doublePointStacks.Count * 2) : 1);
    }

    IEnumerator MultiplierIncreaseRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenMultiplierIncreases);
            pointMultiplier++;
            WriteMultiplierText();
            audioSource.PlayOneShot(multiplierUpSound);
            StartCoroutine(MultiplierTextIncreaseEffectRoutine());
        }
    }

    IEnumerator MultiplierTextIncreaseEffectRoutine()
    {
        multiplierTextEffect.text = multiplierText.text;
        multiplierTextEffect2.text = multiplierText.text;

        float duration = 0.8f;
        float time = 0.0f;

        float fontSizeMultiplier = 3.0f;
        float startingFontSixe = multiplierText.fontSize;
        float endingFontSixe = multiplierText.fontSize * fontSizeMultiplier;

        Color startingColor = multiplierText.color;
        Color endingColor = new Color(multiplierText.color.r, multiplierText.color.g, multiplierText.color.b, 0.0f);

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            multiplierTextEffect.fontSize = Mathf.Lerp(startingFontSixe, endingFontSixe, t);
            multiplierTextEffect.color = Color.Lerp(startingColor, endingColor, t);
            multiplierTextEffect2.fontSize = Mathf.Lerp(startingFontSixe, endingFontSixe, t);
            multiplierTextEffect2.color = Color.Lerp(startingColor, endingColor, t);

            yield return null;
        }

        multiplierTextEffect.fontSize = startingFontSixe;
        multiplierTextEffect.color = startingColor;
        multiplierTextEffect.text = "";

        multiplierTextEffect2.fontSize = startingFontSixe;
        multiplierTextEffect2.color = startingColor;
        multiplierTextEffect2.text = "";
    }

    public void ApplyDoublePointsForSeconds(float seconds)
    {
        doublePointStacks.Add(seconds);
        multiplierText.color = Color.green;
        multiplierText2.color = Color.green;
        WriteMultiplierText();
        StartCoroutine(MultiplierTextIncreaseEffectRoutine());
    }

    public void ResetDoublePoints()
    {
        doublePointStacks.Clear();
        multiplierText.color = Color.white;
        multiplierText2.color = Color.white;
        if (gameUp) WriteMultiplierText();
    }

    private void HandleDoublePointTimers()
    {
        for (int i = 0; i < doublePointStacks.Count; i++)
        {
            doublePointStacks[i] -= Time.deltaTime;
            if (doublePointStacks[i] <= 0.0f)
            {
                doublePointStacks.RemoveAt(i);
                if (doublePointStacks.Count <= 0)
                {
                    ResetDoublePoints();
                }
            }
        }
    }

    public PlayerInput GetPlayerInput()
    {
        return playerInput;
    }

    private void StopGame()
    {
        ballController.gameObject.SetActive(false);
        CameraShake cmrShk = FindFirstObjectByType<CameraShake>();
        if (cmrShk != null) cmrShk.StartShake(0.1f, 0.5f);
        PowerBoxGenerator powerBoxGenerator = FindFirstObjectByType<PowerBoxGenerator>();
        if (powerBoxGenerator != null) powerBoxGenerator.gameObject.SetActive(false);
        audioSource.PlayOneShot(explosionSound);
        audioSource.PlayOneShot(winSound, 2.0f);
        multiplierText.text = "";
        multiplierText2.text = "";
        mobileControls.SetActive(false);
        StopAllCoroutines();
        Invoke(nameof(RedirectToMenu), 2.0f);
    }

    public void RedirectToMenu()
    {
        Time.timeScale = 1.0f;
        SceneTransitionManager.Instance.ChangeScene("Menu");
    }

}
