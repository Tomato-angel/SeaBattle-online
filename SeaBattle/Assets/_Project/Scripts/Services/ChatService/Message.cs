using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class Message
{
    private DateTime _time;
    public DateTime GetLocalTime { get => _time.ToLocalTime(); }
    public DateTime GetUTCTime { get => _time; }

    [SerializeField] public string _senderNickname;
    public string SenderNickname { get => _senderNickname; }

    [SerializeField] public string _text;
    public string Text { get => _text; }

    public override string ToString() 
    {
        return $"[{_time.ToLocalTime().TimeOfDay}] {_senderNickname}: {_text}";
    }

    public Message(){ }
    public Message(string senderNickname, string text) 
    {
        _time = DateTime.UtcNow;
        _senderNickname = senderNickname;
        _text = text;
    }


}

