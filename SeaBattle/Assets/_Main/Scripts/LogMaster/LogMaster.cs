using UnityEngine;

namespace Extensions.LogMaster
{
    public class LogMaster
    {
        public void Log(string message)
        {
#if UNITY_EDITOR
            // �������� ������ � ������� � ����� ������ �� �����
            Debug.Log(message);
#else
            // �������� ������ � �����
            Debug.Log(message);
#endif

        }

        public void Log(object obj)
        {
            Debug.Log(obj);
        }
    }
}

