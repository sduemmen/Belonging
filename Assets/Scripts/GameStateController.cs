using SaveSystem;
using UnityEngine;

public class GameStateController : MonoBehaviour, IDisplayContext
{
    private static GameStateController _instance;
    public static GameStateController Instance {
        get {
            if (_instance == null) _instance = (GameStateController) FindObjectOfType(typeof(GameStateController));
            return _instance;
        }
    }

    [SerializeField] private GameObject _uiPauseMenuTarget;
    private bool _displayContextActive;
    private bool _gamePaused;
    public bool DisplayContextActive => _displayContextActive;
    public bool GamePaused => _gamePaused;

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
        _gamePaused = true;
        _displayContextActive = true;
        _uiPauseMenuTarget.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideDisplayContext()
    {
        _gamePaused = false;
        _displayContextActive = false;
        _uiPauseMenuTarget.SetActive(false);
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}