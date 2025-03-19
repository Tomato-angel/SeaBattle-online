using System;

[Serializable]
public class Ship
{
    public int shipID { get; private set; }
    public int shipAmount;
    public Ship(int shipID, int shipAmount)
    {
        this.shipID = shipID;
        this.shipAmount = shipAmount;
    }
}
