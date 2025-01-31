using Configs.Gameplay.UI;
using Gameplay.Factories;
using UnityEngine;
using Zenject;

namespace Gameplay.Installers
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private InventoryItemIconsConfig _inventoryItemIconsConfig;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameplayModelsFactory>().FromNew().AsSingle();
            Container.BindInterfacesTo<GameplayPresentersFactory>().FromNew().AsSingle();
            
            Container.BindInterfacesTo<InventoryItemIconsConfig>().FromInstance(_inventoryItemIconsConfig).AsSingle();
        }
    }
}