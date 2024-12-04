using AxGrid.Base;
using Items;
using System.Linq;
using UI;
using UnityEngine;

public class Bootstrap : MonoBehaviourExt {
    [SerializeField]
    private ItemConfig itemConfig;

    [SerializeField]
    private SpriteRoulette roulette;

    [SerializeField]
    private ObjectPool<SpriteRouletteItem> itemPool;

    [OnStart]
    private void Init() {
        this.roulette.Init(
            this.itemConfig.Items.Select(i => i.Sprite),
            itemPool: this.itemPool);
    }
}
