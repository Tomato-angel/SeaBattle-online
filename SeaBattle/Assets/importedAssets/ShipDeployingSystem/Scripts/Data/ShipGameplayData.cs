using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ShipGameplayData
{
    public int ID { get; set; }
    public int Health { get; set; }
    public List<Coordinates> Coordinates { get; set; }


    public ShipGameplayData(int id, List<Coordinates> coordinates) 
    {
        ID = id;
        Health = id;
        Coordinates = coordinates;
    }
}

