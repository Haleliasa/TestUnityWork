using AxGrid.FSM;

namespace States {
    public class SpinStartState : FSMState {
        private readonly StateMachineProperties properties;
        private readonly string spinState;

        public SpinStartState(StateMachineProperties properties, string spinState) {
            this.properties = properties;
            this.spinState = spinState;
        }

        [Enter]
        private void Enter() {
            Model.Set(this.properties.canStartField, false);
            Model.Set(this.properties.canStopField, false);
            Invoke(this.properties.startEvent);
        }

        [One(3f)]
        private void EnterSpin() {
            Parent.Change(this.spinState);
        }
    }
}
