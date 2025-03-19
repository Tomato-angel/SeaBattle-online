using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class ShipDeploymentButtonsManager : MonoBehaviour
{
    [SerializeField]
    List<ShipDeploymentButton> _shipDeploymentButtons = new();
    #region [Режим активности менеджера]
    private bool _isActive = true;
    public bool IsActive => _isActive;
    public void SetActive(bool isActive)
    {
        _isActive = isActive;
    }
    #endregion
    public void CheckForAmount(List<Ship> shipsList)
    {
        if (!_isActive) return;
        if (shipsList == null) return;
        for (int id = 1; id < shipsList.Count; id++)
        {
            if (shipsList[id].shipAmount > 0)
                _shipDeploymentButtons[id].interactable = true;
            else
                _shipDeploymentButtons[id].interactable = false;
            
            _shipDeploymentButtons[id].ShipsCountTextField.text = shipsList[id].shipAmount.ToString();
        }
        
    }

    public void PrepareButtons(List<Ship> ships)
    {
        if (!_isActive) return;
        CheckForAmount(ships);
    }
}
