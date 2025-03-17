using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class CountdownPanel : MenuPanel
{
    [SerializeField] Image _background;
    [SerializeField][Range(0, 2)] float _colorChangeSpeed = 1.25f;
    [SerializeField] float _maxValue = 10;
    [SerializeField] float _currentValue = 0;
    [SerializeField] TextMeshProUGUI _countdownView = null;
    public async void ActivateCountdown(Action callback = null)
    {
        float correctedMaxValue = _maxValue + 1;
        _currentValue = correctedMaxValue;
        Show();
        while (_currentValue > 0)
        {
            TimeSpan time = TimeSpan.FromSeconds(_currentValue);

            _background.color = new Color(0, 0, 0, ((correctedMaxValue - _currentValue) / correctedMaxValue + 0.5f));
            _countdownView.color = new Color(1, 1, 1, (correctedMaxValue - _currentValue) / correctedMaxValue * _colorChangeSpeed);
            _countdownView.fontSize = ((correctedMaxValue - _currentValue) / 10f + 1) * 100f;

            _countdownView.text = $"{time.Seconds}";
            await Task.Delay(20);
            _currentValue -= 0.01f;
        }
        {
            _countdownView.text = "0";
            await Task.Delay(1000);
        }
        _countdownView.text = string.Empty;

        callback?.Invoke();
        //Hide();
    }

    private void Start()
    {
        ActivateCountdown();
    }
}

