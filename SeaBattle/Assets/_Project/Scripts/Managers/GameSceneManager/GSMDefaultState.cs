using Scripts.FSM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class GSMDefaultState : FSMState
{
    private bool CheckPlayerDataAvailability()
    {

        PlayerData playerDataTMP = null;
        ProjectManager.root.ProjectServices
            .Resolve<JsonToFileStorageService>()
            .FastLoad<PlayerData>((playerData) =>
            {
                playerDataTMP = playerData;
            });
        return playerDataTMP != null;

    }

    public override void Enter()
    {
        if (CheckPlayerDataAvailability())
            EventBus.OnRequestForOpenMainMenu();
        else
            EventBus.OnRequestForRegistration();
        
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
       
    }

    public GSMDefaultState(FSM fsm) : base(fsm)
    {
    }
}

