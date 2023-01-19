using UnityEngine;
using Zenject;

public class LevelInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerCharacter>().FromComponentInHierarchy().AsSingle();
        Container.BindFactory<UnityEngine.Object, Transform[], EnemyCharacter, EnemyCharacter.Factory>().
            FromFactory<PrefabFactory<Transform[], EnemyCharacter>>();

        Container.Bind<DisplayingUI>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Shop>().FromComponentInHierarchy().AsSingle();
        Container.Bind<JoystickController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Inventory>().FromNew().AsSingle();
    }
}