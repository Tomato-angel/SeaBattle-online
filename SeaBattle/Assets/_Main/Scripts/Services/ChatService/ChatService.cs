using UnityEngine;
using System;
using Mirror;
using System.Text.RegularExpressions;

/// <summary>
/// Синглтон-класс для реализации механики чата
/// </summary>
public class ChatService
{
    
    /* Устаревший синглтон за счёт DIContainer
    #region [ Реализация синглтона ]
    static private ChatService _instance;
    static public ChatService Instance
    {
        get
        {
            if (_instance == null) _instance = new ChatService();
            return _instance;
        }
    }
    #endregion
    */
    

    private ChatData _chatData;
    

    [TargetRpc]
    public void TargetGetMessage(NetworkConnectionToClient target, Message message)
    {
        _chatData.Push(message);
    }

    [Command]
    public void CmdSendMessage(Player target, Message message)
    {
        _chatData.Push(message);

        NetworkIdentity opponentIdentity = target.NetworkIdentity;
        TargetGetMessage(opponentIdentity.connectionToClient, message);
    }

    [Client]
    public void StartChat()
    {
        _chatData.Clear();
    }

    [Client]
    public void StopChat()
    {
        _chatData.Clear();
    }


    public ChatService() 
    {
        _chatData = new ChatData();
    }
}
                                 