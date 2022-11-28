using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Utility
{
    public class Screenshot : MonoBehaviour
    {
        public KeyCode m_autoScreenshotKey = KeyCode.F12;
        
        public KeyCode m_screenshotKey = KeyCode.F12;

        public List<GameObject> m_objects;

        public GameObject m_currentObject;

        public int m_currentIndex;


        [Button("Clear")]
        private void Clear()
        {
            m_objects = new List<GameObject>();
            m_currentIndex = 0;
            m_currentObject = null;
        }

        private void LateUpdate()
        {
            if (Input.GetKeyDown(m_autoScreenshotKey))
            {
                if (m_currentIndex >= m_objects.Count)
                {
                    Debug.LogError("Index out of bounds");
                    return;
                }

                if (m_currentObject != null)
                {
                    m_currentObject.SetActive(false);
                }
                
                GameObject obj = m_objects[m_currentIndex];
                m_currentIndex++;
                m_currentObject = obj;
                obj.SetActive(true);
                TakeScreenshot();
            } 
            else if (Input.GetKeyDown(m_screenshotKey))
            {
                TakeScreenshot();
            }
        }

        private static string GetPath(string filename = "", bool appendTime = false)
        {
            string name = filename;

            if (appendTime)
            {
                string date = DateTime.Now.ToString("u");
                date = date.Replace("/", "-");
                date = date.Replace(" ", "_");
                date = date.Replace(":", "-");
                name += "_" + date;
            }

            return $"{Application.dataPath}/Resources/Textures/Screenshots/{name}.png";
        }

        private void TakeScreenshot()
        {
            string filename = GetPath(m_currentObject.name);
            ScreenCapture.CaptureScreenshot(filename);
            Debug.Log($"Saved screenshot as: {filename}");
        }
    }
}