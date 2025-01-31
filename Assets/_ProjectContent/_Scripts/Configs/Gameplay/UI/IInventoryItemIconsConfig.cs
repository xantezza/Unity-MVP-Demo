using AYellowpaper.SerializedCollections;
using Gameplay.UI.Inventory;
using UnityEngine;

namespace Configs.Gameplay.UI
{
    public interface IInventoryItemIconsConfig
    {
        SerializedDictionary<InventoryItemType, Sprite> ItemIcons { get; }
    }
}