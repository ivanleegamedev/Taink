using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [Header("Menus")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject winMenu;

    [Header("Main Menu Buttons")]
    [SerializeField] private Button mm_PlayButton;
    [SerializeField] private Button mm_QuitButton;

    [Header("Pause Menu Buttons")]
    [SerializeField] private Button pm_ResumeButton;
    [SerializeField] private Button pm_RestartButton;
    [SerializeField] private Button pm_QuitButton;

    [Header("GameOver Menu Buttons")]
    [SerializeField] private Button go_RestartButton;
    [SerializeField] private Button go_QuitButton;

    [Header("Win Menu Buttons")]
    [SerializeField] private Button wm_RestartButton;
    [SerializeField] private Button wm_QuitButton;



    #region Unity Methods
    private void Start()
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

        ActivateMainMenu();
        SubscribeToEvents();
        InitializeButtons();
        GameManager.Instance.ChangeGameState(GameState.MAIN);
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    #endregion



    private void SubscribeToEvents()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        }
        else
        {
            Debug.LogWarning("GameManager.Instance is null");
        }
        //GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
    }

    private void UnsubscribeFromEvents()
    {
        GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState newState)
    {
        // Initialize menu based on game state
        switch (GameManager.Instance.CurrentGameState)
        {
            case GameState.MAIN:
                mainMenu.SetActive(true);
                break;
            case GameState.PLAY:
                break;
            case GameState.PAUSE:
                pauseMenu.SetActive(true);
                break;
            case GameState.GAMEOVER:
                gameOverMenu.SetActive(true);
                break;            
            case GameState.WIN:
                winMenu.SetActive(true);
                break;
            default:
                break;
        }
    }

    private void InitializeButtons()
    {
        mm_PlayButton.onClick.AddListener(PlayGame);
        mm_QuitButton.onClick.AddListener(QuitGame);

        pm_ResumeButton.onClick.AddListener(ResumeGame);
        pm_RestartButton.onClick.AddListener(RestartGame);
        pm_QuitButton.onClick.AddListener(QuitGame);

        go_RestartButton.onClick.AddListener(RestartGame);
        go_QuitButton.onClick.AddListener(QuitGame);
        
        wm_RestartButton.onClick.AddListener(RestartGame);
        wm_QuitButton.onClick.AddListener(QuitGame);
    }

    private void HideAllMenus()
    {
        mainMenu.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        winMenu.SetActive(false);
    }

    private void ActivateMainMenu()
    {
        mainMenu.SetActive(true);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        winMenu.SetActive(false);
    }

    private void PlayGame()
    {
        GameManager.Instance.ChangeGameState(GameState.PLAY);
        HideAllMenus();
    }

    private void RestartGame()
    {
        GameManager.Instance.RestartGame();
        ActivateMainMenu();
    }

    public void ResumeGame()
    {
        GameManager.Instance.ChangeGameState(GameState.PLAY);
        pauseMenu.SetActive(false);
    }

    private void QuitGame()
    {
        // Quit game logic
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}