using UnityEngine;
using Zenject;

public class LevelInstaller : MonoInstaller
{
    [SerializeField] EnemyCharacter enemyPrefab;

    public override void InstallBindings()
    {
        Container.Bind<EnemyBaseContainer>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerCharacter>().FromComponentInHierarchy().AsSingle();
        Container.BindFactory<Transform[], EnemyCharacter, EnemyCharacter.Factory>().FromComponentInNewPrefab(enemyPrefab);

        Container.Bind<DisplayingUI>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Shop>().FromComponentInHierarchy().AsSingle();
        Container.Bind<JoystickController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Inventory>().FromNew().AsSingle();
    }
}