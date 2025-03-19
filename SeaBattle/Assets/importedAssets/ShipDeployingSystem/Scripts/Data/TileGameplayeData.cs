using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum TileGameplayStatus
{
    emptyСell,
    damagedEmptyCell,

    shipCell,
    damagedShipCell,

    EmptyCellNextToShip,
    DamagedEmptyCellNextToShip
}
public class TileGameplayData
{
    //координата выстрела
    public Coordinates Coordinate { get; set; }

    //ID корабля (например: того, что состоит из 2х сегментов)
    public int ID { get; set; }

    //ID выставленного на той ячейке корабля
    public int PlacedObjectID { get; set; }

    //состояние 
    public TileGameplayStatus status { get; set; }

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
}