using UnityEngine;
using AxGrid.Base;
using AxGrid.FSM;
using AxGrid;

namespace States {
    public class StateMachine : MonoBehaviourExt {
        [SerializeField]
        private StateMachineProperties properties = new() {
            canStartField = "CanStart",
            startButton = "StartButton",
            startEvent = "SpinStarted",
            canStopField = "CanStop",
            stopButton = "StopButton",
            stopEvent = "SpinStopped",
        };

        private FSM fsm;

        [OnStart]
        private void Init() {
            this.fsm = new FSM();
            Settings.Fsm = this.fsm;
            this.fsm.Add(
                new ReadyState(this.properties, startState: "SpinStart"),
                "Ready");
            this.fsm.Add(
                new SpinStartState(this.properties, "Spin"),
                "SpinStart");
            this.fsm.Add(
                new SpinState(this.properties, stopState: "Ready", invokeStartEvent: false),
                "Spin");
            this.fsm.Start("Ready");
        }

        [OnUpdate]
        private void UpdateFsm() {
            this.fsm.Update(Time.deltaTime);
        }
    }
}
