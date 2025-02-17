using UnityEngine;
using System;
using Mirror;

/// <summary>
/// Синглтон-класс для реализации механики чата
/// </summary>
public class ChatService : NetworkService
{
    #region [ Реализация синглтона ]
    static private ChatService _instance;
    static public ChatService Instance 
    {
        get
        {
            if(_instance == null)
            {
                throw new Exception(" <color=red> No instance chat in main Scene ! </color>");
            }
            return _instance;
        }
    }
    #endregion

    [SyncVar]
    private SyncList<string> _messages = new SyncList<string>();

    public void AddMessage(string message)
    {
        _messages.Add(message);

        string logMessage = string.Empty;

        foreach(var tmpMessage in _messages)
        {
            logMessage += tmpMessage + '\n';
        }

        Debug.Log(logMessage);
    }

    public void ClearMessages()
    {
        _messages.Clear();
    }

    private void Start()
    {
        if(_instance == null) _instance = this;
        
    }
    private void Update()
    {
        
    }


}
