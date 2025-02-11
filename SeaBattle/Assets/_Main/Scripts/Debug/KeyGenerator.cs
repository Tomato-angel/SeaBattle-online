using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Linq;
using UnityEditorInternal;


/// <summary>
/// Класс, гарантирующий создание уникальных ключей и проверки их валидности
/// </summary>
public class KeyGenerator
{
    private int _keyLength;

    private HashSet<string> _keys;
    public HashSet<string> Keys { get => _keys; }

    private Char[] _pwdChars = new char[62] {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'O', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
    };

    private int _keyUniquenessLimit;

    private static KeyGenerator _instance;
    public static KeyGenerator Instance
    {
        get
        {
            if (_instance == null) _instance = new KeyGenerator();
            return _instance;
        }
    }


    //Переделать на асинхронность, генерация ключа и попытка сделать его валидным и т.п. просто уничтожит производительность сервера

    private string GenerateKey()
    {
        string uniqueKey = string.Empty;
        for (int i = 0; i < _keyLength; ++i)
        {
            uniqueKey += _pwdChars[UnityEngine.Random.Range(0, 62)];
        }
        return uniqueKey;
    }


    public bool TryGenerateKey1(out string key)
    {
        //Task<string> generateKey = new Task<string>(() => { return GenerateKey(); });

        Task<(string, bool)> tryGenerateUniqueKey = new Task<(string, bool)>(() => 
        {
            for(int i = 0; i < _keyUniquenessLimit; ++i)
            {
                string generatedKey = GenerateKey();
                if (_keys.Contains(generatedKey))
                    continue;
                else
                    return (generatedKey, true); 
            }
            return (string.Empty, false);
        });

        tryGenerateUniqueKey.Start();
        tryGenerateUniqueKey.Wait();

        (string tmpKey, bool isSuccesfulyGenerated) = tryGenerateUniqueKey.Result;

        if (isSuccesfulyGenerated)
        {
            Keys.Add(tmpKey);
            key = tmpKey;
            return true;
        }
        else
        {
            key = string.Empty;
            return false;
        }
    }

    // Ещё раз пересмотреть код, возможно придётся его немного изменить для лучшей работы на большом количестве ключей
    // Проработать момент выброса ошибки
    public bool TryGenerateKey(out string key)
    {
        try
        {
            var newKey = GenerateKey();
            key = newKey;
            Keys.Add(newKey);
            return true;
        }
        catch (Exception e)
        {
            throw new Exception($"<color=red>Generating error - the generated keys already exists.</color>\n{e.Message}.");
        }
    }

    
    public bool RemoveKey(string key)
    {
        return _keys.Remove(key);
    }
    
    /* УСТАРЕЛО - проверка валидности ключа
    static public bool CheckValidity(string key)
    {
        string tmp;
        return _keys.TryGetValue(key, out tmp);
    } */

    public Guid KeyToGui(string key)
    {
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
        byte[] inputBytes = Encoding.Default.GetBytes(key);
        byte[] hashBytes = provider.ComputeHash(inputBytes);

        return new Guid(hashBytes);
    }

    public KeyGenerator(int keyLength = 8)
    {
        _keyLength = keyLength;
        _keys = new HashSet<string>();

        _pwdChars = new char[62] {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'O', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

        _keyUniquenessLimit = (int) MathF.Pow(_pwdChars.Length, keyLength);

    }
}
