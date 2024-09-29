#nullable enable

using AxGrid.Base;
using AxGrid.Path;
using System.Collections.Generic;
using UnityEngine;

namespace UI {
    [RequireComponent(typeof(RectTransform))]
    public class SpriteRoulette : MonoBehaviourExt {
        [SerializeField]
        private SpriteRouletteItem? itemPrefab;

        [SerializeField]
        private RectTransform? center;

        [SerializeField]
        private string? spinEvent;

        [SerializeField]
        private string? stopEvent;

        [SerializeField]
        private string? selectedItemField;

        [Min(0f)]
        [SerializeField]
        private float itemHeight = 100f;

        [Min(0f)]
        [SerializeField]
        private float itemSpacing = 10f;

        [Min(1)]
        [SerializeField]
        private int itemCount = 5;

        [Tooltip("units/sec")]
        [Min(0f)]
        [SerializeField]
        private float speed = 1000f;

        [Tooltip("sec")]
        [Min(0f)]
        [SerializeField]
        private float accelerationDuration = 3f;

        private RectTransform rectTransform = null!;
        private readonly List<Sprite> itemSprites = new();
        private readonly Queue<(PooledOrInstantiated<SpriteRouletteItem>, int)> items = new();
        private IObjectPool<SpriteRouletteItem>? itemPool;
        private CPath? accelerationPath;
        private float currentSpeed = 0f;
        private float spinTime = 0f;
        private float spinLoopDist = 0f;

        private Vector2 Center =>
            this.center != null
            ? this.center.anchoredPosition
            : this.rectTransform.rect.center;

        public void Init(
            IEnumerable<Sprite> itemSprites,
            IObjectPool<SpriteRouletteItem>? itemPool = null) {
            this.itemSprites.Clear();
            this.itemSprites.AddRange(itemSprites);
            this.itemPool = itemPool;
            ClearItems();

            if (this.itemSprites.Count == 0
                || (this.itemPrefab == null && this.itemPool == null)) {
                return;
            }

            for (int i = 0; i < this.itemCount; i++) {
                AddItem();
            }
        }

        public void Spin() {
            if (this.items.Count == 0) {
                return;
            }

            if (this.accelerationPath != null) {
                DestroyPath(this.accelerationPath);
                this.accelerationPath = null;
            }

            if (this.accelerationDuration > 0f) {
                this.accelerationPath = CreateNewPath().EasingQuadEaseIn(
                    this.accelerationDuration,
                    0f,
                    this.speed,
                    speed => this.currentSpeed = speed);
            } else {
                this.currentSpeed = this.speed;
            }

            Path.StopPath();
            this.spinTime = 0f;
            this.spinLoopDist = 0f;
            Path = new CPath(loop: true).Add(path => {
                float deltaTime = path.PathStartTimeF - this.spinTime;
                this.spinTime = path.PathStartTimeF;
                float deltaDist = this.currentSpeed * deltaTime;
                foreach ((PooledOrInstantiated<SpriteRouletteItem> item, _) in this.items) {
                    item.Object.SetPosition(
                        item.Object.RectTransform.anchoredPosition - new Vector2(0f, deltaDist));
                }

                this.spinLoopDist += deltaDist;
                if (this.spinLoopDist >= (this.itemHeight + this.itemSpacing)) {
                    this.spinLoopDist = 0f;
                    return Status.OK;
                }

                return Status.Continue;
            }).Action(() => {
                RemoveItem();
                AddItem();
            });
        }

        public void Stop(int? itemIndex = null) {
            // TODO: implement
        }

        [OnAwake]
        private void InitInternal() {
            this.rectTransform = GetComponent<RectTransform>();
        }

        [OnEnable]
        private void Enable() {
            if (!string.IsNullOrEmpty(this.spinEvent)) {
                Model.EventManager.AddAction(this.spinEvent, Spin);
            }
            if (!string.IsNullOrEmpty(this.stopEvent)) {
                Model.EventManager.AddAction(this.stopEvent, StopOnEvent);
            }
        }

        [OnDisable]
        private void Disable() {
            if (!string.IsNullOrEmpty(this.spinEvent)) {
                Model.EventManager.RemoveAction(this.spinEvent, Spin);
            }
            if (!string.IsNullOrEmpty(this.stopEvent)) {
                Model.EventManager.RemoveAction(this.stopEvent, StopOnEvent);
            }
        }

        private void OnValidate() {
            if ((this.itemCount & 1) == 0) {
                this.itemCount++;
            }
        }

        private void StopOnEvent() {
            Stop();
        }

        private void AddItem() {
            if (this.itemSprites.Count == 0) {
                return;
            }
            PooledOrInstantiated<SpriteRouletteItem> item =
                PooledOrInstantiated<SpriteRouletteItem>.Create(this.itemPool, this.itemPrefab);
            item.Object.transform.SetParent(this.rectTransform);
            int index = Random.Range(0, this.itemSprites.Count);
            item.Object.Init(this.itemSprites[index]);
            float y = (this.itemHeight + this.itemSpacing)
                * (this.items.Count - (this.itemCount / 2));
            item.Object.SetPosition(Center + new Vector2(0f, y));
            this.items.Enqueue((item, index));
        }

        private void RemoveItem() {
            if (this.items.TryDequeue(
                out (PooledOrInstantiated<SpriteRouletteItem> item, int) item)) {
                item.item.Destroy();
            }
        }

        private void ClearItems() {
            while (this.items.TryDequeue(
                out (PooledOrInstantiated<SpriteRouletteItem> item, int) item)) {
                item.item.Destroy();
            }
        }
    }
}
