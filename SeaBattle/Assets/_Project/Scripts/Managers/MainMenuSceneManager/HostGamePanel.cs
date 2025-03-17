using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class HostGamePanel : PlayersMatchViewerPanel, IInitializable
{
    public void Initialize()
    {
        _localPlayer = ProjectManager.root.LocalPlayer;

        if (IsLocalPlayerValid())
        {
            _localPlayer.startHostGame += () => ShowMatchKey(_localPlayer.CurrentMatch.Key);
            _localPlayer.startHostGame += () => ShowReadyStatusChangeButtons(); 
            _localPlayer.startHostGame += () => ShowAllPlayersViews(_localPlayer.GetPlayersFromLocalScene());

            _localPlayer.leaveGame += () => HideMatchKey();
            _localPlayer.leaveGame += () => HideReadyStatusChangeButtons();
            _localPlayer.leaveGame += () => HideAllPlayersViews();

            _localPlayer.leaveGame += () => UnreadyToStart();
        }

        EventBus.playerConnectedToGame += (_) => UpdateAllPlayersViews(_localPlayer.GetPlayersFromLocalScene());
        EventBus.playerDisconnectedFromGame += (_) => UpdateAllPlayersViews(_localPlayer.GetPlayersFromLocalScene());
        EventBus.playerInstantiateOnScene += (_) => UpdateAllPlayersViews(_localPlayer.GetPlayersFromLocalScene());
        EventBus.playerDestroyFromScene += (_) => UpdateAllPlayersViews(_localPlayer.GetPlayersFromLocalScene());

        //EventBus.playerUpdatedProperties += (player) => UpdatePlayerView(player);
        EventBus.playerUpdatedProperties += (_) => UpdateAllPlayersViews(_localPlayer.GetPlayersFromLocalScene());

        EventBus.allPlayersReadyToStart += () => ActivateCountdown(() => EventBus.OnRequestForStartGameplay());

        //EventBus.playerConnectedToGame += (player) => ShowPlayerView(player);
        //EventBus.playerDisconnectedFromGame += (player) => HidePlayerView(player);
        /*
        EventBus.playerDisconnectedFromGame += (player) => UnreadyToStart();
        EventBus.playerDisconnectedFromGame += (player) => HidePlayerView(player);
        EventBus.playerDisconnectedFromGame += (player) => HideMatchKey();
        */
        //EventBus.playerInstantiateOnScene += (player) => ShowPlayerView(player);
        //EventBus.playerDestroyFromScene += (player) => HidePlayerView(player);

        HideAllPlayersViews();
        HideReadyStatusChangeButtons();
        HideMatchKey();
    }

    private void OnEnable()
    {
        HideAllPlayersViews();
        HideReadyStatusChangeButtons();
        HideMatchKey();
    }
    private void OnDisable()
    {
        HideAllPlayersViews();
        HideReadyStatusChangeButtons();
        HideMatchKey();
    }

}

