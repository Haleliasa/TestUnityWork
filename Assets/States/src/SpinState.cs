using AxGrid.FSM;
using AxGrid.Model;

namespace States {
    public class SpinState : FSMState {
        private readonly StateMachineProperties properties;
        private readonly string stopState;
        private readonly bool invokeStartEvent;

        public SpinState(
            StateMachineProperties properties,
            string stopState,
            bool invokeStartEvent) {
            this.properties = properties;
            this.stopState = stopState;
            this.invokeStartEvent = invokeStartEvent;
        }

        [Enter]
        private void Enter() {
            Model.Set(this.properties.canStartField, false);
            Model.Set(this.properties.canStopField, true);
            if (this.invokeStartEvent) {
                Invoke(this.properties.startEvent);
            }
        }

        [Exit]
        private void Exit() {
            Invoke(this.properties.stopEvent);
        }

        [Bind("OnBtn")]
        private void OnButton(string button) {
            if (button == this.properties.stopButton) {
                Parent.Change(this.stopState);
            }
        }
    }
}
