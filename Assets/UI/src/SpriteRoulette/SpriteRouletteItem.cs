using AxGrid.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    [RequireComponent(typeof(RectTransform))]
    public class SpriteRouletteItem : MonoBehaviourExt {
        [SerializeField]
        protected Image image;

        public RectTransform RectTransform { get; protected set; }

        public virtual void Init(Sprite sprite) {
            this.image.sprite = sprite;
        }

        public virtual void SetPosition(Vector2 position) {
            RectTransform.anchoredPosition = position;
        }

        [OnAwake]
        private void InitInternal() {
            RectTransform = GetComponent<RectTransform>();
        }
    }
}
