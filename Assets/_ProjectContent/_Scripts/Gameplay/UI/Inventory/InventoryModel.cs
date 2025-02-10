using System.Linq;
using Infrastructure.Services.Logging;
using Infrastructure.Services.Saving;
using UniRx;

namespace Gameplay.UI.Inventory
{
    public class InventoryModel : IDataSaveable<InventoryData>, IInventoryModel
    {
        private readonly InventoryItemData EmptyCell = new(InventoryItemType.None);
        private readonly IConditionalLoggingService _conditionalLoggingService;
        private readonly int _defaultInventorySize = 0;

        private InventoryItemData _itemBuffer;

        public ReactiveProperty<InventoryData> Data { get; } = new();
        public SaveKey SaveId => SaveKey.Inventory;

        public InventoryData SaveData
        {
            get => Data.Value ??= new InventoryData
            {
                InventoryItemData = Enumerable.Repeat(EmptyCell, _defaultInventorySize).ToArray(),
                InventorySize = _defaultInventorySize
            };
            set
            {
                Data.Value = value;
                Data.SetValueAndForceNotify(Data.Value);
            }
        }

        public InventoryModel(int inventorySize, ISaveService saveService, IConditionalLoggingService conditionalLoggingService)
        {
            _defaultInventorySize = inventorySize;
            _conditionalLoggingService = conditionalLoggingService;
            saveService.Process(this);
        }

        public void SetInventorySize(int inventorySize)
        {
            if (Data.Value.InventorySize != 0) return;
            
            Data.Value.InventorySize = inventorySize;

            if (Data.Value.InventoryItemData.Length == _defaultInventorySize)
            {
                Data.Value = new InventoryData
                {
                    InventoryItemData = Enumerable.Repeat(EmptyCell, Data.Value.InventorySize).ToArray()
                };
            }
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
            if (previousIndex < 0 || previousIndex >= SaveData.InventorySize || newIndex < 0 || newIndex >= SaveData.InventorySize)
            {
                _conditionalLoggingService.Log("Invalid parameters.");
                return;
            }

            _itemBuffer = Data.Value.InventoryItemData[newIndex];
            Data.Value.InventoryItemData[newIndex] = Data.Value.InventoryItemData[previousIndex];
            Data.Value.InventoryItemData[previousIndex] = _itemBuffer;
            _itemBuffer = EmptyCell;

            Data.SetValueAndForceNotify(Data.Value);
        }

        public void DropItem(int currentlyDraggingItemIndex)
        {
            if (currentlyDraggingItemIndex < 0 || currentlyDraggingItemIndex >= SaveData.InventorySize)
            {
                _conditionalLoggingService.Log("Invalid parameters.");
                return;
            }

            Data.Value.InventoryItemData[currentlyDraggingItemIndex] = EmptyCell;
            Data.SetValueAndForceNotify(Data.Value);
        }
    }
}