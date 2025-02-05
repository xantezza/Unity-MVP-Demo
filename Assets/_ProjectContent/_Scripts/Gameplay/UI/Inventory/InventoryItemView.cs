using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Inventory
{
    public class InventoryItemView : MonoBehaviour
    {
        [SerializeField] private Image ItemIcon;
        
        public int Index { get; private set; }
        public Sprite Sprite => ItemIcon.sprite;

        public void SetItem(Sprite itemIconSprite, InventoryItemType itemType, int index)
        {
            ItemIcon.enabled = itemType != InventoryItemType.None;
            ItemIcon.sprite = itemIconSprite;
            Index = index;
        }

        public void Hide()
        {
            ItemIcon.enabled = false;
        }
    }
}