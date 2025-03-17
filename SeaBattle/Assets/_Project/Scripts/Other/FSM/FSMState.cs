

namespace Scripts.FSM
{
    public abstract class FSMState
    {
        protected readonly FSM _fsm;

        public FSMState(FSM fsm)
        {
            _fsm = fsm;
        }

        public abstract void Enter();
        public abstract void Exit();
        public abstract void Update();

    }
}

