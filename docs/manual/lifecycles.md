# Жизненный цикл объектов

## Уничтожение объектов

В игре множество сущностей имеют собственный жизненный цикл.
Из соображений оптимизации использования памяти нежелательно,
к примеру, каждый раз, когда патрон попал куда-либо,
уничтожать его, а затем снова создавать из префаба.
Вместо этого патрон отключается на сцене 
и хранится в пуле объектов. А чтобы хранить патрон в пуле,
необходимо его наследовать от базового класса Object

````c#
///<summary>Базовый класс для всех видов патронов</summary>
public abstract class Projectile : Object {
    // ...
    protected abstract void OnCollisionEnter (Collision collision);
    // ...
}
````

Затем, когда патрон столкнулся с чем-либо, мы его "уничтожаем",
добавляя в пул, вместо уничтожения на самом деле. Например, 
пуля попала во врага

````c#
public class Bullet : Projectile {
    // ...
    protected override void OnCollisionEnter (Collision collision) {
        // ...
        if (collision.gameObject.GetComponent<IAttackable>() is { } attackable)
            attackable.Hit(m_damage);
        Destroy();
        // ...
    }
}
````

Здесь при попадании пули мы сразу отключаем её gameObject
и добавляем в пул. На сцене это выглядит аналогично
уничтожению

То же самое справедливо для предметов, собираемых игроком

````c#
///<summary>Базовый класс для всех видов выпадаемых предметов</summary>
public abstract class Item : Object {
    // ...
    public abstract void DestroyItem ();
    // ...
    protected Sequence Collapse () {
        // ...
    }
}
````

Когда игрок соберёт предмет (например, драгоценный камень),
произойдёт следующее

````c#
public sealed class PlayerCharacter : BaseCharacter {
    // ...
    private void OnTriggerEnter (Collider other) {
        if (other.GetComponent<Item>() is { } item)
            switch (item) {
                case Gem gem:
                    m_itemCollecting.PutGem(gem);
                    break;
            // ...
            }
        // ...
    }
    // ...
}
````

````c#
public class ItemCollecting : MonoBehaviour {
    // ...
    ///<summary>Кладёт кристалл в инвентарь</summary>
    public void PutGem (Gem gem) {
        m_inventory.PutItem(gem);
    }
    // ...
}
````

````c#
public class Inventory : MonoBehaviour {
    // ...
    ///<summary>Кладёт предмет в инвентарь</summary>
    public void PutItem (Item item) {
        // ...
        item.DestroyItem();
        // ...
    }
    // ...
}
````

````c#
public class Gem : Item {

    public override void DestroyItem () {
        // ...
        Destroy(tweenTask: Collapse());
    }
    // ...
}
````

В данном случае перед уничтожением драгоценного камня
сначала проиграет его анимация исчезновения (твинер Collapse),
после чего gameObject камня отключится и он добавится в пул.

Подробнее о перегрузках метода Destroy можно посмотреть
в описании класса
[Object](../api/BaseDefense.Object.yml)

## Создание объектов

Когда пуля уничтожена, она на самом деле всё ещё хранится
на сцене, просто её gameObject отключен. Но как только мы 
выстрелим из пистолета, нам нужно будет её снова "создать"

````c#
public class Firearm : Gun {
    // ...
    public override void Shot () {
        // ...
        var bullet = Object.Create(projectilePrefab);
        // ...
        var force = /*...*/;
        bullet.AddImpulse(force);
        // ...
    }
}
````

В данном случае мы просто передаём методу Object.Create
префаб пули, а внутри он уже решает, брать её
из пула, либо создавать новую.

Аналогично с драгоценными камнями. Они выбрасываются, 
когда игрок уничтожает кристалл. У кристалла есть 
компонент 
[ItemDrop](../api/BaseDefense.Items.ItemDrop.yml), 
у которого он вызывает метод DropItems при уничтожении

````c#
public class Crystal : Object, IAttackable {
    // ...
    private void DestroyCrystal () {
        // ...
        m_itemDrop.DropItems();
        Destroy();
}
````
````c#
public class ItemDrop : MonoBehaviour {
    // ...
    ///<summary>Вызывается для выброса предметов</summary>
    ///<remarks>Количество выпадаемых предметов выбирается случайным образом</remarks>
    public void DropItems () {
        var itemsCount = Random.Range(/*...*/);
    
        for (var i = 0; i < itemsCount; i++) {
            // ...
            var item = Object.Create(itemPrefab);
            var force = new Vector3(/*...*/);
            item.Drop(force);
        }
    }
}
````

## Сохранение и загрузка объектов

В классе Object определены методы сохранения и загрузки 
состояния - в базовом варианте сохраняются и загружаются
позиции и ориентации объектов

````c#
public virtual void Save (GameDataWriter writer) {
    writer.Write(transform.position);
    writer.Write(transform.rotation);
}


public virtual void Load (GameDataReader reader) {
    transform.position = reader.ReadPosition();
    transform.rotation = reader.ReadRotation();
}
````

Однако в классах-наследниках можно переопределить
данную реализацию, например, для дополнительного
сохранения и загрузки текущего количества здоровья
у персонажей

````c#
public override void Save (GameDataWriter writer) {
    base.Save(writer);
    writer.Write(CurrentHealthPoints);
}


public override void Load (GameDataReader reader) {
    base.Load(reader);
    CurrentHealthPoints = reader.ReadFloat();
}
````

Затем для сохранения, например, игрока, мы делаем следующее

````c#
var writer = new GameDataWriter(binaryWriter);
m_playerCharacter.Save(writer);
````

Аналогично с загрузкой

````c#
var reader = new GameDataReader(binaryReader);
m_playerCharacter.Load(reader);
````