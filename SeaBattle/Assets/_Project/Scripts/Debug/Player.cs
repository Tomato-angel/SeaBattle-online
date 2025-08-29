using Mirror;
using UnityEngine;

using Scripts.Matchmaking;
using System.Threading.Tasks;
using System;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Linq;




[RequireComponent(typeof(NetworkMatch))]
[RequireComponent(typeof(NetworkIdentity))]
[Serializable]
public class Player : NetworkBehaviour, IInitializable
{
    [Header("----------------------------------------------------------------------------------------------" + 
        "\nBasic and Network Parameters of the Current Player\n")]
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
    [Header("\n----------------------------------------------------------------------------------------------" + 
        "\nCurrent Player Matchmaking Options\n")]
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

    [Server]
    public void ForceLeaveGame()
    {
        if (_currentMatch == null) return;
        Player[] players = _currentMatch.GetPlayers();
        foreach(var player in players)
        {
            ProjectManager.root.ProjectServices
            .Resolve<MatchMaker>()
            .LeaveMatch(player, _currentMatch.Key, (isLeaveGameSuccessfully) =>
            {
                player.TargetNotifyAboutDisconnectionAsync(this);
                player.isHost = false;
                player.TargetNotifyPlayerAboutLeaveGameAsync();
                //player.TargetSetCurrentMatchAsync(_currentMatch);
            });
        }
        
    }

