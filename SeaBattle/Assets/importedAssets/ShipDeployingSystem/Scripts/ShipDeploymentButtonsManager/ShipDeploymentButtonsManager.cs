using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class ShipDeploymentButtonsManager : MonoBehaviour
{
    [SerializeField]
    List<ShipDeploymentButton> _shipDeploymentButtons = new();
    #region [Режим активности менеджера]
    private bool _isActive;
    public void SetActive(bool isActive)
    {
        _isActive = isActive;
        SetButtonsInteractable(isActive);
    }

    #endregion
    public void CheckForAmount(List<Ship> shipsList)
    {
        //if (!_isActive) return;
        //if (shipsList == null) return;
        for (int id = 1; id < shipsList.Count; id++)
        {
            if (shipsList[id].shipAmount > 0)
                _shipDeploymentButtons[id].IsInteractable = true;
            else
                _shipDeploymentButtons[id].IsInteractable = false;
            
            _shipDeploymentButtons[id].ShipsCountTextField.text = shipsList[id].shipAmount.ToString();
        }
        
    }
    
    public void SetButtonsInteractable(bool isActive)
    {
        foreach(var shipDeploymentButton in _shipDeploymentButtons)
        {
            shipDeploymentButton.IsInteractable = isActive;
        }
    }


    public void PrepareButtons(List<Ship> ships)
    {
        CheckForAmount(ships);
    }
}
