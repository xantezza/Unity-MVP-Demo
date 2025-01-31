using System;
using System.Linq;
using Gameplay.Factories;
using Infrastructure.Providers.AssetReferenceProvider;
using JetBrains.Annotations;
using TMPro;
using UniRx;

namespace Gameplay.UI.Inventory
{
    [UsedImplicitly]
    public class InventoryPresenter : IDisposable
    {
        private readonly IAssetReferenceProvider _assetReferenceProvider;
        private readonly InventoryModel _inventoryModel;
        private readonly CompositeDisposable _disposables = new();

        private IDisposable _modelDataUpdateSubscription;
        private InventoryView _inventoryView;

        private int _currentlyDraggingItemIndex = -1;

        public InventoryPresenter(IGameplayModelsFactory gameplayModelsFactory, IAssetReferenceProvider assetReferenceProvider)
        {
            _inventoryModel = gameplayModelsFactory.InventoryModel;
            _assetReferenceProvider = assetReferenceProvider;

            InstantiateView();
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private async void InstantiateView()
        {
            var viewGameObject = await _assetReferenceProvider.InventoryViewAssetReference.InstantiateAsync().Task;
            _inventoryView = viewGameObject.GetComponent<InventoryView>();
            
            _inventoryView.InitializeInventoryAddItemDropdown(
                Enum.GetNames(typeof(InventoryItemType))
                    .Select(x => new TMP_Dropdown.OptionData(x))
                    .ToList()
            );
            
            _disposables.Add(_inventoryView.BeginDragEvent.Subscribe(OnBeginDragEvent));
            _disposables.Add(_inventoryView.EndDragEvent.Subscribe(OnEndDragEvent));
            _disposables.Add(_inventoryView.OnAddItemDropDown.Subscribe(OnAddItemDropdown));
            _disposables.Add(_inventoryView.DropItemEvent.Subscribe(OnDropItemEvent));
            _disposables.Add(_inventoryModel.Data.Subscribe(ModelDataUpdated));
        }

        private void ModelDataUpdated(InventoryData inventoryData)
        {
            _inventoryView.UpdateInventoryView(inventoryData.InventoryItemData);
        }

        private void OnBeginDragEvent(int itemIndex)
        {
            _currentlyDraggingItemIndex = itemIndex;
        }

        private void OnEndDragEvent(int itemIndex)
        {
            _inventoryModel.SwitchItems(_currentlyDraggingItemIndex, itemIndex);
            _currentlyDraggingItemIndex = -1;
        }
        
        private void OnAddItemDropdown(InventoryItemType type)
        {
            _inventoryModel.AddItem(type);
        }

        private void OnDropItemEvent(Unit _)
        {
            if (_currentlyDraggingItemIndex is >= 0 and < InventoryModel.INVENTORY_SIZE)
            {
                _inventoryModel.DropItem(_currentlyDraggingItemIndex);
            }

            _currentlyDraggingItemIndex = -1;
        }
    }
}