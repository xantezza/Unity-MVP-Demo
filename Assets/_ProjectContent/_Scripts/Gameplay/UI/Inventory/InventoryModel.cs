using System.Linq;
using Infrastructure.Services.Logging;
using Infrastructure.Services.Saving;
using UniRx;

namespace Gameplay.UI.Inventory
{
    public class InventoryModel : IDataSaveable<InventoryData>, IInventoryModel
    {

        public ReactiveProperty<InventoryData> Data { get; } = new();
        private readonly InventoryItemData EmptyCell = new(InventoryItemType.None);
        private readonly IConditionalLoggingService _conditionalLoggingService;

        private InventoryItemData _itemBuffer;

        public SaveKey SaveId => SaveKey.Inventory;

        public InventoryData SaveData
        {
            get => Data.Value ??= new InventoryData
            {
                InventoryItemData = Enumerable.Repeat(EmptyCell, IInventoryModel.INVENTORY_SIZE).ToArray()
            };
            set
            {
                Data.Value = value;
                Data.SetValueAndForceNotify(Data.Value);
            }
        }

        public InventoryModel(ISaveService saveService, IConditionalLoggingService conditionalLoggingService)
        {
            _conditionalLoggingService = conditionalLoggingService;
            saveService.Process(this);
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
            if (previousIndex is < 0 or >= IInventoryModel.INVENTORY_SIZE || newIndex is < 0 or >= IInventoryModel.INVENTORY_SIZE)
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
            if (currentlyDraggingItemIndex is < 0 or >= IInventoryModel.INVENTORY_SIZE)
            {
                _conditionalLoggingService.Log("Invalid parameters.");
                return;
            }
            
            Data.Value.InventoryItemData[currentlyDraggingItemIndex] = EmptyCell;
            Data.SetValueAndForceNotify(Data.Value);
        }
    }
}