using System;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    public bool IsActive { get; set; } = true;

    [SerializeField]
    private float previewOffset = 0.06f;

    [SerializeField]
    private GameObject cellIndicatorPrefab;
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialPrefab;
    private Material previewMaterialInstance;

    private Renderer cellIndicatorRenderer;
    private float rotation = 0;

    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialPrefab);
        cellIndicator = Instantiate(cellIndicatorPrefab, gameObject.transform);
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartShowingPreview(GameObject prefab, Coordinates size)
    {
        if (!IsActive) return;
        previewObject = Instantiate(prefab);
        previewObject.name = "previewObject";
        PreparePreview(previewObject);
        cellIndicator.SetActive(true);
        if (size.x > size.z)
            rotation = 0;
        else
            rotation = 90;
        previewObject.transform.rotation = Quaternion.Euler(0, rotation, 0);
    }

    private void PreparePreview(GameObject previewObject)
    {
        if (!IsActive) return;
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material = previewMaterialInstance;
        }
    }

    public void StopShowingPreview()
    {
        if (!IsActive) return;
        cellIndicator.SetActive(false);
        Destroy(previewObject);
    }

    public void UpdatePosition(Vector3 position, bool validity)
    {
        if (!IsActive) return;
        if (previewObject != null)
        {
            MovePreview(position);
            ApplyFeedbackPreview(validity);
        }
        MoveCursor(position);
        ApplyFeedbackCursor(validity);
    }

    private void MoveCursor(Vector3 position)
    {
        if (!IsActive) return;
        cellIndicator.transform.position = position;
    }

    private void MovePreview(Vector3 position)
    {
        if (!IsActive) return;
        previewObject.transform.position = new Vector3(
            position.x, previewOffset, position.z);
    }
    public void RotatePreview(bool validity)
    {
        if (!IsActive) return;
        if (previewObject == null)
            return;

        if (rotation == 0)
            rotation = 90;
        else
            rotation = 0;
        previewObject.transform.rotation = Quaternion.Euler(0, rotation, 0);
        ApplyFeedbackCursor(validity);
        ApplyFeedbackPreview(validity);
    }

    private void ApplyFeedbackCursor(bool validity)
    {
        if (!IsActive) return;
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;

        cellIndicatorRenderer.material.color = c;
    }
    private void ApplyFeedbackPreview(bool validity)
    {
        if (!IsActive) return;
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        
        previewMaterialInstance.color = c;
    }

    internal void StartShowingPreview()
    {
        if (!IsActive) return;
        cellIndicator.SetActive(true);
    }
}
