using TMPro;
using UnityEngine;

public class RegistrationField : MonoBehaviour
{
    
    [SerializeField] bool _isSelected;
    [SerializeField] TMP_InputField _inputField;

    public string Text { get; private set; } = "";
    public bool IsEmpty { get => Text.Length == 0; }

    private void SetSelect(bool selectStatus)
    {
        if (selectStatus)
        {
            _inputField.ActivateInputField();
        }
        else
        {
            _inputField.DeactivateInputField();
        }
    }

    void Start()
    {
        _isSelected = true;
        SetSelect(_isSelected);
    }

    void Update()
    {
        _inputField.ActivateInputField();
        _inputField.caretPosition = _inputField.text.Length;
        Text = _inputField.text;
        Debug.Log(Text + "  " + (Text.Length > 0).ToString());
    }
}
