using Mirror;
using Scripts.Matchmaking;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MatchMakerService
{
    MatchMaker _matchMaker;

    

    [Client]
    public void JoinGame(Player player, string key, Action<bool> callback = null)
    {

    }
    [Command]
    public void CmdJoinGame(Player player, string key, Action<bool> callback = null)
    {

    }


    public MatchMakerService()
    {
        _matchMaker = new MatchMaker();
    }
}

