using UnityEngine;

public class ShipDeploymentSegment : MonoBehaviour
{
    public bool IsDragging { get; set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(IsDragging)
        {
            MoveToHeight(1, Time.deltaTime);
        }
        MoveToHeight(0, Time.deltaTime);
        IsDragging = false;
    }

    public void MoveToHeight(float height, float stepSpeed)
    {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                new Vector3(
                    transform.localPosition.x,
                    height,
                    transform.localPosition.z
                ), 
                stepSpeed
            );
    }
    
}
