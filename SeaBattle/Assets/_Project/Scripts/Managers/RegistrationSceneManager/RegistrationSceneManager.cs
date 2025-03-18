using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RegistrationSceneManager : MonoBehaviour, IInitializable
{
    [SerializeField] Player _localPlayer = null;

    [SerializeField] AvatarsDatabase _avatarsDatabase = null;
    [SerializeField] GameObject _avatarView = null;
    [SerializeField] int _currentAvatarIndex = 0;
    [SerializeField] Sprite _currentAvatarSprite = null;
    [SerializeField] string _currentAvatarKey = null;

    [SerializeField] GameObject _nextAvatarButton;
    [SerializeField] GameObject _beforeAvatarButton;
 
    [SerializeField] TMP_InputField _registrationField = null;
    [SerializeField] GameObject _registrationButton = null;

    public void Initialize()
    {
        _localPlayer = ProjectManager.root?.LocalPlayer;
        _currentAvatarSprite = _avatarsDatabase.Get(0).Value;
        _nextAvatarButton.SetActive(true);
        _beforeAvatarButton.SetActive(false);

    }

    public void Registration()
    {
        PlayerData playerData = new PlayerData()
        {
            avatarKey = _avatarsDatabase.Get(_currentAvatarIndex).Key,
            nickName = _registrationField.text
        };
        
        ProjectManager.root?.ProjectServices
            .Resolve<JsonToFileStorageService>()
            .FastSave(playerData, (isSaveSuccessfully) =>
            {
                if(isSaveSuccessfully)
                {
                    _localPlayer.CmdSetPlayerData(playerData);
                    EventBus.OnRequestForOpenMainMenu();
                }
            });
        
    }


    public void NextAvatar()
    {
        _nextAvatarButton.SetActive(true);
        _beforeAvatarButton.SetActive(true);
        _currentAvatarIndex = (_currentAvatarIndex + 1) % _avatarsDatabase.Count;
        if (_currentAvatarIndex > _avatarsDatabase.Count)
        {
            _nextAvatarButton.SetActive(false); 
            _currentAvatarIndex = _avatarsDatabase.Count - 1;
            return;
        }  
        _currentAvatarSprite = _avatarsDatabase.Get(_currentAvatarIndex).Value;
        _currentAvatarKey = _avatarsDatabase.Get(_currentAvatarIndex).Key;
        _avatarView.GetComponent<Image>().sprite = _currentAvatarSprite;
    }
    public void BeforeAvatar()
    {
        _nextAvatarButton.SetActive(true);
        _beforeAvatarButton.SetActive(true);
        _currentAvatarIndex = (_currentAvatarIndex - 1) % _avatarsDatabase.Count;
        if (_currentAvatarIndex < 0)
        {
            _beforeAvatarButton.SetActive(false);
            _currentAvatarIndex = 0;
            return;
        }
        _currentAvatarSprite = _avatarsDatabase.Get(_currentAvatarIndex).Value;
        _currentAvatarKey = _avatarsDatabase.Get(_currentAvatarIndex).Key;
        _avatarView.GetComponent<Image>().sprite = _currentAvatarSprite;
    }

    void Start()
    {
        Debug.Log(-1 % 10);
    }

    void Update()
    {
        if(_registrationField) _registrationButton.SetActive(_registrationField.text != string.Empty);
    }
}
