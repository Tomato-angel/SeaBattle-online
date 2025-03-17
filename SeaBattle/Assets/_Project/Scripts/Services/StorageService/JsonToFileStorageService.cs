using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;



internal class JsonToFileStorageService : IStorageService
{
    private bool _isInProgressNow;
    public void Save(string key, object data, Action<bool> callback = null)
    {
        if(!_isInProgressNow)
        {
            SaveAsync(key, data, callback);
        }
        else
        {
            callback.Invoke(false);
        }
    }
    public async void SaveAsync(string key, object data, Action<bool> callback)
    {
        _isInProgressNow = true;
        string path = BuildPath(key);
        string json = JsonConvert.SerializeObject(data);

        using (var fileStream = new StreamWriter(path))
        {
            
            await fileStream.WriteAsync(json);
        }
        _isInProgressNow = false;
        callback.Invoke(true);
    }
    public void Load<T>(string key, Action<T> callback)
    {
        Exist<T>(key, (isExist) => {
            if (isExist)
            {
                string path = BuildPath(key);

                using (var fileStream = new StreamReader(path))
                {
                    var json = fileStream.ReadToEnd();
                    var data = JsonConvert.DeserializeObject<T>(json);

                    callback.Invoke(data);
                    return;
                }
            }
        });
    }
    public void Exist<T>(string key, Action<bool> callback)
    {
        string path = BuildPath(key);
        if (File.Exists(path))
        {
            callback.Invoke(true);
        }
        callback.Invoke(false);
    }


    public void FastSave<T>(T data, Action<bool> callback = null)
    {
        try
        {
            string key = GenerateKey(typeof(T).FullName);
            string path = BuildPath(key);
            string json = JsonConvert.SerializeObject(data);
            using (var fileStream = new StreamWriter(path))
            {
                fileStream.Write(json);
            }
            callback?.Invoke(true);
        }
        catch
        {
            callback?.Invoke(false);
        }
    }
    public void FastLoad<T>(Action<T> callback = null)
    {
        string key = GenerateKey(typeof(T).FullName);
        Exist<T>(key, (isExist) => {
            if (isExist)
            {
                string path = BuildPath(key);
                using (var fileStream = new StreamReader(path))
                {
                    var json = fileStream.ReadToEnd();
                    var data = JsonConvert.DeserializeObject<T>(json);

                    callback?.Invoke(data);
                    return;
                }
            }
        });
    }

    private string BuildPath(string key)
    {
        return Path.Combine(Application.persistentDataPath, key);
    }

    private char[] _prohibitedSymbols = { '~','#','%','&','*','{','}','\\',':','<','>','?','/','+','|','\"','\''};
    private string GenerateKey(string input)
    {
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            string key = Convert.ToBase64String(hashBytes);
            for(int i = 0; i < _prohibitedSymbols.Length; ++i)
            {
                key = key.Replace(_prohibitedSymbols[i].ToString(), $"_{i}");
            }

            return  key;
        }
    }
}

