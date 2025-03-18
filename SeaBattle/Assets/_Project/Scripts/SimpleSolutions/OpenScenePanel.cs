using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OpenScenePanel : MenuPanel
{
    [SerializeField] Image _image;
    [SerializeField][Range(0, 1000)] int _millisecondsDelay = 0;
    
    
    void Start()
    {
        StartTurnOffPanelAsync();
    }

    public async void StartTurnOffPanelAsync()
    {
        Show();
        float remainingTime = _millisecondsDelay;
        while(remainingTime > 0)
        {
            _image.color = new Color(0,0,0, remainingTime / _millisecondsDelay);
            remainingTime -= 1f;
            await Task.Delay(1);
        }
        Hide();
    }


}
