using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Message
{
    private DateTime _time;
    public DateTime GetLocalTime { get => _time.ToLocalTime(); }
    public DateTime GetUTCTime { get => _time; }

    private string _senderNickname;
    public string SenderNickname { get => _senderNickname; }

    private string _text;
    public string Text { get => _text; }

    public override string ToString() 
    {
        return $"[{_time.ToLocalTime().TimeOfDay}] {_senderNickname}: {_text}";
    }

    public Message(string senderNickname, string text) 
    {
        _time = DateTime.UtcNow;
        _senderNickname = senderNickname;
        _text = text;
    }


}

