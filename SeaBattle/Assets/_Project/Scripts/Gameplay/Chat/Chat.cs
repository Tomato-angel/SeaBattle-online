using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Chat : MonoBehaviour, IInitializable
{
    [SerializeField] Player _localPlayer;

    [SerializeField] GameObject _messageTextFieldPrefab;
    [SerializeField] List<GameObject> _messageTextFieldsObj;
    [SerializeField] GameObject _contentView;

    [SerializeField] TMP_InputField _messageTextInputField;

    public void Initialize()
    {
        if (ProjectManager.root == null) return;
        _localPlayer = ProjectManager.root.LocalPlayer;
        _localPlayer.newMessageInChat += GetMessage;
        _localPlayer.updateCurrentChatData += RedrawChat;
    }

    public void SendMessage()
    {
        if (_localPlayer == null)
        {
            GetMessage(new Message($"<color=yellow>NoPlayer", $"{_messageTextInputField.text}</color>"));
            return;
        }
        _localPlayer.CmdSendMessageInChat(_messageTextInputField.text);
    }

    public void GetMessage(Message message)
    {
        GameObject newMessageTextFieldObj = Instantiate(_messageTextFieldPrefab, _contentView.transform);
        MessageTextField newMessageTextField = newMessageTextFieldObj.GetComponent<MessageTextField>();
        newMessageTextField.Text = $"{message.SenderNickname}: {message.Text}";
        _messageTextFieldsObj.Add(newMessageTextFieldObj);
    }

    public void RedrawChat()
    {

        foreach(var messageTextField in _messageTextFieldsObj)
        {
            Destroy(messageTextField);
        }
        _messageTextFieldsObj.Clear();

        foreach(var message in _localPlayer.CurrentChatData.Messages)
        {
            GetMessage(message);
        }
    }

}
