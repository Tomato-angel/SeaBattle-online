using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class PlacementState : IBuildingState
{
    private int selectedShipIndex = -1;
    int ID;
    PreviewSystem previewSystem;
    ShipDatabaseSO database;
    GridData GridData;
    ObjectPlacer objectPlacer;
    Vector3 origin;
    float cellSize;
    List<Ship> shipsList;

    public PlacementState(int id,
        PreviewSystem previewSystem,
        GridData GridData,
        ShipDatabaseSO database,
        ObjectPlacer objectPlacer,
        Vector3 origin,
        float cellSize,
        List<Ship> shipsList)
    {
        ID = id;
        this.previewSystem = previewSystem;
        this.GridData = GridData;
        this.database = database;
        this.objectPlacer = objectPlacer;
        this.origin = origin;
        this.cellSize = cellSize;
        this.shipsList = shipsList;

        selectedShipIndex = database.shipsData.FindIndex(data => data.ID == ID);
        if (selectedShipIndex > -1)
        {
            previewSystem.StartShowingPreview(
                database.shipsData[selectedShipIndex].ShipPrefab,
                database.shipsData[selectedShipIndex].Size);
        }
        else
            throw new System.Exception($"No object with ID {id}");
    }
    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void RotateObject(Vector3 mousePosition)
    {
        database.shipsData[selectedShipIndex].RotateShip();
        bool placementValidity = CheckPlacementValidity(
            Coordinates.WorldToCoordinates(mousePosition, origin, cellSize), 
            selectedShipIndex, shipsList[selectedShipIndex].shipAmount);
        previewSystem.RotatePreview(placementValidity);
    }

    public void OnAction(Vector3 mousePosition)
    {
        Coordinates objectRootPos = Coordinates.WorldToCoordinates(mousePosition, origin, cellSize);
        bool placementValidity = CheckPlacementValidity(objectRootPos, selectedShipIndex, shipsList[selectedShipIndex].shipAmount);
        int objectPlacedID = GridData.GetCalculatedID();


        if (placementValidity == false)
        {
            previewSystem.UpdatePosition(Coordinates.WorldInGrid(mousePosition, origin, cellSize), placementValidity);
            Debug.Log("NUH HUH! нельзя поставить корабль " + shipsList[selectedShipIndex].shipID);
            return;
        }


        //костыль, чтобы обойти проблему неверного поворота корабля
        Coordinates size = database.shipsData[selectedShipIndex].Size;
        float rotation = size.x > size.z ? 0 : 90f;
        int id = database.shipsData[selectedShipIndex].ID;

        //Debug.Log("UHHHHHHH " + objectPlacedID);

        //спавн корабля на нужной точке и с нужным поворотом
        objectPlacer.PlaceOnGrid(GridData.CalculatePositions(objectRootPos, size), database.shipsData[selectedShipIndex].ShipPrefab,
            Coordinates.WorldInGrid(mousePosition, origin, cellSize),
            rotation);
        GridData.AddObjectAt(objectRootPos, size, id, objectPlacedID);

        EmptyObjectPlacement(mousePosition, size, objectPlacedID);
        placementValidity = false;
        previewSystem.UpdatePosition(Coordinates.WorldInGrid(mousePosition, origin, cellSize), placementValidity);
        shipsList[selectedShipIndex].shipAmount--;
        //GridData.DebugLog();
    }
    private void EmptyObjectPlacement(Vector3 mousePosition, Coordinates shipSize, int ID)
    {
        int count = ID;
        Coordinates objectRootPos = Coordinates.WorldToCoordinates(mousePosition, origin, cellSize);
        for (int x = objectRootPos.x - 1; x <= objectRootPos.x + shipSize.x; x++)
        {
            for (int z = objectRootPos.z - 1; z <= objectRootPos.z + shipSize.z; z++)
            {
                /*Debug.Log("-------[" + x + "; " + z + "]-------[" 
                    + objectRootPos.x + "; " + objectRootPos.z 
                    + "]-------[" + shipSize.x + "; " + shipSize.z
                    + "]-------" + GridData.CanPlaceObjectAt(new Coordinates(x, z), shipSize));*/
                Coordinates emptyCoordinate = new Coordinates(x, z);

                if (GridData.CanPlaceObjectAt(emptyCoordinate, new Coordinates(1, 1)))
                {
                    /*Debug.Log(database.shipsData[0].ShipPrefab.name + "]-------["
                    + emptyCoordinate.x + "; " + emptyCoordinate.z
                    + "]");*/
                    objectPlacer.PlaceEmptyOnGrid(emptyCoordinate, database.shipsData[0].ShipPrefab,
                        Coordinates.CoordinatesToWorld(emptyCoordinate, origin, cellSize),
                        0);
                    //добавляем пустую ячейку (ID = 0) и приписываем ей новый индекс корабля, к которому принадлежит
                    GridData.AddObjectAt(emptyCoordinate, new Coordinates(1, 1), 0, count);
                }
                GridData.ModifyEmptyAt(emptyCoordinate, count);
            }
        }
    }
    private bool CheckPlacementValidity(Coordinates gridPositionInt, int selectedShipIndex, int shipAmount)
    {
        //Debug.Log($"[{gridPositionInt.x}, {gridPositionInt.x}], {selectedShipIndex} = [{database.shipsData[selectedShipIndex].Size.x} , {database.shipsData[selectedShipIndex].Size.z}]");
        if (shipAmount <=0)
            return false;
        else
            return GridData.CanPlaceObjectAt(gridPositionInt, database.shipsData[selectedShipIndex].Size);
    }

    public Coordinates UpdateState(Vector3 mousePosition)
    {
        Coordinates tileGridPos = Coordinates.WorldToCoordinates(mousePosition, origin, cellSize);
        Vector3 tileWorldPos = Coordinates.WorldInGrid(mousePosition, origin, cellSize);

        bool placementValidity = CheckPlacementValidity(tileGridPos, 
                                                        selectedShipIndex, 
                                                        shipsList[selectedShipIndex].shipAmount);
        previewSystem.UpdatePosition(tileWorldPos, placementValidity);
        return tileGridPos;
    }

    public bool CheckEndPlacement()
    {
        for (int i = 1; i < database.shipsData.Count - 1; i++)
        {
            if (shipsList[i].shipAmount != 0)
                return false;
        }
        return true;
    }
}
