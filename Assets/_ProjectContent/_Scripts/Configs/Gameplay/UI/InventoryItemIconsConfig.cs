using AYellowpaper.SerializedCollections;
using Gameplay.UI.Inventory;
using UnityEngine;

namespace Configs.Gameplay.UI
{
    [CreateAssetMenu]
    public class InventoryItemIconsConfig : ScriptableObject, IInventoryItemIconsConfig
    {
        [field: SerializeField] public SerializedDictionary<InventoryItemType, Sprite> ItemIcons { get; private set; }
    }
}