using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public enum OpponentGamingSegmentStatus
{
    Default,
    Checked,
    Missed,
    Hit,
}

public class OpponentGamingSegment : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [field: SerializeField]
    public bool IsInteractable { get; set; } = true;
    public bool IsInitialized { get; private set; } = false;

    [SerializeField] OpponentGamingField _parentGamingField;
    public OpponentGamingField PparentGamingField => _parentGamingField;
    [SerializeField] Coordinates _targetCoordinates;
    public Coordinates TargetCoordinates => _targetCoordinates;
    public void InitializeBy(OpponentGamingField parentGamingField, Coordinates targetCoordinates)
    {
        IsInteractable = true;
        IsInitialized = true;
        _parentGamingField = parentGamingField;
        _targetCoordinates = targetCoordinates;
        _currentStatus = OpponentGamingSegmentStatus.Default;
    }
 


    [SerializeField] Image _segmentView;
    [SerializeField] Image _interactView;
    [SerializeField] OpponentGamingSegmentStatus _currentStatus = OpponentGamingSegmentStatus.Default;
    [SerializeField] GamingSegmentSpritesDatabase _spritesDatabase;
    public void SetStatus(OpponentGamingSegmentStatus status)
    {
        _currentStatus = status;
        _segmentView.sprite = _spritesDatabase.Get(status);
        if(_currentStatus > OpponentGamingSegmentStatus.Checked)
            IsInteractable = false;

    }

    [Space]
    [Header("On pointer events activity")]
    private string test;
    public UnityEvent onDown = null;
    public UnityEvent OnDown => onDown;
    [SerializeField] UnityEvent onUp = null;
    public UnityEvent OnUp => onUp;
    [SerializeField] UnityEvent onEnter = null;
    public UnityEvent OnEnter => onEnter;
    [SerializeField] UnityEvent onExit = null;
    public UnityEvent OnExit => onExit;

    public void OnPointerDown(PointerEventData eventData)
    {
        //if (!IsInteractable) return;
        //if (!IsInitialized) return;

        if(_currentStatus <= OpponentGamingSegmentStatus.Checked)
        {
            _interactView.gameObject.SetActive(true);
            _interactView.color = Color.blue;
        }
   
        onDown?.Invoke();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        //if (!IsInteractable) return;
        //if (!IsInitialized) return;


        _interactView.gameObject.SetActive(false);
        _interactView.color = Color.white;

        onUp?.Invoke();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //if (!IsInteractable) return;
        //if (!IsInitialized) return;

        _interactView.gameObject.SetActive(true);
        _interactView.color = Color.green;

        onEnter?.Invoke();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //if (!IsInteractable) return;
        //if (!IsInitialized) return;

        _interactView.gameObject.SetActive(false);
        _interactView.color = Color.white;

        onExit?.Invoke();
    }

    private void Awake()
    {
        
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
