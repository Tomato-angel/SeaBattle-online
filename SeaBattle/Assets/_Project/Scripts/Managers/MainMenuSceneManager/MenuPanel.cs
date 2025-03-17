using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

//[RequireComponent(typeof(NetworkIdentity))]
public abstract class MenuPanel : MonoBehaviour
{
    public void Hide()
    {
        Reload();
        gameObject.SetActive(false);
    }

    public void Show()
    {
        Reload();
        gameObject.SetActive(true);
    }

    public void SetActive(bool isActive)
    {
        if(isActive)
            Show();
        else
            Hide();
    }

    public virtual void Reload() { }
}

