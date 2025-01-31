using System;
using Gameplay.UI.Inventory;
using Zenject;

namespace Gameplay.Factories
{
    public class GameplayPresentersFactory : IDisposable
    {
        private readonly InventoryPresenter _inventoryPresenter;

        [Inject]
        public GameplayPresentersFactory(IInstantiator instantiator)
        {
            _inventoryPresenter = instantiator.Instantiate<InventoryPresenter>();
        }

        public void Dispose()
        {
            _inventoryPresenter.Dispose();
        }
    }
}