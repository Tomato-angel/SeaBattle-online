using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OpponentGamingField : MonoBehaviour, IInitializable
{
    private bool _isActive = true;
    public void SetActive(bool isActive)
    {
        _isActive = isActive;
    }

    [Header("Панель ожидания ")]
    [SerializeField] GameObject _waitingPanel;
    [Header("Панель Блокировки хода ")]
    [SerializeField] GameObject _lockPanel;

    [Space]
    [Header("Игровое поле и сегменты")]
    [SerializeField] GameplayManager _gameplayManager;
    [SerializeField] GameObject _opponentGamingSegmentPrefab;
    [SerializeField] OpponentGamingSegment[,] _opponentGamingSegments;
    [SerializeField] GameObject _gameplayPanel;

    [Space]
    [Header("Игровой процесс")]
    [SerializeField] Ability _currentAbility;
    [SerializeField] List<Coordinates> _targetAbilityCoordinates;

    [Space]
    [Header("Информация по оппоненту")]
    [SerializeField] Player _localPlayer;

    [SerializeField] Player _opponentPlayer;

    [SerializeField] TextMeshProUGUI _opponentMoney;

    [SerializeField] AvatarsDatabase _avatarsDatabase;
    [SerializeField] Image _opponentAvatar;
    [SerializeField] TextMeshProUGUI _opponentNicknameField;
    [SerializeField] AbilitiesSpritesDatabase _abilitiesSpritesDatabase;
    [SerializeField] Image[] _opponentAbilitiesViews;

    private IEnumerator DynamicGenerateGrid()
    {
        SetActive(false);
        int gridSize = 10;
        _opponentGamingSegments = new OpponentGamingSegment[gridSize, gridSize];
        for (int i = 0; i < gridSize; ++i)
        {
            for (int j = 0; j < gridSize; ++j)
            {
                _opponentGamingSegments[i, j] = new OpponentGamingSegment();
                _opponentGamingSegments[i, j] = Instantiate(_opponentGamingSegmentPrefab, _gameplayPanel.transform).GetComponent<OpponentGamingSegment>();
                _opponentGamingSegments[i, j].InitializeBy(this, new Coordinates(j, i));
                _opponentGamingSegments[i, j].name = $"Segment: X={j}, Z={i}";

                OpponentGamingSegment opponentGamingSegment = _opponentGamingSegments[i, j];

                opponentGamingSegment.OnEnter.AddListener(() =>
                {
                    SegmentInteractAbilityHovered(opponentGamingSegment);
                });
                opponentGamingSegment.OnExit.AddListener(() =>
                {
                    AllSegmentsInteractNormalize();
                });
                opponentGamingSegment.OnDown.AddListener(() =>
                {
                    SegmentInteractAbilityPush(opponentGamingSegment);
                });
                opponentGamingSegment.OnUp.AddListener(() =>
                {
                    AllSegmentsInteractNormalize();
                });

                //yield return new WaitForSeconds(0.1f);
            }
        }
        SetActive(true);
        yield return null;
    }
    public void GenerateGrid()
    {
        StartCoroutine(DynamicGenerateGrid());
    }
    public void GenerateGridAfterOpponentReady()
    {
        _waitingPanel.SetActive(false);
        GenerateGrid();
    }

    public void AllSegmentsInteractNormalize()
    {
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                _opponentGamingSegments[i, j].SetInteractNormal();
            }
        }
        _targetAbilityCoordinates.Clear();
    }
    public void SegmentInteractAbilityNormalize()
    {
        if (!_isActive) return;

        foreach(var coordinate in _targetAbilityCoordinates)
        {
            _opponentGamingSegments[coordinate.z, coordinate.x].SetInteractNormal();
        }
        _targetAbilityCoordinates.Clear();
    }
    public void SegmentInteractAbilityHovered(OpponentGamingSegment opponentGamingSegment)
    {
        if (!_isActive) return;
        if (_currentAbility == null) return;

        _targetAbilityCoordinates.Clear();
        for (int i = 0; i < _currentAbility.TargetCoordinates.Count; ++i)
        {
            var calculatedCoordinateX = _currentAbility.TargetCoordinates[i].x + opponentGamingSegment.TargetCoordinates.x;
            var calculatedCoordinateZ = _currentAbility.TargetCoordinates[i].z + opponentGamingSegment.TargetCoordinates.z;

            if(calculatedCoordinateX < 10 && calculatedCoordinateX >= 0)
            {
                if (calculatedCoordinateZ < 10 && calculatedCoordinateZ >= 0)
                {
                    if (!_opponentGamingSegments[calculatedCoordinateZ, calculatedCoordinateX].IsInteractable) continue;
                    
                    _targetAbilityCoordinates.Add(new Coordinates(calculatedCoordinateX, calculatedCoordinateZ));
                    _opponentGamingSegments[calculatedCoordinateZ, calculatedCoordinateX].SetInteractHover();
                }
            }
        }
    }
    public void SegmentInteractAbilityPush(OpponentGamingSegment opponentGamingSegment)
    {
        if (!_isActive) return;
        if (_currentAbility == null) return;
        if (_currentAbility.TargetCoordinates.Count == 0) return;
        //if (!opponentGamingSegment.IsInteractable) return;

        foreach (var coordinate in _targetAbilityCoordinates)
        {
            _opponentGamingSegments[coordinate.z, coordinate.x].SetInteractPush();
        }
        
        if (_localPlayer == null) return;
        _localPlayer.CmdUseAbility(_currentAbility, _targetAbilityCoordinates);
        _currentAbility = null;
        _targetAbilityCoordinates.Clear();
    }

    public void MarkScannedSegments(List<TileGameplayData> tilesGameplayData)
    {
        foreach(var tileGameplayData in tilesGameplayData)
        {
            Coordinates coordinates = tileGameplayData.Coordinate;
            if(tileGameplayData.status == TileGameplayStatus.shipCell)
                _opponentGamingSegments[coordinates.z, coordinates.x].SetStatus(OpponentGamingSegmentStatus.Checked);
            else
                _opponentGamingSegments[coordinates.z, coordinates.x].SetStatus(OpponentGamingSegmentStatus.Default);
            _opponentGamingSegments[coordinates.z, coordinates.x].IsInteractable = true;
        }
    }
    public void MarkAttackedSegments(List<TileGameplayData> tilesGameplayData)
    {
        string s = "";
        foreach (var element in tilesGameplayData)
        {
            s += element.Coordinate.x + " " + element.Coordinate.z + "\n";
        }
        Debug.Log(s);

        foreach (var tileGameplayData in tilesGameplayData)
        {
            Coordinates coordinates = tileGameplayData.Coordinate;
            if (tileGameplayData.status == TileGameplayStatus.shipCell)
                _opponentGamingSegments[coordinates.z, coordinates.x].SetStatus(OpponentGamingSegmentStatus.Hit);
            else
                _opponentGamingSegments[coordinates.z, coordinates.x].SetStatus(OpponentGamingSegmentStatus.Missed);
            _opponentGamingSegments[coordinates.z, coordinates.x].IsInteractable = false;
        }
    }


    //РАБОТА С ОППОНЕНТОМ
    public void SetOpponentPlayer(Player player)
    { 
        _opponentPlayer = player;
    }

    public void UpdateOpponentPlayerView()
    {
        _opponentAvatar.gameObject.SetActive(true);
        _opponentAvatar.sprite = _avatarsDatabase.Get(_opponentPlayer.playerData.avatarKey);

        _opponentNicknameField.gameObject.SetActive(true);
        _opponentNicknameField.text = _opponentPlayer.playerData.nickName;

        _opponentMoney.text = _opponentPlayer.Money.ToString();

        for (int i = 0; i < _opponentAbilitiesViews.Length; ++i)
        {
            _opponentAbilitiesViews[i].sprite = null;
            _opponentAbilitiesViews[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < _opponentAbilitiesViews.Length; ++i)
        {
            if (i >= _opponentPlayer.abilities.Count) break;
            _opponentAbilitiesViews[i].sprite = _abilitiesSpritesDatabase.Get(_opponentPlayer.abilities[i].ID);
            _opponentAbilitiesViews[i].gameObject.SetActive(true);
        }
    }

    public void SetCurrentAbility(Ability ability)
    {
        _currentAbility = ability;
    }

    public void Initialize()
    {
        /*
        _currentAbility = new SimpleShoot();

        GenerateGridAfterOpponentReady();
        */

        _waitingPanel.SetActive(true);

        _localPlayer = ProjectManager.root.LocalPlayer;
        _localPlayer.attackOpponent += MarkAttackedSegments;
        _localPlayer.scanOpponent += MarkScannedSegments;

        _localPlayer.setCurrentAbility += SetCurrentAbility;

        _localPlayer.moneyUpdate += (playerMoney, opponentMoney) => { _opponentMoney.text = opponentMoney.ToString(); };

        Player opponentPlayer = _localPlayer.GetAnotherPlayerFromLocalScene();
        SetOpponentPlayer(opponentPlayer);
        UpdateOpponentPlayerView();
        _localPlayer.opponentBuyAbility += UpdateOpponentPlayerView;
        _localPlayer.opponentUsedAbility += UpdateOpponentPlayerView;

        
        _lockPanel.SetActive(!_localPlayer.isHost);
        // _localPlayer.opponentUsedAbility += LockGameplay;
        // _localPlayer.playerUsedAbility += LockGameplay;
        _localPlayer.opponentUsedAbility += () => _lockPanel.SetActive(false);
         _localPlayer.playerUsedAbility += () => _lockPanel.SetActive(true);

        _localPlayer.allOpponentShipsAreDeployed += GenerateGridAfterOpponentReady;
        _localPlayer.allOpponentShipsAreDeployed += () => _waitingPanel.SetActive(false);

        if (_localPlayer.IsAllOpponentShipsAreDeployed)
        {
            GenerateGridAfterOpponentReady();
            _waitingPanel.SetActive(false);
        }
    }

    public void LockGameplay()
    {
        //_lockPanel.SetActive(!_localPlayer._isPlayerTurn);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        //_currentAbility = _localPlayer.CurrentAbility;
        /*
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _currentAbility = new QuadricShoot();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            _currentAbility = new SimpleShoot();
        }
        */
    }
}
