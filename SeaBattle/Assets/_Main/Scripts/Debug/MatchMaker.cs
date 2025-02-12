using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

namespace Matchmaking
{
    public class MatchMaker
    {
        #region [Реализация синглтона]
        private static MatchMaker _instance;
        public static MatchMaker Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MatchMaker();
                }
                return _instance;
            }
        }
        #endregion

        #region [Реализация работы с единым хранилищем матчей]

        private int _maxMatchPlayers = 2;
        public int MaxMatchPlayers { get => _maxMatchPlayers; }

        private int _matchesLimitCount;
        public int MatchesLimitCount { get => _matchesLimitCount; }


        private Dictionary<string, Match> _matches;
        public Dictionary<string, Match> Matches { get => _matches; }

        // Дополнить везде Callback.Invoke() нужно будет тут написать возможность заранее брать из класса коды коллбеков

        private bool CreateMatch(out string key, out Match match, bool isOpen = false, Action<Callback> callback = null)
        {
            key = string.Empty;
            match = Match.Empty;
            if (_matches.Count < _matchesLimitCount)
            {
                string tmpKey = string.Empty;
                if (KeyGenerator.Instance.TryGenerateKey(out tmpKey))
                {
                    key = tmpKey;
                    Match tmpMatch = new Match(tmpKey, isOpen);
                    match = tmpMatch;
                    _matches.Add(tmpKey, tmpMatch);
                    Debug.Log($"-- Created game with key: {tmpKey}");
                    return true;
                }
                return false;
            }
            return false;
        }

        private bool DeleteMatch(string key, Action<Callback> callback = null)
        {
            DisconnectAllPlayersFromMatch(key);
            if (_matches.ContainsKey(key))
            {
                KeyGenerator.Instance.RemoveKey(key);
                _matches.Remove(key);
                return true;
            }
            return false;
        }


        private void RemoveEmptyMatches()
        {
            foreach ((string key, Match match) in _matches)
            {
                if (match.PlayersCount == 0)
                {
                    DeleteMatch(key);
                }
            }
        }

        private bool ConnectPlayerToMatch(string key, Player player, Action<Callback> callback = null)
        {
            Debug.Log($"-- Starts connect to game with key: {key} the Player: {player.connectionToClient.connectionId}");
            if (_matches.ContainsKey(key))
            {
                if (_matches[key].PlayersCount < _maxMatchPlayers)
                {
                    _matches[key].AddPlayer(player);

                    if (_matches[key].PlayersCount == _maxMatchPlayers)
                    {
                        _matches[key].IsFull = true;
                    }

                    player.CurrentMatch = _matches[key];
                    player.NetworkMatch.matchId = _matches[key].ID;

                    return true;
                }
                return false;
            }
            return false;
        }

        // При подключении и отключении игрока к матчу должно меняться некоторое PlayerState - состояние игрока
        // по которому отслеживается переход по гл.меню и прочее
        private bool DisconnectPlayerFromMatch(string key, Player player, Action<Callback> callback = null)
        {
            Debug.Log($"-- Starts disconnect the Player: {player.connectionToClient.connectionId} from match with key: {key}");
            if (_matches.ContainsKey(key))
            {
                if (_matches[key].DeletePlayer(player))
                {
                    /* Нужно придумать куда вынести очистку пустых матчей или мёртвых матчей
                    if (_matches[key].PlayersCount == 0)
                    {
                        DeleteMatch(key);
                    }
                    */

                    player.CurrentMatch = null;
                    player.NetworkMatch.matchId = Guid.Empty;

                    return true;
                }
                return false;
            }
            return false;
        }

        private bool DisconnectAllPlayersFromMatch(string key, Action<Callback> callback = null)
        {
            Debug.Log($"-- Starts disconnect All Players from match with key: {key}");
            for(int i = 0; i < _matches[key].PlayersCount; ++i)
            {
                Player tmpPlayer;
                _matches[key].GetPlayer(i, out tmpPlayer);
                DisconnectPlayerFromMatch(key, tmpPlayer);
            }
            if(_matches[key].PlayersCount == 0)
                return true;
            return false;
        }

        private bool ForceDisconnectPlayerFromMatch(Player player, Action<Callback> callback = null)
        {
            player.CurrentMatch = null;
            player.NetworkMatch.matchId = Guid.Empty;

            return true;
        }


        // Реализовать некоторый PlayerNotifier который будет передавать на клиент данные об ошибке с сервера о его операции
        public bool HostGame(Player player, Action<Callback> callback = null)
        {
            Debug.Log($"- Player connId: {player.connectionToClient.connectionId}, starts host game");

            string key;
            Match match;
            if (CreateMatch(out key, out match))
            {
                if (ConnectPlayerToMatch(key, player, callback))
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool JoinGame(Player player, string key, Action<Callback> callback = null)
        {
            Debug.Log($"- Player connId: {player.connectionToClient.connectionId}, starts join game");
            if (ConnectPlayerToMatch(key, player, callback))
            {
                return true;
            }
            return false;
        }

        public bool SearchGame(Player player, Action<Callback> callback = null)
        {
            Debug.Log($"- Player connId: {player.connectionToClient.connectionId}, starts search game");

            Debug.Log($"-- Try connect to opens matches");
            foreach ((string tmpKey, Match tmpMatch) in _matches)
            {
                if (tmpMatch.IsOpen)
                {
                    if (tmpMatch.PlayersCount < _maxMatchPlayers)
                    {
                        if (ConnectPlayerToMatch(tmpKey, player, callback))
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }

            Debug.Log($"-- Try create the open matches");
            string key;
            Match match;
            if (CreateMatch(out key, out match))
            {
                if (ConnectPlayerToMatch(key, player, callback))
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool LeaveGame(Player player, string key, Action<Callback> callback = null)
        {
            Debug.Log($"- Player connId: {player.connectionToClient.connectionId}, leave the game from match key: {key}");
            if (DisconnectPlayerFromMatch(key, player, callback))
            {
                return true;
            }
            return false;
        }

        #endregion

        public MatchMaker(int maxMatchPlayers = 2, int matchesLimitCount = 10)
        {
            _maxMatchPlayers = maxMatchPlayers;
            _matchesLimitCount = matchesLimitCount;
            _matches = new Dictionary<string, Match>();
        }

    }
}

