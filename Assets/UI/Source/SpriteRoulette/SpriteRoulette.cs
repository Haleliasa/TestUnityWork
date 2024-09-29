#nullable enable

using AxGrid.Base;
using AxGrid.Path;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Item = PooledOrInstantiated<UI.SpriteRouletteItem>;

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

        [Min(0f)]
        [SerializeField]
        private float stopOverspinDistance = 50f;

        [Tooltip("sec")]
        [Min(0f)]
        [SerializeField]
        private float stopOverspinDuration = 0.5f;

        [Tooltip("sec")]
        [Min(0f)]
        [SerializeField]
        private float stopRollbackDuration = 0.5f;

        private RectTransform rectTransform = null!;
        private CPath? spinPath;
        private CPath? accelerationPath;
        private CPath? stopPath;
        private readonly List<Sprite> itemSprites = new();
        private readonly Queue<(Item, int)> items = new();
        private IObjectPool<SpriteRouletteItem>? itemPool;
        private float currentSpeed = 0f;
        private float itemsOffset = 0f;
        private float spinTime = 0f;

        private Vector2 Center =>
            this.center != null
            ? this.center.anchoredPosition
            : this.rectTransform.rect.center;

        private float ItemInterval => this.itemHeight + this.itemSpacing;

        private bool IsSpinning => this.spinPath?.Count > 0;

        public void Init(
            IEnumerable<Sprite> itemSprites,
            IObjectPool<SpriteRouletteItem>? itemPool = null) {
            this.itemSprites.Clear();
            this.itemSprites.AddRange(itemSprites);
            this.itemPool = itemPool;
            ClearItems();
            if (this.itemSprites.Count == 0
                || (this.itemPrefab == null
                    && this.itemPool == null)) {
                return;
            }
            for (int i = 0; i < this.itemCount; i++) {
                AddTopItem();
            }
        }

        public void Spin() {
            if (this.items.Count == 0
                || IsSpinning) {
                return;
            }

            this.stopPath?.StopPath();

            if (this.accelerationDuration > 0f) {
                this.currentSpeed = 0f;
                this.accelerationPath = CreateNewPath().EasingQuadEaseIn(
                    this.accelerationDuration,
                    0f,
                    this.speed,
                    speed => this.currentSpeed = speed);
            } else {
                this.currentSpeed = this.speed;
            }

            this.spinTime = 0f;
            this.spinPath = CreateNewPath()
                .Add(path => {
                    float deltaTime = path.PathStartTimeF - this.spinTime;
                    this.spinTime = path.PathStartTimeF;
                    float offset = this.itemsOffset + (this.currentSpeed * deltaTime);
                    MoveItems(offset);
                    return offset >= ItemInterval ? Status.OK : Status.Continue;
                }).Action(() => {
                    RemoveBottomItem();
                    AddTopItem();
                });
            this.spinPath.Loop = true;
        }

        public void Stop() {
            if (!IsSpinning) {
                return;
            }

            this.spinPath?.StopPath();
            this.accelerationPath?.StopPath();

            int selectedIndex = this.items.Count / 2;
            float rollbackOffset = 0f;
            if (this.itemsOffset > (ItemInterval / 2)) {
                selectedIndex++;
                rollbackOffset = ItemInterval;
            }

            if (!string.IsNullOrEmpty(this.selectedItemField)) {
                (_, int itemIndex) = this.items.ElementAt(selectedIndex);
                Model.Set(this.selectedItemField, itemIndex);
            }

            float overspinOffset = this.itemsOffset + this.stopOverspinDistance;
            this.stopPath = CreateNewPath()
                .EasingCircEaseOut(
                    this.stopOverspinDuration,
                    this.itemsOffset,
                    overspinOffset,
                    offset => MoveItems(offset))
                .EasingQuadEaseIn(
                    this.stopRollbackDuration,
                    overspinOffset,
                    rollbackOffset,
                    offset => MoveItems(offset));
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
                Model.EventManager.AddAction(this.stopEvent, Stop);
            }
        }

        [OnDisable]
        private void Disable() {
            if (!string.IsNullOrEmpty(this.spinEvent)) {
                Model.EventManager.RemoveAction(this.spinEvent, Spin);
            }
            if (!string.IsNullOrEmpty(this.stopEvent)) {
                Model.EventManager.RemoveAction(this.stopEvent, Stop);
            }
        }

        private void OnValidate() {
            if ((this.itemCount & 1) == 0) {
                this.itemCount++;
            }
        }

        private void MoveItems(float offset) {
            int i = 0;
            float interval = ItemInterval;
            int countHalf = this.itemCount / 2;
            foreach ((Item item, _) in this.items) {
                float y = (interval * (i++ - countHalf)) - offset;
                item.Object.SetPosition(Center + new Vector2(0f, y));
            }
            this.itemsOffset = offset;
        }

        private void AddTopItem() {
            if (this.itemSprites.Count == 0) {
                return;
            }
            Item item = Item.Create(this.itemPool, this.itemPrefab);
            item.Object.transform.SetParent(this.rectTransform);
            int index = Random.Range(0, this.itemSprites.Count);
            item.Object.Init(this.itemSprites[index]);
            float y = (ItemInterval * (this.items.Count - (this.itemCount / 2)))
                - this.itemsOffset;
            item.Object.SetPosition(Center + new Vector2(0f, y));
            this.items.Enqueue((item, index));
        }

        private void RemoveBottomItem() {
            if (this.items.TryDequeue(out (Item item, int) item)) {
                item.item.Destroy();
                this.itemsOffset -= ItemInterval;
            }
        }

        private void ClearItems() {
            while (this.items.TryDequeue(out (Item item, int) item)) {
                item.item.Destroy();
            }
            this.itemsOffset = 0f;
        }
    }
}
