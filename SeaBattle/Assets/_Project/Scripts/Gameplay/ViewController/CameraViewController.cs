using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CameraViewController : MonoBehaviour, IInitializable
{
    private bool _isActive = true;
    public void SetActive(bool isActive)
    {
        _isActive = isActive;
    }

    #region [ View Point change logic ]

    [Space]
    [Header("ViewPoint change logic")]
    public bool IsMovable;
    [SerializeField] Camera _camera;

    [SerializeField] CameraViewPoint _lastViewPoint = null;
    [SerializeField] CameraViewPoint _currentViewPoint = null;

    [SerializeField] CameraViewPoint _mainViewPoint;
    public void ToMainViewPoint()
    {
        ChangeViewPoint(_mainViewPoint);
        IsRotatable = true;
    }
    [SerializeField] CameraViewPoint _gameplayViewPoint;
    public void ToGameplayViewPoint()
    {
        ChangeViewPoint(_gameplayViewPoint);
        IsRotatable = false;
    }
    [SerializeField] CameraViewPoint _chatViewPoint;
    public void ToChatViewPoint()
    {
        ChangeViewPoint(_chatViewPoint);
        IsRotatable = false;
    }
    [SerializeField] CameraViewPoint _boardViewPoint;
    public void ToBoardViewPoint()
    {
        ChangeViewPoint(_boardViewPoint);
        IsRotatable = false;
    }
    [SerializeField] CameraViewPoint _shopViewPoint;
    public void ToShopViewPoint()
    {
        ChangeViewPoint(_shopViewPoint);
        IsRotatable = false;
    }


    [SerializeField][Range(0, 10)] float _speed = 1;

    [SerializeField] MainMenu _mainMenu;

    private bool CheckChangeViewPointPosition(float deltaPosition = 0.01f)
    {
        bool result = false;
        float difference = Vector3.Distance(_camera.transform.position, _currentViewPoint.transform.position);
        result = difference <= deltaPosition;
        return result;
    }
    
    /*private bool CheckChangeViewPointRotation(float deltaAngle = 5)
    {
        bool result = false;
        float angle = Quaternion.Angle(_camera.transform.rotation, _currentViewPoint.transform.rotation);
        result = angle <= deltaAngle;
        return result;
    }*/

    private IEnumerator DynamicChangeViewPointToCurrent()
    {
        while(!CheckChangeViewPointPosition())
        {
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, _currentViewPoint.transform.position, Time.deltaTime * _speed);
            /*
            if(!CheckChangeViewPointRotation())
            {
                _camera.transform.rotation = Quaternion.Lerp(_camera.transform.rotation, _currentViewPoint.transform.rotation, Time.deltaTime * _speed); 
            }*/
            yield return new WaitForSeconds(Time.deltaTime);
        }

        yield return null;
    }
    public void ChangeViewPoint(CameraViewPoint cameraViewPoint)
    {
        if (cameraViewPoint == null) return;
        StopAllCoroutines();
        _currentViewPoint = cameraViewPoint;
        StartCoroutine(DynamicChangeViewPointToCurrent());
    }



    private void EnterViewPointFromKeyboard()
    {
        if (!_isActive) return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (!IsMovable) return;
            if (_currentViewPoint != _gameplayViewPoint)
            {
                _lastViewPoint = _currentViewPoint;
                ChangeViewPoint(_gameplayViewPoint);
                IsRotatable = false;
                _mainMenu.ShowGameplayMenu();
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (!IsMovable) return;
            if (_currentViewPoint != _shopViewPoint)
            {
                _lastViewPoint = _currentViewPoint;
                ChangeViewPoint(_shopViewPoint);
                IsRotatable = false;
                _mainMenu.ShowGameplayMenu();
            }
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            if (!IsMovable) return;
            if (_currentViewPoint != _chatViewPoint)
            {
                _lastViewPoint = _currentViewPoint;
                ChangeViewPoint(_chatViewPoint);
                IsMovable = false;
                IsRotatable = false;
                _mainMenu.HideAllMenu();
            }
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.M))
        {
            if (!IsMovable) return;
            if (_currentViewPoint != _boardViewPoint)
            {
                _lastViewPoint = _currentViewPoint;
                ChangeViewPoint(_boardViewPoint);
                IsRotatable = false;
                _mainMenu.HideAllMenu();
            }
            /*
            else if (_currentViewPoint == _boardViewPoint)
            {
                ChangeViewPoint(_lastViewPoint);
                IsRotatable = _lastViewPoint == _mainViewPoint;
                _lastViewPoint = null;
            }*/
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsMovable = true;
            /*
            if (_currentViewPoint == _lastViewPoint)
            {
                ChangeViewPoint(_mainViewPoint);
                IsRotatable = true;
            }
            if (_currentViewPoint != _mainViewPoint)
            { 
                ChangeViewPoint(_lastViewPoint);
                if (_lastViewPoint == _mainViewPoint)
                    IsRotatable = true;
                _lastViewPoint = _currentViewPoint;
            }*/

            if (_currentViewPoint == _mainViewPoint)
            {
                _mainMenu.ShowPauseMenu();
            }
            if (_currentViewPoint != _mainViewPoint)
            {
                ChangeViewPoint(_mainViewPoint);
                IsRotatable = true;
                _mainMenu.HideAllMenu();
            }
            
        }

        /*
        if(Input.GetKeyDown(KeyCode.W))
        {
            ChangeViewPoint(_currentViewPoint.TopCameraViewPoint);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            ChangeViewPoint(_currentViewPoint.LeftCameraViewPoint);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            ChangeViewPoint(_currentViewPoint.BottomCameraViewPoint);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            ChangeViewPoint(_currentViewPoint.RightCameraViewPoint);
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeViewPoint(_currentViewPoint.MainCameraViewPoint);
        }*/

    }
    #endregion

    #region [ Camera control logic ]
    [Space]
    [Header("Camera control logic")]

    public bool IsRotatable;
    [Range(0, 90)]
    [SerializeField] private float _widthDeviationAngleLimit = 45;

    [Range(0, 90)]
    [SerializeField] private float _heightDeviationAngleLimit = 45;

    [Range(0f, 100f)]
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


        result = mousePositionTmp;

        return result;
    }

    public void CameraControl()
    {
        if (IsRotatable && _isActive)
        {
            _mousePosition = NormalizeMousePosition(Input.mousePosition);
            _sensevity = _sensevityMin;
        }
        else
        {
            _mousePosition = Vector2.zero;
            _sensevity = _sensevityMax;
        }
            

        _rotationAngles.x = (-_mousePosition.y / (Screen.height / 2)) * _widthDeviationAngleLimit;
        _rotationAngles.y = (_mousePosition.x / (Screen.width / 2)) * _heightDeviationAngleLimit;

        /*
        if (_rotationAngles.x == 0 && _rotationAngles.y == 0)
            _sensevity = _sensevityMax;
        else
            _sensevity = _sensevityMin;
        */

        _camera.transform.rotation = Quaternion.Lerp(
            _camera.transform.rotation,
            Quaternion.Euler(
                _rotationAngles.x +  _currentViewPoint.transform.rotation.eulerAngles.x,
                _rotationAngles.y + _currentViewPoint.transform.rotation.eulerAngles.y,
                0 + _currentViewPoint.transform.eulerAngles.z
                ),
            Time.deltaTime * _sensevity
            );
    }
    #endregion


    public void Initialize()
    {
        IsMovable = true;
        IsRotatable = true;
        _camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        ChangeViewPoint(_mainViewPoint);
    }

    private void Start()
    {

    }

    private void Update()
    {

        EnterViewPointFromKeyboard();
        CameraControl();
    }

    
}
