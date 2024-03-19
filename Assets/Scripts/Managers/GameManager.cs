using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MAIN,
    PLAY,
    PAUSE,
    GAMEOVER,
    WIN,
}

public class GameManager : MonoBehaviour
{
    #region Variables
    public static GameManager Instance;

    public event Action<GameState> OnGameStateChanged;
    private GameState _currentGameState = GameState.MAIN;

    public GameState CurrentGameState => _currentGameState;
    #endregion



    #region Unity Methods
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        Time.timeScale = 0;
    }
    #endregion



    #region Custom Methods
    public void ChangeGameState(GameState newState)
    {
        if (newState == GameState.PAUSE && _currentGameState != GameState.PLAY) return;

        if (_currentGameState == newState) return;

        _currentGameState = newState;
        HandleStateChange(newState);
    }

    private void HandleStateChange(GameState newState)
    {
        switch (newState)
        {
            case GameState.MAIN:
                Time.timeScale = 0;
                break;
            case GameState.PLAY:
                Time.timeScale = 1;
                break;
            case GameState.PAUSE:
                Time.timeScale = 0;
                break;
            case GameState.GAMEOVER:
                Time.timeScale = 0;
                break;
            case GameState.WIN:
                Time.timeScale = 0;
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    public void InitializeGame()
    {
        ChangeGameState(GameState.MAIN);
    }

    public void StartGame()
    {
        ChangeGameState(GameState.PLAY);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("TAINK");
        InitializeGame();
    }

    public void GameOver()
    {
        ChangeGameState(GameState.GAMEOVER);
    }

    public void WinGame()
    {
        ChangeGameState(GameState.WIN);
    }
    #endregion
}