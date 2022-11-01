using UnityEngine;

namespace UI.MainMenu
{
    public class QuitGame : MonoBehaviour
    {
        public void Quit()
        {
            Debug.Log("Quitting game");
            Application.Quit();
        }
    }
}