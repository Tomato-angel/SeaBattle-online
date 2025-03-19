using System;

using UnityEngine;

[Serializable]
#pragma warning disable CS0660 // Тип определяет оператор == или оператор !=, но не переопределяет Object.Equals(object o)
public class Coordinates
#pragma warning restore CS0660 // Тип определяет оператор == или оператор !=, но не переопределяет Object.Equals(object o)
{
    [field: SerializeField]
    public int x = 1;
    [field: SerializeField]
    public int z = 1;

    public Coordinates(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public Coordinates(Coordinates pos, int x, int z)
    {
        this.x = pos.x + x;
        this.z = pos.z + z;
    }

    public Coordinates()
    {
        x = 0;
        z = 0;
    }

    public bool Equals(Coordinates p)
    {
        if (p is null)
        {
            return false;
        }

        // Optimization for a common success case.
        if (System.Object.ReferenceEquals(this, p))
        {
            return true;
        }

        // If run-time types are not exactly the same, return false.
        if (this.GetType() != p.GetType())
        {
            return false;
        }

        // Return true if the fields match.
        // Note that the base class is not invoked because it is
        // System.Object, which defines Equals as reference equality.
        return (x == p.x) && (z == p.z);
    }

    public static bool operator ==(Coordinates p, Coordinates q)
    {
        if (p is null)
        {
            if (q is null)
            {
                return true;
            }

            // Only the left side is null.
            return false;
        }
        // Equals handles case of null on right side.
        return p.Equals(q);
    }

    public static bool operator !=(Coordinates p, Coordinates q) => !(p == q);

    public override int GetHashCode() => (x, z).GetHashCode();

    public static Coordinates WorldToCoordinates(Vector3 position, Vector3 gridOrigin, float cellSize)
    {
        int x = (int)Mathf.RoundToInt((gridOrigin.x - position.x) / cellSize);
        int z = (int)Mathf.RoundToInt((gridOrigin.z - position.z) / cellSize);
        Coordinates cellPos = new Coordinates(Mathf.Abs(x), Mathf.Abs(z));
        return cellPos;
    }

    public static Vector3 CoordinatesToWorld(Coordinates coordinate, Vector3 gridOrigin, float cellSize)
    {
        Vector3 worldPos = new Vector3(
            gridOrigin.x + coordinate.x * cellSize, gridOrigin.y,
            gridOrigin.z - coordinate.z * cellSize);
        return worldPos;
    }

    public static Vector3 WorldInGrid(Vector3 position, Vector3 gridOrigin, float cellSize)
    {
        Coordinates temp = WorldToCoordinates(position, gridOrigin, cellSize);
        Vector3 worldCellPos = CoordinatesToWorld(temp, gridOrigin, cellSize);
        return worldCellPos;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////

    public static Coordinates WorldToCoordinatesXY(Vector3 position, Vector3 gridOrigin, float cellSize)
    {
        int x = (int)Mathf.RoundToInt((gridOrigin.x - position.x) / cellSize);
        int y = (int)Mathf.RoundToInt((gridOrigin.y - position.y) / cellSize);
        Coordinates cellPos = new Coordinates(Mathf.Abs(x), Mathf.Abs(y));
        return cellPos;
    }

    public static Vector3 CoordinatesToWorldXY(Coordinates coordinate, Vector3 gridOrigin, float cellSize)
    {
        Vector3 worldPos = new Vector3(
            gridOrigin.x + coordinate.x * cellSize,
            gridOrigin.y + coordinate.z * cellSize,
            gridOrigin.z);
        return worldPos;
    }

    public static Vector3 WorldInGridXY(Vector3 position, Vector3 gridOrigin, float cellSize)
    {
        Coordinates temp = WorldToCoordinatesXY(position, gridOrigin, cellSize);
        Vector3 worldCellPos = CoordinatesToWorldXY(temp, gridOrigin, cellSize);
        return worldCellPos;
    }
}