using System;
using UnityEngine;

namespace Gameplay.UI.Inventory
{
    public enum InventoryItemType
    {
        None = 0,
        Apple = 1,
        Axe = 2,
        Bracers = 3,
        Book = 4,
        Boots = 5,
        Scroll = 6,
        Meat = 7,
        Shield = 8,
    }

    [Serializable]
    public class InventoryItemData
    {
        public InventoryItemData(InventoryItemType type)
        {
            Type = type;
        }
        
        public readonly InventoryItemType Type;
    }
    [Serializable]
    public class InventoryData
    {
        public InventoryItemData[] InventoryItemData;
        public int InventorySize;

    }
}