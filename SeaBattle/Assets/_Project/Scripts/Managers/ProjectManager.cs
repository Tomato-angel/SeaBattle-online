using DI;
using Mirror;
using Scripts.Matchmaking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ConnectionProtocol
{
    Client,
    Server,
    CombinedMode
}

[RequireComponent(typeof(NetworkIdentity))]
public class ProjectManager : NetworkBehaviour, IInitializable
{
    [Header("Сетевой протокол (сетевая роль) заданной сборки:")]
    [Space(10)]
    [SerializeField] ConnectionProtocol _currentConnectionProtocol = ConnectionProtocol.Client;
    [Space(50)]
    [Header("Настройка сетевого подключения и сетевых компонентов: ")]
    [Space(10)]
    [SerializeField][Range(0, 10000)] int _millisecondsDelay = 0;
    
    [field: SerializeField] public NetworkManager CurrentNetworkManager 
    { get; private set; }
    [field: SerializeField] public bool IsInitialized 
    { get; private set; }
    [field: SerializeField] public static ProjectManager root 
    { get; private set; }

    private DIContainer _projectServices;
    public DIContainer ProjectServices 
    { get => _projectServices; }


    [field: SerializeField] public ProjectData ProjectData 
    { get; private set; } = null;


    [field: SerializeField] public Player LocalPlayer 
    { get; private set; } = null;
    [Client]
    public void SetLocalPlayer(Player player)
    {
        if (!player.isLocalPlayer) return;
        Debug.Log("[ProjectManager] Set local player");
        LocalPlayer = player;
    }

    [SerializeField] public List<Player> LocalPlayers 
    { get; private set; } = new List<Player>();
    [Client]
    public void AddLocalPlayer(Player player)
    {
        LocalPlayers.Add(player);
    }
    [Client]
    public void RemoveLocalPlayer(Player player)
    {
        LocalPlayers.Remove(player);
    }


    #region [ Initialization ]

    public void GeneralInitialize()
    {
        Debug.Log("[ProjectManager] <b>GENERAL</b> initialize");

        root = this;
        _projectServices = new DIContainer();
        _projectServices.RegisterSingleton((_) => new JsonToFileStorageService());

        IsInitialized = true;
        DontDestroyOnLoad(gameObject);
    }
    public void ClientInitialize()
    {
        Debug.Log("[ProjectManager] <b>CLIENT</b> initialize");
        _projectServices.RegisterSingleton((_) => new SceneLoadService());

        EventBus.playerConnectedToGame += AddLocalPlayer;
        EventBus.playerDisconnectedFromGame += RemoveLocalPlayer;
        
    }
    public void ServerInitialize()
    {
        Debug.Log("[ProjectManager] <b>SERVER</b> initialize");
        _projectServices.RegisterSingleton((_) => new MatchMaker());
    }
    public async void LaunchNetworkConnectionAsync()
    {
        CurrentNetworkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
        if (CurrentNetworkManager == null) return;

        await Task.Delay(_millisecondsDelay);

        switch (_currentConnectionProtocol)
        {
            case ConnectionProtocol.Client:
                {
                    CurrentNetworkManager.StartClient();
                    break;
                }
            case ConnectionProtocol.Server:
                {
                    CurrentNetworkManager.StartServer();
                    break;
                }
            case ConnectionProtocol.CombinedMode:
                {
                    CurrentNetworkManager.StartHost();
                    break;
                }
        }
    }

    public void Initialize()
    {
        {
            GeneralInitialize();
        }
        if (_currentConnectionProtocol == ConnectionProtocol.Client || _currentConnectionProtocol == ConnectionProtocol.CombinedMode)
        {
            ClientInitialize();
        }
        if (_currentConnectionProtocol == ConnectionProtocol.Server || _currentConnectionProtocol == ConnectionProtocol.CombinedMode)
        {
            ServerInitialize();
        }

        LaunchNetworkConnectionAsync();
    }
    #endregion



    
    
    
}

 