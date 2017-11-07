using Stateless;
using System;
using System.Collections.Generic;
using System.Text;

namespace LutronCaseta.State
{
    public class BridgeStateMachine
    {

        #region available states and triggers

        enum State { Disconnected, Connected, LoggedIn }
        enum Trigger { Connect, Close, LogIn }

        #endregion

        static readonly State INITIAL_STATE = State.Disconnected;

        State _state = INITIAL_STATE;
        StateMachine<State, Trigger> _machine;
        StateMachine<State, Trigger>.TriggerWithParameters<string> _assignTrigger;
    }
}
