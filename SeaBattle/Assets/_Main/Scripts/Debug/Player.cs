using Mirror;
using UnityEngine;

using Matchmaking;
using Match = Matchmaking.Match;

[RequireComponent(typeof(NetworkMatch))]
[RequireComponent(typeof(NetworkIdentity))]

public class Player : NetworkBehaviour
{
    // Добавить Player State для отслеживания состояния игрока в целом

    [SerializeField] string _nickName;

    #region [Объявление Mirror - компонентов]
    private NetworkMatch _networkMatch;
    public NetworkMatch NetworkMatch { get => _networkMatch; }

    private NetworkIdentity _networkIdentity;
    public NetworkIdentity NetworkIdentity { get => _networkIdentity; }
    #endregion

    private Match _currentMatch;
    public Match CurrentMatch { get => _currentMatch; set => _currentMatch = value; }

    
    public void Awake()
    {
        _networkMatch = GetComponent<NetworkMatch>();
        _networkIdentity = GetComponent<NetworkIdentity>();
    }

    public void Start() 
    {
        // CLIENT
        if(isClient)
        {
            CmdSearchGame();
            CmdLeaveGame();
            CmdHostGame();
            CmdLeaveGame();
            CmdJoinGame("XXXXxxxx");
        }

        // SERVER
        if(isServer)
        {

        }
    }

    public void Update()
    {
        // CLIENT
        if (isClient)
        {

        }

        // SERVER
        if (isServer)
        {

        }
    }

    #region ["Player" на стороне клиента]

    [TargetRpc]
    public void TargetViewMatchInfo(NetworkConnectionToClient target, string matchInfo)
    {
        if(matchInfo == null) 
            Debug.Log("No information about match");
        else
            Debug.Log(matchInfo);
    }

    #endregion

    #region ["Player" на стороне сервера]

    [Command]
    public void CmdSearchGame()
    {
        MatchMaker.Instance.SearchGame(this);
        TargetViewMatchInfo(connectionToClient, _currentMatch?.ToString());
    }

    [Command]
    public void CmdHostGame()
    {
        MatchMaker.Instance.HostGame(this);
        TargetViewMatchInfo(connectionToClient, _currentMatch?.ToString());
    }

    [Command]
    public void CmdJoinGame(string key)
    {
        MatchMaker.Instance.JoinGame(this, key);
        TargetViewMatchInfo(connectionToClient, _currentMatch?.ToString());
    }

    [Command]
    public void CmdLeaveGame()
    {
        MatchMaker.Instance.LeaveGame(this, _currentMatch.Key);
        TargetViewMatchInfo(connectionToClient, _currentMatch?.ToString());
    }

    #endregion
}
