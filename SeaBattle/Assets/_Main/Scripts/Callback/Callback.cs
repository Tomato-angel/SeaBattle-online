using UnityEngine;

public class Callback<T>
{
    private bool _isOperationSuccessfuly;
    public bool IsOperationSuccessfuly { get => _isOperationSuccessfuly; }

    private T _data;
    public T Data { get => _data; }

    private string _message;
    public object Message { get => _message; }


    public Callback(bool isOperationSuccessfully, T data, string message = "")
    {
        _isOperationSuccessfuly = isOperationSuccessfully;
        _data = data;
        _message = message;
    }

}
