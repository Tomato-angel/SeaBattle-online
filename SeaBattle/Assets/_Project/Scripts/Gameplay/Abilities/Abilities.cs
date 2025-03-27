using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Abilities
{
}

public enum AbilityTypes
{
    Attack,
    Scan
}

[Serializable]
public class Ability
{
    [SerializeField] public int _id;
    public int ID => _id;
    [SerializeField] public string _description;
    public string Description => _description;
    [SerializeField] public AbilityTypes _type;
    public AbilityTypes Type => _type;
    [SerializeField] public List<Coordinates> _targetCoordinates;
    public List<Coordinates> TargetCoordinates => _targetCoordinates;

    public Ability()
    {
        _targetCoordinates = new();
        _type = AbilityTypes.Attack;
    }
}

public class SimpleShoot : Ability
{
    public SimpleShoot()
    {
        _id = 0;
        _targetCoordinates = new List<Coordinates>
        {
            new() {x = 0, z = 0},
        };
        _type = AbilityTypes.Attack;
        _description = "Простой выстрел по одной ячейке";
    }
}

public class QuadricShoot : Ability
{
    public QuadricShoot()
    {
        _id = 1;
        _targetCoordinates = new List<Coordinates>
        {
            new() {x = 0, z = 0},
            new() {x = 0, z = -1},
            new() {x = 0, z = 1},
            new() {x = -1, z = 0},
            new() {x = 1, z = 0},
            new() {x = 1, z = 1},
            new() {x = 1, z = -1},
            new() {x = -1, z = 1},
            new() {x = -1, z = -1},
        };
        _type = AbilityTypes.Attack;
        _description = "Выстрел по ячейкам площадью 3х3";
    }
}

public class LineShoot : Ability
{
    public LineShoot()
    {
        _id = 2;
        _targetCoordinates = new List<Coordinates>
        {
            new() {x = 0, z = 0},
            new() {x = 0, z = -1},
            new() {x = 0, z = 1},
            new() {x = 0, z = -2},
            new() {x = 0, z = 2},
            new() {x = 0, z = -3},
            new() {x = 0, z = 3},
            new() {x = 0, z = -4},
            new() {x = 0, z = 4},
            new() {x = 0, z = -5},
            new() {x = 0, z = 5},
            new() {x = 0, z = -6},
            new() {x = 0, z = 6},
            new() {x = 0, z = -7},
            new() {x = 0, z = 7},
            new() {x = 0, z = -8},
            new() {x = 0, z = 8},
            new() {x = 0, z = -9},
            new() {x = 0, z = 9},
        };
        _type = AbilityTypes.Attack;
        _description = "Выстрел, уничтожающий весь ряд ячеек соперника";
    }
}

public class QuadricScan : Ability
{
    public QuadricScan()
    {
        _id = 3;
        _targetCoordinates = new List<Coordinates>
        {
            new() {x = 0, z = 0},
            new() {x = 0, z = -1},
            new() {x = 0, z = 1},
            new() {x = -1, z = 0},
            new() {x = 1, z = 0},
            new() {x = 1, z = 1},
            new() {x = 1, z = -1},
            new() {x = -1, z = 1},
            new() {x = -1, z = -1},
        };
        _type = AbilityTypes.Scan;
        _description = "Просканируйте область ячеек площадью 3х3 и узнайте, где корабли соперника";
    }
}