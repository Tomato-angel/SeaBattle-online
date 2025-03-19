using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DisplayMaterialsSO", menuName = "Scriptable Objects/DisplayMaterialsSO")]
public class DisplayMaterialsSO : ScriptableObject
{
    public List<MaterialsData> materialsData;

    public DisplayMaterialsSO(DisplayMaterialsSO database)
    {
        foreach (var data in database.materialsData)
        {
            this.materialsData.Add(data);
        }
    }
}

[Serializable]
public class MaterialsData
{
    [field: SerializeField]
    public int ID { get; private set; }

    [field: SerializeField]
    public Material material { get; private set; }
}
