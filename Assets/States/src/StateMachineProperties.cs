using System;
using UnityEngine;

namespace States {
    [Serializable]
    public struct StateMachineProperties {
        [SerializeField]
        public string canStartField;

        [SerializeField]
        public string startButton;

        [SerializeField]
        public string startEvent;

        [SerializeField]
        public string canStopField;

        [SerializeField]
        public string stopButton;

        [SerializeField]
        public string stopEvent;
    }
}
