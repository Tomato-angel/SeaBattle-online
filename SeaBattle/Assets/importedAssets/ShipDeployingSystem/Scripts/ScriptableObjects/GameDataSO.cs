using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameDataSO", menuName = "Scriptable Objects/GameDataSO")]
public class GameDataSO : ScriptableObject
{
    public GameData data;

    public GameDataSO(GameData database)
    {
        this.data = database;
    }
}

[Serializable]
public class GameData
{
    [field: SerializeField]
    public int gridSize { get; private set; }

    [field: SerializeField]
    public Material material { get; private set; }
}