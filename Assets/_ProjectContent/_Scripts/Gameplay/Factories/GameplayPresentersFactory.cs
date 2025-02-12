using System;
using Configs.RemoteConfig;
using Gameplay.UI.Inventory;
using Infrastructure.Providers.AssetReferenceProvider;
using Infrastructure.Services.Saving;
using Zenject;

namespace Gameplay.Factories
{
    public class GameplayPresentersFactory : IInitializable, IDisposable
    {
        private readonly IAssetReferenceProvider _assetReferenceProvider;
        private readonly IGameplayModelsFactory _gameplayModelsFactory;
        
        private InventoryPresenter _playerInventoryPresenter;

        [Inject]
        public GameplayPresentersFactory(IAssetReferenceProvider assetReferenceProvider, IGameplayModelsFactory gameplayModelsFactory)
        {
            _gameplayModelsFactory = gameplayModelsFactory;
            _assetReferenceProvider = assetReferenceProvider;
        }

        public void Initialize()
        {
            _playerInventoryPresenter = CreateNewPlayerInventory(_assetReferenceProvider, _gameplayModelsFactory);
            _playerInventoryPresenter.Initialize();
        }

        public void Dispose()
        {
            _playerInventoryPresenter?.Dispose();
        }

        private InventoryPresenter CreateNewPlayerInventory(IAssetReferenceProvider assetReferenceProvider, IGameplayModelsFactory gameplayModelsFactory)
        {
            return new InventoryPresenter(
                RemoteConfig.Gameplay.PlayerInventorySize,
                assetReferenceProvider.PlayerInventoryViewAssetReference,
                gameplayModelsFactory.CreateNewInventoryModel(
                    RemoteConfig.Gameplay.PlayerInventorySize,
                    SaveKey.PlayerInventory
                )
            );
        }
    }
}