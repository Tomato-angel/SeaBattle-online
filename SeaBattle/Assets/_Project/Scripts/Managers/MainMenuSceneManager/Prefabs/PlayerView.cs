using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PlayerView : MonoBehaviour
{
    [field: SerializeField] private Player _localPlayer;
    [field: SerializeField] private AvatarsDatabase _avatarsDatabase;
    [field: SerializeField] private Image _playerIconView;
    [field: SerializeField] private Image _readyStatusView;
    [field: SerializeField] private TextMeshProUGUI _playerNickNameView;

    public void SetPlayer(Player player)
    {
        _localPlayer = player;
    }
    public Player GetPlayer()
    {
        return _localPlayer;
    }
    public void SetIcon(Player player)
    {
        _playerIconView.sprite = _avatarsDatabase.Get(player.playerData.avatarKey);
    }
    public void SetNickName(string nickName)
    {
        _playerNickNameView.text = nickName;
    }

    public void SetReadyStatus(bool isReady)
    {
        _readyStatusView.gameObject.SetActive(isReady);
    }
}
