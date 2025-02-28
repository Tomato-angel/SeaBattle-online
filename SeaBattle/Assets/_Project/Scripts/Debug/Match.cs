using System;
using System.Collections.Generic;

namespace Scripts.Matchmaking
{
    [Serializable]
    public class Match
    {

        //Добавить некоторый MatchState для случаев когда матч запущен, игра идёт и всё закончено и прочее...
        public bool IsFull { get; set; }
        public bool IsOpen { get; set; }

        public string Key { get; set; }

        private Guid _id;
        public Guid ID { get => _id; }

        private List<Player> _players;
        public int PlayersCount { get => _players.Count; }

        //public Player[] GetAnotherPlayers()
        public bool GetAnotherPlayers(Player player, out Player[] anotherPlayers) 
        {
            anotherPlayers = new Player[_players.Count - 1];
            int i = 0;
            foreach (Player tmpPlayer in _players)
            {
                if (tmpPlayer != player)
                {
                    anotherPlayers[i] = tmpPlayer;
                    ++i;
                }
            }
            return true;
        }
        public bool GetAnotherPlayer(Player player, out Player anotherPlayer)
        {
            anotherPlayer = player;
            foreach (Player tmpPlayer in _players)
            {
                if (tmpPlayer != player)
                {
                    anotherPlayer = tmpPlayer;
                    return true;
                }
            }
            return false;
        }
        public bool GetPlayer(int index, out Player player)
        {
            player = null;
            if (index < _players.Count)
            {
                player = _players[index];
                return true;
            }
            return false;
        }
        public bool GetPlayerIndex(Player player, out int index)
        {
            index = -1;
            for(int i = 0; i < _players.Count; ++i)
            {
                if (_players[i] == player)
                {
                    index = i;
                    return true;
                } 
            }
            return false;
        }
        public bool AddPlayer(Player player)
        {
            if (!_players.Contains(player))
            {
                _players.Add(player);
                player.NetworkMatch.matchId = _id;
                return true;
            }
            return false;
        }
        public bool DeletePlayer(Player player)
        {
            if (_players.Remove(player))
            {
                return true;
            }
            return false;

        }
        public bool ContainsPlayer(Player player)
        {
            if (_players.Contains(player))
            {
                return true;
            }
            return false;
        }
        public void Clear()
        {
            _players.Clear();
        }


        public event Action StartMatch;
        public void OnStartMatch() => StartMatch?.Invoke();
        public event Action EndMatch;
        public void OnEndMatch() => EndMatch?.Invoke();


        public override string ToString()
        {
            string result = string.Empty;

            result = $"Match: {{\n" +
                $"IsFull: {IsFull}\n" +
                $"IsOpen: {IsOpen}\n" +
                $"Key: {Key}\n" +
                $"}}";

            return result;
        }


        public static Match Empty { get => new Match(); }


        public Match()
        {
            _id = new Guid();
            IsOpen = false;
            Key = string.Empty;
            _players = new List<Player>();
        }
        public Match(string key, bool isOpen)
        {
            _id = KeyGenerator.Instance.KeyToGui(key);
            IsOpen = isOpen;
            Key = key;
            _players = new List<Player>();
        }
    }
}

