using System.Linq;
using UnityEngine;

public class CameraViewPoint : MonoBehaviour
{
    [Header("For GamePlay")]
    [SerializeField] private CameraViewPoint _mainViewPoint = null;
    public CameraViewPoint MainCameraViewPoint { get => _mainViewPoint; }
    [SerializeField] private CameraViewPoint _topCameraViewPoint = null;
    public CameraViewPoint TopCameraViewPoint { get => _topCameraViewPoint; }
    [SerializeField] private CameraViewPoint _bottomCameraViewPoint = null;
    public CameraViewPoint BottomCameraViewPoint { get => _bottomCameraViewPoint; }
    [SerializeField] private CameraViewPoint _leftCameraViewPoint = null;
    public CameraViewPoint LeftCameraViewPoint { get => _leftCameraViewPoint; }
    [SerializeField] private CameraViewPoint _rightCameraViewPoint = null;
    public CameraViewPoint RightCameraViewPoint { get => _rightCameraViewPoint; }



    private void Awake()
    {
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }



    [Space]
    [Header("For Editor")]
    [SerializeField] bool _isDrawGizmos = true;

    [SerializeField] Mesh _cameraArrowMesh;
    [SerializeField] Mesh _cameraMesh;
    private void OnDrawGizmos()
    {
        if(_isDrawGizmos)
        if(_cameraMesh != null && _cameraArrowMesh != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawMesh(_cameraMesh, 0, gameObject.transform.position, gameObject.transform.rotation);
        }
        
    }
    private void OnDrawGizmosSelected()
    {
        if (_isDrawGizmos)
        if (_cameraMesh != null && _cameraArrowMesh != null)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawMesh(_cameraArrowMesh, 0, gameObject.transform.position, gameObject.transform.rotation);
            Gizmos.color = Color.blue;
            Gizmos.DrawMesh(_cameraMesh, 0, gameObject.transform.position, gameObject.transform.rotation);
        }
    }
}
