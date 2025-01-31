using UniRx;

namespace Gameplay.UI.Inventory
{
    public interface IInventoryModel
    {
        public const int INVENTORY_SIZE = 40;
        ReactiveProperty<InventoryData> Data { get; }
        bool AddItem(InventoryItemType type);
        void SwitchItems(int previousIndex, int newIndex);
        void DropItem(int currentlyDraggingItemIndex);
    }
}