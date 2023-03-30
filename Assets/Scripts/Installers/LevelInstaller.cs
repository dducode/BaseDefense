using Zenject;
using BaseDefense.Characters;
using UnityEngine;

namespace BaseDefense.Installers {

    public class LevelInstaller : MonoInstaller {

        [SerializeField]
        private Game game;
        
        [SerializeField]
        private PlayerCharacter player;

        [SerializeField]
        private Inventory inventory;

        [SerializeField]
        private Shop shop;


        public override void InstallBindings () {
            Container.Bind<Game>().FromInstance(game).AsSingle();
            Container.Bind<PlayerCharacter>().FromInstance(player).AsSingle();
            Container.Bind<Inventory>().FromInstance(inventory).AsSingle();
            Container.Bind<Shop>().FromInstance(shop).AsSingle();

            Container.BindFactory<UnityEngine.Object, EnemyCharacter, EnemyCharacter.Factory>()
                .FromFactory<PrefabFactory<EnemyCharacter>>();
            Container.BindFactory<UnityEngine.Object, EnemyBase, EnemyBase.Factory>()
                .FromFactory<PrefabFactory<EnemyBase>>();
            Container.BindFactory<UnityEngine.Object, EnemyStation, EnemyStation.Factory>()
                .FromFactory<PrefabFactory<EnemyStation>>();
        }

    }

}