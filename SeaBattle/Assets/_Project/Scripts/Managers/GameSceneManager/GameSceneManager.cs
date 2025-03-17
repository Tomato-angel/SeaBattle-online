using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Collections;
using Unity.VisualScripting;
using Scripts.FSM;

[RequireComponent(typeof(NetworkIdentity))]
public class GameSceneManager : MonoBehaviour, IInitializable
{
    private FSM _fsm;

    public void Initialize()
    {
        _fsm = new FSM();
        _fsm.AddState(new GSMDefaultState(_fsm));
        _fsm.AddState(new GSMGameplayState(_fsm));
        _fsm.AddState(new GSMRegistrationState(_fsm));
        _fsm.AddState(new GSMMainMenuState(_fsm));

        EventBus.requestForRegistration += SetRegistrationState;
        EventBus.requestForStartGameplay += SetGameplayState;
        EventBus.requestForOpenMainMenu += SetMainMenuState;

        _fsm.SetState<GSMDefaultState>();
    }
    

    public void SetRegistrationState() => _fsm.SetState<GSMRegistrationState>();
    public void SetGameplayState() => _fsm.SetState<GSMGameplayState>();
    public void SetMainMenuState() => _fsm.SetState<GSMMainMenuState>();

    private void Awake()
    {
        
    }
    private void Start()
    {

    }
    private void Update()
    {
        _fsm.Update();
    }

    


}
