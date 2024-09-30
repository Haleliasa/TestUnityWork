#nullable enable

using AxGrid.Base;
using UnityEngine;

namespace AxGrid.Tools.Binders {
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemBind : Binder {
        [Header(EditorHeaders.Events)]
        [SerializeField]
        private string? playOnEvent;

        [SerializeField]
        private string? stopOnEvent;

        [Header(EditorHeaders.Properties)]
        [SerializeField]
        private bool withChildren = true;

        [SerializeField]
        private ParticleSystemStopBehavior stopBehavior = ParticleSystemStopBehavior.StopEmitting;

        private ParticleSystem system = null!;

        [OnAwake]
        private void Init() {
            this.system = GetComponent<ParticleSystem>();
        }

        [OnEnable]
        private void Enable() {
            if (!string.IsNullOrEmpty(this.playOnEvent)) {
                Model.EventManager.AddAction(this.playOnEvent, Play);
            }
            if (!string.IsNullOrEmpty(this.stopOnEvent)) {
                Model.EventManager.AddAction(this.stopOnEvent, Stop);
            }
        }

        [OnDisable]
        private void Disable() {
            if (!string.IsNullOrEmpty(this.playOnEvent)) {
                Model.EventManager.RemoveAction(this.playOnEvent, Play);
            }
            if (!string.IsNullOrEmpty(this.stopOnEvent)) {
                Model.EventManager.RemoveAction(this.stopOnEvent, Stop);
            }
        }

        private void Play() {
            this.system.Play(this.withChildren);
        }

        private void Stop() {
            this.system.Stop(this.withChildren, this.stopBehavior);
        }
    }
}