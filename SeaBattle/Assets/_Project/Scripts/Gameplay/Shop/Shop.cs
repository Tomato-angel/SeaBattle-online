using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class Shop : MonoBehaviour, IInitializable
{
    [SerializeField] Player _localPlayer;
    public void Initialize()
    {
        {
            if (ProjectManager.root == null) return;
            if (ProjectManager.root.LocalPlayer == null) return;
            _localPlayer = ProjectManager.root.LocalPlayer;

            _localPlayer.playerBuyAbility += UpdateShopView;
            _localPlayer.playerUsedAbility += UpdateShopView;

            UpdateShopView();
        }
    }

    [SerializeField] GameObject _buyView;
    [SerializeField] TextMeshProUGUI _shopMessageField;
    [SerializeField] readonly string[] _messasges = {
        "- Ты хочешь приобрести способености? Что-ж, у меня демократичные цены",
        "- И снова ты, что на сей раз?",
        "- Новые способности, ну давай посмотрим что у меня есть",
        "- По-моему тебя решили одолеть, что-ж склоним чашу весов в нашу сторону",
        "- Вижу, азарт приключений не покидает тебя. Что на этот раз привлекло твой взор?",
        "- Мой ассортимент словно сокровищница знаний, и ты ищешь ключ к новым вершинам?",
        "- Хм, ты вернулся. Что ж, давай посмотрим, что я могу предложить для усиления твоих талантов.",
        "- Готов ли ты к следующему уровню? У меня есть именно то, что поможет тебе превзойти себя.",
        "- Твоё упорство достойно восхищения! Но помни, даже герою иногда нужна небольшая помощь.",
        "- Что ж, снова ты... что, не можешь оторваться от моей лавки? Да и кто бы смог.",
        "- Могущество ждёт тебя, друг. Что ты выберешь сегодня, чтобы укрепить свой дух и тело?",
        "- Мне льстит твоё постоянство, но не забывай, что истинная сила – в умении владеть тем, что имеешь. Но если тебе нужно больше, я здесь.",
        "- А вот и ты! Пришёл за добавкой, чтобы сразить врагов наповал? Выбирай с умом, у меня тут сокровища.",
        "- Мои способности не сделают тебя непобедимым, но они дадут тебе преимущество. Готов рискнуть?"
    };
    private IEnumerator DynamicWriteNewShopMessage(string message)
    {
        _shopMessageField.text = "";
        for (int i = 0; i < message.Length; ++i )
        {
            _shopMessageField.text += message[i];
            yield return new WaitForSeconds(0.05f);
        }
        yield return null;
    }
    public void WriteNewShopMessage()
    {
        StopAllCoroutines();
        StartCoroutine(DynamicWriteNewShopMessage(_messasges[UnityEngine.Random.Range(0, _messasges.Length - 1)]));
    }
    public void WriteNoMoneyMessage() 
    {
        StopAllCoroutines();
        StartCoroutine(DynamicWriteNewShopMessage("- У тебя нет денег, чтобы заинтересовать меня, сначала накопи"));
    }

    public void UpdateShopView()
    {
        if (_localPlayer.Money < 25) 
        {
            WriteNoMoneyMessage();
            _buyView.SetActive(false);
        }
        else
        {
            WriteNewShopMessage();
            _buyView.SetActive(true);
        }
    }



    public void BuyAbility()
    {
        if (_localPlayer == null) return;
        _localPlayer.CmdBuyNewAbility();
    }
}

