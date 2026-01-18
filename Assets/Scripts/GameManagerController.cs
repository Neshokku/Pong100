using System;
using System.Collections;
using TMPro;
using UnityEngine;

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
    [SerializeField] private TextMeshProUGUI player1ScoreText;
    [SerializeField] private TextMeshProUGUI player2ScoreText;
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private TextMeshProUGUI multiplierText;

    [Header("Config")]
    [SerializeField] private float timeBetweenMultiplierIncreases = 30.0f;
    [SerializeField] private int pointMultiplier = 1;

    // Game Variables
    private int player1Score = 0;
    private int player2Score = 0;
    

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
    }

    void Start()
    {
        onPlayerScoreChanged += WriteScores;
        multiplierText.text = "x" + pointMultiplier.ToString();
        StartCoroutine(StartGameRoutine());
        StartCoroutine(MultiplierIncreaseRoutine());
    }

    public void AddScoreToPlayer1(int scoreToAdd)
    {
        player1Score += scoreToAdd * pointMultiplier;
        onPlayerScoreChanged.Invoke();
    }
    public void AddScoreToPlayer2(int scoreToAdd)
    {
        player2Score += scoreToAdd * pointMultiplier;
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
    }

    IEnumerator MultiplierIncreaseRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenMultiplierIncreases);
            pointMultiplier++;
            multiplierText.text = "x" + pointMultiplier.ToString();
        }
    }


}
