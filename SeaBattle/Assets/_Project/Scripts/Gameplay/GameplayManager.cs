using Unity.VisualScripting;
using UnityEngine;

public class GameplayManager : MonoBehaviour, IInitializable
{
    [SerializeField] Player _localPlayer;
    [SerializeField] GameplayData _gameplayData;
    public GameplayData GameplayData => _gameplayData;

    public void Initialize()
    {
        _localPlayer = ProjectManager.root.LocalPlayer;
    }

    public void SetGameplayData(GameplayData gameplayData)
    {
        _gameplayData = gameplayData;
    }

   
}
