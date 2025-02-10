using Gameplay.UI.Inventory;
using Infrastructure.Services.Logging;
using Infrastructure.Services.Saving;
using JetBrains.Annotations;
using Zenject;

namespace Gameplay.Factories
{
    public interface IGameplayModelsFactory
    {
        IInventoryModel CreateNewInventoryModel(int inventorySize);
    }

    [UsedImplicitly]
    public class GameplayModelsFactory : IGameplayModelsFactory
    {
        private IInventoryModel _inventoryModel;
        private ISaveService _saveService;
        private IConditionalLoggingService _conditionalLoggingService;

        [Inject]
        private void Inject(ISaveService saveService, IConditionalLoggingService conditionalLoggingService)
        {
            _conditionalLoggingService = conditionalLoggingService;
            _saveService = saveService;
        }


        public IInventoryModel CreateNewInventoryModel(int inventorySize)
        {
            return new InventoryModel(inventorySize, _saveService, _conditionalLoggingService);
        }
    }
}