using DI;
using Mirror;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using Scripts.Matchmaking;

public class ProjectManager_Server : NetworkBehaviour
{
    #region [ ProjectData & PlayerData ]
    private ProjectData _projectData;
    public ProjectData ProjectData { get => _projectData; }
    #endregion

    public static ProjectManager_Server root { get; private set; }

    private NetworkIdentity _networkIdentity;
    private NetworkManager _networkManager;

    private DIContainer _projectServices;
    public DIContainer ProjectServices { get => _projectServices; }


    private IEnumerator LaunchProjectManager()
    {
        // __INIT__

        root = this;
        _networkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
        _networkIdentity = GetComponent<NetworkIdentity>();

        _projectServices = new DIContainer();
        DontDestroyOnLoad(gameObject);

        _projectServices.RegisterSingleton((_) => new KeyGenerator());
        _projectServices.RegisterSingleton((_) => new MatchMaker());
        _projectServices.RegisterSingleton((_) => new JsonToFileStorageService());

        _projectData = null;
        _projectServices.Resolve<JsonToFileStorageService>().FastLoad<ProjectData>((data) => { _projectData = data; });

        yield return null;
    }

    private void Start()
    {
        StartCoroutine(LaunchProjectManager());
    }
}
