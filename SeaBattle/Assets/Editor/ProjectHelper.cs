using System;
using System.Diagnostics;
using System.Security.Policy;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Debug = UnityEngine.Debug;
using Codice.CM.Common;
using System.IO;

public class ProjectHelper : EditorWindow
{
    const string GitHub_URL = "https://github.com/";
    const string Diagram_Path = "Assets\\External Resources\\SeaBattle_OnlineGameDev.drawio";
    const string DrawIO_URL = "https://app.diagrams.net/";
    const string README_Path = "Assets/External Resources/README.asset";


    [MenuItem("Project Helper/Full Workflow", priority = 0)]
    private static void OpenWorkflow()
    {
        OpenReadme();
        OpenGitHub();
        OpenDiagrams();
    }

    [MenuItem("Project Helper/README", priority = 20)]
    private static void OpenReadme()
    {
        FocusOnResource(README_Path);
    }
    private static void FocusOnResource(string path)
    {
        EditorUtility.FocusProjectWindow();

        Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);

        Selection.activeObject = obj;
    }

    [MenuItem("Project Helper/Github", priority = 40)]
    private static void OpenGitHub()
    { 
        Application.OpenURL(GitHub_URL);
    }

    [MenuItem("Project Helper/Diagrams", priority = 40)]
    private static void OpenDiagrams()
    {
        // Формируем URL для открытия файла в app.diagrams.net
        string url = $"{DrawIO_URL}";
        // Открываем URL в браузере по умолчанию
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при открытии браузера: {ex.Message}");
        }
    }

    
}
