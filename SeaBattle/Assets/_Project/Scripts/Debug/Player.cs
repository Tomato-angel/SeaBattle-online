using Mirror;
using UnityEngine;

using Scripts.Matchmaking;
using System.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using Mirror.BouncyCastle.Asn1.X509;
using System.Runtime.CompilerServices;



[RequireComponent(typeof(NetworkMatch))]
[RequireComponent(typeof(NetworkIdentity))]
[Serializable]
public class Player : NetworkBehaviour, IInitializable
{
    [Header("Basic and Network Parameters of the Current Player")]
    [SyncVar]
    public PlayerData playerData;
    [Command]
    public void CmdSetPlayerData(PlayerData newPlayerData)
    {
        if (newPlayerData == null) return;
        playerData = newPlayerData;
    }

    [field: SerializeField] public NetworkMatch NetworkMatch { get; private set; }
    [field: SerializeField] public NetworkIdentity NetworkIdentity { get; private set; }
    

    #region [ Initialization ]
    public void GeneralInitialize()
    {
        DontDestroyOnLoad(gameObject);
        NetworkMatch = GetComponent<NetworkMatch>();
        NetworkIdentity = GetComponent<NetworkIdentity>();
    }
    public void LocalPlayerInitialize()
    {
        ProjectManager.root.SetLocalPlayer(this);
        ProjectManager.root.ProjectServices
            .Resolve<JsonToFileStorageService>()
            .FastLoad<PlayerData>((loadPlayerData) =>
            {
                CmdSetPlayerData(loadPlayerData);
            });
    }
    public void ClientInitialize()
    {

    }
    public void ServerInitialize()
    {

    }

    public void Initialize()
    {
        {
            GeneralInitialize();
        }
        if (isClient)
        {
            ClientInitialize();
        }
        if (isLocalPlayer)
        {
            LocalPlayerInitialize();
        }
        if (isServer)
        {
            ServerInitialize();
        }
    }
    #endregion

    #region [ Special Methods]
    const float _pingCorrection = 100;
    [Client]
    public double PingServer()
    {
        float lastMessageTime = connectionToServer.lastMessageTime;
        float nowTime = UnityEngine.Time.time;
        float ping = Mathf.Abs(lastMessageTime - nowTime);
        float correctedPing = ping + _pingCorrection;

        Debug.Log($"[Client] Try ping server:\n " +
            $"last message time: {lastMessageTime}\n " +
            $"current time: {nowTime}\n " +
            $"ping: {ping}\n" +
            $"corrected ping: {correctedPing}");
        return correctedPing;
    }
    #endregion

    #region [ Match Making ]


    public event Action startHostGame;
    public event Action stopHostGame;
    public event Action joinGame;
    public event Action kickGame;
    public event Action leaveGame;
    public event Action updateProperties;

    [Space]
    [Header("Current Player Matchmaking Options")]
    [SyncVar]
    public bool isHost = false;

    [SyncVar]
    [SerializeField] private Match _currentMatch = null;
    public Match CurrentMatch
    { get => _currentMatch; set => _currentMatch = value; }

    /*
    [SyncVar]
    [SerializeField] private MatchData _currentMatchData = null;
    public MatchData CurrentMatchData => _currentMatchData;
    */

    [SyncVar]
    [SerializeField] private bool _isReady = false;
    public bool IsReady => _isReady;

    [Command]
    public void CmdHostGame()
    {
        ProjectManager.root.ProjectServices
            .Resolve<MatchMaker>()
            .HostMatch(this, (isHostGameSuccessfully) =>
            {
                if(!isHostGameSuccessfully) return;

                isHost = true;
                TargetNotifyPlayerAboutStartHostGameAsync();

                TargetSetCurrentMatchAsync(_currentMatch);
            });
    }
    [TargetRpc]
    public async void TargetNotifyPlayerAboutStartHostGameAsync()
    {
        await Task.Delay((int) PingServer());
        Debug.Log("[Client] This player start host the game");
        startHostGame?.Invoke();
    }
    [TargetRpc]
    public async void TargetNotifyPlayerAboutStopHostGameAsync()
    {
        await Task.Delay((int)PingServer());
        Debug.Log("[Client] This player stop host the game");
        stopHostGame?.Invoke();
    }

