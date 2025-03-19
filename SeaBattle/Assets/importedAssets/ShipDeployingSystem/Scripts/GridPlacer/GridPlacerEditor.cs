using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridPlacer))]
public class GridPlacerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GridPlacer gridPlacer = (GridPlacer)target;
        if (GUILayout.Button("Generate new tile grid"))
        {
            Debug.Log("[Editor] Try to generate tile grid");
            gridPlacer.ClearGrid();
            gridPlacer.GenerateGrid();
        }
    }
}