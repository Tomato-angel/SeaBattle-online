using Mirror;
using Mirror.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Match
{
    public bool IsFull { get; set; }
    public bool IsOpen { get; set; }

    public string Key { get; set; }

    private Guid _id;
    public Guid ID { get => _id; }

    private List<Player> _players;
    public int PlayersCount { get => _players.Count; }
    public Player GetPlayer(int index)
    {
        try
        {
            return _players[index];
        }
        catch (Exception e)
        {
            throw e;
        }
    }
    public void AddPlayer(Player player)
    {
        _players.Add(player);
        player.NetworkMatch.matchId = _id;
    }
    public void DeletePlayer(Player player)
    {
        try
        {
            _players.Remove(player);
        }
        catch (Exception e)
        {
            throw e;
        }
    }
    public bool ContainsPlayer(Player player)
    {
        foreach(var tmpPlayer in _players)
        {
            if (tmpPlayer == player)
                return true;
        }
        return false;
    }
    public void Clear()
    {
        _players.Clear();
    }


    public event Action StartMatch;
    public void OnStartMatch() => StartMatch?.Invoke();
    public event Action EndMatch;
    public void OnEndMatch() => EndMatch?.Invoke();

    public override string ToString()
    {
        string result = string.Empty;

        result = $"Match: {{\n" +
            $"IsFull: {IsFull}\n" +
            $"IsOpen: {IsOpen}\n" +
            $"Key: {Key}\n" +
            $"}}";

        return result;
    }

    public static Match Empty { get => new Match(); }

    /*
    public Match() 
    {
        _id = Guid.NewGuid();
    }*/
    /*
    public Match(Guid matchID, bool isOpen = true) 
    {
        _id = matchID;
        IsOpen = isOpen;
        Key = key;
    } */
    public Match()
    {
        _id = new Guid();
        IsOpen = false;
        Key = string.Empty;
        _players = new List<Player>();
    }
    public Match(string key, bool isOpen)
    {
        _id = KeyGenerator.Instance.KeyToGui(key);
        IsOpen = isOpen;
        Key = key;
        _players = new List<Player>();
    }
}
