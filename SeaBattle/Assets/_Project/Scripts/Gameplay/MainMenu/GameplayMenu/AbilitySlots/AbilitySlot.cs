using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class AbilitySlot : MonoBehaviour
{
    [SerializeField] Ability _currentAbility = null;
    public Ability CurrentAbility => _currentAbility;

    [SerializeField] AbilitiesSpritesDatabase _abilitiesSpritesDatabase;
    [SerializeField] Image _abilityView;
    public void SetEmpty()
    {
        _currentAbility = null;
        _abilityView.sprite = null;
        _abilityView.gameObject.SetActive(false);
    } 
    public void SetAbility(Ability ability)
    {
        _currentAbility = ability;
        _abilityView.sprite = _abilitiesSpritesDatabase.Get(ability.ID);
        _abilityView.gameObject.SetActive(true);
    }

    [SerializeField] bool _isActive;
    [SerializeField] GameObject _interactView;
    public void SetActive(bool isActive)
    {
        _isActive = isActive;
        _interactView.SetActive(isActive);
    }
}

