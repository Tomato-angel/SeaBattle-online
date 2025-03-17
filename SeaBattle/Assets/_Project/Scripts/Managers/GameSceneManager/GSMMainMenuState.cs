using Scripts.FSM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;


public class GSMMainMenuState : FSMState
{
    public GSMMainMenuState(FSM fsm) : base(fsm)
    {

    }

    public override void Enter()
    {
        SceneManager.LoadSceneAsync("MainMenuScene", LoadSceneMode.Additive);
    }

    public override void Exit()
    {
        SceneManager.UnloadSceneAsync("MainMenuScene");
    }

    public override void Update()
    {
        // Релизация...
    }
}

