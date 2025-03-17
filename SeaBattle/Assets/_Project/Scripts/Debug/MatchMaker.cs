using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Mirror;
using Mirror.BouncyCastle.Asn1.BC;

namespace Scripts.Matchmaking
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

        // Необходимо обновить т.к. синхронный метод обзавёлся проверкой на пустые матчи и теперь умеет выдывать значение пустого матча
        private async Task CreateMatchAsync(Action<bool, string, Match> callback, bool isOpen = false)
        {
            await Task.Run(async () =>
            {
                Debug.Log($"\t-- Starts create the match");
                if (_matches.Count < _matchesLimitCount)
                {
                    string key = string.Empty;
                    Match match = Match.Empty;
                    await KeyGenerator.Instance.GenerateUniqueKeyAsync((uniqueKey) =>
                    {
                        key = uniqueKey;
                    });
                    if (key != string.Empty)
                    {
                        match = new Match(key, isOpen);
                        _matches.Add(key, match);
                        
                        Debug.Log($"\t-- Created match with key: {key}");
                        callback?.Invoke(true, key, match);

                        return;
                    }
                }
                Debug.Log($"\t-- Can't create the match");
                callback?.Invoke(false, string.Empty, null);
            });
        }

        private bool CreateMatch(out (string, Match) pairKeyMatch, bool isOpen = false)
        {
            Debug.Log($"\t-- Starts create the match");
            if (_matches.Count < _matchesLimitCount)
            {
                string key = string.Empty;
                if (KeyGenerator.Instance.TryGenerateKey(out key))
                {
                    Match match = new Match(key, isOpen);
                    _matches.Add(key, match);
                    Debug.Log($"\t-- Created match with key: {key}");
                    pairKeyMatch = (key, match);
                    return true;
                }
            }
            else
            {
                if (FindEmptyMatch(out Match match))
                {
                    string key = string.Empty;
                    if (KeyGenerator.Instance.TryGenerateKey(out key))
                    {
                        _matches.Remove(match.Key);
                        match = new Match(key, isOpen);
                        _matches.Add(key, match);
                        Debug.Log($"\t-- Created match with key: {key}");
                        pairKeyMatch = (key, match);
                        return true;
                    }
                }
            } 
            Debug.Log($"\t-- Can't create the match");
            pairKeyMatch = (null, null);
            return false;
        }
        private bool DebugCreateMatch(string matchKey, bool isOpen = false)
        {
            Debug.Log($"\t-- Starts create the match");
            if (_matches.Count < _matchesLimitCount)
            {

                Match match = new Match(matchKey, isOpen);
                _matches.Add(matchKey, match);
                Debug.Log($"\t-- Created match with key: {matchKey}");
                return true;

            }
            Debug.Log($"\t-- Can't create the match");
            return false;
        }

        private bool DeleteMatch(string key)
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

        private bool FindEmptyMatch(out Match emptyMatch)
        {
            foreach ((string key, Match match) in _matches)
            {
                if (match.PlayersCount == 0)
                {
                    emptyMatch = match;
                    return true;
                }
            }
            emptyMatch = null;
            return false;

        }

        private int RemoveEmptyMatches()
        {
            int emptyMatchesCount = 0;
            foreach ((string key, Match match) in _matches)
            {
                if (match.PlayersCount == 0)
                {
                    ++emptyMatchesCount;
                    DeleteMatch(key);
                }
            }
            return emptyMatchesCount;
        }

        private void UpdateCurrentMatchInfoForAllMatchPlayers(string key)
        {
            if(_matches.ContainsKey(key))
            {
                foreach(Player player in _matches[key].GetPlayers())
                {
                    player.CurrentMatch = _matches[key];
                }
            }
        }
        /*
        public async void RunEmptyMatchesCleanupCycleAsync(int millisecondsDelay)
        {
            while (true)
            {
                Debug.Log("AAAAAAAAAAAAAAAAA");
                int emptyMatchesCount = RemoveEmptyMatches();
                if (emptyMatchesCount > 0)
                {
                    Debug.Log($"[MatchMaker] {emptyMatchesCount} Empty matches were removed");
                }
                await Task.Delay(millisecondsDelay);
            }
        }
        */

        private bool ConnectPlayerToMatch(string key, Player player)
        {
            Debug.Log($"\t-- Starts connect to match with key: {key} the Player: {player.connectionToClient.connectionId}");
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

                    Debug.Log($"\t-- Connect to match with key: {key} the Player: {player.connectionToClient.connectionId}");
                    //UpdateCurrentMatchInfoForAllMatchPlayers(key);
                    return true;
                }
            }
            Debug.Log($"\t-- Can't connect to match with key: {key}");
            return false;
        }

        // При подключении и отключении игрока к матчу должно меняться некоторое PlayerState - состояние игрока
        // по которому отслеживается переход по гл.меню и прочее
        private bool DisconnectPlayerFromMatch(string key, Player player)
        {
            Debug.Log($"\t-- Starts disconnect the Player: {player.connectionToClient.connectionId} from match with key: {key}");
            if (_matches.ContainsKey(key))
            {
                if (_matches[key].DeletePlayer(player))
                {
                    if (_matches[key].PlayersCount == 0)
                    {
                        DeleteMatch(key);
                    }
                    

                    player.CurrentMatch = null;
                    player.NetworkMatch.matchId = Guid.NewGuid();

                    Debug.Log($"\t-- Player: {player.connectionToClient.connectionId} was disconnected from match with key: {key}");
                    return true;
                }
            }
            Debug.Log($"\t-- Player: {player.connectionToClient.connectionId} can't disconnected from match with key: {key}");
            return false;
        }
        private bool DisconnectAllPlayersFromMatch(string key)
        {
            Debug.Log($"\t-- Starts disconnect All Players from match with key: {key}");
            for(int i = 0; i < _matches[key].PlayersCount; ++i)
            {
                _matches[key].GetPlayer(i, out Player tmpPlayer);
                DisconnectPlayerFromMatch(key, tmpPlayer);
            }
            if(_matches[key].PlayersCount == 0)
            {
                Debug.Log($"\t-- All Players was disconected from match with key: {key}");
                return true;
            }
            Debug.Log($"\t-- Can't disconnected All Players from match with key: {key}");
            return false;
        }

        private bool ForceDisconnectPlayerFromMatch(Player player)
        {
            /*
            Match match = player.CurrentMatch;
            if(match != null)
                if(match.ContainsPlayer(player))
                    match.DeletePlayer(player);
            */
            player.CurrentMatch = null;
            player.NetworkMatch.matchId = Guid.Empty;

            return true;
        }
        private bool ForceDisconnectAllPlayersFromMatch(string key)
        {
            Debug.Log($"\t-- Starts FORCE disconnect All Players from match with key: {key}");
            for (int i = 0; i < _matches[key].PlayersCount; ++i)
            {
                _matches[key].GetPlayer(i, out Player tmpPlayer);
                ForceDisconnectPlayerFromMatch(tmpPlayer);
            }
            return true;
        }


        public async void HostMatchAsync(Player player, Action<bool> callback = null)
        {
            Debug.Log($"- Player connId: {player.connectionToClient.connectionId}, starts host match");

            //ForceDisconnectPlayerFromMatch(player);

            bool isSuccessfullyCreated = false;
            string key = string.Empty;
            Match match;
            await CreateMatchAsync((tmpIsSuccessfullyCreated, tmpKey, tmpMatch) => {
                isSuccessfullyCreated = tmpIsSuccessfullyCreated;
                key = tmpKey;
                match = tmpMatch;
            });
            if (isSuccessfullyCreated)
            {
                bool isSuccessfullyConnected =
                await Task.Run(() => ConnectPlayerToMatch(key, player));
                if (isSuccessfullyConnected)
                {
                    callback?.Invoke(isSuccessfullyConnected);
                    return;
                }
            }
            Debug.Log($"\t-- Player connId: {player.connectionToClient.connectionId}, can't host match");
            callback?.Invoke(false);
        }

        // Устаревшее : синхронный хостинг матча
        // БЫЛ ПЕРЕДЕЛАН, НУЖНО ИСПРАВИТЬ АСИНХРОННУЮ ВАРИАЦИЮ
        // Добавилась обработка, теперь при запросе на хостинг игрок автоматически отключается от своего предыдущего матча
        public void HostMatch(Player player, Action<bool> callback = null)
        {
            Debug.Log($"- Player connId: {player.connectionToClient.connectionId}, starts host match");

            //ForceDisconnectPlayerFromMatch(player);
            if(player.CurrentMatch != null)
                DisconnectPlayerFromMatch(player.CurrentMatch.Key, player);

            if (CreateMatch(out (string, Match) pairKeyMatch, true))
            {
                string key = pairKeyMatch.Item1;
                callback?.Invoke(ConnectPlayerToMatch(key, player));
                return;
            }
            Debug.Log($"\t-- Player connId: {player.connectionToClient.connectionId}, can't host match");
            callback?.Invoke(false);
        }
        public void DebugHostMatch(Player player, string matchKey, Action<bool> callback = null)
        {
            Debug.Log($"- Player connId: {player.connectionToClient.connectionId}, starts host match");

            if (DebugCreateMatch(matchKey, true))
            {
                callback?.Invoke(ConnectPlayerToMatch(matchKey, player));
                return;
            }
            Debug.Log($"\t-- Player connId: {player.connectionToClient.connectionId}, can't host match");
            callback?.Invoke(false);
        }

        public async void JoinMatchAsync(Player player, string key, Action<bool> callback = null)
        {
            Debug.Log($"- Player connId: {player.connectionToClient.connectionId}, starts join match with key: {key}");
            await Task.Run(() => {
                // Можно упростить до: 
                // callback?.Invoke(ConnectPlayerToMatch(key, player));
                bool isSuccessfullyConnected = ConnectPlayerToMatch(key, player);
                callback?.Invoke(isSuccessfullyConnected);
            });
        }

        // Устаревшее : синхронное подключение к матчу
        public void JoinMatch(Player player, string key, Action<bool> callback = null)
        {
            Debug.Log($"- Player connId: {player.connectionToClient.connectionId}, starts join match with key: {key}");
            bool isSuccessfullyConnected = ConnectPlayerToMatch(key, player);
            callback?.Invoke(isSuccessfullyConnected);
        }


        public async void SearchMatchAsync(Player player, Action<bool> callback = null)
        {
            {
                Debug.Log($"- Player connId: {player.connectionToClient.connectionId}, starts search game");
                Debug.Log($"\t-- Try connect to opens matches");
                foreach ((string key, Match match) in _matches)
                {
                    if (match.IsOpen && match.PlayersCount < _maxMatchPlayers)
                    {
                        bool isSuccessfullyConnected =
                        await Task.Run(() => ConnectPlayerToMatch(key, player));
                        if (isSuccessfullyConnected)
                        {
                            Debug.Log($"\t-- Player connected to match: {key}");
                            callback?.Invoke(isSuccessfullyConnected);
                            return;
                        }
                    }
                }
            }
            {
                Debug.Log($"\t-- Try create the open match");

                bool isSuccessfullyCreated = false;
                string key = string.Empty;
                Match match;
                await CreateMatchAsync((tmpIsSuccessfullyCreated, tmpKey, tmpMatch) =>
                {
                    isSuccessfullyCreated = tmpIsSuccessfullyCreated;
                    key = tmpKey;
                    match = tmpMatch;
                });
                if (isSuccessfullyCreated)
                {
                    bool isSuccessfullyConnected =
                    await Task.Run(() => ConnectPlayerToMatch(key, player));
                    if (isSuccessfullyConnected)
                    {
                        Debug.Log($"\t-- Player connected to newly created match: {key}");
                        callback?.Invoke(isSuccessfullyConnected);
                        return;
                    }
                }
                Debug.Log($"\t-- Can't search match");
                callback?.Invoke(false);
            }
        }
        
        // Устаревшее : синхронный поиск матча
        public void SearchMatch(Player player, Action<bool> callback = null)
        {
            Debug.Log($"- Player connId: {player.connectionToClient.connectionId}, starts search game");
            Debug.Log($"\t-- Try connect to opens matches");
            foreach ((string key, Match match) in _matches)
            {
                if (match.IsOpen && match.PlayersCount < _maxMatchPlayers)
                {
                    bool isSuccessfullyConnected = ConnectPlayerToMatch(key, player);
                    if (isSuccessfullyConnected)
                    {
                        Debug.Log($"\t-- Player connected to match: {key}");
                        callback?.Invoke(isSuccessfullyConnected);
                        return;
                    }
                }
            }
            Debug.Log($"\t-- Try create the open match");
            if (CreateMatch(out (string, Match) pairKeyMatch, true))
            {
                string key = pairKeyMatch.Item1;
                bool isSuccessfullyConnected = ConnectPlayerToMatch(key, player);
                if (isSuccessfullyConnected)
                {
                    Debug.Log($"\t-- Player connected to newly created match: {key}");
                    callback?.Invoke(isSuccessfullyConnected);
                    return;
                }
            }
            Debug.Log($"\t-- Can't search match");
            callback?.Invoke(false);
        }


        public async void LeaveMatchAsync(Player player, string key, Action<bool> callback = null)
        {
            Debug.Log($"- Player connId: {player.connectionToClient.connectionId}, starts leave the game from match key: {key}");
            await Task.Run(() =>
            {
                Debug.Log($"\t-- Player connId: {player.connectionToClient.connectionId}, left the game from match key: {key}");
                callback?.Invoke(DisconnectPlayerFromMatch(key, player));
                return;
            });
            Debug.Log($"\t-- Player connId: {player.connectionToClient.connectionId}, Can't left the game from match key: {key}");
        }

        // Устаревшее : синхронный выход из матча
        public void LeaveMatch(Player player, string key, Action<bool> callback = null)
        {
            Debug.Log($"- Player connId: {player.connectionToClient.connectionId}, starts leave the game from match key: {key}");
            if (DisconnectPlayerFromMatch(key, player))
            {
                Debug.Log($"\t-- Player connId: {player.connectionToClient.connectionId}, left the game from match key: {key}");
                callback?.Invoke(true);
                return;
            }
            callback?.Invoke(false);
            Debug.Log($"\t-- Player connId: {player.connectionToClient.connectionId}, Can't left the game from match key: {key}");
        }

        #endregion

        public MatchMaker(int maxMatchPlayers = 2, int matchesLimitCount = 5)
        {
            _maxMatchPlayers = maxMatchPlayers;
            _matchesLimitCount = matchesLimitCount;
            _matches = new Dictionary<string, Match>();
        }

    }

}

