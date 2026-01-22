using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Config")]
    [SerializeField] private float timeBetweenMultiplierIncreases = 30.0f;
    [SerializeField] private int pointMultiplier = 1;

    [Header("Sounds")]
    [SerializeField] private AudioClip multiplierUpSound;
    

    // Game Variables
    private int player1Score = 0;
    private int player2Score = 0;

    private int doublePointsStacks = 0;


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
        onPlayerScoreChanged += WriteScores;
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

    public void WriteScores()
    {
        player1ScoreText.text = player1Score.ToString();
        player2ScoreText.text = player2Score.ToString();
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

            yield return null;
        }

        multiplierTextEffect.fontSize = startingFontSixe;
        multiplierTextEffect.color = startingColor;
        multiplierTextEffect.text = "";
    }

    public void ApplyDoublePointsForSeconds(float seconds)
    {
        StartCoroutine(DoublePointsForSecondsRoutine(seconds));
    }

    IEnumerator DoublePointsForSecondsRoutine(float seconds)
    {
        doublePointsStacks++;
        multiplierText.color = Color.green;
        WriteMultiplierText();
        StartCoroutine(MultiplierTextIncreaseEffectRoutine());
        yield return new WaitForSeconds(seconds);
        if (doublePointsStacks > 0) doublePointsStacks--;
        if (doublePointsStacks <= 0)
        {
            multiplierText.color = Color.white;
            WriteMultiplierText(); 
        }

    }

    public PlayerInput GetPlayerInput()
    {
        return playerInput;
    }

}
