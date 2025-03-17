using System;
using System.Collections.Generic;
using UnityEngine;


public class ProjectBootstrap : MonoBehaviour
{
    [SerializeField] ProjectManager _projectManager;
    /*
    private ProjectManager InstantiateProjectManager()
    {
        Debug.Log("[Project - Bootstrap] Initialize and instantiate project manager...");
        var projectManagerObj = GameObject.FindGameObjectWithTag(ProjectManager.SceneTag);
        if (!projectManagerObj)
        {
            projectManagerObj = new GameObject()
            {
                tag = ProjectManager.SceneTag,
                name = ProjectManager.SceneName
            };
            var projectManager = projectManagerObj.AddComponent<ProjectManager>();
        }
        projectManagerObj.SetActive(true);
        return projectManagerObj.GetComponent<ProjectManager>();
    }
    */
    private void DestroyMyself()
    {
        Destroy(gameObject);
    }

    private void Start()
    {
        //InstantiateProjectManager();

        // Сюда можно реализовать подгрузку всех основных конфигов проекта и прочее...

        //_projectManager.gameObject.SetActive(true);
        //StartCoroutine(_projectManager.LaunchProjectManager());


        DestroyMyself();
    }
}

