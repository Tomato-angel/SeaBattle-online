using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class CameraControlController : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    [Range(0, 90)]
    [SerializeField] private float _widthDeviationAngleLimit = 45;

    [Range(0, 90)]
    [SerializeField] private float _heightDeviationAngleLimit = 45;

    [Range(0f,10f)]
    [SerializeField] private float _sensevityMax = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float _sensevityMin = 0.1f;
    [SerializeField] private float _sensevity;

    [SerializeField] private Vector2 _mousePosition = Vector2.zero;
    [SerializeField] private Vector2 _rotationAngles = Vector2.zero;

    private Vector2 NormalizeMousePosition(Vector2 mousePosition)
    {
        Vector2 mousePositionTmp = mousePosition;
        Vector2 result = Vector2.zero;

        if (mousePositionTmp.x > Screen.width) mousePositionTmp.x = Screen.width;
        if (mousePositionTmp.x < 0) mousePositionTmp.x = 0;
        if (mousePositionTmp.y > Screen.height) mousePositionTmp.y = Screen.height;
        if (mousePositionTmp.y < 0) mousePositionTmp.y = 0;


        mousePositionTmp.x = mousePositionTmp.x - Screen.width / 2;
        mousePositionTmp.y = mousePositionTmp.y - Screen.height / 2;

        /*
        if (mousePositionTmp.x > -Screen.width / 4 && mousePositionTmp.x < Screen.width / 4)
            mousePositionTmp.x = 0;
        if (mousePositionTmp.y > -Screen.height / 4 && mousePositionTmp.y < Screen.height / 4)
            mousePositionTmp.y = 0;*/

        result = mousePositionTmp;

        return result;
    }

    private void Awake()
    {
        

    }

    void Start()
    {
        
    }

    // Похиционирование происходит относительно нуля, однако должно идти относительно некоторого observation point, значит его нужно передавать
    void Update()
    {
        _mousePosition = NormalizeMousePosition(Input.mousePosition);

        _rotationAngles.x = (-_mousePosition.y / (Screen.height / 2)) * _widthDeviationAngleLimit;
        _rotationAngles.y = (_mousePosition.x / (Screen.width / 2)) * _heightDeviationAngleLimit;

        if (_rotationAngles.x == 0 && _rotationAngles.y == 0)
            _sensevity = _sensevityMax;
        else
            _sensevity = _sensevityMin;

        _camera.transform.rotation = Quaternion.Lerp(
            _camera.transform.rotation,
            Quaternion.Euler(
                _rotationAngles.x,
                _rotationAngles.y,
                0
                ),
            Time.deltaTime * _sensevity
            );
        /*
        _camera.transform.rotation = Quaternion.Euler(
            _rotationAngles.x,
            _rotationAngles.y, 
            0
            );*/
    }
}
