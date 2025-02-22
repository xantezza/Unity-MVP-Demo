﻿using UniRx;

namespace Gameplay.UI.Inventory
{
    public interface IInventoryModel
    {
        void Initialize();
        ReactiveProperty<InventoryData> Data { get; }
        bool AddItem(InventoryItemType type);
        void SwitchItems(int previousIndex, int newIndex);
        void DropItem(int currentlyDraggingItemIndex);
    }
}