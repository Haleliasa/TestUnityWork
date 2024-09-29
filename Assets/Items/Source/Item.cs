using UnityEngine;

namespace Items {
    [CreateAssetMenu(
        fileName = nameof(Item),
        menuName = ItemsMenuNames.Items + nameof(Item))]
    public class Item : ScriptableObject {
        [SerializeField]
        private Sprite sprite;

        public Sprite Sprite => this.sprite;
    }
}
