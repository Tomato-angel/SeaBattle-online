using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AbilitiesSpritesDatabaseElement
{
    [SerializeField] private int _abilityID;
    public int AbilityID => _abilityID;
    /*
    [SerializeField] private string _description;
    public string Description => _description;*/
    
    [SerializeField] private Sprite _abilitySprite;
    public Sprite AbilitySprite => _abilitySprite; 
}

[CreateAssetMenu(fileName = "AbilitiesSpritesDatabase", menuName = "Scriptable Objects/AbilitiesSpritesDatabase")]
public class AbilitiesSpritesDatabase : ScriptableObject
{
    [SerializeField] private List<AbilitiesSpritesDatabaseElement> _abilitiesSprites;

    public Sprite Get(int abilityID)
    {
        foreach (var abilitySprite in _abilitiesSprites)
        {
            if (abilitySprite.AbilityID == abilityID)
                return abilitySprite.AbilitySprite;
        }
        return null;
    }
}
