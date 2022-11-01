using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Utility
{
    public class Screenshot : MonoBehaviour
    {
        private void LateUpdate()
        {
            if (Keyboard.current.f12Key.wasPressedThisFrame) StartCoroutine(TakeScreenshot());
        }

        private static string ScreenShotName()
        {
            string date = DateTime.Now.ToString("u");
            date = date.Replace("/", "-");
            date = date.Replace(" ", "_");
            date = date.Replace(":", "-");

            return string.Format("{0}/{1}/screen_{2:u}.png",
                Application.dataPath,
                "Resources\\Textures\\Screenshots",
                date);
        }

        private IEnumerator TakeScreenshot()
        {
            yield return new WaitForEndOfFrame();

            string filename = ScreenShotName();
            ScreenCapture.CaptureScreenshot(filename);
            Debug.Log($"Saved screenshot as: {filename}");
        }
    }
}