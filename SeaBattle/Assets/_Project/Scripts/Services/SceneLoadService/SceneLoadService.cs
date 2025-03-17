using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SceneLoadService
{
    private Scene _currentScene;

    public event Action BeginLoadScene;
    public event Action EndLoadScene;

    public async void LoadSceneAsync(string sceneName, int millisecondsDelay = 2000, LoadSceneMode loadSceneMode = LoadSceneMode.Additive)
    {
        BeginLoadScene?.Invoke();

        await Task.Delay(millisecondsDelay);
        SceneManager.UnloadSceneAsync(_currentScene);
        SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        _currentScene = SceneManager.GetSceneByName(sceneName);

        EndLoadScene?.Invoke();
    }
    public async void UnloadScene(int millisecondsDelay = 2000)
    {
        await Task.Delay(millisecondsDelay);
        SceneManager.UnloadSceneAsync(_currentScene);
    }
}

