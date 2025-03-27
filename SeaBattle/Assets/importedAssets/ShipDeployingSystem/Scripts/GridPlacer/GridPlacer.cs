using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[ExecuteInEditMode]
public class GridPlacer : MonoBehaviour
{
    [SerializeField] List<GameObject> _tiles;
    public List<GameObject> Tiles => _tiles;

    [SerializeField] private int _xCoordinates;
    [SerializeField] private int _zCoordinates;

    [SerializeField] private GameObject _tilePrefab;
    public void GenerateGrid()
    {
        for (int x = 0; x < _xCoordinates; x++)
        {
            for (int z = 0; z < _zCoordinates; z++)
            {
                Vector3 tilePos = new Vector3(transform.position.x + x, transform.position.y, transform.position.z - z);
                var tile = Instantiate(_tilePrefab, tilePos, Quaternion.identity, transform);
                tile.name = $"Tile {x}; {z}";

                _tiles.Add(tile);
            }
        }
    }
    public void ClearGrid()
    {
        foreach(var tile in _tiles)
        {
            DestroyImmediate(tile);
        }
        _tiles.Clear();
    }

}
