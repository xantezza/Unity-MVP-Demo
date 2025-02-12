using Gameplay.UI.Inventory;
using Infrastructure.Services.Saving;

namespace Gameplay.Factories
{
    public interface IGameplayModelsFactory
    {
        IInventoryModel CreateNewInventoryModel(int inventorySize, SaveKey saveKey);
    }
}