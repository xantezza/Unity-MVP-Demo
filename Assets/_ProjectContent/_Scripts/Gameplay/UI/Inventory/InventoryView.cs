using System.Collections.Generic;
using Configs.Gameplay.UI;
using NUnit.Framework;
using TMPro;
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
        public readonly ReactiveCommand<InventoryItemType> OnAddItemDropDown = new();
        public readonly ReactiveCommand DropItemEvent = new();

        [field: HideInInspector] [field: SerializeField] public InventoryItemView[] InventoryItemViews { get; private set; }
        [SerializeField] private Image _dragAndDropImage;
        [SerializeField] private TMP_Dropdown _addItemDropDown;
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
            _addItemDropDown.onValueChanged.AddListener(OnAddItemDropDownValueChanged);
        }

        private void OnDestroy()
        {
            _addItemDropDown.onValueChanged.RemoveListener(OnAddItemDropDownValueChanged);
        }

        private void OnValidate()
        {
            InventoryItemViews = GetComponentsInChildren<InventoryItemView>(true);
            Assert.True(InventoryItemViews.Length == IInventoryModel.INVENTORY_SIZE);
        }

        public void UpdateInventoryView(InventoryItemData[] inventoryItemData)
        {
            for (var i = 0; i < IInventoryModel.INVENTORY_SIZE; i++)
            {
                InventoryItemType type = inventoryItemData[i].Type;
                InventoryItemViews[i].SetItem(_inventoryItemIconsConfig.ItemIcons[type], type, i);
            }
        }

        public void InitializeInventoryAddItemDropdown(List<TMP_Dropdown.OptionData> options)
        {
            _addItemDropDown.options = options;
        }
        
        private void OnAddItemDropDownValueChanged(int itemTypeID)
        {
            InventoryItemType itemType = (InventoryItemType) itemTypeID;
            OnAddItemDropDown.Execute(itemType);
        }

        private void Update()
        {
            _mousePosition = Input.mousePosition;

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
            _pointerEventData.position = _mousePosition;
            _graphicRaycaster.Raycast(_pointerEventData, _results);

            if (_results.Count <= 0) return;

            if (_results[0].gameObject.TryGetComponent<InventoryItemView>(out var inventoryItemView))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    BeginDragEvent.Execute(inventoryItemView.Index);

                    if (inventoryItemView.Sprite != null)
                    {
                        inventoryItemView.Hide();
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