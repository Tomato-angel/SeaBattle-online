using Mirror;
using System;
using System.Text.RegularExpressions;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(NetworkMatch))]
[RequireComponent(typeof(NetworkIdentity))]
public abstract class NetworkService: NetworkBehaviour
{
    private Player _currentPlayer;
    public Player CurrentPlayer { get => _currentPlayer; }
    private Match _currentMatch;
    public Match CurrentMatch => throw new NotImplementedException();

    public void ConnectToMatch(Match match)
    {
        throw new NotImplementedException();
    }

    public void DisconnectFromMatch()
    {
        throw new NotImplementedException();
    }
}
