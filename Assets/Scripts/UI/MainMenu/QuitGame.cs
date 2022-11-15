using UnityEngine;

namespace UI.MainMenu
{
    public class QuitGame : MonoBehaviour
    {
        public void Quit()
        {
            Debug.Log("Quitting game");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
# endif
            Application.Quit();
        }
    }
}