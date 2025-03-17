using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] bool _doRaycast;
    [SerializeField] GameObject _targetObject;

    private void Awake()
    {
        _doRaycast = true;
        _targetObject = null;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (_doRaycast)
        {
            Vector3 screenPosition = Input.mousePosition;

            // Если вдруг понадобятся различные преобразования вынесем это
            float coordinate_X = screenPosition.x;
            float coordinate_Y = screenPosition.y;
            float coordinate_z = screenPosition.z;

            Vector3 screenPosition_changed = new Vector3()
            {
                x = coordinate_X,
                y = coordinate_Y,
                z = coordinate_Y
            };

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(screenPosition_changed);
            Physics.Raycast(ray, out hit);

            if (hit.collider != null)
            {
                Debug.DrawLine(ray.origin, hit.point, Color.green);
                _targetObject = hit.collider.gameObject;
            }
            else
            {
                Debug.DrawLine(ray.origin, ray.direction * 100f, Color.red);
                _targetObject = null;
            }
        }
        {
            IInteractable interactable = GetComponent<IInteractable>();
            interactable.InteractPointerEnter();
        }
    }
}
