using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;


public class MessageTextField : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textField;
    public string Text { get => _textField.text; set => _textField.text = value; }
}


