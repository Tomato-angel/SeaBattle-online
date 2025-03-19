using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsLogic : MonoBehaviour
{
    [SerializeField]
    PlacementSystem_new placementSystem;
    [SerializeField]
    Button prefab;
    [SerializeField]
    Transform parent;
    [SerializeField]
    List<Button> buttons = new();

    List<TextMeshProUGUI> tmpro = new();

    public virtual void CheckForAmount(List<Ship> shipsList)
    {
        for (int id = 1; id < shipsList.Count; id++)
        {
            if (shipsList[id].shipAmount > 0)
                buttons[id].interactable = true;
            else
                buttons[id].interactable = false;
            tmpro[id - 1].text = shipsList[id].shipAmount.ToString();
        }
    }

    public virtual void PrepareButtons(List<Ship> ships)
    {
        foreach (var shipData in ships)
        {
            Button button = Instantiate(prefab, parent);
            buttons.Add(button);
            button.gameObject.SetActive(true);
            TextMeshProUGUI[] TMPro = button.GetComponentsInChildren<TextMeshProUGUI>();
            if (shipData.shipID != 0)
            {
                TMPro[0].text = shipData.shipID.ToString();
                TMPro[1].text = shipData.shipAmount.ToString();
                tmpro.Add(TMPro[1]);
                button.onClick.AddListener(() => placementSystem.StartPlacement(shipData.shipID));
            }
            else
            {
                TMPro[0].text = "X";
                TMPro[1].gameObject.SetActive(false);
                button.onClick.AddListener(() => {
                    placementSystem.StartRemoval();
                });
                button.image.color = new Color(0.56f, 0.16f, 0.19f, 1.0f);
            }
        }
    }
}