    [Command] 
    public void CmdLeaveGame()
    {
        if (_currentMatch == null) return;
        //ForceLeaveGame();
        
        Player[] players = _currentMatch.GetPlayers();
        ProjectManager.root.ProjectServices
            .Resolve<MatchMaker>()
            .LeaveMatch(this, _currentMatch.Key, (isLeaveGameSuccessfully) =>
            {
                
                
                foreach (Player player in players)
                {
                    player.TargetNotifyAboutDisconnectionAsync(this);
                    player.CmdLeaveGame();
                    if(isHost)
                    {
                        //player.TargetForcePlayerLeaveGameAsync();
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
        EventBus.OnRequestForOpenMainMenu();
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
    [Space]
    [Header("\n----------------------------------------------------------------------------------------------" + 
        "\nCurrent Player Chat Options")]
    [SyncVar]
    [SerializeField] public ChatData _currentChatData;
    public ChatData CurrentChatData => _currentChatData;
    public event Action<Message> newMessageInChat;
    public event Action updateCurrentChatData;

    [Command]
    public void CmdClearChatData()
    {
        foreach (var player in _currentMatch.GetPlayers())
        {
            //_currentChatData.Push(message);
            player._currentChatData.Clear();
            player.TargetSetChatData(player._currentChatData);
            player.TargetNotifyPlayerAboutUpdateChatData();
        }
    }

    [Command]
    public void CmdSendMessageInChat(string text)
    {
        Message message = new Message(playerData.nickName, text);
        
        foreach (var player in _currentMatch.GetPlayers())
        { 
            //_currentChatData.Push(message);
            player._currentChatData.Push(message);
            player.TargetSetChatData(player._currentChatData);
            player.TargetNotifyPlayerAboutNewMessageInChat(message);
        }
    }
    [TargetRpc]
    public void TargetSetChatData(ChatData chatData)
    {
        _currentChatData = chatData;
    }
    [TargetRpc]
    public void TargetNotifyPlayerAboutUpdateChatData()
    {
        updateCurrentChatData?.Invoke();
    }
    [TargetRpc]
    public void TargetNotifyPlayerAboutNewMessageInChat(Message message)
    {
        Debug.Log($"[Player] New message with text {message._text} sended to Chat");
        newMessageInChat?.Invoke(message);
        updateCurrentChatData?.Invoke();
    }



    #endregion

    #region [ Gameplay ]
    [Command]
    public void CmdPrepareForStartGameplay()
    {
        Debug.Log($"[Player] Prepare to Gameplay");
        money = 10;
        SimpleShoot simpleShoot = new SimpleShoot();
        _currentAbility = simpleShoot;
        abilities.Add(simpleShoot);
        _isPlayerTurn = isHost;

        /*
        TargetNotifyAboutPlayerBuyAbility();
        _currentMatch.GetAnotherPlayer(this, out Player opponentPlayer);
        opponentPlayer.TargetNotifyAboutOpponentBuyAbility();
        */

        _currentMatch.GetAnotherPlayer(this, out Player opponentPlayer);

        opponentPlayer.TargetForceSetPlayerAbilities(opponentPlayer.abilities);
        opponentPlayer.TargetForceSetOpponentAbilities(abilities);
        opponentPlayer.TargetNotifyAboutOpponentBuyAbility();


        TargetForceSetPlayerAbilities(abilities);
        TargetForceSetOpponentAbilities(opponentPlayer.abilities);
        TargetNotifyAboutPlayerBuyAbility();
    }

    [Space]
    [Header("\n----------------------------------------------------------------------------------------------" + 
        "\nCurrent Player Gameplay Options\n")]
    public List<TileGameplayData> currentTilesGameplayData = new();
    public bool IsAllOpponentShipsAreDeployed = false;
    public event Action allOpponentShipsAreDeployed;
    public event Action allShipsAreDeployed;
    [Command]
    public void CmdSetTilesGameplayData(List<TileGameplayData> tilesGameplayData)
    {
        currentTilesGameplayData = tilesGameplayData;
        GenerateShipsDatabaseByTiles(tilesGameplayData);

        _currentMatch.GetAnotherPlayer(this, out Player opponentPlayer);
        allShipsAreDeployed?.Invoke();
        opponentPlayer.TargetNotifyAboutAllOpponentShipsAreDeployed();  
        opponentPlayer.IsAllOpponentShipsAreDeployed = true;  
    }
    [TargetRpc]
    public void TargetNotifyAboutAllOpponentShipsAreDeployed()
    {
        IsAllOpponentShipsAreDeployed = true;
        allOpponentShipsAreDeployed?.Invoke();
    }

    public Dictionary<int, ShipGameplayData> _shipsDatabase;
    [Server]
    public bool IsAllShipsDestroyed()
    {
        bool result = true;
        foreach(var ship in _shipsDatabase)
        {
            if(ship.Value.Health > 0)
            {
                result = false;
                break;
            }
        }
        return result;
    }
    [Server]
    public void GenerateShipsDatabaseByTiles(List<TileGameplayData> tilesGameplayData)
    {
        _shipsDatabase = new Dictionary<int, ShipGameplayData>();
        foreach (var tileGameplayData in tilesGameplayData)
        {
            if(tileGameplayData.status == TileGameplayStatus.shipCell || tileGameplayData.status == TileGameplayStatus.EmptyCellNextToShip)
            {
                if(_shipsDatabase.ContainsKey(tileGameplayData.PlacedObjectID))
                {
                    _shipsDatabase[tileGameplayData.PlacedObjectID].Coordinates.Add(tileGameplayData.Coordinate);
                }
                else
                {
                    _shipsDatabase.Add(tileGameplayData.PlacedObjectID, new ShipGameplayData(tileGameplayData.ID, new List<Coordinates>()));
                }
            }
        }
    }


    public event Action<List<TileGameplayData>> attackOpponent;
    public event Action<List<TileGameplayData>> getAttackFromOpponent;
    //
    [Server]
    public bool CmdAttackOpponent(Coordinates[] targetCoordinates)
    {
        bool isDamagedOpponent = false;
        Debug.Log($"[Player] {connectionToClient.connectionId} attack his opponent on coordinates {targetCoordinates.ToString()}");
        _currentMatch.GetAnotherPlayer(this, out Player opponentPlayer);
        HashSet<TileGameplayData> responseTileGameplayData = new();
        /*
        foreach (var targetCoordinate in targetCoordinates)
        {
            foreach (var tileGameplayData in _tilesGameplayData)
            {
                if (tileGameplayData.Coordinate == targetCoordinate)
                {
                    if(tileGameplayData.status == TileGameplayStatus.shipCell)
                    {
                        int placedObjID = tileGameplayData.PlacedObjectID;

                        if (_shipsGameplayData[placedObjID].Health == 0)
                            continue;

                        _shipsGameplayData[placedObjID].Health -= 1;
                        if(_shipsGameplayData[placedObjID].Health <= 0)
                        {
                            foreach(var nextToShipTileCoordinate in _shipsGameplayData[placedObjID].Coordinates)
                            {
                                foreach(var nextToShipTileGameplayData in _tilesGameplayData)
                                {
                                    if(nextToShipTileGameplayData.Coordinate == nextToShipTileCoordinate)
                                    {
                                        responseTileGameplayData.Add(nextToShipTileGameplayData);
                                    }
                                }
                            }
                        }
                        else
                        {
                            responseTileGameplayData.Add(tileGameplayData);
                        }
                    }
                }
            }
        }

        TargetNotifyAboutOpponentGetAttack(responseTileGameplayData.ToList());
        opponentPlayer.TargetNotifyAboutOpponentAttack(responseTileGameplayData.ToList());
        */

        // Это для простой проверки работы кода, настоящее составление ответа в комментарии выше
        foreach (var targetCoordinate in targetCoordinates)
        {
            foreach (var tileGameplayData in opponentPlayer.currentTilesGameplayData)
            {
                if (tileGameplayData.Coordinate == targetCoordinate)
                {
                    responseTileGameplayData.Add(tileGameplayData);

                    if(tileGameplayData.status == TileGameplayStatus.shipCell)
                    {
                        opponentPlayer._shipsDatabase[tileGameplayData.PlacedObjectID].Health -= 1;
                        isDamagedOpponent = true;
                        AddMoney(10);
                    }
                }
            }
        }

        TargetNotifyAboutOpponentGetAttack(responseTileGameplayData.ToList());
        opponentPlayer.TargetNotifyAboutOpponentAttack(responseTileGameplayData.ToList());

        if(opponentPlayer.IsAllShipsDestroyed())
        {
            TargetNotifyAboutWin();
            opponentPlayer.TargetNotifyAboutLose();
        }

        return isDamagedOpponent;
    }
    [TargetRpc]
    public void TargetNotifyAboutOpponentGetAttack(List<TileGameplayData> tilesGameplayData)
    {
        attackOpponent?.Invoke(tilesGameplayData);
    }
    [TargetRpc]
    public void TargetNotifyAboutOpponentAttack(List<TileGameplayData> tilesGameplayData)
    {
        getAttackFromOpponent?.Invoke(tilesGameplayData);
    }

    public event Action<List<TileGameplayData>> scanOpponent;
    public event Action<List<TileGameplayData>> getScanFromOpponent;
    [Server]
    public void CmdScanOpponent(Coordinates[] targetCoordinates)
    {
        Debug.Log($"[Player] {connectionToClient.connectionId} scan his opponent");

        _currentMatch.GetAnotherPlayer(this, out Player opponentPlayer);
        HashSet<TileGameplayData> responseTileGameplayData = new();

        /*
        foreach (var targetCoordinate in targetCoordinates)
        {
            foreach (var tileGameplayData in _tilesGameplayData)
            {
                if (tileGameplayData.Coordinate == targetCoordinate)
                {
                    if (tileGameplayData.status == TileGameplayStatus.shipCell)
                    {
                        int placedObjID = tileGameplayData.PlacedObjectID;
                        responseTileGameplayData.Add(tileGameplayData);
                    }
                }
            }
        }*/

        // Это для простой проверки работы кода, настоящее составление ответа в комментарии выше
        foreach (var targetCoordinate in targetCoordinates)
        {
            foreach (var tileGameplayData in opponentPlayer.currentTilesGameplayData)
            {
                if (tileGameplayData.Coordinate == targetCoordinate)
                {
                    responseTileGameplayData.Add(tileGameplayData);
                }
            }
        }

        TargetNotifyAboutOpponentGetScan(responseTileGameplayData.ToList());
        opponentPlayer.TargetNotifyAboutOpponentScan(responseTileGameplayData.ToList());
    }
    [TargetRpc]
    public void TargetNotifyAboutOpponentGetScan(List<TileGameplayData> tilesGameplayData)
    {
        scanOpponent?.Invoke(tilesGameplayData);
    }
    [TargetRpc]
    public void TargetNotifyAboutOpponentScan(List<TileGameplayData> tilesGameplayData)
    {
        getScanFromOpponent?.Invoke(tilesGameplayData);
    }

    /*
    public event Action updateGameplayProperties;
    public event Action updateOpponentGameplayProperties;
    [TargetRpc]
    public void TargetNotifyAboutUpdateGameplayProperties()
    {
        updateGameplayProperties?.Invoke();
    }
    [TargetRpc]
    public void TargetNotifyAboutOpponentUpdateGameplayProperties()
    {
        updateOpponentGameplayProperties?.Invoke();
    }
    */

    public event Action youWin;
    public event Action youLose;
    [TargetRpc]
    public void TargetNotifyAboutWin()
    {
        youWin?.Invoke();
    }
    [TargetRpc]
    public void TargetNotifyAboutLose()
    {
        youLose?.Invoke();
    }


    #endregion

    #region [ Abilities ]
    [Space]
    [Header("\n----------------------------------------------------------------------------------------------" +
        "\nCurrent Player Abilities Options\n")]
    [SyncVar]
    public Ability _currentAbility;
    public Ability CurrentAbility => _currentAbility;
    public List<Ability> abilities = new();
    private int _maxAbilityCount = 4;
    [SyncVar]
    public bool _isPlayerTurn;
    public bool IsPlayerTurn => _isPlayerTurn;
    
    [Command]
    public void CmdSetCurrentAbility(Ability ability)
    {
        Debug.Log($"[Server] Player set new current ability with ID: {ability.ID}");
        _currentAbility = ability;
        TargetNotifyAboutSetCurrentAbility(ability);
    }
    public event Action<Ability> setCurrentAbility;
    [TargetRpc]
    public void TargetNotifyAboutSetCurrentAbility(Ability ability)
    {
        setCurrentAbility?.Invoke(ability);
    }
    [TargetRpc]
    public void TargetForceSetPlayerAbilities(List<Ability> newAbilities)
    {
        abilities = newAbilities;
    }
    [TargetRpc]
    public void TargetForceSetOpponentAbilities(List<Ability> newAbilities)
    {
        Player opponentPlayer = GetAnotherPlayerFromLocalScene();
        opponentPlayer.abilities = newAbilities;
    }

    public event Action playerUsedAbility;
    public event Action opponentUsedAbility;
    [TargetRpc]
    public void TargetNotifyAboutPlayerUsedAbility()
    {
        playerUsedAbility?.Invoke();  
    }
    [TargetRpc]
    public void TargetNotifyAboutOpponentUsedAbility() 
    {
        opponentUsedAbility?.Invoke(); 
    }

    /*
    [Command]
    public void CmdUseAbility(Ability ability, Coordinates origin)
    {
        Debug.Log($"[Player] Used ability: {ability.GetType().FullName} on origin: X={origin.x} Z={origin.z}");

        if (ability == null) return;

        if (ability.GetType().FullName != new SimpleShoot().GetType().FullName)
        {
            if (abilities.Contains(ability))
            {
                abilities.Remove(ability);
            }
        }
        
        List<Coordinates> targetCoordinates = new List<Coordinates>();
        foreach (var abilityCoordinate in ability.TargetCoordinates)
        {
            var calculatedCoordinateX = abilityCoordinate.x + origin.x;
            var calculatedCoordinateZ = abilityCoordinate.z + origin.z;

            if (calculatedCoordinateX < 10 && calculatedCoordinateX >= 0)
            {
                if (calculatedCoordinateZ < 10 && calculatedCoordinateZ >= 0)
                {
                    targetCoordinates.Add(new Coordinates(calculatedCoordinateX, calculatedCoordinateZ));
                }
            }
        }

        switch(ability.Type)
        {
            case AbilityTypes.Attack:
                {
                    CmdAttackOpponent(targetCoordinates.ToArray());
                    break;
                }
            case AbilityTypes.Scan:
                {
                    CmdScanOpponent(targetCoordinates.ToArray());
                    break;
                }
        }

        _currentAbility = null;

        
        
        _currentMatch.GetAnotherPlayer(this, out Player opponentPlayer);
        opponentPlayer.TargetNotifyAboutOpponentUsedAbility();
        opponentPlayer.TargetForceSetPlayerAbilities(opponentPlayer.abilities);
        opponentPlayer.TargetForceSetOpponentAbilities(abilities);

        TargetNotifyAboutPlayerUsedAbility();
        TargetForceSetPlayerAbilities(abilities);
        TargetForceSetOpponentAbilities(opponentPlayer.abilities);

    }*/

    [Command]
    public void CmdUseAbility(Ability ability, List<Coordinates> targetCoordinates)
    {
        Debug.Log($"[Player] Used ability: {ability.GetType().FullName} on range of coordinates {targetCoordinates.ToArray().ToString()}");
        
        if (ability == null) return;

        if (ability.ID != 0)
        {
            foreach(var targetAbility in abilities)
            {
                if(targetAbility.ID == ability.ID)
                {
                    abilities.Remove(targetAbility);
                    break;
                }
            }
        }

        bool tmpIsDamagedOponent = false;
        switch (ability.Type)
        {
            case AbilityTypes.Attack:
                {
                    tmpIsDamagedOponent = CmdAttackOpponent(targetCoordinates.ToArray());
                    break;
                }
            case AbilityTypes.Scan:
                {
                    CmdScanOpponent(targetCoordinates.ToArray());
                    break;
                }
        }
        /*
        TargetNotifyAboutPlayerUsedAbility();
        _currentMatch.GetAnotherPlayer(this, out Player opponentPlayer);
        opponentPlayer.TargetNotifyAboutOpponentUsedAbility();*/

        _currentAbility = null;

        _currentMatch.GetAnotherPlayer(this, out Player opponentPlayer);
        Debug.Log($"[Player] ConnID={connectionToClient} player used ability with ID:{ability.ID} with damage: {tmpIsDamagedOponent}");
        if (ability.ID == 0 && tmpIsDamagedOponent)
        {
            _isPlayerTurn = true;
            opponentPlayer._isPlayerTurn = false;
        }
        else
        {
            _isPlayerTurn = false;
            opponentPlayer._isPlayerTurn = true;
        }

        opponentPlayer.TargetForceSetPlayerAbilities(opponentPlayer.abilities);
        opponentPlayer.TargetForceSetOpponentAbilities(abilities);
        opponentPlayer.TargetNotifyAboutOpponentUsedAbility();

        TargetForceSetPlayerAbilities(abilities);
        TargetForceSetOpponentAbilities(opponentPlayer.abilities);
        TargetNotifyAboutPlayerUsedAbility();

        

    }

    #endregion

    #region [ Shop ]
    [Space]
    [Header("\n----------------------------------------------------------------------------------------------" + 
        "\nCurrent Player Shop Options\n")]
    [SyncVar]
    public int money = 0;
    public int Money => money;
    public event Action<int, int> moneyUpdate;
    [TargetRpc]
    public void TargetNotifyAbouyMoneyUpdate(int money, int opponentMoney)
    {
        moneyUpdate?.Invoke(money, opponentMoney);
    }
    
    [Server]
    public void AddMoney(int value)
    {
        money += value;
        if (money > 100) money = 100;
        _currentMatch.GetAnotherPlayer(this, out Player opponentPlayer);
        TargetNotifyAbouyMoneyUpdate(Money, opponentPlayer.Money);
    }
    [Server]
    public void SubstractMoney(int value)
    {
        money -= value;
        if (money < 0) money = 0;
        _currentMatch.GetAnotherPlayer(this, out Player opponentPlayer);
        TargetNotifyAbouyMoneyUpdate(Money, opponentPlayer.Money);
    }

    private Ability[] _abilitiesInShop = new Ability[]
    {
        new SimpleShoot(),
        new QuadricShoot(),
        new LineShoot(),
        new QuadricScan()
    };

    public event Action playerBuyAbility;
    public event Action opponentBuyAbility;
    [TargetRpc]
    public void TargetNotifyAboutPlayerBuyAbility()
    {
        playerBuyAbility?.Invoke();
    }
    [TargetRpc]
    public void TargetNotifyAboutOpponentBuyAbility()
    {
        opponentBuyAbility?.Invoke();
    }
    [Command]
    public void CmdBuyNewAbility() 
    {
        var abilityPrice = 25;

        if (money < abilityPrice) return;
        if (abilities.Count >= _maxAbilityCount) return;
        
        int abilityID = UnityEngine.Random.Range(1, _abilitiesInShop.Length);
        switch(abilityID)
        {
            case 0:
                {
                    if (abilities.Count < _maxAbilityCount)
                    {
                        abilities.Add(new SimpleShoot());
                    }
                    break;
                }
            case 1:
                {
                    if (abilities.Count < _maxAbilityCount)
                    {
                        abilities.Add(new QuadricShoot());
                    }
                    break;
                }
            case 2:
                {
                    if (abilities.Count < _maxAbilityCount)
                    {
                        abilities.Add(new LineShoot());
                    }
                    break;
                }
            case 3:
                {
                    if (abilities.Count < _maxAbilityCount)
                    {
                        abilities.Add(new QuadricScan());
                    }
                    break;
                }
        }
        SubstractMoney(abilityPrice);
        

        /*
        TargetNotifyAboutPlayerBuyAbility();
        _currentMatch.GetAnotherPlayer(this, out Player opponentPlayer);
        opponentPlayer.TargetNotifyAboutOpponentBuyAbility();
        */
        _currentMatch.GetAnotherPlayer(this, out Player opponentPlayer);
        
        opponentPlayer.TargetForceSetPlayerAbilities(opponentPlayer.abilities);
        opponentPlayer.TargetForceSetOpponentAbilities(abilities);
        opponentPlayer.TargetNotifyAboutOpponentBuyAbility();



        TargetForceSetPlayerAbilities(abilities);
        TargetForceSetOpponentAbilities(opponentPlayer.abilities);
        TargetNotifyAboutPlayerBuyAbility();

    }
    #endregion

    #region [ Description ]
    [Space]
    [Header("\n----------------------------------------------------------------------------------------------" + 
        "\nDescription\n")]
    [SerializeField] string _description;
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
