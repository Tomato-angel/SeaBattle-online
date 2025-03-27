using DI;
using Mirror;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameplaySceneManager : MonoBehaviour, IInitializable
{
    [SerializeField] Player _localPlayer;
    [SerializeField] Player _opponentPlayer;

    [SerializeField] GameplayManager _gameplayManager;

    [SerializeField] Chat _chat;
    [SerializeField] CameraViewController _cameraViewController;
    [SerializeField] OpponentGamingField _opponentGamingField;
    [SerializeField] ShipsDeploymentSystem _shipDeploymentSystem;


    public void Initialize()
    {
        if(ProjectManager.root != null)
        {
            _localPlayer = ProjectManager.root.LocalPlayer;
            _localPlayer.CmdPrepareForStartGameplay();

            _opponentPlayer = _localPlayer.GetAnotherPlayerFromLocalScene();

            _opponentGamingField.SetOpponentPlayer(_opponentPlayer);
        }
        else
        {

        }
        StartDeployShip();
    }

    public void StartDeployShip()
    {
        _cameraViewController.ToBoardViewPoint();
        _cameraViewController.IsMovable = false;
        _cameraViewController.IsRotatable = false;
        _cameraViewController.SetActive(false);

        _shipDeploymentSystem.SetActive(true);
        _shipDeploymentSystem.allShipsDeployed += ShowCompleteDeployShipButton;

        ShowCompleteDeployShipButton(false);
    }
    public void CompleteDeployShip()
    {
        _cameraViewController.ToMainViewPoint();
        _cameraViewController.IsMovable = true;
        _cameraViewController.IsRotatable = true;
        _cameraViewController.SetActive(true);

        _shipDeploymentSystem.SetActive(false);
        _shipDeploymentSystem.allShipsDeployed -= ShowCompleteDeployShipButton;

        ShowCompleteDeployShipButton(false);


        GameplayData gameplayData = _shipDeploymentSystem.GetGameplayData();

        Debug.Log(gameplayData.ToString());
        //_gameplayManager.SetGameplayData(gameplayData);
        List<TileGameplayData> tilesGameplayData = new();
        for(int i = 0; i < 10; ++i)
        {
            for(int j = 0; j < 10; ++j)
            {
                tilesGameplayData.Add(gameplayData.TilesData[i,j]);
            }
        }
        _localPlayer.CmdSetTilesGameplayData(tilesGameplayData);
    }

    [SerializeField] GameObject _completeDeployShipButton;
    public void ShowCompleteDeployShipButton(bool isShow)
    {
        if (_completeDeployShipButton == null) return;
        _completeDeployShipButton.SetActive(isShow);
    }


}
