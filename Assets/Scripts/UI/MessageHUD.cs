using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class MessageHUD : MonoBehaviour
    {
        public enum MsgType
        {
            Info,
            Unlock,
            Error,
        }

        public enum MsgPosition
        {
            TopLeft,
            BottomLeft,
            BottomRight,
            Center,
        }

        public class MsgData
        {
            public MsgType m_msgType;

            public MsgPosition m_msgPosition;

            public float m_displayTime;
            
            public float m_defaultDisplayTime;

            public string m_msgText;

            public bool m_allowsStacking;

            public int m_stackAmount = 1;

            public bool m_allowsDuplicateQueueing;

            public MsgData(MsgType type, MsgPosition position, float displayTime, string msgText, bool allowStacking, bool allowDuplicateQueueing)
            {
                m_msgType = type;
                m_msgPosition = position;
                m_displayTime = displayTime;
                m_defaultDisplayTime = displayTime;
                m_msgText = msgText;
                m_allowsStacking = allowStacking;
                m_allowsDuplicateQueueing = allowDuplicateQueueing;
            }

            public static bool CheckEquality(MsgData msgData, MsgData other)
            {
                return msgData.m_msgType == other.m_msgType && 
                       msgData.m_msgPosition == other.m_msgPosition &&
                       Mathf.Abs(msgData.m_displayTime - other.m_displayTime) < 0.01f &&
                       msgData.m_msgText == other.m_msgText &&
                       msgData.m_allowsStacking == other.m_allowsStacking && 
                       msgData.m_allowsDuplicateQueueing == other.m_allowsDuplicateQueueing;
            }
        }

        public class MessageQueue
        {
            private List<MsgData> m_msgDataQueue;

            public int Count => m_msgDataQueue.Count;

            public MessageQueue()
            {
                m_msgDataQueue = new List<MsgData>();
            }
            
            public void Enqueue(MsgData msgData)
            {
                if (m_msgDataQueue.Count > 10)
                {
                    Debug.LogWarning("MessageQueue contains more than 10 elements. Skipped enqueue");
                    return;
                }

                if (this.Contains(msgData, out int foundIndex))
                {
                    if (!msgData.m_allowsDuplicateQueueing && !msgData.m_allowsStacking)
                    {
                        return;
                    }

                    if (msgData.m_allowsStacking)
                    {
                        m_msgDataQueue[foundIndex].m_stackAmount++;
                    }
                    else if (msgData.m_allowsDuplicateQueueing)
                    {
                        m_msgDataQueue.Add(msgData);
                    }
                }
                else
                {
                    m_msgDataQueue.Add(msgData);
                }
            }

            public MsgData Dequeue()
            {
                MsgData msgData = null;
                if (m_msgDataQueue.Count > 0)
                {
                    msgData = m_msgDataQueue[0];
                    m_msgDataQueue.RemoveAt(0);
                }
                else
                {
                    // Debug.LogWarning("MessageQueue doesn't contain any elements");
                }
                return msgData;
            }

            public bool Contains(MsgData msgData)
            {
                for (int i = 0; i < m_msgDataQueue.Count; i++)
                {
                    if (MsgData.CheckEquality(m_msgDataQueue[i], msgData))
                    {
                        return true;
                    }
                }

                return false;
            }
            
            public bool Contains(MsgData msgData, out int index)
            {
                index = -1;

                for (int i = 0; i < m_msgDataQueue.Count; i++)
                {
                    if (MsgData.CheckEquality(m_msgDataQueue[i], msgData))
                    {
                        index = i;
                        return true;
                    }
                }

                return false;
            }
        }

        [Serializable]
        public class MessageDisplay
        {
            public MessageQueue m_queue;

            public TextMeshProUGUI m_text;

            public MsgData m_currentMsgData;

            public MessageDisplay()
            {
                m_queue = new MessageQueue();
                m_currentMsgData = null;
            }
        }
        
        
        private static MessageHUD _instance;
        public static MessageHUD Instance {
            get {
                if (_instance == null)
                {
                    _instance = (MessageHUD) FindObjectOfType(typeof(MessageHUD));
                }

                return _instance;
            }
        }

        public MessageDisplay m_topLeftDisplay = new MessageDisplay();

        public MessageDisplay m_bottomLeftDisplay = new MessageDisplay();

        public MessageDisplay m_bottomRightDisplay = new MessageDisplay();

        public MessageDisplay m_centerDisplay = new MessageDisplay();

        private List<MessageDisplay> m_messageDisplays;

        public static Color unlockColor = new Color(.9f, .8f, .4f);
        
        public static Color errorColor = new Color(.8f, .4f, .4f);
        
        public static Color defaultColor = new Color(1f, 1f, 1f);
        
        [SerializeField] private Hint _hintPrefab;
       
        private List<Hint> _activeHints;

        private void Awake()
        {
            m_messageDisplays = new List<MessageDisplay> { m_topLeftDisplay, m_bottomLeftDisplay, m_bottomRightDisplay, m_centerDisplay };
        }

        public void Update()
        {
            foreach (MessageDisplay messageDisplay in m_messageDisplays)
            {
                if (messageDisplay.m_currentMsgData != null)
                {
                    messageDisplay.m_currentMsgData.m_displayTime -= Time.unscaledDeltaTime;

                    if (messageDisplay.m_currentMsgData.m_displayTime <= 0)
                    {
                        messageDisplay.m_currentMsgData = null;
                    }
                }
                else
                {
                    MsgData msgData = messageDisplay.m_queue.Dequeue();

                    if (msgData != null)
                    {
                        messageDisplay.m_currentMsgData = msgData;
                        string stack = msgData.m_allowsStacking ? $" {msgData.m_stackAmount}x" : "";
                        messageDisplay.m_text.text = msgData.m_msgText + stack;
                        messageDisplay.m_text.color = MessageColorFromType(msgData.m_msgType);
                        messageDisplay.m_text.canvasRenderer.SetAlpha(1f);
                        messageDisplay.m_text.CrossFadeAlpha(0f, msgData.m_displayTime, true);
                    }
                }
            }
        }
        
        public void AddMessage(MsgData msgData)
        {
            switch (msgData.m_msgPosition)
            {
                case MsgPosition.TopLeft:
                    m_topLeftDisplay.m_queue.Enqueue(msgData);
                    break;
                case MsgPosition.BottomLeft:
                    m_bottomLeftDisplay.m_queue.Enqueue(msgData);
                    break;
                case MsgPosition.BottomRight:
                    m_bottomRightDisplay.m_queue.Enqueue(msgData);
                    break;
                case MsgPosition.Center:
                    m_centerDisplay.m_queue.Enqueue(msgData);
                    break;
            }
        }

        public void AddMessage(string message)
        {
            m_centerDisplay.m_queue.Enqueue(new MsgData(MsgType.Info, MsgPosition.Center, 5f, message, false, false));
        }

        public static Color MessageColorFromType(MsgType type)
        {
            switch (type)
            {
                case MsgType.Info:
                    return defaultColor;
                case MsgType.Error:
                    return errorColor;
                case MsgType.Unlock:
                    return unlockColor;
                default:
                    return Color.clear;
            }
        }
    }
}
