using UnityEngine;

public class GameStateController : MonoBehaviour, IDisplayContext
{
    private static GameStateController instance;
    public static GameStateController Instance {
        get {
            if (instance == null)
            {
                instance = (GameStateController) FindObjectOfType(typeof(GameStateController));
            }
            
            return instance;
        }
    }

    [SerializeField] private GameObject _uiPauseMenuTarget;
    private bool m_displayContextActive;
    private bool m_gamePaused;
    public bool DisplayContextActive => m_displayContextActive;
    public bool GamePaused => m_gamePaused;

    private void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        if (InputSystem.GetKeyDown(InputSystem.KeyBinds.Pause_Game) && !Flags.UI_ELEMENT_OPEN && !m_displayContextActive)
        {
            PauseGame();
        }
        else if (InputSystem.GetKeyDown(InputSystem.KeyBinds.Pause_Game) && m_displayContextActive)
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        ShowDisplayContext();
    }

    public void ResumeGame()
    {
        HideDisplayContext();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
# endif
        Application.Quit();
    }
    
    public void ShowDisplayContext()
    {
        m_gamePaused = true;
        m_displayContextActive = true;
        _uiPauseMenuTarget.SetActive(true);
    }

    public void HideDisplayContext()
    {
        m_gamePaused = false;
        m_displayContextActive = false;
        _uiPauseMenuTarget.SetActive(false);
    }
}