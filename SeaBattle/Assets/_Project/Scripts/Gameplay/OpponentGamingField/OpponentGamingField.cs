using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OpponentGamingField : MonoBehaviour, IInitializable
{
    [SerializeField] GameplayManager _gameplayManager;

    [SerializeField] GameObject _opponentGamingSegmentPrefab;
    [SerializeField] OpponentGamingSegment[,] _opponentGamingSegments;
    [SerializeField] GameObject _gameplayPanel;


    [SerializeField] Player _opponentPlayer;

    [SerializeField] AvatarsDatabase _avatarsDatabase;
    [SerializeField] Image _opponentAvatar;
    [SerializeField] TextMeshProUGUI _opponentNicknameField;

    [SerializeField] Image _opponentAbility1;
    [SerializeField] Image _opponentAbility2;
    [SerializeField] Image _opponentAbility3;

    private IEnumerator DynamicGenerateGrid()
    {
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

                _opponentGamingSegments[i, j].OnUp.AddListener(() => 
                {
                    Debug.Log(opponentGamingSegment.TargetCoordinates.x + " - " + opponentGamingSegment.TargetCoordinates.z);
                });

                //yield return new WaitForSeconds(0.5f);
            }
        }
        yield return null;
    }


    public void GenerateGrid()
    {
        StartCoroutine(DynamicGenerateGrid());
    }

    //РАБОТА С ОППОНЕНТОМ
    public void SetOpponentPlayer(Player player)
    {
        _opponentPlayer = player;
        _opponentAvatar.sprite = _avatarsDatabase.Get(player.playerData.avatarKey);
        _opponentNicknameField.text = player.playerData.nickName;

        //Тут также нужно оформить подписку на события изменения абилок, а также сделать сами абилки
    }

    public void Initialize()
    {
        GenerateGrid();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
