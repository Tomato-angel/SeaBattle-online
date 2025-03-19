using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class GridData
{
    public List<TileData> occupiedTiles = new();

    public int gridSize = 10;

    [SerializeField]
    private List<int> objectsIDs = new();

    #region [ Для работы с PlacementSystem ]
    public int GetCalculatedID()
    {
        int emptyID = 0;
        if (objectsIDs.Count == 0) 
        {
            return emptyID;
        }
        //string message = "";
        while (objectsIDs.Contains(emptyID))
        {
            //message += $"{emptyID} || {objectsIDs[emptyID]} == {objectsIDs.Contains(emptyID)} // ";
            emptyID++;
        }
        //Debug.Log(message);
        /*string message = "ERMMM ";
        for (int i = 0; objectsIDs.Count > i; i++) 
        {
            message += objectsIDs[i] + " __ ";
        }
        Debug.Log(message);*/

        return emptyID;
    }

    public int GetShipID(Coordinates gridPos)
    {
        foreach (var temp in occupiedTiles)
        {
            if(temp.ContainsKey(gridPos))
            {
                //Debug.Log(temp.placedObjectID[0]);
                return temp.placedObjectID[0];
            }
        }
        return -1;
    }

    public List<Coordinates> CalculatePositions(Coordinates gridPosition, Coordinates objectSize)
    {
        List<Coordinates> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int z = 0; z < objectSize.z; z++)
            {
                Coordinates temp = new Coordinates(gridPosition, x, z);
                //Debug.Log(temp.x + " " + temp.z + " = [" + objectSize.x + "; " + objectSize.z + "]");

                returnVal.Add(temp);
            }
        }
        /*for (int i = 0; i < returnVal.Count; i++)
        {
            Debug.Log($"POS TO OCCUPY: [{returnVal[i].x}; {returnVal[i].z}]");
        }*/
        return returnVal;
    }

    public bool CanPlaceObjectAt(Coordinates gridPosition, Coordinates objectSize)
    {
        List<Coordinates> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        //
        /*for (int i = 0; i < positionToOccupy.Count - 1; i++)
        {
            Debug.Log($"POS TO OCCUPY: [{positionToOccupy[i].x}; {positionToOccupy[i].z}]");
        }*/
        foreach (Coordinates pos in positionToOccupy)
        {
            /*если в нашем словаре уже:
                есть эта координата
                координаты выходят за рамки массива
                координата по X меньше нуля
              мы не можем поставить объект
            */
            if (pos.x > gridSize - 1 || pos.z > gridSize - 1 || pos.x < 0 || pos.z < 0)
                return false;
            foreach (TileData posOccupiedTiles in occupiedTiles)
            {
                //Debug.Log(pos.x + " !! " + pos.z);
                if (posOccupiedTiles.ContainsKey(pos))
                    return false;
            }
        }
        return true;
    }

    public void AddObjectAt(Coordinates gridPosition,
        Coordinates objectSize,
        int id,
        int placedObjectID)
    {
        List<Coordinates> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        TileData data = new TileData(positionToOccupy, id, placedObjectID);
        if (!objectsIDs.Contains(placedObjectID))
            objectsIDs.Add(placedObjectID);
        foreach (var pos in positionToOccupy)
        {
            /*for (int i = 0; i < occupiedTiles.Count; i++)
            {
                if (occupiedTiles[i].ContainsKey(pos))
                {
                    throw new Exception($"List already contains this tile position {pos}");
                }
            }*/
            occupiedTiles.Add(data);
        }
    }

    public void ModifyEmptyAt(Coordinates posOfEmpty, int addIndex)
    {
        if (posOfEmpty.x > gridSize - 1 ||
            posOfEmpty.z > gridSize - 1 ||
            posOfEmpty.x < 0 ||
            posOfEmpty.z < 0)
            return;
        if (addIndex == -1)
            return;
        foreach (var pos in occupiedTiles)
        {
            if (pos.ContainsKey(posOfEmpty))
            {
                if (!pos.placedObjectID.Contains(addIndex))
                    pos.placedObjectID.Add(addIndex);
            }
        }
    }

    public List<Coordinates> RemoveObjectWithPlacedID(int objectID)
    {
        List<Coordinates> list = new();
        List <TileData> data = new();        
        foreach (var pos in occupiedTiles)
        {
            if (pos.placedObjectID.Contains(objectID))
            {

                pos.placedObjectID.Remove(objectID);
                if (pos.placedObjectID.Count == 0)
                {
                    data.Add(pos);
                }
            }
        }
        //string message = "WHAT NOW";
        foreach (var dat in data)
        {
            foreach (Coordinates cor in dat.GetCoordinates())
            {
                //message += ", [" + cor.x + "; " + cor.z + "]";
                list.Add(cor);
                occupiedTiles.Remove(dat);
            }
        }
        /*if (objectsIDs[objectID] != -1)
            objectsIDs[objectID] = -1;*/
        if (objectsIDs.Contains(objectID))
            objectsIDs.Remove(objectID);
        //Debug.Log(message);
        return list;
    }

    public bool ContainsEmpty(Coordinates gridPosition)
    {
        foreach(var pos in occupiedTiles)
        {
            if (pos.ID == 0 && pos.ContainsKey(gridPosition))
                return true;
        }
        return false;
    }
    public int GetRepresentationIndex(Coordinates gridPosition)
    {
        foreach (var pos in occupiedTiles)
        {
            if (pos.ContainsKey(gridPosition) == true)
                return pos.ID;
        }
        return -1;
    }

    public void DebugLog()
    {
        string message = "____";
        foreach(var pos in occupiedTiles)
        {
            message = "ID of object: " + pos.ID + " with placedObjectIDes: ";
            foreach (int id in pos.placedObjectID)
            {
                message += id + ", ";
            }
            Debug.Log(message);
        }
    }
    #endregion
}



