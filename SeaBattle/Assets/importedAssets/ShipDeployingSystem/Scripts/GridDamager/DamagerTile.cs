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
    [SerializeField] GameObject _checkedView;
    [SerializeField] GameObject _missedView;
    [SerializeField] GameObject _hitView;
    
}

