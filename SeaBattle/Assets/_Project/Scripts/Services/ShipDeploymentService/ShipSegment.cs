using UnityEngine;

public class ShipSegment : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawCube(gameObject.transform.position, new Vector3(1,1,1));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(gameObject.transform.position, new Vector3(1, 1, 1));
    }
}
