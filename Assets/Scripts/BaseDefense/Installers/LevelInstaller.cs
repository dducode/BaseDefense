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
            
            Container.BindFactory<UnityEngine.Object, EnemyCharacter, EnemyCharacter.Factory>().
                FromFactory<PrefabFactory<EnemyCharacter>>();
            Container.BindFactory<UnityEngine.Object, EnemyBase, EnemyBase.Factory>().
                FromFactory<PrefabFactory<EnemyBase>>();
            Container.BindFactory<UnityEngine.Object, EnemyStation, EnemyStation.Factory>()
                .FromFactory<PrefabFactory<EnemyStation>>();

            Container.Bind<DisplayingUI>().FromComponentInHierarchy().AsSingle();
            Container.Bind<Shop>().FromComponentInHierarchy().AsSingle();
            Container.Bind<JoystickController>().FromComponentInHierarchy().AsSingle();
            Container.Bind<Inventory>().FromNew().AsSingle();
        }
    }
}

