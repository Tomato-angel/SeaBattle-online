using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RemovalState : IBuildingState
{
    private int gameObjectIndex = -1;
    ShipDatabaseSO database;
    PreviewSystem previewSystem;
    GridData GridData;
    ObjectPlacer objectPlacer;
    Vector3 origin;
    float cellSize;
    List<Ship> shipsList;

    public RemovalState(
        ShipDatabaseSO database,
        PreviewSystem previewSystem,
        GridData GridData,
        ObjectPlacer objectPlacer,
        Vector3 origin,
        float cellSize,
        List<Ship> shipsList)
    {
        this.database = database;
        this.previewSystem = previewSystem;
        this.GridData = GridData;
        this.objectPlacer = objectPlacer;
        this.origin = origin;
        this.cellSize = cellSize;
        this.shipsList = shipsList;

        previewSystem.StartShowingPreview();
    }
    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3 mousePosition)
    {
        bool selectedData = false;
        Coordinates objectRootPos = Coordinates.WorldToCoordinates(mousePosition, origin, cellSize);

        //Debug.Log("[" + objectRootPos.x + "; " + objectRootPos.z + "] ++ " + GridData.ContainsEmpty(objectRootPos));

        if (CheckIfSelectionIsValid(objectRootPos))
        {
            selectedData = true;
        }
        //Debug.Log(objectID);
        if (selectedData == true)
        {
            int objectID = GridData.GetShipID(objectRootPos);
            gameObjectIndex = GridData.GetRepresentationIndex(objectRootPos);
            if (gameObjectIndex == -1)
                return;
            List<Coordinates> toDelete = GridData.RemoveObjectWithPlacedID(objectID);
            objectPlacer.RemoveFromGrid(toDelete);
            shipsList[gameObjectIndex].shipAmount++;
        }
        previewSystem.UpdatePosition(Coordinates.WorldInGrid(mousePosition, origin, cellSize), false);
    }

    //если €чейка зан€та "пустой" €чейкой или туда можно поставить объект - нельз€ удалить корабль с этой координаты
    private bool CheckIfSelectionIsValid(Coordinates gridPosition)
    {
        if (GridData.ContainsEmpty(gridPosition))
            return false;
        else
            return !(GridData.CanPlaceObjectAt(gridPosition, new Coordinates(1,1)));
    }

    public void RotateObject(Vector3 mousePosition)
    {
        return;
    }

    public Coordinates UpdateState(Vector3 mousePosition)
    {
        Coordinates tileGridPos = Coordinates.WorldToCoordinates(mousePosition, origin, cellSize);
        bool validity = CheckIfSelectionIsValid(tileGridPos);
        Vector3 tileWorldPos = Coordinates.WorldInGrid(mousePosition, origin, cellSize);

        previewSystem.UpdatePosition(tileWorldPos, validity);
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
