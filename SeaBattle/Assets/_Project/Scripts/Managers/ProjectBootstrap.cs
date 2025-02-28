using System;
using System.Collections.Generic;
using UnityEngine;


public class ProjectBootstrap : MonoBehaviour
{
    private ProjectManager InstantiateProjectManager()
    {
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

    private void DestroyMyself()
    {
        Destroy(gameObject);
    }

    private void Start()
    {
        InstantiateProjectManager();

        // Сюда можно реализовать подгрузку всех основных конфигов проекта и прочее...

        DestroyMyself();
    }
}

