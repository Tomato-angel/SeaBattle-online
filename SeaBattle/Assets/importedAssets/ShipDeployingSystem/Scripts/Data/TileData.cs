using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class TileData
{
    //какие ячейки занимает корабль
    [SerializeField] List<Coordinates> coordinate;

    //каким видом корабля была занята ячейка (1*3)
    [SerializeField] public int ID { get; private set; }

    //каким конкретно поставленным кораблем занята ячейка (2 по счету)
    //или с какими конкретно кораблями пересекается пустая ячейка
    [SerializeField] public List<int> placedObjectID { get; private set; }

    public TileData(List<Coordinates> coordinates, int id, int placedObjectInd)
    {
        this.coordinate = coordinates;
        ID = id;
        if (placedObjectID == null)
        {
            placedObjectID = new List<int>();
        }
        placedObjectID.Add(placedObjectInd);
    }

    public bool ContainsKey(Coordinates pos)
    {
        for (int i = 0; i < coordinate.Count; i++)
        {
            if (coordinate[i] == pos) return true;
        }
        return false;
    }

    public Coordinates ZeroCoordinate()
    {
        return coordinate[0];
    }

    public List<Coordinates> GetCoordinates()
    {
        return coordinate;
    }
}
