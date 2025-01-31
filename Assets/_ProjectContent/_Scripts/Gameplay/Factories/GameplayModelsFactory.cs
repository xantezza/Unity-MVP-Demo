using Gameplay.UI.Inventory;
using Infrastructure.Services.Logging;
using Infrastructure.Services.Saving;
using JetBrains.Annotations;
using Zenject;

namespace Gameplay.Factories
{
    public interface IGameplayModelsFactory
    {
        IInventoryModel InventoryModel { get; }
    }

    [UsedImplicitly]
    public class GameplayModelsFactory : IGameplayModelsFactory
    {
        private IInventoryModel _inventoryModel;
        private ISaveService _saveService;
        private IConditionalLoggingService _conditionalLoggingService;

        public IInventoryModel InventoryModel => _inventoryModel ??= new InventoryModel(_saveService, _conditionalLoggingService);
        
        [Inject]
        private void Inject(ISaveService saveService, IConditionalLoggingService conditionalLoggingService)
        {
            _conditionalLoggingService = conditionalLoggingService;
            _saveService = saveService;
        }
    }
}