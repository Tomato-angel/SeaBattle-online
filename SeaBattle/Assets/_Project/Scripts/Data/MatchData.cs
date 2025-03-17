using Mirror;
using Scripts.Matchmaking;
using System;
using UnityEngine;


[Serializable]
public class MatchData
{
    
    [ReadOnly] public bool isFull;
    public bool isOpen;
    public string publicKey;
    public Guid privateID;
    public int playersCount;


    public MatchData()
    {
        isFull = false;
        isOpen = true;
        publicKey = string.Empty;
        privateID = Guid.Empty;
        playersCount = 0;
    }

    public MatchData(Match match)
    {
        isFull = match.IsFull;
        isOpen = match.IsOpen;
        publicKey = match.Key;
        privateID = match.ID;
        playersCount = match.PlayersCount;
    }
}

