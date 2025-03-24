using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChatData
{
    public List<Message> _messages;

    public int Count { get => _messages.Count; }
    public Message[] Messages { get => _messages.ToArray(); }
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
    public void Remove(int index)
    {
        _messages.RemoveRange(index, 1);
    }

    public void Clear()
    {
        _messages.Clear();
    }

    public ChatData() { }
}
