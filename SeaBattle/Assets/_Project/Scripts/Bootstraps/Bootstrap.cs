using UnityEngine;
using System;
using Unity.VisualScripting;
using System.Collections.Generic;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] GameObject[] _initializableObjects;
    [SerializeField] GameObject[] _dontDestroyOnLoadObjects;

    void Start()
    {
        foreach(GameObject initializableObject in _initializableObjects)
        {
            IInitializable[]? initializablesComponents = initializableObject.GetComponents<IInitializable>();
            foreach(IInitializable initComponent in initializablesComponents)
            if(initComponent != null)
            {
                initComponent.Initialize();
            }
        }

        foreach(GameObject dontDestroyOnLoadObject in _dontDestroyOnLoadObjects)
        {
            GameObject.DontDestroyOnLoad(dontDestroyOnLoadObject);
        }
    }
}
