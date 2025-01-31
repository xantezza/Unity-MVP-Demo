using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Inventory
{
    public class InventoryItemView : MonoBehaviour
    {
        [SerializeField] private Image ItemIcon;

        [field: SerializeField, HideInInspector] public int Index { get; private set; }
        [field: SerializeField, HideInInspector] public Sprite Sprite { get; private set; }

        public void SetItem(Sprite itemIconSprite, InventoryItemType itemType, int index)
        {
            ItemIcon.enabled = itemType != InventoryItemType.None;
            ItemIcon.sprite = itemIconSprite;
            Sprite = itemIconSprite;
            Index = index;
        }
    }
}