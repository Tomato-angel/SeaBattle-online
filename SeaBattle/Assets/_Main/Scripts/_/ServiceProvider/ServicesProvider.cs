using Mirror;
using Scripts.Matchmaking;
using System;
using System.Collections.Generic;



public class ServicesProvider
{
    #region [ Реализация синглтона ]
    static private ServicesProvider _instance;
    static public ServicesProvider Instance
    {
        get
        {
            if (_instance == null) _instance = new ServicesProvider();
            return _instance;
        }
    }
    #endregion


    #region [ Хранение и обработка сервисов ]

    private Dictionary<Type, IService> _services = new Dictionary<Type, IService>();
    private Dictionary<Type, object> _services2 = new Dictionary<Type, object>();

    private HashSet<IService> _services1;

    /*
    public IService[] Services 
    {
        get
        {
            IService[] result = new IService[_services.Count];
            int i = 0;
            foreach((Type type, IService service) in _services)
            {
                result[i] = service;
                ++i;
            }
            return result;
        }
    }

    public int Count 
    {
        get => _services.Count; 
    }

    
    public bool Contains(Type type)
    {
        return _services.ContainsKey(type);
    }

    public T AddService<T>() where T : IService
    {
        T service = default;
        _services.Add(typeof(T), service);
        return service;
    }
    public void AddService(params IService[] services)
    {
        try
        {
            foreach (IService service in services) _services.Add(service.GetType(), service);
        }
        catch(Exception e)
        {
            throw e;
        }
        
    }

    public void RemoveService(params IService[] services)
    {
        try
        {
            foreach (IService service in services) _services.Remove(service.GetType());
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public T GetService<T>()
    {
        _services.TryGetValue(typeof(T), out IService result);
        return (T) result;
    }
    public IService GetService(Type type)
    {
        _services.TryGetValue(type, out IService result);
        return result;
    }
    */

    #endregion


    // ПОД ВОПРОСОМ (зависит от реализации)
    /*
    public bool Instantiate<T>() where T : NetworkBehaviour, IService
    {
        if (Contains(typeof(T)))
        {
            return false;
        }

        GameObject servicesProviderObj = GameObject.FindGameObjectsWithTag("ServicesProvider")[0];
        if (servicesProviderObj == null)
        {
            return false;
        }

        GameObject serviceObj = new GameObject()
        {
            name = typeof(T).ToString(),
            tag = "Service"
        };
        T service = serviceObj.AddComponent<T>();
       
        AddService(service);

        serviceObj.transform.SetParent(servicesProviderObj.transform);

        return true;
    }
    */

    // НЕ ИСПОЛЬЗУЕТСЯ
    /*
    #region [ Network Behaviour - методы ]

    private void Awake()
    {
        
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        
    }

    #endregion
    */

    public ServicesProvider()
    {
        _services = new Dictionary<Type, IService>();
    }

}
