using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManagerController : MonoBehaviour
{
    // Singleton
    private static GameManagerController instance;
    public static GameManagerController Instance { get { return instance; } }

    [Header("Actors")]
    [SerializeField] PlayerController player1Controller;
    [SerializeField] PlayerController player2Controller;
    [SerializeField] GameObject ballPrefab;
    private BallController ballController;



    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private TextMeshProUGUI player1ScoreText;
    [SerializeField] private TextMeshProUGUI player2ScoreText;
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private TextMeshProUGUI multiplierTextEffect;
    [SerializeField] private TextMeshProUGUI multiplierText2;
    [SerializeField] private TextMeshProUGUI multiplierTextEffect2;
    [SerializeField] private GameObject mobileControls;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject allUI;

    [Header("Config")]
    [SerializeField] private float timeBetweenMultiplierIncreases = 30.0f;
    [SerializeField] private int pointMultiplier = 1;

    [Header("Sounds")]
    [SerializeField] private AudioClip multiplierUpSound;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip winSound;
    

    // Game Variables
    private int player1Score = 98;
    private int player2Score = 98;

    private int doublePointsStacks = 0;

    private bool paused = false;

    private bool gameUp = false;

    // Components
    private AudioSource audioSource;

    event Action onPlayerScoreChanged;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }

        audioSource = gameObject.GetComponent<AudioSource>();
    }

    void Start()
    {
        gameUp = true;
        onPlayerScoreChanged += OnScoreChanged;
        playerInput.actions["Pause"].performed += TogglePause;
        StartCoroutine(StartGameRoutine());
        StartCoroutine(MultiplierIncreaseRoutine());
    }

    public void AddScoreToPlayer1(int scoreToAdd)
    {
        player1Score += scoreToAdd * GetFinalMultiplier();
        onPlayerScoreChanged.Invoke();
    }
    public void AddScoreToPlayer2(int scoreToAdd)
    {
        player2Score += scoreToAdd * GetFinalMultiplier();
        onPlayerScoreChanged.Invoke();
    }

    private void OnScoreChanged()
    {
        WinCheck();
        if (gameUp) WriteScores();
    }

    public void WriteScores()
    {
        player1ScoreText.text = player1Score.ToString();
        player2ScoreText.text = player2Score.ToString();
    }

    private void WinCheck()
    {
        if (player2Score >= 100)
        {
            if (player2Controller.gameObject.TryGetComponent<PaddleInput>(out PaddleInput p2Input))
            {
                p2Input.enabled = false;
            }
            player2ScoreText.text = "100";
            player1ScoreText.text = "";
            player1Controller.Lose();
            gameUp = false;
        } else if (player1Score >= 100)
        {
            if (player1Controller.gameObject.TryGetComponent<PaddleInput>(out PaddleInput p1Input))
            {
                p1Input.enabled = false;
            }
            player1ScoreText.text = "100";
            player2ScoreText.text = "";
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

        WriteScores();
        WriteMultiplierText();
    }

    private void WriteMultiplierText()
    {
        int finalMultiplier = GetFinalMultiplier();
        multiplierText.text = "x" + finalMultiplier.ToString();
        multiplierText2.text = "x" + finalMultiplier.ToString();
    }

    private int GetFinalMultiplier()
    {
        return pointMultiplier * (doublePointsStacks > 0 ? doublePointsStacks * 2 : 1);
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
        StartCoroutine(DoublePointsForSecondsRoutine(seconds));
    }

    IEnumerator DoublePointsForSecondsRoutine(float seconds)
    {
        doublePointsStacks++;
        multiplierText.color = Color.green;
        multiplierText2.color = Color.green;
        WriteMultiplierText();
        StartCoroutine(MultiplierTextIncreaseEffectRoutine());
        yield return new WaitForSeconds(seconds);
        if (doublePointsStacks > 0) doublePointsStacks--;
        if (doublePointsStacks <= 0)
        {
            multiplierText.color = Color.white;
            multiplierText2.color = Color.white;
            WriteMultiplierText(); 
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

    private void RedirectToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

}
