using Mirror;
using Scripts.Matchmaking;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSceneManager : MonoBehaviour
{

    [SerializeField] MenuPanel _currentPanel;

    [SerializeField] MainMenuPanel _mainMenuPanel;
    [SerializeField] HostGamePanel _hostGamePanel;
    [SerializeField] JoinGamePanel _joinGamePanel;
    [SerializeField] FindGamePanel _findGamePanel;

    [Client]
    void Start()
    {
        _currentPanel = _mainMenuPanel;
        ShowOnlyCurrentPanel();
    }

    [Client]
    public void ShowOnlyCurrentPanel()
    {
        _mainMenuPanel.SetActive(_mainMenuPanel == _currentPanel);
        _hostGamePanel.SetActive(_hostGamePanel == _currentPanel);
        _joinGamePanel.SetActive(_joinGamePanel == _currentPanel);
        _findGamePanel.SetActive(_findGamePanel == _currentPanel);
    }

    [Client]
    public void ToMainMenu()
    {
        _currentPanel = _mainMenuPanel;
        ShowOnlyCurrentPanel();
    }

    [Client]
    public void ToFindGameMenu()
    {
        _currentPanel = _findGamePanel;
        ShowOnlyCurrentPanel();
    }
    [Client]
    public void ToHostGameMenu()
    {
        _currentPanel = _hostGamePanel;
        ShowOnlyCurrentPanel();
    }
    [Client]
    public void ToJoinGameMenu()
    {
        _currentPanel = _joinGamePanel;
        ShowOnlyCurrentPanel();
    }
    
}