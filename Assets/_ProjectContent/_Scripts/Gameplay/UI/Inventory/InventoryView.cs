using System.Collections.Generic;
using Configs.Gameplay.UI;
using NUnit.Framework;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Gameplay.UI.Inventory
{
    public class InventoryView : MonoBehaviour
    {
        public readonly ReactiveCommand<int> BeginDragEvent = new();
        public readonly ReactiveCommand<int> EndDragEvent = new();
        public readonly ReactiveCommand DropItemEvent = new();

        [field: HideInInspector] [field: SerializeField] public InventoryItemView[] InventoryItemViews { get; private set; }
        [SerializeField] private Image _dragAndDropImage;
        [SerializeField] private GraphicRaycaster _graphicRaycaster;

        private readonly List<RaycastResult> _results = new();
        private RectTransform _temporalDragAndDropImageRectTransform;
        private IInventoryItemIconsConfig _inventoryItemIconsConfig;
        private PointerEventData _pointerEventData;
        private Vector2 _mousePosition;

        [Inject]
        private void Inject(IInventoryItemIconsConfig inventoryItemIconsConfig)
        {
            _inventoryItemIconsConfig = inventoryItemIconsConfig;
        }

        private void Awake()
        {
            _pointerEventData = new PointerEventData(EventSystem.current);
            _temporalDragAndDropImageRectTransform = _dragAndDropImage.GetComponent<RectTransform>();
        }

        private void OnValidate()
        {
            InventoryItemViews = GetComponentsInChildren<InventoryItemView>(true);
            Assert.True(InventoryItemViews.Length == InventoryModel.INVENTORY_SIZE);
        }

        public void UpdateInventoryView(InventoryData inventoryData)
        {
            for (var i = 0; i < InventoryModel.INVENTORY_SIZE; i++)
            {
                InventoryItemType type = inventoryData.InventoryItemData[i].Type;
                InventoryItemViews[i].SetItem(_inventoryItemIconsConfig.ItemIcons[type], type, i);
            }
        }

        private void Update()
        {
            _mousePosition = Input.mousePosition;
            _pointerEventData.position = _mousePosition;

            if (_dragAndDropImage.enabled)
            {
                _temporalDragAndDropImageRectTransform.anchoredPosition = _mousePosition;
            }

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
            {
                ProcessDragAndDrop();
            }
        }

        private void ProcessDragAndDrop()
        {
            _results.Clear();
            _graphicRaycaster.Raycast(_pointerEventData, _results);

            if (_results.Count <= 0) return;

            if (_results[0].gameObject.TryGetComponent<InventoryItemView>(out var inventoryItemView))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    BeginDragEvent.Execute(inventoryItemView.Index);

                    if (inventoryItemView.Sprite != null)
                    {
                        _dragAndDropImage.sprite = inventoryItemView.Sprite;
                        _temporalDragAndDropImageRectTransform.anchoredPosition = _mousePosition;
                        _dragAndDropImage.enabled = true;
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    EndDragEvent.Execute(inventoryItemView.Index);
                    _dragAndDropImage.enabled = false;
                }
            }

            if (_results[0].gameObject.TryGetComponent<InventoryView>(out _) && Input.GetMouseButtonUp(0))
            {
                DropItemEvent.Execute();
                _dragAndDropImage.enabled = false;
            }
        }
    }
}