using System;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShipsDatabase", menuName = "Scriptable Objects/ShipDatabaseSO", order = 1)]
public class ShipDatabaseSO : ScriptableObject
{
    public List<ShipData> shipsData;

    public ShipDatabaseSO(ShipDatabaseSO database) 
    {
        foreach (var data in database.shipsData)
        {
            this.shipsData.Add(data);
        }
    }
}

[Serializable]
public class ShipData
{
    [field: SerializeField]
    public int ID { get; private set; }
    [field: SerializeField]
    public Coordinates Size { get; private set; }
    [field: SerializeField]
    public GameObject ShipPrefab { get; private set; }

    [field: SerializeField]
    public int ShipAmount { get; set; }

    public void RotateShip()
    {
        int temp;
        temp = Size.x;
        Size.x = Size.z;
        Size.z = temp;
    }
}
