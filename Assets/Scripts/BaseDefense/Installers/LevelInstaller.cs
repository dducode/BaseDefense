using UnityEngine;
using Zenject;
using BaseDefense.Characters;
using BaseDefense.UI;

namespace BaseDefense.Installers
{
    public class LevelInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<Game>().FromComponentInHierarchy().AsSingle();
            Container.Bind<PlayerCharacter>().FromComponentInHierarchy().AsSingle();
            Container.BindFactory<Object, EnemyCharacter, EnemyCharacter.Factory>().
                FromFactory<PrefabFactory<EnemyCharacter>>();
            Container.BindFactory<Object, EnemyFactory, EnemyFactory.Factory>().
                FromFactory<PrefabFactory<EnemyFactory>>();

            Container.Bind<DisplayingUI>().FromComponentInHierarchy().AsSingle();
            Container.Bind<Shop>().FromComponentInHierarchy().AsSingle();
            Container.Bind<JoystickController>().FromComponentInHierarchy().AsSingle();
            Container.Bind<Inventory>().FromNew().AsSingle();
        }
    }
}

