using System.Collections.Generic;
using UnityEngine;

namespace Items {
    [CreateAssetMenu(
        fileName = nameof(ItemConfig),
        menuName = ItemsMenuNames.Items + nameof(ItemConfig))]
    public class ItemConfig : ScriptableObject {
        [SerializeField]
        private Item[] items;

        public IReadOnlyList<Item> Items => this.items;
    }
}
