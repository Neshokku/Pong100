using System;
using TMPro;
using UnityEngine;

public class GameManagerController : MonoBehaviour
{
    // Singleton
    private static GameManagerController instance;
    public static GameManagerController Instance { get { return instance; } }

    [Header("References")]
    [SerializeField] private TextMeshProUGUI player1ScoreText;
    [SerializeField] private TextMeshProUGUI player2ScoreText;


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
        WriteScores();
    }

    public void AddScoreToPlayer1(int scoreToAdd)
    {
        player1Score += scoreToAdd;
        onPlayerScoreChanged.Invoke();
    }
    public void AddScoreToPlayer2(int scoreToAdd)
    {
        player2Score += scoreToAdd;
        onPlayerScoreChanged.Invoke();
    }

    public void WriteScores()
    {
        player1ScoreText.text = player1Score.ToString();
        player2ScoreText.text = player2Score.ToString();
    }
}
