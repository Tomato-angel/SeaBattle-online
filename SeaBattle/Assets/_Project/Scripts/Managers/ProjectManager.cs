using DI;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class ProjectManager : NetworkBehaviour
{
    #region [ Bind fields for ProjectBootstrap.cs  ]
    public static string SceneTag { get => "ProjectManager"; }
    public static string SceneName { get => "--- Project Manager"; }
    #endregion

    private NetworkIdentity _networkIdentity;

    private DIContainer _projectServices;

    private void Awake()
    {
        _networkIdentity = GetComponent<NetworkIdentity>();

        _projectServices = new DIContainer();



        // Пример регистрации(подключения) некого сервиса
        // _projectServices.RegisterSingleton( (_) => new ...Service(... -> сюда можно передавать свои параметры конструктора));
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        
    }
}

