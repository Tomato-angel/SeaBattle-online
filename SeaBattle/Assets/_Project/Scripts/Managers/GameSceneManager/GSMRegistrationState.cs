using Scripts.FSM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;


public class GSMRegistrationState : FSMState
{
    public GSMRegistrationState(FSM fsm) : base(fsm)
    {

    }

    // Где стоит вызывать загрузку сцен, тут или в отдельном сервисе
    public override void Enter()
    {
        SceneManager.LoadSceneAsync("RegistrationScene",LoadSceneMode.Additive);
    }

    public override void Exit()
    {
        SceneManager.UnloadSceneAsync("RegistrationScene");
    }

    public override void Update()
    {
        // Релизация...
    }
}

