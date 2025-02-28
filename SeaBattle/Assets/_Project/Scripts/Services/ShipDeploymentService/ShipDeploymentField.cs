using UnityEngine;

public class ShipDeploymentField : MonoBehaviour
{
    [SerializeField] private GameObject _shipDeploymentSegmentPrefab;
    [SerializeField] private ShipDeploymentSegment[,] _shipDeploymentSegmentsObj;

    [SerializeField] private int _countOfSegments;
 
    private void Awake()
    {
        _countOfSegments = (_countOfSegments == 0) ? 10 : _countOfSegments;
        _shipDeploymentSegmentsObj = new ShipDeploymentSegment[_countOfSegments, _countOfSegments];
    }

    void Start()
    {
        for(int i = 0; i < _countOfSegments; ++i)
        {
            for (int j = 0; j < _countOfSegments; ++j)
            {
                GameObject shipDeploymentSegmentObj = Instantiate(_shipDeploymentSegmentPrefab, transform, false);
                shipDeploymentSegmentObj.transform.localPosition = new Vector3(i - 5, 0 ,j - 5);
                shipDeploymentSegmentObj.name = $"({0},{0}) - segment";
                // Инициализация сегмента, присваивание id и прочее

                ShipDeploymentSegment shipDeploymentSegment = shipDeploymentSegmentObj.GetComponent<ShipDeploymentSegment>();
                _shipDeploymentSegmentsObj[i, j] = shipDeploymentSegment;
            }
        }
    }

    void Update()
    {
        DraggingObject();
    }

    public void DraggingObject()
    {
        Vector3 screenPosition = Input.mousePosition;
        Debug.Log($"[{screenPosition}], []");
        screenPosition.x = screenPosition.x / Screen.width * 1920;
        screenPosition.y = screenPosition.y / Screen.height * 1080;
        Debug.Log($"[{screenPosition}], []");
        //screenPosition.z = Camera.main.nearClipPlane;
        //Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);


        RaycastHit hit;

        //Debug.DrawRay(Camera.main.transform.position, worldPosition);
        //Debug.Log($"[{worldPosition}], []");

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        //Debug.Log($"[{ray.origin}], [{ray.direction}]");
        //Ray ray = new Ray (Camera.main.transform.position, Camera.main.ScreenToWorldPoint(screenPosition));

        
        Physics.Raycast(ray, out hit);

        if (hit.collider != null)
        {

            Debug.DrawLine(ray.origin, hit.point, Color.green);
            //if (hit.collider.CompareTag("DebugObj"))
            {
                Debug.DrawLine(hit.point, Vector3.down, Color.green);
                //Gizmos.DrawCube(hit.point, new Vector3(1,1,1));
                //gameObject.transform.position = hit.point;
            }

            ShipDeploymentSegment shipDeploymentSegment = hit.collider.GetComponent<ShipDeploymentSegment>();

            if(shipDeploymentSegment != null)
            {
                shipDeploymentSegment.IsDragging = true;
            }
        }
        else
        {
            Debug.DrawLine(ray.origin, ray.direction * 100, Color.red);
        }
        
    }
}
