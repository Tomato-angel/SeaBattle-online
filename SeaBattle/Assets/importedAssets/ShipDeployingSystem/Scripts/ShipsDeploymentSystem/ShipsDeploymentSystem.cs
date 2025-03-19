using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class ShipsDeploymentSystem : MonoBehaviour
{
    [SerializeField] PlacementSystem_new _placementSystem_New;
    [field: SerializeField] int[,] _binaryTilesData;
    [field: SerializeField] TileGameplayData[,] _tilesData;
    [field: SerializeField] Dictionary<int, ShipGameplayData> _shipsDatabase;
    public void GetGameplayData()
    {
        _binaryTilesData = _placementSystem_New.GetBinaryTilesGameplayData();
        _tilesData = _placementSystem_New.GetTilesGameplayData();
        _shipsDatabase = _placementSystem_New.GetInstalledShips();
    }
}

