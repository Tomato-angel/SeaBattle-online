using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class GameplayData
{
    [field: SerializeField] public int[,] _binaryTilesData;
    public int[,] BinaryTilesData => _binaryTilesData;
    [field: SerializeField] public TileGameplayData[,] _tilesData;
    public TileGameplayData[,] TilesData => _tilesData;
    [field: SerializeField] public Dictionary<int, ShipGameplayData> _shipsDatabase;
    public Dictionary<int, ShipGameplayData> ShipsDatabase => _shipsDatabase;


    public override string ToString()
    {
        string result = string.Empty;
        result += "GameplayData: {\n";
        for(int i = 0; i < 10; ++i )
        {
            for(int j = 0; j < 10; ++j)
            {
                if (_tilesData[i,j].status == TileGameplayStatus.shipCell)
                {
                    result += "1";
                }
                else
                {
                    result += "0";
                }
                result += " ";
            }
            result += "\n";
        }
        result += "\n}";
        return result;
    }

    public GameplayData()
    {

    }
    public GameplayData(int[,] binaryTilesData, TileGameplayData[,] tilesData, Dictionary<int, ShipGameplayData> shipsDatabase)
    {
        _binaryTilesData = binaryTilesData;
        _tilesData = tilesData;
        _shipsDatabase = shipsDatabase;
    }

}

