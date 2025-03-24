using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] GameplayData _gameplayData;
    public GameplayData GameplayData => _gameplayData;
    public void SetGameplayData(GameplayData gameplayData)
    {
        _gameplayData = gameplayData;
    }

   
}
