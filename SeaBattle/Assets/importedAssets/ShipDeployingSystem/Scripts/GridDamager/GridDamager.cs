using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;


public class GridDamager : MonoBehaviour
{
    [SerializeField] Player _localPlayer;
    [SerializeField] GridPlacer _gridPlacer;

    public void Start()
    {

        if (ProjectManager.root == null) return;
        if (ProjectManager.root.LocalPlayer == null) return;
        _localPlayer = ProjectManager.root.LocalPlayer;
        _localPlayer.getAttackFromOpponent += MarkDamagedTiles;
        _localPlayer.getScanFromOpponent += MarkCheckedTiles;
    }

    public void MarkDamagedTiles(List<TileGameplayData> tilesGameplayData)
    {
        List<DamagerTile> damagerTiles = new();
        foreach (var tile in _gridPlacer.Tiles)
        {
            damagerTiles.Add(tile.GetComponent<DamagerTile>());
        }
        foreach (var tileGameplayData in tilesGameplayData)
        {
            Coordinates coordinates = tileGameplayData.Coordinate;
            if (tileGameplayData.status == TileGameplayStatus.shipCell)
                damagerTiles[(coordinates.z * 10) + coordinates.x].SetStatus(DamagerTileStatus.Hit);
            else
                damagerTiles[(coordinates.z * 10) + coordinates.x].SetStatus(DamagerTileStatus.Missed);
            
        }
    }
    public void MarkCheckedTiles(List<TileGameplayData> tilesGameplayData)
    {
        List<DamagerTile> damagerTiles = new();
        foreach (var tile in _gridPlacer.Tiles)
        {
            damagerTiles.Add(tile.GetComponent<DamagerTile>());
        }
        foreach (var tileGameplayData in tilesGameplayData)
        {
            Coordinates coordinates = tileGameplayData.Coordinate;
            if (tileGameplayData.status == TileGameplayStatus.shipCell)
                damagerTiles[(coordinates.z * 10) + coordinates.x].SetStatus(DamagerTileStatus.Checked);
            else
                damagerTiles[(coordinates.z * 10) + coordinates.x].SetStatus(DamagerTileStatus.Default);

        }
    }


}

