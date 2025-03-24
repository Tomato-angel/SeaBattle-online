using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;


public class ShipsDeploymentSystem : MonoBehaviour, IInitializable
{
    [SerializeField] PlacementSystem_new _placementSystem_New;

    private bool _isActive;
    public void SetActive(bool isActive)
    {
        _isActive = isActive;
        _placementSystem_New.SetActive(isActive);
    }

    public event Action<bool> allShipsDeployed;
    public void OnAllShipsDeployed(bool isAllShipsDeployed)
    {
        allShipsDeployed?.Invoke(isAllShipsDeployed);
    }

    public void Initialize()
    {
        _placementSystem_New.allShipsSeployed += OnAllShipsDeployed;
    }

    public GameplayData GetGameplayData()
    {
        GameplayData gameplayData = new GameplayData(
            binaryTilesData: _placementSystem_New.GetBinaryTilesGameplayData(),
            tilesData: _placementSystem_New.GetTilesGameplayData(),
            shipsDatabase: _placementSystem_New.GetInstalledShips()
            );

        return gameplayData;
    }    
}

