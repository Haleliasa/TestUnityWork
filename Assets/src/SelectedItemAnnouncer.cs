using AxGrid;
using AxGrid.Base;
using AxGrid.Model;
using Items;
using UnityEngine;

public class SelectedItemAnnouncer : MonoBehaviourExtBind {
    [SerializeField]
    private ItemConfig itemConfig;

    [SerializeField]
    private string selectedItemField = "SelectedItem";

    [Bind("On{selectedItemField}Changed")]
    private void AnnounceSelectedItem(int index) {
        if (index < 0 || index >= this.itemConfig.Items.Count) {
            return;
        }
        Log.Info($"Selected Item: {this.itemConfig.Items[index].name}");
    }
}
