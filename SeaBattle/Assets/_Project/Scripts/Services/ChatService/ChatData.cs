using Mirror;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChatData
{
    [SerializeField]
    public List<Message> _messages;

    public int Count { get => _messages.Count; }
    public Message[] Messages 
    {
        get => _messages.ToArray();
        /*
        get 
        {
            Message[] messages = new Message[_messages.Count];
            for(int i = 0; i < _messages.Count; ++i)
            {
                messages[i] = _messages[i];
            }
            return messages;
        }
        */

    }
    public Message this[int index]
    {
        get
        {
            return _messages[index];
        }
    }
    public void Push(Message message)
    {
        _messages.Add(message);
    }

    public Message Pop()
    {
        return _messages[_messages.Count - 1];
    }

    public void Remove(Message message)
    {
        _messages.Remove(message);
    }
    /*
    public void Remove(int index)
    {
        _messages.RemoveRange(index, 1);
    }
    */

    public void Clear()
    {
        _messages.Clear();
    }

    public ChatData() { }
}
