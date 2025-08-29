using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum DamagerTileStatus
{
    Default,
    Checked,
    Missed,
    Hit
}

public class DamagerTile : MonoBehaviour
{
    DamagerTileStatus _currentStatus;
    [SerializeField] GameObject _checkedView;
    [SerializeField] GameObject _missedView;
    [SerializeField] GameObject _hitView;

    public void SetStatus(DamagerTileStatus status)
    {
        _currentStatus = status;
        switch (status)
        {
            case DamagerTileStatus.Default:
            {
                _checkedView.SetActive(false);
                _missedView.SetActive(false);
                _hitView.SetActive(false);
                break;
            }
            case DamagerTileStatus.Checked:
                {
                    _checkedView.SetActive(true);
                    _missedView.SetActive(false);
                    _hitView.SetActive(false);
                    break;
                }
            case DamagerTileStatus.Missed:
                {
                    _checkedView.SetActive(false);
                    _missedView.SetActive(true);
                    _hitView.SetActive(false);
                    break;
                }
            case DamagerTileStatus.Hit:
                {
                    _checkedView.SetActive(false);
                    _missedView.SetActive(false);
                    _hitView.SetActive(true);
                    break;
                }
        }
    }
    
}

