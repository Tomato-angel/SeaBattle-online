using UnityEngine;

namespace Scripts.LogService
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

