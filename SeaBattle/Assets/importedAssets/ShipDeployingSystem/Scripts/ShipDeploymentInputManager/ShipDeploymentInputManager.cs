using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShipDeploymentInputManager : MonoBehaviour
{
    //взаимодействует с действиями игрока, не знает о наличии других переменных
    //создает луч, может быть заменен на штуку, которую написал Женя
    [SerializeField]
    private Camera _sceneCamera;
    [SerializeField]
    private Vector3 _lastPosition;
    [SerializeField]
    private LayerMask _layerMask;

    

    #region [Режим активности менеджера]
    private bool _isActive = true;
    public bool IsActive => _isActive;
    public void SetActive(bool isActive)
    {
        _isActive = isActive;
    }
    #endregion

    #region [ Отлавливание положение и поведения мыши ]
    public event Action OnClicked, OnDrag, OnExit, OnRotate;
    private Vector2 _oldMousePosition = Vector2.zero;
    private float _deltaMouseOffset = 10;
    private Vector2 _currentMousePosition = Vector2.zero;
    [SerializeField] private string _mouseState = "none";
    public void CatchingInput()
    {
        if (!_isActive) return;

        _currentMousePosition = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            OnClicked?.Invoke();
            _oldMousePosition = _currentMousePosition;
            _mouseState = "click";
        }
        if (Input.GetMouseButtonUp(0))
        {
            _mouseState = "none";
        }
        if (Input.GetMouseButton(0))
        {
            if (Mathf.Abs(Vector2.Distance(_oldMousePosition, _currentMousePosition)) > _deltaMouseOffset)
            {
                OnDrag?.Invoke();
                _mouseState = "drag";
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            OnRotate?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnExit?.Invoke();
        }
    }
    #endregion

    private void Start()
    {
        
    }

    public void Update()
    {
        CatchingInput();
    }

    //public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetSelectedMousePosition()
    {
        if (!_isActive) return new Vector3(100, 100, 100);

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = _sceneCamera.nearClipPlane;
        Ray ray = _sceneCamera.ScreenPointToRay(mousePosition);

        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 20,Color.green);

        if (Physics.Raycast(ray, out hit, 20, _layerMask))
        { 
            _lastPosition = hit.point;
            return _lastPosition;
        }
        return new Vector3(100, 100, 100);
        
    }
}
