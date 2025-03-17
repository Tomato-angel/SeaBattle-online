using Scripts.Matchmaking;
using System;

public interface IMatchBinding
{
    public Match CurrentMatch { get; }
    public void BindMatch(Match match) { }
    public void UnbindMatch() { }

}

