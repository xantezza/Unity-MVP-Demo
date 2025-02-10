using System;
using Configs.RemoteConfig;
using Gameplay.UI.Inventory;
using Infrastructure.Providers.AssetReferenceProvider;
using Zenject;

namespace Gameplay.Factories
{
    public class GameplayPresentersFactory : IDisposable
    {
        private readonly InventoryPresenter _playerInventoryPresenter;

        [Inject]
        public GameplayPresentersFactory(IAssetReferenceProvider assetReferenceProvider, IGameplayModelsFactory gameplayModelsFactory)
        {
            _playerInventoryPresenter = CreateNewPlayerInventory(assetReferenceProvider, gameplayModelsFactory);
        }

        private InventoryPresenter CreateNewPlayerInventory(IAssetReferenceProvider assetReferenceProvider, IGameplayModelsFactory gameplayModelsFactory)
        {
            return new InventoryPresenter(
                RemoteConfig.InfrastructureConfig.PlayerInventorySize,
                assetReferenceProvider.PlayerInventoryViewAssetReference,
                gameplayModelsFactory.CreateNewInventoryModel(
                    RemoteConfig.InfrastructureConfig.PlayerInventorySize
                )
            );
        }

        public void Dispose()
        {
            _playerInventoryPresenter.Dispose();
        }
    }
}