using AxGrid.FSM;
using AxGrid.Model;

namespace States {
    public class ReadyState : FSMState {
        private readonly StateMachineProperties properties;
        private readonly string startState;

        public ReadyState(StateMachineProperties properties, string startState) {
            this.properties = properties;
            this.startState = startState;
        }

        [Enter]
        private void Enter() {
            Model.Set(this.properties.canStartField, true);
            Model.Set(this.properties.canStopField, false);
        }

        [Bind("OnBtn")]
        private void OnButton(string button) {
            if (button == this.properties.startButton) {
                Parent.Change(this.startState);
            }
        }
    }
}
