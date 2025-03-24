using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class GamingSegmentSpriteDBElement
{
    [SerializeField] OpponentGamingSegmentStatus _status;
    public OpponentGamingSegmentStatus Status => _status;
    [SerializeField] string _key;
    public string Key => _key;
    [SerializeField] Sprite _sprite;
    public Sprite Sprite => _sprite;
}
 
[CreateAssetMenu(fileName = "GamingSegmentSpritesDatabase", menuName = "Scriptable Objects/GamingSegmentSpritesDatabase")]
[Serializable]
public class GamingSegmentSpritesDatabase : ScriptableObject
{
    [SerializeField] List<GamingSegmentSpriteDBElement> _gamingSegmentSprites;

    public Sprite Get(string key)
    {
        foreach(var gamingSegmentSprite in _gamingSegmentSprites)
        {
            if(gamingSegmentSprite.Key == key)
            {
                return gamingSegmentSprite.Sprite;
            }
        }
        return null;
    }

    public Sprite Get(OpponentGamingSegmentStatus status)
    {
        foreach (var gamingSegmentSprite in _gamingSegmentSprites)
        {
            if (gamingSegmentSprite.Status == status)
            {
                return gamingSegmentSprite.Sprite;
            }
        }
        return null;
    }
}
