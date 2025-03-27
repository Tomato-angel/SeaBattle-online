using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum TileGameplayStatus
{
    emptyСell,
    damagedEmptyCell,

    shipCell,
    damagedShipCell,

    EmptyCellNextToShip,
    DamagedEmptyCellNextToShip
}
[Serializable]
public class TileGameplayData
{
    //координата выстрела
    [SerializeField]
    public Coordinates Coordinate;

    //ID корабля (например: того, что состоит из 2х сегментов)
    [SerializeField]
    public int ID;

    //ID выставленного на той ячейке корабля
    [SerializeField]
    public int PlacedObjectID;

    //состояние 
    [SerializeField]
    public TileGameplayStatus status;

    public TileGameplayData(
        Coordinates coordinates, 
        int id = 0, 
        int placedObjectID = -1, 
        TileGameplayStatus tileGameplayStatus = TileGameplayStatus.emptyСell)
    {
        Coordinate = coordinates;
        ID = id;
        PlacedObjectID = placedObjectID;
        status = tileGameplayStatus;
    }

    public TileGameplayData() { }
}