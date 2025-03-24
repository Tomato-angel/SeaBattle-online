using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;



public class ShipDeploymentButton : MonoBehaviour
{
    [SerializeField] public bool IsInteractable { get; set; } = true;


    [SerializeField] TextMeshProUGUI _shipsCountTextField;
    public TextMeshProUGUI ShipsCountTextField { get => _shipsCountTextField; }
    [SerializeField] GameObject _pushingElement;

    [SerializeField] Vector3 _pushingElementNormalScale = new Vector3(1,1,1);
    [SerializeField] Vector3 _pushingElementPressedScale = new Vector3(2,2,2);
    [SerializeField][Range(0,1)] float _speed = 0.1f;

    [Header("Activate if on click")]
    [SerializeField] private UnityEvent _onClick;

    private IEnumerator OnClick()
    {
        yield return DynamicScaleToPressed();
        yield return DynamicScaleToNormal();
    }
    private IEnumerator DynamicScaleToPressed()
    {
        while(Vector3.Distance(_pushingElement.transform.localScale, _pushingElementPressedScale) > 0.01f)
        {
            _pushingElement.transform.localScale =  Vector3.Lerp(_pushingElement.transform.localScale, _pushingElementPressedScale, _speed);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return null;
    }
    private IEnumerator DynamicScaleToNormal()
    {
        while (Vector3.Distance(_pushingElement.transform.localScale, _pushingElementNormalScale) > 0.01f)
        {
            _pushingElement.transform.localScale = Vector3.Lerp(_pushingElement.transform.localScale, _pushingElementNormalScale, _speed);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return null;
    }
    private void ScaleToNormal()
    {
        _pushingElement.transform.localScale = _pushingElementNormalScale;
    }
    private void OnMouseDown()
    {
        if (!IsInteractable) return;

        StopAllCoroutines();
        _onClick?.Invoke();
        ScaleToNormal();
        StartCoroutine(OnClick());
    }

}

