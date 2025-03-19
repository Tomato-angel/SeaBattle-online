using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlacementSystem_new : MonoBehaviour
{
    [SerializeField]
    private ShipDeploymentInputManager inputManager;

    [SerializeField]
    private ShipDeploymentButtonsManager buttonLogic;

    [SerializeField]
    private ObjectPlacer objectPlacer;
    IBuildingState buildingState;

    [SerializeField]
    Vector3 gridOrigin;
    [SerializeField]
    float cellSize = 1;

    #region [ Работа с базой данных кораблей ]
    //shipDatabase - та база данных, которую мы создали в эдиторе
    //shipList - та база, с которой мы работаем и которую изменяем, чтобы не влиять на первую.
    [SerializeField]
    private ShipDatabaseSO shipDatabase;
    [SerializeField]
    private List<Ship> shipsList = new List<Ship>();
    public void ShipsInitialize()
    {
        foreach (var ship in shipDatabase.shipsData)
        {
            shipsList.Add(new Ship(ship.ID, ship.ShipAmount));
        }
    }
    #endregion

    [SerializeField]
    public GridData gridData;

    [SerializeField]
    private PreviewSystem previewSystem;

    private Coordinates lastDetectedPosition;
    bool placementValidity;

    private void Start()
    {
        ShipsInitialize();

        StopPlacement();
        gridData = new();
        lastDetectedPosition = new Coordinates(0, 0);
        buttonLogic.PrepareButtons(shipsList);
    }

    //отправная точка процесса расположения
    //проверяется индекс и ИнпутМенеджеру передаются функции для прослушивания
    public void StartPlacement(int ID)
    {
        StopPlacement();
        buildingState = new PlacementState(ID,
            previewSystem,
            gridData,
            shipDatabase,
            objectPlacer,
            gridOrigin,
            cellSize,
            shipsList);

        inputManager.OnClicked += ProceedObjectPlacement;
        inputManager.OnDrag += ProceedObjectPlacement;
        inputManager.OnExit += StopPlacement;
        inputManager.OnRotate += RotateObjectPlacement;
    }

    public void StartRemoval()
    {
        StopPlacement();
        buildingState = new RemovalState(
            shipDatabase, 
            previewSystem,
            gridData, 
            objectPlacer, 
            gridOrigin, 
            cellSize, 
            shipsList);
        inputManager.OnClicked += ProceedObjectPlacement;
        inputManager.OnDrag += ProceedObjectPlacement;
        inputManager.OnExit += StopPlacement;
    }

    //поворот объекта
    public void RotateObjectPlacement()
    {
        Vector3 mousePosition = inputManager.GetSelectedMousePosition();
        buildingState.RotateObject(mousePosition);
    }

    //конец процесса расположения, убираем прослушивание у функций
    private void StopPlacement()
    {
        if (buildingState == null)
            return;
        buildingState.EndState();

        inputManager.OnClicked -= ProceedObjectPlacement;
        inputManager.OnDrag -= ProceedObjectPlacement;
        inputManager.OnExit -= StopPlacement;
        inputManager.OnRotate -= RotateObjectPlacement;
        lastDetectedPosition = new Coordinates();

        buildingState = null;
    }

    //проверки на возможность установить объект и его дальнейшее появление на карте/в матрице
    private void ProceedObjectPlacement()
    {
        /*
        if (inputManager.IsPointerOverUI())
        {
            //buildingState.EndState();
            return;
        } */
        Vector3 mousePosition = inputManager.GetSelectedMousePosition();

        buildingState.OnAction(mousePosition);

        CheckEnd();
    }

    public void CheckEnd()
    {
        //if (buildingState.CheckEndPlacement())
        buildingState.CheckEndPlacement();
        buttonLogic.CheckForAmount(shipsList);
    }

    private void Update()
    {
        if (buildingState == null)
            return;
        Vector3 mousePosition = inputManager.GetSelectedMousePosition();
        
        //Debug.Log(lastDetectedPosition.x + ", " + lastDetectedPosition.z +"] - [" +  WorldToCoordinates(mousePosition).x + ", " + WorldToCoordinates(mousePosition).z);

        Coordinates temp = Coordinates.WorldToCoordinates(mousePosition, gridOrigin, cellSize);
        if (lastDetectedPosition.x != temp.x || lastDetectedPosition.z != temp.z)
        {
            lastDetectedPosition = buildingState.UpdateState(mousePosition);
        }
    }
    public List<TileGameplayData> ToGameplayData()
    {
        //ужас а не функция, но только так представляю костыль, по хорошему переписать TilesData GridData 
        List<TileGameplayData> gameplayList = new List<TileGameplayData>();
        foreach (var tile in gridData.occupiedTiles)
        {
            for (int x = 0; x < gridData.gridSize; x++)
            {
                for (int z = 0; z < gridData.gridSize; z++)
                {
                    Coordinates find = new Coordinates(x, z);
                    TileGameplayData tileToAdd = new TileGameplayData(find, 0, -1);
                    if (tile.ContainsKey(find) && tile.ID != 0)
                    {
                        tileToAdd = new TileGameplayData(find, tile.ID, tile.placedObjectID[0]);
                    }
                    if (!gameplayList.Contains(tileToAdd))
                    {
                        gameplayList.Add(tileToAdd);
                    }
                }
            }
        }
        return gameplayList;
    }
    public Dictionary<int, ShipGameplayData> GetInstalledShips()
    {
        Dictionary<int, ShipGameplayData> _installedShips = new();
        foreach (var tile in gridData.occupiedTiles)
        {
            _installedShips.TryAdd(tile.placedObjectID[0], new ShipGameplayData(tile.ID, tile.GetCoordinates()));
        }
        return _installedShips;
    }

    public TileGameplayData[,] GetTilesGameplayData()
    {
        TileGameplayData[,] _gameplayData = new TileGameplayData[gridData.gridSize, gridData.gridSize];
        for (int i = 0; i < gridData.gridSize; ++i)
        {
            for (int j = 0; j < gridData.gridSize; ++j)
            {
                _gameplayData[j, i] = new TileGameplayData(new Coordinates(j, i)); 
            }
        }
        foreach (var tile in gridData.occupiedTiles)
        {
            foreach (var coordinate in tile.GetCoordinates())
            {
                int i = coordinate.z;
                int j = coordinate.x;

                if (!gridData.ContainsEmpty(new Coordinates(j, i)))
                {
                    _gameplayData[i, j].status = TileGameplayStatus.shipCell;
                    _gameplayData[i, j].ID = tile.ID;
                    _gameplayData[i, j].PlacedObjectID = tile.placedObjectID[0];
                } 
            }
        }
        return _gameplayData;
    }

    public int[,] GetBinaryTilesGameplayData()
    {
        int[,] _gameplayData = new int[gridData.gridSize, gridData.gridSize];
        for(int i = 0; i < gridData.gridSize; ++i)
        {
            for (int j = 0; j < gridData.gridSize; ++j)
            {
                _gameplayData[i, j] = 0;
            }
        }
        foreach (var tile in gridData.occupiedTiles)
        {
            
            foreach(var coordinate in tile.GetCoordinates())
            {
                int i = coordinate.z;
                int j = coordinate.x;
                if (!gridData.ContainsEmpty(new Coordinates(j, i)))
                {
                    _gameplayData[i, j] = 1;
                }
            }
        }
        return _gameplayData;
    }

}
