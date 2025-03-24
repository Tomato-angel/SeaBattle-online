using UnityEngine;
using System;
using Mirror;
using System.Text.RegularExpressions;

/// <summary>
/// Синглтон-класс для реализации механики чата
/// </summary>
public class ChatService
{
    public event Action newMessageInChat;

    private ChatData _chatData;
    

    [TargetRpc]
    public void TargetGetMessage(NetworkConnectionToClient target, Message message)
    {
        Debug.Log($"New message in chat: {message}");
        _chatData.Push(message);
    }
    /*
    [Command]
    public void CmdSendMessage(Player target, Message message)
    {
        _chatData.Push(message);

        NetworkIdentity opponentIdentity = target.NetworkIdentity;
        TargetGetMessage(opponentIdentity.connectionToClient, message);
    }*/
    [Command]
    public void CmdSendMessage(Player sender, Message message)
    {
        sender.CurrentMatch.GetAnotherPlayers(sender, out Player[] anotherPlayers);
        
        foreach(var anotherPlayer in anotherPlayers)
        {
            TargetGetMessage(anotherPlayer.connectionToClient, message);
        }
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
                                 