    [Command]
    public void CmdJoinGame(string key)
    {
        ProjectManager.root.ProjectServices
            .Resolve<MatchMaker>()
            .JoinMatch(this, key, (isJoinGameSuccessfully) =>
            {
                if (!isJoinGameSuccessfully) return;

                Player[] players = _currentMatch.GetPlayers();
                foreach (Player player in players)
                {
                    player.TargetSetCurrentMatchAsync(_currentMatch);
                    player.TargetNotifyAboutConnectionAsync(this);
                }
                TargetNotifyPlayerAboutJoinGameAsync();
                TargetSetCurrentMatchAsync(_currentMatch);
            });
    }
    [TargetRpc]
    public async void TargetNotifyPlayerAboutJoinGameAsync()
    {
        await Task.Delay((int)PingServer());
        Debug.Log("[Client] This player join to game");
        joinGame?.Invoke();
    }
    /*
    // Уведомляет игрока, который подключался к матчу о результате его запроса на подключение к игре
    [TargetRpc]
    private async void TargetPlayerJoinGameResult(NetworkConnection conn, bool isJoinGameSuccessfully, string publicMatchKey)
    {
        await Task.Delay(1000);
        if(isJoinGameSuccessfully)
        {
            EventBus.OnJoinGame(publicMatchKey);
        }
        else
        {
            Debug.Log("[Player] Can't join game");
        }
    }
    // Уведомляет заданного игрока матча о том, что к матчу подключился новый мгрок
    [TargetRpc]
    private void TargetNotifyAboutNewPlayerConnection(NetworkConnection conn)
    {
        EventBus.OnNotifyPlayerConnectionToMatch();
    }
    */

    [Command] 
    public void CmdLeaveGame()
    {
        if (_currentMatch == null) return;
        Player[] players = _currentMatch.GetPlayers();

        ProjectManager.root.ProjectServices
            .Resolve<MatchMaker>()
            .LeaveMatch(this, _currentMatch.Key, (isLeaveGameSuccessfully) =>
            {
                foreach (Player player in players)
                {
                    player.TargetNotifyAboutDisconnectionAsync(this);
                    if(isHost)
                    {
                        player.TargetForcePlayerLeaveGameAsync();
                    }
                }
                isHost = false;
                TargetNotifyPlayerAboutLeaveGameAsync();

                TargetSetCurrentMatchAsync(_currentMatch);
            });
    }
    [TargetRpc]
    public async void TargetForcePlayerLeaveGameAsync()
    {
        await Task.Delay((int)PingServer());
        Debug.Log("[Client] This player must leave the game");
        CmdLeaveGame();
    }
    [TargetRpc]
    public async void TargetNotifyPlayerAboutLeaveGameAsync()
    {
        await Task.Delay((int)PingServer());
        Debug.Log("[Client] This player leave the game");
        leaveGame?.Invoke();
    }
    /*
    // Уведомляет игрока, который отключался от матча о результате его запроса на отключение от игры
    [TargetRpc]
    private void TargetPlayerLeaveGameResult(NetworkConnection conn, bool isLeaveGameSuccessfully)
    {
        EventBus.OnLeaveGame(isLeaveGameSuccessfully);
    }
    // Уведомляет заданного игрока матча о том, что от матча отключился игрок
    [TargetRpc]
    private void TargetNotifyAboutNewPlayerDisconnection(NetworkConnection conn)
    {
        EventBus.OnNotifyPlayerDisconnectionFromMatch();
    }*/

    [Command]
    public void CmdUpdateReadyStatus(bool isReady)
    {
        _isReady = isReady;
        if (_currentMatch == null) return;
        Player[] players = _currentMatch.GetPlayers();

        bool isAllPlayersReady = true;
        foreach (Player player in players)
        {
            if (!player.IsReady) isAllPlayersReady = false;
            player.TargetNotifyAboutUpdatePropertiesAsync(this);
        }

        if(isAllPlayersReady && _currentMatch.IsFull)
        {
            foreach (Player player in players)
            {
                player.TargetNotifyAboutAllPlayersAreReadyToStart();
            }
        }
    }
    [TargetRpc]
    public async void TargetNotifyAboutUpdatePropertiesAsync(Player player)
    {
        await Task.Delay((int)PingServer());
        Debug.Log("[Client] Another player update properties");
        EventBus.OnPlayerUpdateProperties(player);
    }
    [TargetRpc]
    public async void TargetNotifyAboutAllPlayersAreReadyToStart()
    {
        await Task.Delay((int)PingServer());
        Debug.Log("[Client] All players are ready to start");
        EventBus.OnAllPlayersReadyToStart();
    }

