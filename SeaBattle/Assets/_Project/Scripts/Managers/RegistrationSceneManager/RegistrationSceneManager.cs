using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class RegistrationSceneManager : MonoBehaviour, IInitializable
{
    [SerializeField] Player _localPlayer;
    [SerializeField] RegistrationField _registrationField;
    [SerializeField] GameObject _registrationButton;

    public void Initialize()
    {
        _localPlayer = ProjectManager.root?.LocalPlayer;
    }

    public void Registration()
    {
        PlayerData playerData = new PlayerData()
        {
            nickName = _registrationField.Text
        };
        
        ProjectManager.root?.ProjectServices.Resolve<JsonToFileStorageService>().FastSave(playerData, (isSaveSuccessfully) =>
        {
            if(isSaveSuccessfully)
            {
                _localPlayer.CmdSetPlayerData(playerData);
            }
        });
    }

    void Start()
    {
        
    }

    void Update()
    {
        if(_registrationField) _registrationButton.SetActive(!_registrationField.IsEmpty);
    }
}
