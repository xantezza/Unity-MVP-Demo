using System;
using Configs.RemoteConfig;
using Gameplay.UI.Inventory;
using Infrastructure.Providers.AssetReferenceProvider;
using Infrastructure.Services.Saving;
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
                RemoteConfig.Gameplay.PlayerInventorySize,
                assetReferenceProvider.PlayerInventoryViewAssetReference,
                gameplayModelsFactory.CreateNewInventoryModel(
                    RemoteConfig.Gameplay.PlayerInventorySize,
                    SaveKey.PlayerInventory
                )
            );
        }

        public void Dispose()
        {
            _playerInventoryPresenter.Dispose();
        }
    }
}