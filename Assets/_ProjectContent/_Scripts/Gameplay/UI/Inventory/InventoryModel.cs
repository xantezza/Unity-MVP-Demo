using System.Linq;
using Infrastructure.Services.Logging;
using Infrastructure.Services.Saving;
using UniRx;

namespace Gameplay.UI.Inventory
{
    public class InventoryModel : IDataSaveable<InventoryData>
    {
        public const int INVENTORY_SIZE = 40;
        private readonly InventoryItemData EmptyCell = new(InventoryItemType.None);

        public readonly ReactiveProperty<InventoryData> Data = new();
        private float _multipliedTime;
        private InventoryItemData _itemBuffer;
        private IConditionalLoggingService _conditionalLoggingService;

        public SaveKey SaveId => SaveKey.Inventory;

        public InventoryData SaveData
        {
            get => Data.Value ??= new InventoryData
            {
                InventoryItemData = Enumerable.Repeat(EmptyCell, INVENTORY_SIZE).ToArray()
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

        public void SwitchItems(int previousIndex, int newIndex)
        {
            if (previousIndex is < 0 or >= INVENTORY_SIZE || newIndex is < 0 or >= INVENTORY_SIZE)
            {
                _conditionalLoggingService.LogError("Index is out of range.");
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
            if (currentlyDraggingItemIndex is < 0 or >= INVENTORY_SIZE)
            {
                _conditionalLoggingService.LogError("Index is out of range.");
                return;
            }
            
            Data.Value.InventoryItemData[currentlyDraggingItemIndex] = EmptyCell;
            Data.SetValueAndForceNotify(Data.Value);
        }
    }
}