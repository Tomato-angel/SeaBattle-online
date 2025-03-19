using UnityEngine;

public interface IBuildingState
{
    void EndState();
    void OnAction(Vector3 mousePosition);
    Coordinates UpdateState(Vector3 mousePosition);
    void RotateObject(Vector3 mousePosition);
    bool CheckEndPlacement();
}