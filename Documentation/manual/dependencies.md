# Зависимости

Есть множество способов связки классов между собой - 
можно находить объекты нужных классов через метод
[FindObjectOfType](https://docs.unity3d.com/ScriptReference/Object.FindObjectOfType.html),
можно использовать паттерн Service Locator. В данном проекте
для установки зависимостей между классами используется
DI-фреймворк
[Zenject](https://github.com/Mathijs-Bakker/Extenject).
С его помощью создаются специальные установщики, внутри
которых объекты классов "складываются" в один контейнер.

Всего в проекте 2 таких установщика -
[Level Installer](../api/BaseDefense.Installers.LevelInstaller.yml) 
и
[User Interface Installer](../api/BaseDefense.Installers.UserInterfaceInstaller.yml)

## Простые зависимости

Допустим, игрок решил сменить оружие. Для этого ему нужно
сходить в магазин и выбрать оружие. Объекту игрового
персонажа изначально неизвестно про объект магазина, но
мы может написать следующее

````c#
[Inject]
private Shop m_shop;
````

Теперь Zenject при инициализации игры позаботится о том,
чтобы игрок знал, где находится магазин, и смог
выбрать оружие

````c#
m_gun = m_shop.TakeGun(gunId);
````

Это будет работать, т.к. ранее в LevelInstaller мы
"сложили" объект магазина в общую кучу (контейнер)

````c#
Container.Bind<Shop>().FromInstance(shop).AsSingle();
````

## Зависимости для создаваемых объектов

Ранее писалось, что Zenject заботится об установке
зависимостей сразу после <b>запуска игры</b>. Это означает,
что если мы <b>во время игры</b> создадим объект, которому
нужны какие-то другие объекты, он не будет про них
знать. Для решения этой проблемы используются фабрики.
Хороший пример - спавн врага

````c#
var enemy = Object.CreateFromFactory(enemyPrefab, m_enemyFactory);
````

Чтобы это работало, спавнеру необходимо знать про фабрику
врага

````c#
[Inject]
private EnemyCharacter.Factory m_enemyFactory;
````

А чтобы Zenject "рассказал" спавнеру про фабрику, нужно,
чтобы она в принципе существовала

````c#
public sealed class EnemyCharacter : BaseCharacter {
    // ...
    public class Factory : PlaceholderFactory<UnityEngine.Object, EnemyCharacter> { }
}
````

А также необходимо "положить" её в контейнер

````c#
Container.BindFactory<UnityEngine.Object, EnemyCharacter, EnemyCharacter.Factory>()
    .FromFactory<PrefabFactory<EnemyCharacter>>();
````

Теперь, при спавне врага методом Object.CreateFromFactory,
все зависимости, нужные врагу, будут установлены.