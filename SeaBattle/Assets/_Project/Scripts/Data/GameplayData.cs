using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GameplayData
{
    [field: SerializeField] int[,] _binaryTilesData;
    [field: SerializeField] TileGameplayData[,] _tilesData;
    [field: SerializeField] Dictionary<int, ShipGameplayData> _shipsDatabase;


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

