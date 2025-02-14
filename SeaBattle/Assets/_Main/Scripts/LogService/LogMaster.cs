using UnityEngine;

namespace Scripts.LogService
{
    public class LogMaster
    {
        public void Log(string message)
        {
#if UNITY_EDITOR
            // Работает только в эдиторе в билде такого не будет
            Debug.Log(message);
#else
            // Работает только в билде
            Debug.Log(message);
#endif

        }

        public void Log(object obj)
        {
            Debug.Log(obj);
        }
    }
}

