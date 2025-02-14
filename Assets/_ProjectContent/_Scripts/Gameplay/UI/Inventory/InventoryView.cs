using System.Collections.Generic;
using Configs.Gameplay.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils.Extensions;
using Zenject;

namespace Gameplay.UI.Inventory
{
    public class InventoryView : MonoBehaviour
    {
        public readonly ReactiveCommand<int> BeginDragEvent = new();
        public readonly ReactiveCommand<int> EndDragEvent = new();
        public readonly ReactiveCommand<InventoryItemType> OnAddItemDropDown = new();
        public readonly ReactiveCommand DropItemEvent = new();

        private readonly List<RaycastResult> _results = new();
        private readonly List<InventoryItemView> _instantiatedInventoryItemViews = new();

        [SerializeField] private Image _dragAndDropImage;
        [SerializeField] private TMP_Dropdown _addItemDropDown;
        [SerializeField] private GraphicRaycaster _graphicRaycaster;
        [SerializeField] private InventoryItemView _inventoryItemViewPrefab;
        [SerializeField] private RectTransform _inventoryItemViewContentRoot;
        [SerializeField] private RectTransform _canvasTransform;

        private RectTransform _temporalDragAndDropImageRectTransform;
        private IInventoryItemIconsConfig _inventoryItemIconsConfig;
        private PointerEventData _pointerEventData;
        private Vector2 _mousePosition;
        private int _inventorySize;

        [Inject]
        private void Inject(IInventoryItemIconsConfig inventoryItemIconsConfig)
        {
            _inventoryItemIconsConfig = inventoryItemIconsConfig;
        }

        private void Start()
        {
            _pointerEventData = new PointerEventData(EventSystem.current);
            _temporalDragAndDropImageRectTransform = _dragAndDropImage.GetComponent<RectTransform>();
            _addItemDropDown.onValueChanged.AddListener(OnAddItemDropDownValueChanged);
        }

        private void OnDestroy()
        {
            _addItemDropDown.onValueChanged.RemoveListener(OnAddItemDropDownValueChanged);
        }

        public void SetInventorySize(int newInventorySize)
        {
            if (newInventorySize > _inventorySize)
            {
                for (int i = 0; i < newInventorySize - _inventorySize; i++)
                {
                    _instantiatedInventoryItemViews.Add(Instantiate(_inventoryItemViewPrefab, _inventoryItemViewContentRoot));
                }
            }

            if (newInventorySize < _inventorySize)
            {
                for (int i = 0; i < _inventorySize - newInventorySize; i++)
                {
                    Destroy(_instantiatedInventoryItemViews.FastLast());
                    _instantiatedInventoryItemViews.RemoveAt(_instantiatedInventoryItemViews.Count - 1);
                }
            }
            
            _inventorySize = newInventorySize;
        }

        public void UpdateInventoryView(in InventoryItemData[] inventoryItemData)
        {
            for (var i = 0; i < _inventorySize; i++)
            {
                InventoryItemType type = inventoryItemData[i].Type;
                _instantiatedInventoryItemViews[i].SetItem(_inventoryItemIconsConfig.ItemIcons[type], type, i);
            }
        }

        public void InitializeInventoryAddItemDropdown(in List<TMP_Dropdown.OptionData> options)
        {
            _addItemDropDown.options = options;
        }

        private void OnAddItemDropDownValueChanged(int itemTypeID)
        {
            OnAddItemDropDown.Execute((InventoryItemType) itemTypeID);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
            {
                ProcessDragAndDrop();
            }
            
            if (_dragAndDropImage.enabled)
            {
                _temporalDragAndDropImageRectTransform.anchoredPosition = (Vector2)Input.mousePosition /_canvasTransform.localScale;
            }
        }

        private void ProcessDragAndDrop()
        {
            _results.Clear();
            _mousePosition = Input.mousePosition;
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
                        _temporalDragAndDropImageRectTransform.anchoredPosition = _mousePosition /_canvasTransform.localScale;
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