using Scripts.FSM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.SceneManagement;


public class GSMGameplayState : FSMState
{
    public GSMGameplayState(FSM fsm) : base(fsm)
    {

    }

    public override void Enter()
    {
        SceneManager.LoadSceneAsync("GameplayScene", LoadSceneMode.Additive);
    }

    public override void Exit()
    {
        SceneManager.UnloadSceneAsync("GameplayScene");
    }

    public override void Update()
    {
        // Релизация...
    }
}

