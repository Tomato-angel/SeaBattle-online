using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Threading;


/// <summary>
/// Класс, гарантирующий создание уникальных ключей
/// </summary>
public class KeyGenerator
{
    private int _keyLength;
    private int _keyUniquenessLimit;

    private HashSet<string> _keys;
    public HashSet<string> Keys { get => _keys; }

    private Char[] _pwdChars = new char[62] {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'O', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
    };


    private static KeyGenerator _instance;
    public static KeyGenerator Instance
    {
        get
        {
            if (_instance == null) _instance = new KeyGenerator();
            return _instance;
        }
    }

    /// <summary>
    /// Создаёт произвольный ключ
    /// </summary>
    /// <returns> 
    /// Возвращает строку - некоторый ключ
    /// </returns>
    private string GenerateKey()
    {
        string uniqueKey = string.Empty;
        for (int i = 0; i < _keyLength; ++i)
        {
            Random rnd = new Random();
            uniqueKey += _pwdChars[rnd.Next(0, 62)];
        }
        return uniqueKey;
    }


    /// <summary>
    /// Асинхронно генерирует уникальный ключ и запоминает его для дальнейшей верификации
    /// </summary>
    /// /// <returns> 
    /// Возвращает строку - некоторый уникальный ключ
    /// </returns>
    public async Task GenerateUniqueKeyAsync(Action<string> callback = null)
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < 100; ++i)
            {
                string generatedKey = GenerateKey();
                if (!_keys.Contains(generatedKey))
                {
                    _keys.Add(generatedKey);
                    callback?.Invoke(generatedKey);
                    return;
                }
            }
            callback?.Invoke(string.Empty);
        });
    }



    // Устаревшее : генерация ключа
    /// <summary>
    /// [ УСТАРЕВШЕЕ ] Пытается сгенерировать ключ
    /// </summary>
    /// <param name="key">
    /// Возвращаемый параметр - уникальный ключ
    /// </param>
    /// <returns>
    /// Возваращет логическое значение - прошла ли попытка генерации ключа успешно
    /// </returns>
    public bool TryGenerateKey(out string key)
    {
        try
        {
            string newKey = GenerateKey();
            key = newKey;
            _keys.Add(newKey);
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

        _keyUniquenessLimit = (int) MathF.Pow(_pwdChars.Length, keyLength);
    }
}
