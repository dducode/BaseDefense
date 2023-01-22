using UnityEngine;

///<summary>Базовый класс для всех видов оружия</summary>
public abstract class Gun : MonoBehaviour
{
    [Header("Общие характеристики оружия")]
    [SerializeField] protected GunName gunName;
    public GunName GunName => gunName;

    ///<summary>Префаб патрона. Каждому оружию соответствует свой патрон</summary>
    [Tooltip("Префаб патрона. Каждому оружию соответствует свой патрон")]
    [SerializeField] protected Projectile projectilePrefab;

    ///<summary>Временной интервал между выстрелами</summary>
    ///<value>[0, infinity]</value>
    [Tooltip("Временной интервал между выстрелами. [0, infinity]")]
    [SerializeField, Min(0)] protected float intervalOfShots;
    protected float timeOfLastShot;

    ///<summary>Дула оружия, преобразования, из которого вылетают патроны</summary>
    [Tooltip("Дула оружия, преобразования, из которого вылетают патроны")]
    [SerializeField] protected Transform[] muzzles;

    ///<summary>Производит выстрел из оружия</summary>
    ///<param name="target">Позиция цели, в которую необходимо выстрелить</param>
    public abstract void Shot();

    public virtual void Awake()
    {
        timeOfLastShot = Time.time;
    }
}
