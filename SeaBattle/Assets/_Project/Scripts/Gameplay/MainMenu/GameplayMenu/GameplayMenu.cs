using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class GameplayMenu : MonoBehaviour, IInitializable
{
    [SerializeField] Player _localPlayer;

    [SerializeField] public bool _isActive = true;
    public void SetActive(bool isActive)
    {
        _isActive = isActive;
    }

    [SerializeField] TextMeshProUGUI _abilityDescription;
    [SerializeField] AbilitySlot[] _abilitySlots = new AbilitySlot[4];
    [SerializeField] TextMeshProUGUI _playerMoney;

    public void Initialize()
    {
        _isActive = true;

        if (ProjectManager.root == null) return;
        if (ProjectManager.root.LocalPlayer == null) return;
        _localPlayer = ProjectManager.root.LocalPlayer;

        UpdateGameplayMenu();
        _localPlayer.playerUsedAbility += UpdateGameplayMenu;
        //_localPlayer.opponentUsedAbility += UpdateGameplayMenu;
        //_localPlayer.opponentBuyAbility += UpdateGameplayMenu;
        _localPlayer.playerBuyAbility += UpdateGameplayMenu;

        _localPlayer.moneyUpdate += (money, opponentMoney) => { _playerMoney.text = money.ToString();  };
    }

    private IEnumerator DynamicShowAbilityDescription(string abilityDescription)
    {
        _abilityDescription.text = abilityDescription;
        _abilityDescription.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        _abilityDescription.text = "";
        _abilityDescription.gameObject.SetActive(false);
        yield return null;
    }
    public void ShowAbilityDescription(string abilityDescription)
    {
        StartCoroutine(DynamicShowAbilityDescription(abilityDescription));
    }
    public void HideAbilityDescription()
    {
        _abilityDescription.gameObject.SetActive(false);
    }

    public void UpdateGameplayMenu()
    {
        HideAbilityDescription();
        foreach (var abilitySlot in _abilitySlots)
        {
            abilitySlot.SetEmpty();
            abilitySlot.SetActive(false);
        }
        int index = 0;
        foreach(var ability in _localPlayer.abilities)
        {
            if (index >= _abilitySlots.Length) break;

            _abilitySlots[index].SetAbility(ability);
            _abilitySlots[index].SetActive(false);
            ++index;
        }
        //_playerMoney.text = _localPlayer.Money.ToString();
    }

    public void Update()
    {
        if (!_isActive) return;
        
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(_abilitySlots[0].CurrentAbility != null)
            {
                _abilitySlots[0].SetActive(true);
                _abilitySlots[1].SetActive(false);
                _abilitySlots[2].SetActive(false);
                _abilitySlots[3].SetActive(false);
                ShowAbilityDescription(_abilitySlots[0].CurrentAbility.Description);
                Debug.Log($"[Client] Player set new current ability with ID: {_abilitySlots[0].CurrentAbility.ID}");
                _localPlayer.CmdSetCurrentAbility(_abilitySlots[0].CurrentAbility);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_abilitySlots[1].CurrentAbility != null)
            {
                _abilitySlots[0].SetActive(false);
                _abilitySlots[1].SetActive(true);
                _abilitySlots[2].SetActive(false);
                _abilitySlots[3].SetActive(false);
                ShowAbilityDescription(_abilitySlots[1].CurrentAbility.Description);

                _localPlayer.CmdSetCurrentAbility(_abilitySlots[1].CurrentAbility);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (_abilitySlots[2].CurrentAbility != null)
            {
                _abilitySlots[0].SetActive(false);
                _abilitySlots[1].SetActive(false);
                _abilitySlots[2].SetActive(true);
                _abilitySlots[3].SetActive(false);
                ShowAbilityDescription(_abilitySlots[2].CurrentAbility.Description);

                _localPlayer.CmdSetCurrentAbility(_abilitySlots[2].CurrentAbility);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (_abilitySlots[3].CurrentAbility != null)
            {
                _abilitySlots[0].SetActive(false);
                _abilitySlots[1].SetActive(false);
                _abilitySlots[2].SetActive(false);
                _abilitySlots[3].SetActive(true);
                ShowAbilityDescription(_abilitySlots[3].CurrentAbility.Description);

                _localPlayer.CmdSetCurrentAbility(_abilitySlots[3].CurrentAbility);
            }
        }
    }
}

