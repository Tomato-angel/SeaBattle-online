using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


public abstract class Service : IService
{
    private Player _currentPlayer;
    public Player CurrentPlayer { get => _currentPlayer; }
    private Match _currentMatch;
    public Match CurrentMatch { get => _currentMatch; }

    public virtual Service ServiceInstance { get; }

    public virtual void BindMatch(Match match)
    {
        throw new NotImplementedException();
    }

    public virtual void UnbindMatch()
    {
        throw new NotImplementedException();
    }
}

