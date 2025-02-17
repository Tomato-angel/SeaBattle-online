using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class ServiceProvider
{
    private Dictionary<Type, IService> _services = new Dictionary<Type, IService>();
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
    public int ServicesCount 
    {
        get => _services.Count; 
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

    public IService GetService(Type type)
    {

        _services.TryGetValue(type, out IService result);
        return result;

    }

}