    /*
    // Уведомляет игрока, который менял статус готовности о результате
    [TargetRpc]
    private void TargetUpdateReadyStatus(NetworkConnection conn, bool isAllPlayersReady)
    {
        EventBus.OnAllPlayersReadyToStartTheGame(isAllPlayersReady);
        EventBus.OnUpdateReadyStatus();
    }
    // Уведомляет заданного игрока матча о том, что у игрока изменился статус готовности к игре
    [TargetRpc]
    private void TargetNotifyAboutPlayerUpdateReadyStatus(NetworkConnection conn, bool isAllPlayersReady)
    {
        EventBus.OnAllPlayersReadyToStartTheGame(isAllPlayersReady);
        EventBus.OnNotifyPlayerUpdateReadyStatus();
    }
    */

    
  
    [TargetRpc]
    public async void TargetSetCurrentMatchAsync(Match match)
    {
        await Task.Delay((int)PingServer());
        if (match == null) return;
        Debug.Log("[Client] This player update the current match data from locally data and(or) scene (based on server current match data)");
        _currentMatch = match;
        ForceSetPlayersFromSceneToCurrentMatch();
    }
    [Client]
    public async void ForceSetPlayersFromSceneToCurrentMatch(int millisecondsDelay = 100)
    {
        await Task.Delay(millisecondsDelay);
        if (!_currentMatch.IsAllPlayersValid())
        {
            SetPlayersOnCurrentMatch(GetPlayersFromLocalScene());
        }
    }
    [Client]
    public void SetPlayersOnCurrentMatch(Player[] players)
    {
        for (int i = 0; i < _currentMatch.PlayersCount; ++i)
        {
            _currentMatch._players[i] = players[i];
        }
    }

    [Client]
    public Player GetPlayerFromLocalScene(Player player)
    {
        Player result = null;
        Player[] playersOnLocalScene = GetPlayersFromLocalScene();
        foreach(Player playerOnLocalScene in playersOnLocalScene)
        {
            if(playerOnLocalScene == player)
            {
                result = playerOnLocalScene;
                break;
            }
        }
        return result;
    }
    [Client]
    public Player GetAnotherPlayerFromLocalScene()
    {
        foreach (Player player in GetPlayersFromLocalScene())
        { 
            if(player != this)
            {
                return player;
            }
        }
        return null;
    }
    [Client]
    public Player[] GetPlayersFromLocalScene()
    {
        GameObject[] playersObj = GameObject.FindGameObjectsWithTag("Player");
        Player[] players = new Player[playersObj.Length];
        for(int i = 0; i < players.Length; ++i)
        {
            players[i] = playersObj[i].GetComponent<Player>();
        }
        return players;
    }
    


    /// <summary>
    /// Уведомляет о подключении стороннего клиента, вызывая глобальное событие 
    /// </summary>
    [TargetRpc]
    public async void TargetNotifyAboutConnectionAsync(Player player)
    {
        await Task.Delay((int)PingServer());
        Debug.Log("New player was <color=green>connected</color>");
        EventBus.OnPlayerConnectedToGame(player);
    }
    /// <summary>
    /// Уведомляет об отключении стороннего клиента, вызывая глобальное событие 
    /// </summary>
    [TargetRpc]
    public async void TargetNotifyAboutDisconnectionAsync(Player player)
    {
        await Task.Delay((int)PingServer());
        Debug.Log("New player was <color=orange>disconnected</color>");
        EventBus.OnPlayerDisconnectedFromGame(player);
    }
    #endregion

    #region [ Chat ]
    [SyncVar]
    [SerializeField] ChatData _currentChatData;
    public ChatData CurrentChatData => _currentChatData;
    public event Action<Message> newMessageInChat;
    public event Action updateCurrentChatData;

    public void ClearChatData()
    {
        _currentChatData.Clear();
        TargetNotifyPlayerAboutUpdateChatData();
    }

    [Command]
    public void CmdSendMessageInChat(string text)
    {
        Message message = new Message(playerData.nickName, text);
        _currentChatData.Push(message);
        foreach (var player in _currentMatch.GetPlayers())
        {
            TargetNotifyPlayerAboutNewMessageInChat(message);
        }
    }
    [TargetRpc]
    public void TargetNotifyPlayerAboutUpdateChatData()
    {
        updateCurrentChatData?.Invoke();
    }
    [TargetRpc]
    public void TargetNotifyPlayerAboutNewMessageInChat(Message message)
    {
        newMessageInChat?.Invoke(message);
        updateCurrentChatData?.Invoke();
    }

    #endregion

    public void Awake()
    {

    }

    public async void Start()
    {
        EventBus.OnPlayerInstantiateOnScene(this);

        Debug.Log("<b>[PLAYER]</b> Was created on Scene");
        Debug.Log("<b>[PLAYER]</b> Start Initialize");
        Initialize();
    }

    public async void Update()
    {

    }

    private void OnDestroy()
    {
        EventBus.OnPlayerDestroyFromScene(this);
    }

}
