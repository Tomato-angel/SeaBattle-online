using Mirror;
using UnityEngine;

using Scripts.Matchmaking;
using System.Threading.Tasks;



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

    [SyncVar]
    private Match _currentMatch;
    public Match CurrentMatch { get => _currentMatch; set => _currentMatch = value; }

    
    public void Awake()
    {
        _networkMatch = GetComponent<NetworkMatch>();
        _networkIdentity = GetComponent<NetworkIdentity>();
    }

    public async void Start() 
    {
        // CLIENT
        if (isClient)
        {
            //LogMaster.Instance.Test();
            //CmdSearchMatch();
            //CmdLeaveMatch();
            //CmdHostMatch();
            //CmdLeaveMatch();
            //CmdJoinMatch("Xxxxxxxx");
        }

        // SERVER
        if (isServer)
        {

        }
    }

    public async void Update()
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
    public void CmdSearchMatch()
    {
        MatchMaker.Instance.SearchMatch(this, (isSuccessfullySearch) => {
            TargetViewMatchInfo(connectionToClient, _currentMatch?.ToString());
        });
        
    }

    [Command]
    public void CmdHostMatch()
    {
        MatchMaker.Instance.HostMatchAsync(this, (isSuccessfullySearch) => {
            TargetViewMatchInfo(connectionToClient, _currentMatch?.ToString());
        });

    }

    [Command]
    public void CmdJoinMatch(string key)
    {
        MatchMaker.Instance.JoinMatch(this, key, (isSuccessfullySearch) => {
            TargetViewMatchInfo(connectionToClient, _currentMatch?.ToString());
        });

    }

    [Command]
    public void CmdLeaveMatch()
    {
        MatchMaker.Instance.LeaveMatch(this, _currentMatch.Key, (isSuccessfullySearch) => {
            TargetViewMatchInfo(connectionToClient, _currentMatch?.ToString());
        });

    }

    #endregion
}
