using System;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UniRx;
using UnityEngine.AddressableAssets;

namespace Gameplay.UI.Inventory
{
    [UsedImplicitly]
    public class InventoryPresenter : IDisposable
    {
        private readonly IInventoryModel _inventoryModel;
        private readonly CompositeDisposable _disposables = new();
        private readonly int _inventorySize;
        private InventoryView _inventoryView;

        private int _currentlyDraggingItemIndex = -1;

        public InventoryPresenter(int inventorySize, AssetReferenceGameObject inventoryViewReference, IInventoryModel inventoryModel)
        {
            _inventorySize = inventorySize;
            _inventoryModel = inventoryModel;

            InstantiateView(inventoryViewReference);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private async void InstantiateView(AssetReferenceGameObject inventoryViewReference)
        {
            var viewGameObject = await inventoryViewReference.InstantiateAsync().Task;
            _inventoryView = viewGameObject.GetComponent<InventoryView>();
            _inventoryView.SetInventorySize(_inventorySize);

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
            if (_currentlyDraggingItemIndex >= 0 && _currentlyDraggingItemIndex < _inventorySize)
            {
                _inventoryModel.DropItem(_currentlyDraggingItemIndex);
            }

            _currentlyDraggingItemIndex = -1;
        }
    }
}