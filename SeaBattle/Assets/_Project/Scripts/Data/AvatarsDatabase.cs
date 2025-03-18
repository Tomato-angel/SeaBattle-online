using System;
using UnityEngine;

[Serializable]
public class Pair<T1, T2>
{
    [SerializeField] T1 _value1;
    [SerializeField] T2 _value2;
}

[Serializable]
public class KeyValuePair<T1, T2>
{
    [SerializeField] private T1 _key;
    public T1 Key => _key;
    [SerializeField] private T2 _value;
    public T2 Value => _value;
}

[CreateAssetMenu(fileName = "AvatarsDatabase", menuName = "Scriptable Objects/AvatarsDatabase")]
[Serializable]
public class AvatarsDatabase : ScriptableObject
{
    [field: SerializeField] KeyValuePair<string, Sprite>[] _database;

    public Sprite Get(string key)
    {
        foreach(var element in _database)
        {
            if(element.Key == key)
            {
                return element.Value;
            }
        }
        return null;
    }

    public KeyValuePair<string, Sprite> Get(int index)
    {
        return _database[index];
    }

    public int Count { get => _database.Length; }

}
