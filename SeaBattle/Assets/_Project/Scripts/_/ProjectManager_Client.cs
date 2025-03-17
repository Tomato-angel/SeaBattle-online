using DI;
using Mirror;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class ProjectManager_Client : MonoBehaviour
{
    #region [ ProjectData & PlayerData ]
    private ProjectData _projectData;
    public ProjectData ProjectData { get => _projectData; }
    private PlayerData _playerData;
    public PlayerData PlayerData { get => _playerData; }
    #endregion

    public static ProjectManager_Client root { get; private set; }

    [SerializeField] private NetworkManager _networkManager;
    [SerializeField] private Player _localPlayer;
    public Player LocalPlayer { get => _localPlayer; }

    private DIContainer _projectServices;
    public DIContainer ProjectServices { get => _projectServices; }


    private IEnumerator LaunchProjectManager()
    {
        {
            root = this;
            _localPlayer = null;
            _networkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
        }
       
        switch (_networkManager.headlessStartMode)
        {
            case HeadlessStartOptions.AutoStartClient:
                {
                    _projectServices = new DIContainer();
                    DontDestroyOnLoad(gameObject);
                    _projectServices.RegisterSingleton((_) => new JsonToFileStorageService());

                    _projectData = null;
                    _projectServices.Resolve<JsonToFileStorageService>().FastLoad<ProjectData>((data) => { _projectData = data; });
                    _playerData = null;
                    _projectServices.Resolve<JsonToFileStorageService>().FastLoad<PlayerData>((data) => { _playerData = data; });

                    yield return new WaitForSeconds(10f);
                    yield return StartCoroutine(FindPlayerOnScene());
                    SceneManager.LoadScene("MainMenuScene");

                    break;
                }
            case HeadlessStartOptions.AutoStartServer:
                {
                    Destroy(gameObject);
                    break;
                }
            case HeadlessStartOptions.DoNothing:
                {
                    break;
                }
        }

        yield return null;
    }

    IEnumerator FindPlayerOnScene()
    {
        while(_localPlayer == null)
        {
            if(!GameObject.FindGameObjectWithTag("Player"))
            {
                yield return new WaitForSeconds(1f);
            }
            else
            {
                _localPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            }
        }
        yield return null;
    }

    private void Start()
    {
        StartCoroutine(LaunchProjectManager());
    }
}

