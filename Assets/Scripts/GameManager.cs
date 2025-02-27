using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;
using Slider = UnityEngine.UI.Slider;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        DontDestroyOnLoad(gameObject); // Keeps it across scenes
    }
    [SerializeField] TextMeshProUGUI startText;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI triesText;
    [SerializeField] TextMeshProUGUI progressionText;
    [SerializeField] Slider difficulitySlider;
    [SerializeField] Button startStopButton;
    [SerializeField] TextMeshProUGUI startStopButtonText;
    [SerializeField] Button pauseUnpauseButton;
    [SerializeField] TextMeshProUGUI pauseUnpauseButtonText;
    [SerializeField] TextMeshProUGUI wonText;
    [SerializeField] TextMeshProUGUI wonTimeText;
    [SerializeField] TextMeshProUGUI wonTriesText;
    float timer = 0;
    int secondsPassed = 0;
    public int tries = 0;
    public int progression = 0;
    int pairNum = 0;
    List<GameObject> flippedCards = new();
    enum States
    {
        Stop,
        Playing,
        Pause,
        Won,
    }
    States gameState = States.Stop;
    public void Start()
    {
        InitializingTheGame();
    }
    private void Update()
    {
        if (gameState == States.Playing)
        {
            if (timer >= 1f)
            {
                timer = 0;
                secondsPassed++;
                timeText.text = TimeTextGenerator(secondsPassed, true);
            }
            timer += Time.deltaTime;
            progressionText.text = ProgressionTextGenerator(progression, pairNum, true);
            triesText.text = TriesTextGenerator(tries, true);
            if (progression == pairNum)
            {
                WonTheGame();
            }
        }
    }
    private void InitializingTheGame()
    {
        gameState = States.Stop;

        CardManager.Instance.Flippable = true;
        CardManager.Instance.CleaningCards();

        StartSection(true);
        ScoreSection(false);
        ActionSection();
        WonSection(false);
    }
    private void StartingTheGame()
    {
        gameState = States.Playing;

        CardManager.Instance.Flippable = true;
        CardManager.Instance.GameStart((int)difficulitySlider.value);
        timer = 0;
        secondsPassed = 0;
        tries = 0;
        progression = 0;
        pairNum = CardManager.Instance.cards.Count / 2;


        StartSection(false);
        ScoreSection(true);
        ActionSection();
        WonSection(false);
    }
    private void PausingTheGame()
    {
        gameState = States.Pause;

        flippedCards = new();
        foreach (GameObject card in CardManager.Instance.cards)
        {
            Card cardInstance = card.GetComponent<Card>();
            if (cardInstance.flipped)
            {
                flippedCards.Add(card);
                cardInstance.FlipTheCard();
            }
        }
        CardManager.Instance.Flippable = false;

        StartSection(false);
        ScoreSection(true);
        ActionSection();
        WonSection(false);
    }
    private void UnpausingTheGame()
    {
        gameState = States.Playing;

        foreach (GameObject card in flippedCards)
        {
            Card cardInstance = card.GetComponent<Card>();
            cardInstance.FlipTheCard();
        }
        CardManager.Instance.Flippable = true;

        StartSection(false);
        ScoreSection(true);
        ActionSection();
        WonSection(false);
    }
    private void WonTheGame()
    {
        gameState = States.Won;

        StartSection(false);
        ScoreSection(false);
        ActionSection();
        WonSection(true);
    }
    public void StartStopButtonPressed()
    {
        switch (gameState)
        {
            case States.Stop:
                StartingTheGame();
                break;
            case States.Pause:
                InitializingTheGame();
                break;
            case States.Won:
                StartingTheGame();
                break;
        }
    }
    public void PauseUnpauseButtonPressed()
    {
        switch (gameState)
        {
            case States.Playing:
                PausingTheGame();
                break;
            case States.Pause:
                UnpausingTheGame();
                break;
        }

    }
    private string TimeTextGenerator(int secondsPassed, bool breakLine)
    {
        int minutes = secondsPassed / 60;
        int seconds = secondsPassed % 60;
        string minutesString = "", secondsString = "";
        if (minutes < 10)
        {
            minutesString = "0" + minutes.ToString();
        }
        else
        {
            minutesString = minutes.ToString();
        }
        if (seconds < 10)
        {
            secondsString = "0" + seconds.ToString();
        }
        else
        {
            secondsString = seconds.ToString();
        }
        if (breakLine) return $"Time\n{minutesString}:{secondsString}";
        else return $"Time: {minutesString}:{secondsString}";
    }
    private string TriesTextGenerator(int tries, bool breakLine)
    {
        if (breakLine) return $"Tries\n{tries}";
        else return $"Tries: {tries}";
    }
    private string ProgressionTextGenerator(int progression, int pairNum, bool breakLine)
    {
        if (breakLine) return $"Progression\n{progression}/{pairNum}";
        else return $"Progression: {progression}/{pairNum}";
    }
    private void StartSection(bool active)
    {
        startText.enabled = active;
    }
    private void ScoreSection(bool active)
    {
        if (active)
        {
            timeText.enabled = true;
            triesText.enabled = true;
            progressionText.enabled = true;

            timeText.text = TimeTextGenerator(secondsPassed, true);
            progressionText.text = ProgressionTextGenerator(progression, pairNum, true);
            triesText.text = TriesTextGenerator(tries, true);
        }
        else
        {
            timeText.enabled = false;
            triesText.enabled = false;
            progressionText.enabled = false;
        }
    }
    private void WonSection(bool active)
    {
        if (active)
        {
            wonText.enabled = true;
            wonTimeText.enabled = true;
            wonTriesText.enabled = true;
            wonTimeText.text = TimeTextGenerator(secondsPassed, false);
            wonTriesText.text = TriesTextGenerator(tries, false);
        }
        else
        {
            wonText.enabled = false;
            wonTimeText.enabled = false;
            wonTriesText.enabled = false;
        }
    }
    private void ActionSection()
    {
        switch (gameState)
        {
            case States.Stop:
                difficulitySlider.interactable = true;

                startStopButton.interactable = true;
                startStopButtonText.text = "Start";

                pauseUnpauseButton.interactable = false;
                pauseUnpauseButtonText.text = "Pause";
                break;
            case States.Playing:
                difficulitySlider.interactable = false;

                startStopButton.interactable = false;
                startStopButtonText.text = "Stop";

                pauseUnpauseButton.interactable = true;
                pauseUnpauseButtonText.text = "Pause";
                break;
            case States.Pause:
                difficulitySlider.interactable = false;

                startStopButton.interactable = true;
                startStopButtonText.text = "Stop";

                pauseUnpauseButton.interactable = true;
                pauseUnpauseButtonText.text = "Unpause";
                break;
            case States.Won:
                difficulitySlider.interactable = true;

                startStopButton.interactable = true;
                startStopButtonText.text = "Start";

                pauseUnpauseButton.interactable = false;
                pauseUnpauseButtonText.text = "Pause";
                break;
        }
    }
}
