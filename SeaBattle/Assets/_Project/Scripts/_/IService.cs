using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.VisualScripting;


public interface IService
{
    public Player CurrentPlayer { get; }
    public Match CurrentMatch { get; }
    
    public void BindMatch(Match match);
    public void UnbindMatch();

}

