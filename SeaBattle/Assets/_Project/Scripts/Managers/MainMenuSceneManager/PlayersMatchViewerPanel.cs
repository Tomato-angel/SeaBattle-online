using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PlayersMatchViewerPanel : MenuPanel
{
    [SerializeField] protected Player _localPlayer;

    [SerializeField] protected TextMeshProUGUI _matchKeyView = null;
    [SerializeField] protected TMP_InputField _matchKeyInputField = null;

    [SerializeField] protected GameObject _playersViewer = null;
    [SerializeField] protected GameObject _playerViewPrefab = null;
    [field: SerializeField] protected HashSet<PlayerView> _playersViews = new();

    [SerializeField] protected GameObject _readyButton = null;
    [SerializeField] protected GameObject _unreadyButton = null;

    [SerializeField] protected CountdownPanel _countDownPanel;

    public void ShowReadyButton()
    {
        if (_readyButton == null) return;
        _readyButton.SetActive(true);
    }
    public void ShowUnreadyButton()
    {
        if (_unreadyButton == null) return;
        _unreadyButton.SetActive(true);
    }
    public void HideReadyButton()
    {
        if (_readyButton == null) return;
        _readyButton.SetActive(false);
    }
    public void HideUnreadyButton()
    {
        if (_unreadyButton == null) return;
        _unreadyButton.SetActive(false);
    }

    protected void ShowMatchKeyInputField(bool isShow)
    {
        if (_matchKeyInputField == null) return;
        _matchKeyInputField.gameObject.SetActive(isShow);
    }

    protected void ShowReadyStatusChangeButtons()
    {
        if (_readyButton == null) return;
        if (_unreadyButton == null) return;
        if (_localPlayer.CurrentMatch == null) return;
        if (_localPlayer.CurrentMatch != null)
        {
            if (!_localPlayer.IsReady)
            {
                _readyButton.SetActive(true);
                _unreadyButton.SetActive(false);
            }
            else
            {
                _readyButton.SetActive(false);
                _unreadyButton.SetActive(true);
            }
        }
        else
        {
            _readyButton.SetActive(false);
            _unreadyButton.SetActive(false);
        }
    }
    protected void HideReadyStatusChangeButtons()
    {
        if (_readyButton == null) return;
        if (_unreadyButton == null) return;
        _readyButton.SetActive(false);
        _unreadyButton.SetActive(false);
    }


    protected void ShowMatchKey(string key)
    {
        if (key == null) return;
        if (_matchKeyView == null) return;
        _matchKeyView.text = key;
    }
    protected void HideMatchKey()
    {
        if (_matchKeyView == null) return;
        _matchKeyView.text = "********";
    }


    protected bool ContainsPlayerView(Player player)
    {
        if (player == null) return false;
        if (_playerViewPrefab == null) return false;
        if (_playersViewer == null) return false;

        bool contains = false;
        foreach (PlayerView playerView in _playersViews)
        {
            if (playerView.GetPlayer() == player)
            {
                contains = true;
                break;
            }
        }
        return contains;
    }
    protected void ShowPlayerView(Player player)
    {
        if (player == null) return;
        if (player.playerData == null) return;
        if (_playerViewPrefab == null) return;
        if (_playersViewer == null) return;
        if (ContainsPlayerView(player)) return;
        
        PlayerView playerView = Instantiate(_playerViewPrefab, _playersViewer.transform).GetComponent<PlayerView>();
        playerView.SetNickName(player.playerData.nickName);
        playerView.SetReadyStatus(player.IsReady);
        playerView.SetPlayer(player);
        _playersViews.Add(playerView);
        /*
        player.updateProperties += async () => {
            UpdatePlayerView(player);
        };*/
    }
    protected void HidePlayerView(Player player)
    {
        if (player == null) return;
        if (player.playerData == null) return;
        if (_playerViewPrefab == null) return;
        if (_playersViewer == null) return;
        if (ContainsPlayerView(player)) return;

        foreach (PlayerView playerView in _playersViews)
        {
            if (playerView.GetPlayer() == player)
            {
                _playersViews.Remove(playerView);
                Destroy(playerView.gameObject);
                break;
            }
        }
        return;
    }
    protected void UpdatePlayerView(Player player)
    {
        if (player == null) return;
        if (player.playerData == null) return;
        if (_playerViewPrefab == null) return;
        if (_playersViewer == null) return;
        if (!ContainsPlayerView(player)) return;
        foreach (PlayerView playerView in _playersViews)
        {
            if (playerView.GetPlayer() == player)
            {
                playerView.SetNickName(player.playerData.nickName);
                playerView.SetReadyStatus(player.IsReady);
                playerView.SetPlayer(player);
            }
        }
    }

    protected void ShowAllPlayersViews(Player[] players)
    {
        if (players == null) return;
        foreach (Player player in players)
        {
            ShowPlayerView(player);
        }
    }
    protected void HideAllPlayersViews()
    {
        foreach (PlayerView playerView in _playersViews)
        {
            Destroy(playerView.gameObject);
        }
        _playersViews.Clear();
    }
    protected void UpdateAllPlayersViews(Player[] players)
    {
        HideAllPlayersViews();
        ShowAllPlayersViews(players);
    }

    protected bool IsLocalPlayerValid()
    {
        return _localPlayer != null;
    }

    public void CreateGame()
    {
        _localPlayer.CmdHostGame();
    }
    public void JoinGame()
    {
        string key = _matchKeyInputField.text;
        if (key == null) return;
        Debug.Log(key);
        Debug.Log(_localPlayer);
        Debug.Log(_localPlayer.isLocalPlayer);
        _localPlayer.CmdJoinGame(key);
        _matchKeyInputField.text = string.Empty;
    }
    public void LeaveGame()
    {
        _localPlayer.CmdLeaveGame();
    }
    public void ReadyToStart()
    {
        _localPlayer.CmdUpdateReadyStatus(true);
    }
    public void UnreadyToStart()
    {
        _localPlayer.CmdUpdateReadyStatus(false);
    }

    public void ActivateCountdown(Action callback = null)
    {
        if (_countDownPanel == null) return;
        _countDownPanel.ActivateCountdown(() => { callback?.Invoke(); });
    }

}
