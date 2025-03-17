using UnityEngine;
using Mirror;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UIElements;


public class MatchIDViewer: NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI _textField;

    [SerializeField] Dictionary<int, GameObject> _players;


    [SerializeField] NetworkManager _networkManager;
    [SerializeField] MatchInterestManagement _matchInterestedManagment;
    [SerializeField] NetworkMatch _networkMatch;

    private void Start()
    {
        

        var textFieldObj = GameObject.FindGameObjectsWithTag("DebugField");
        _textField = textFieldObj[0].GetComponent<TextMeshProUGUI>();

        //_textField.text = "Hello World";
        if (isServer)
        {
            NetworkServer.OnConnectedEvent += AddToPlayers;

            var networkManagerObj = GameObject.FindGameObjectsWithTag("NetworkManager");
            _networkManager = networkManagerObj[0].GetComponent<NetworkManager>();
            _matchInterestedManagment = networkManagerObj[0].GetComponent<MatchInterestManagement>();
            _networkMatch = gameObject.GetComponent<NetworkMatch>();

            
        }
        
        if(isClient)
        {
            _networkMatch = gameObject.GetComponent<NetworkMatch>();
        }

    }

    [Server]
    public void AddToPlayers(NetworkConnectionToClient conn)
    {
        //_players.Add(Random.Range(0,100), gameObject);
        Debug.Log(conn.connectionId);
        
    }

    private void Update()
    {
        if(isServer)
        {
            if (Input.GetKey(KeyCode.E))
            {
                byte[] bytes = new byte[16];
                System.BitConverter.GetBytes(100).CopyTo(bytes, 0);
                _networkMatch.matchId = new System.Guid(bytes);
            }

            if(_networkManager.numPlayers == 1)
            {

                //Debug.Log($"<color=blue> On SERVER {}:  </color>");
                //Debug.Log(GameObject.FindGameObjectsWithTag("Player")[0]);
                //Debug.Log(GameObject.FindGameObjectsWithTag("Player")[1]);
            }
        }
        _textField.text = _networkMatch.matchId.ToString();


        

        Debug.Log(_networkMatch.matchId.ToString());



    }


}
