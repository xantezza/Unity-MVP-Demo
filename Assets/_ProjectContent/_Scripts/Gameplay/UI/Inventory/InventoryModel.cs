﻿using System.Linq;
using Infrastructure.Services.Logging;
using Infrastructure.Services.Saving;
using UniRx;

namespace Gameplay.UI.Inventory
{
    public class InventoryModel : IDataSaveable<InventoryData>, IInventoryModel
    {
        private readonly InventoryItemData EmptyCell = new(InventoryItemType.None);
        private readonly IConditionalLoggingService _conditionalLoggingService;
        private readonly int _inventorySize;
        private readonly ISaveService _saveService;

        private InventoryItemData _itemBuffer;

        public ReactiveProperty<InventoryData> Data { get; } = new();
        public SaveKey SaveKey { get; }
        public InventoryData SaveData => Data.Value;

        public InventoryModel(int inventorySize, SaveKey saveKey, ISaveService saveService, IConditionalLoggingService conditionalLoggingService)
        {
            SaveKey = saveKey;
            _saveService = saveService;
            _inventorySize = inventorySize;
            _conditionalLoggingService = conditionalLoggingService;
        }

        public void Initialize()
        {
            Data.Value = _saveService.Load(this) ?? new InventoryData
            {
                InventoryItemData = Enumerable.Repeat(EmptyCell, _inventorySize).ToArray(),
                InventorySize = _inventorySize
            };
            _saveService.AddToSaveables(this);
        }

        public bool AddItem(InventoryItemType type)
        {
            for (var index = 0; index < Data.Value.InventoryItemData.Length; index++)
            {
                if (Data.Value.InventoryItemData[index].Type == InventoryItemType.None)
                {
                    Data.Value.InventoryItemData[index] = new InventoryItemData(type);
                    Data.SetValueAndForceNotify(Data.Value);
                    return true;
                }
            }

            _conditionalLoggingService.Log("Inventory is full!");
            return false;
        }

        public void SwitchItems(int previousIndex, int newIndex)
        {
            if (previousIndex < 0 || previousIndex >= Data.Value.InventorySize || newIndex < 0 || newIndex >= Data.Value.InventorySize)
            {
                _conditionalLoggingService.Log("Invalid parameters.");
                return;
            }

            _itemBuffer = Data.Value.InventoryItemData[newIndex];
            SaveData.InventoryItemData[newIndex] = Data.Value.InventoryItemData[previousIndex];
            SaveData.InventoryItemData[previousIndex] = _itemBuffer;
            _itemBuffer = EmptyCell;

            Data.SetValueAndForceNotify(SaveData);
        }

        public void DropItem(int currentlyDraggingItemIndex)
        {
            if (currentlyDraggingItemIndex < 0 || currentlyDraggingItemIndex >= SaveData.InventorySize)
            {
                _conditionalLoggingService.Log("Invalid parameters.");
                return;
            }

            SaveData.InventoryItemData[currentlyDraggingItemIndex] = EmptyCell;
            Data.SetValueAndForceNotify(SaveData);
        }
    }
}