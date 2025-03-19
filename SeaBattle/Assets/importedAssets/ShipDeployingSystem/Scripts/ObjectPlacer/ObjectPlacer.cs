using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private Dictionary<Vector2, GameObject> objects = new ();
    [SerializeField]
    private List<Vector2> placer = null;

    public void PlaceOnGrid(List<Coordinates> coordinate, 
        GameObject gameObject, 
        Vector3 position, 
        float rotation)
    {
        GameObject newTile = Instantiate(gameObject, position, Quaternion.Euler(0, rotation, 0), transform);
        foreach (var pos in coordinate)
        {
            Place(pos, newTile);
        }
    }
    public void PlaceEmptyOnGrid(Coordinates pos,
        GameObject gameObject,
        Vector3 position,
        float rotation)
    {
        GameObject newTile = Instantiate(gameObject, position, Quaternion.Euler(0, rotation, 0), transform);
        Place(pos, newTile);
    }
    public void Place(Coordinates pos, GameObject gameObject)
    {
        gameObject.name = $"{gameObject.name}: [{pos.x}; {pos.z}]";
        objects.Add(new Vector2(pos.x, pos.z), gameObject);
        placer.Add(new Vector2(pos.x, pos.z));
    }

    public void RemoveFromGrid(List<Coordinates> tilePos)
    {
        if (tilePos == null)
            return;
        foreach (Coordinates gridPos in tilePos)
        {
            GameObject newTilee;
            Vector2 newCoordinate = new Vector2(gridPos.x,gridPos.z);
            if (objects.ContainsKey(newCoordinate))
                newTilee = objects[newCoordinate];
            else
            {
                throw new System.Exception($"Key [{gridPos.x},{gridPos.z}] was not found");
            }
            if (!objects.Keys.Contains(new Vector2(gridPos.x, gridPos.z)))
                return;
            Destroy(objects[new Vector2(gridPos.x, gridPos.z)]);
            placer.Remove(new Vector2(gridPos.x, gridPos.z));
            objects.Remove(new Vector2(gridPos.x, gridPos.z));
        }
    }
}