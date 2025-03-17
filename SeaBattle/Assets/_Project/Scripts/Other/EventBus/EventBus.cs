using System;
using UnityEngine;
using Scripts.Matchmaking;
using Match = Scripts.Matchmaking.Match;

public static class EventBus
{
    #region [Events for Game Scene Manager]

    /*
    public static event Action<PlayerData> registrationApproved;
    public static void OnRegistrationApproved(PlayerData playerData)
    {
        registrationApproved?.Invoke(playerData);
    }*/

    public static event Action requestForRegistration;
    public static void OnRequestForRegistration()
    {
        requestForRegistration?.Invoke();
    }
    public static event Action requestForStartGameplay;
    public static void OnRequestForStartGameplay()
    {
        requestForStartGameplay?.Invoke();
    }
    public static event Action requestForOpenMainMenu;
    public static void OnRequestForOpenMainMenu()
    {
        requestForOpenMainMenu?.Invoke();
    }

    #endregion


    #region [ Под вопросом существования ]
    public static event Action<int> Test;
    public static void OnTest(int value)
    {
        Test?.Invoke(value);
    }

    public static event Action AnotherPlayerConnectedToMatch;
    public static event Action CurrentPlayerConnectedToMatch;
    public static event Action CurrentPlayerDisconnectedFromMatch;
    public static event Action AnotherPlayerDisconnectedFromMatch;
    public static event Action CurrentPlayerUpdateReadyStatus;
    public static event Action AnotherPlayerUpdateReadyStatus;

    

    public static event Action<string> StartHostGame;
    public static void OnHostGame(string publicMatchKey)
    {
        StartHostGame?.Invoke(publicMatchKey);
        CurrentPlayerConnectedToMatch?.Invoke();
    }
    public static event Action<string> StartJoinGame;
    public static void OnJoinGame(string publicMatchKey)
    {
        StartJoinGame?.Invoke(publicMatchKey);
        CurrentPlayerConnectedToMatch?.Invoke();
    }
    public static void OnLeaveGame(bool isLeaveGameSuccessfully)
    {
        CurrentPlayerDisconnectedFromMatch?.Invoke();
    }
    public static void OnUpdateReadyStatus()
    {
        CurrentPlayerUpdateReadyStatus?.Invoke();
    }
    public static void OnNotifyPlayerConnectionToMatch()
    {
        AnotherPlayerConnectedToMatch?.Invoke();
    }
    public static void OnNotifyPlayerDisconnectionFromMatch()
    {
        AnotherPlayerDisconnectedFromMatch?.Invoke();
    }
    public static void OnNotifyPlayerUpdateReadyStatus()
    {
        AnotherPlayerUpdateReadyStatus?.Invoke();
    }

    public static event Action<bool> AllPlayersReadyToStartTheGame;
    public static void OnAllPlayersReadyToStartTheGame(bool isAllReady)
    {
        AllPlayersReadyToStartTheGame?.Invoke(isAllReady);
    }
    #endregion


    #region [Events for MatchMaking]

    public static event Action<Player> playerInstantiateOnScene;
    public static void OnPlayerInstantiateOnScene(Player player)
    {
        playerInstantiateOnScene?.Invoke(player);
    }
    public static event Action<Player> playerDestroyFromScene;
    public static void OnPlayerDestroyFromScene(Player player)
    {
        playerDestroyFromScene?.Invoke(player);
    }
    public static event Action<Player> playerConnectedToGame;
    public static void OnPlayerConnectedToGame(Player player)
    {
        playerConnectedToGame?.Invoke(player);
    }
    public static event Action<Player> playerDisconnectedFromGame;
    public static void OnPlayerDisconnectedFromGame(Player player)
    {
        playerDisconnectedFromGame?.Invoke(player);
    }

    public static event Action<Player> playerUpdatedProperties;
    public static void OnPlayerUpdateProperties(Player player)
    {
        playerUpdatedProperties?.Invoke(player);
    }
    public static event Action allPlayersReadyToStart;
    public static void OnAllPlayersReadyToStart()
    {
        allPlayersReadyToStart?.Invoke();
    }

    #endregion
}
