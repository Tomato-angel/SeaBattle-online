using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;


public class MainMenu : MonoBehaviour, IInitializable
{
    [SerializeField] Player _localPlayer;

    [SerializeField] GameplayMenu _gameplayMenu;
    [SerializeField] PauseMenu _pauseMenu;
    [SerializeField] GameObject _deployingMenu;
    [SerializeField] GameObject _winMenu;
    [SerializeField] GameObject _loseMenu;

    public void LeaveGameplay()
    {
        _localPlayer.CmdLeaveGame();
    }


    public void HideAllMenu()
    {
        _gameplayMenu.gameObject.SetActive(false);
        _pauseMenu.gameObject.SetActive(false);
        _deployingMenu.gameObject.SetActive(false);
    }

    public void ShowGameplayMenu()
    {
        HideAllMenu();
        _gameplayMenu.gameObject.SetActive(true);
    }

    public void ShowPauseMenu()
    {
        HideAllMenu();
        _pauseMenu.gameObject.SetActive(true);
    }

    public void ShowWinMenu()
    {
        HideAllMenu();
        _winMenu.gameObject.SetActive(true);
    }

    public void ShowLoseMenu()
    {
        HideAllMenu();
        _loseMenu.gameObject.SetActive(true);
    }

    public void Initialize()
    {
        _gameplayMenu.Initialize();
        _pauseMenu.Initialize();
        //_deployingMenu.Initialize();

        if (ProjectManager.root == null) return;
        if (ProjectManager.root.LocalPlayer == null) return;
        _localPlayer = ProjectManager.root.LocalPlayer;

        _localPlayer.youLose += ShowLoseMenu;
        _localPlayer.youWin += ShowWinMenu;

    }
}
